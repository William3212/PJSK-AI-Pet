using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;

public class DeepSeekClient : MonoBehaviour
{
    [Header("DeepSeek")]
    public string apiKey;
    public CharacterManager characterManager;
    //const string API_URL = "https://api.deepseek.com/v1/chat/completions";
    public string API_URL;

    void Start()
    {
        ApplyApiSetting(
            SettingsStorage.LoadApiUrl(),
            SettingsStorage.LoadApiKey()
        );
    }

    public IEnumerator SendMessage(string userInput, Action<AIResponse> onResult)
    {
        string characterPrompt = "你是一个桌宠角色，需要和用户简短对话。";
        if (characterManager != null &&
            characterManager.CurrentCharacter != null)
        {
            characterPrompt =
                characterManager.CurrentCharacter.runtimeAIPrompt;
        }
        string prompt = BuildPrompt(
        userInput,
        characterManager.CurrentCharacter.runtimeAIPrompt
    );

        DeepSeekRequest requestData = new DeepSeekRequest
        {
            model = "deepseek-chat",
            messages = new Message[]
            {
                new Message { role = "user", content = prompt }
            }
        };

        string json = JsonUtility.ToJson(requestData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(API_URL, "POST");
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

        AIResponse aiResponse = ParseAIResponse(content);
        onResult?.Invoke(aiResponse);
    }

    // ===== Prompt =====

    string BuildPrompt(string userInput, string characterPrompt)
    {
        return
$@"你是一个桌宠角色，需要和用户简短对话

【角色性格设定】
{characterPrompt}

【对话规则】
请根据用户的话回复一句自然的话，并判断是否存在明显情绪。

可用情绪只有以下这些（如果没有明显情绪，就返回 null）：
Joy, Sad, Angry, Surprise, Doubt, Laugh

请严格以 JSON 格式返回，不要包含任何多余文字。

返回格式示例：
{{
  ""text"": ""你的回复内容"",
  ""emotion"": ""Joy""
}}

如果没有情绪：
{{
  ""text"": ""你的回复内容"",
  ""emotion"": null
}}

【用户的话】
{userInput}";
    }

    public void ApplyApiSetting(string url, string key)
    {
        API_URL = url;
        apiKey = key;
    }


    // ===== JSON 解析 =====
    AIResponse ParseAIResponse(string jsonText)
    {
        try
        {
            SimpleAIResponse data =
                JsonUtility.FromJson<SimpleAIResponse>(jsonText);

            Emotion? emotion = null;
            if (!string.IsNullOrEmpty(data.emotion))
            {
                emotion = Enum.Parse<Emotion>(data.emotion);
            }

            return new AIResponse(data.text, emotion);
        }
        catch
        {
            return new AIResponse("我有点没听懂呢。", null);
        }
    }
    public void RefreshSystemPrompt()
    {
        // 如果你有缓存 Prompt
        // 就在这里更新
    }
}
