using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;

public class DeepSeekClient : MonoBehaviour
{
    [Header("DeepSeek")]
    public string apiKey;
    public string API_URL;

    public CharacterManager characterManager;

    [Header("Memory")]
    public int maxMemoryCount = 20;
    public int keepRecentCount = 6; // 压缩后保留最近对话
    public int summarizeTriggerCount = 24;

    void Start()
    {
        ApplyApiSetting(
            SettingsStorage.LoadApiUrl(),
            SettingsStorage.LoadApiKey()
        );
    }

    // ================= 主入口 =================

    public IEnumerator SendMessage(string userInput, Action<AIResponse> onResult)
    {
        if (characterManager == null || characterManager.CurrentCharacter == null)
        {
            onResult?.Invoke(new AIResponse("（角色未初始化）", null));
            yield break;
        }

        var character = characterManager.CurrentCharacter;
        var memory = character.memory;

        // ⚠️ 如果记忆过多，先进行总结
        if (memory.messages.Count >= summarizeTriggerCount)
        {
            yield return StartCoroutine(SummarizeMemory(character));
        }

        List<Message> messages = new();

        messages.Add(new Message
        {
            role = "system",
            content = BuildSystemPrompt(character.runtimeAIPrompt)
        });

        if (memory.messages.Count > 0)
            messages.AddRange(memory.messages);

        messages.Add(new Message
        {
            role = "user",
            content = userInput
        });

        DeepSeekRequest requestData = new DeepSeekRequest
        {
            model = "deepseek-chat",
            messages = messages.ToArray()
        };

        string json = JsonUtility.ToJson(requestData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(API_URL, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                onResult?.Invoke(new AIResponse("（网络出错了）", null));
                yield break;
            }

            DeepSeekResponse response =
                JsonUtility.FromJson<DeepSeekResponse>(request.downloadHandler.text);

            string content = response.choices[0].message.content;

            memory.AddUser(userInput);
            memory.AddAssistant(content);

            ChatMemoryStorage.Save(character.characterName, memory);

            AIResponse aiResponse = ParseAIResponse(content);
            onResult?.Invoke(aiResponse);
        }
    }

    // ======================================================
    // 🧠 自动总结旧对话（核心）
    // ======================================================

    IEnumerator SummarizeMemory(CharacterData character)
    {
        var memory = character.memory;

        if (memory.messages.Count < summarizeTriggerCount)
            yield break;

        // 取前面要被压缩的部分
        int summarizeCount = memory.messages.Count - keepRecentCount;
        if (summarizeCount <= 0)
            yield break;

        List<Message> toSummarize = memory.messages.GetRange(0, summarizeCount);

        StringBuilder historyBuilder = new();
        foreach (var msg in toSummarize)
        {
            historyBuilder.AppendLine($"{msg.role}: {msg.content}");
        }

        Debug.Log("【准备总结的原始对话】\n" + historyBuilder.ToString());


        string summarizePrompt =
$@"请将下面的对话总结成一段【长期记忆摘要】，用于之后继续对话。
要求：
- 用第三人称
- 保留关键信息、人物关系、重要事实
- 不要加入分析过程
- 不要分点
- 不超过 150 字

对话内容：
{historyBuilder}";

        DeepSeekRequest req = new DeepSeekRequest
        {
            model = "deepseek-chat",
            messages = new[]
            {
                new Message
                {
                    role = "system",
                    content = summarizePrompt
                }
            }
        };

        string json = JsonUtility.ToJson(req);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using UnityWebRequest request = new UnityWebRequest(API_URL, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning("总结失败，跳过压缩");
            yield break;
        }

        DeepSeekResponse response =
            JsonUtility.FromJson<DeepSeekResponse>(request.downloadHandler.text);

        string summaryText = response.choices[0].message.content;
        Debug.Log("【对话摘要生成】\n" + summaryText);


        // ===== 重建 memory =====
        List<Message> newMemory = new();

        newMemory.Add(new Message
        {
            role = "system",
            content = "【对话摘要】" + summaryText
        });

        // 保留最近几条真实对话
        int start = Mathf.Max(0, memory.messages.Count - keepRecentCount);
        for (int i = start; i < memory.messages.Count; i++)
            newMemory.Add(memory.messages[i]);

        memory.messages = newMemory;

        ChatMemoryStorage.Save(character.characterName, memory);

        Debug.Log("已自动压缩历史对话");
    }

    // ================= Prompt =================

    string BuildSystemPrompt(string characterPrompt)
    {
        return
$@"你是一个桌宠角色，需要和用户进行自然、简短、有情绪的对话。

【角色性格设定】
{characterPrompt}

【回复规则】
- 回复要简短自然
- 使用口语化表达
- 只回复一句话
- 判断是否包含明显情绪
- 情绪只能是以下之一（否则为 null）：
Sad, Angry, Surprise, Doubt, Laugh

【返回格式（必须严格 JSON，不要任何多余文字）】
{{
  ""text"": ""你的回复内容"",
  ""emotion"": ""Sad""
}}

若无情绪：
{{
  ""text"": ""你的回复内容"",
  ""emotion"": null
}}";
    }

    // ================= JSON 解析 =================

    AIResponse ParseAIResponse(string jsonText)
    {
        try
        {
            SimpleAIResponse data =
                JsonUtility.FromJson<SimpleAIResponse>(jsonText);

            Emotion? emotion = null;

            if (!string.IsNullOrEmpty(data.emotion))
                emotion = Enum.Parse<Emotion>(data.emotion);

            return new AIResponse(data.text, emotion);
        }
        catch
        {
            return new AIResponse("我有点没听懂呢。", null);
        }
    }

    // ================= 外部接口 =================

    public void ClearCurrentCharacterMemory()
    {
        if (characterManager?.CurrentCharacter == null)
            return;

        string name = characterManager.CurrentCharacter.characterName;

        characterManager.CurrentCharacter.memory.Clear();
        ChatMemoryStorage.Clear(name);

        Debug.Log("已清空该角色的对话记忆");
    }

    public void ApplyApiSetting(string url, string key)
    {
        API_URL = url;
        apiKey = key;
    }
}
