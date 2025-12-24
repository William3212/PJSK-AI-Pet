using UnityEngine;
using TMPro;


public class ChatController : MonoBehaviour
{
    public DeepSeekClient deepSeekClient;
    [Header("UI")]
    public TMP_InputField inputField;
    public TMP_Text replyText;

    [Header("DeskPet")]
    public DeskPetAnimator petAnimator;

    public void OnSendClicked()
    {
        string userText = inputField.text;
        if (string.IsNullOrEmpty(userText)) return;

        inputField.text = "";

        replyText.text = "……";

        StartCoroutine(
            deepSeekClient.SendMessage(userText, OnAIResponse)
        );
    }


    AIResponse FakeAI(string userText)
    {
        if (userText.Contains("开心"))
            return new AIResponse("我也觉得很开心呢！", Emotion.Joy);

        if (userText.Contains("难过"))
            return new AIResponse("别难过，我在呢。", Emotion.Sad);

        // 没有明显情绪
        return new AIResponse("我在听你说哦。", null);
    }

    void Start()
    {
        inputField.onSelect.AddListener(OnInputSelected);
        inputField.onDeselect.AddListener(OnInputDeselected);
    }

    void OnInputSelected(string _)
    {
        petAnimator.PlayListen();
    }

    void OnInputDeselected(string _)
    {
        petAnimator.PlayIdle();
    }
    void OnAIResponse(AIResponse response)
    {
        replyText.text = response.text;
        petAnimator.PlayAIResponse(response.emotion);
    }


}
