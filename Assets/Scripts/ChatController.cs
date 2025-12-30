using UnityEngine;
using TMPro;
using System.Collections;



public class ChatController : MonoBehaviour
{
    public DeepSeekClient deepSeekClient;
    public DialogBubbleController dialogBubble;

    [Header("UI")]
    public TMP_InputField inputField;
    public TMP_Text replyText;

    [Header("DeskPet")]
    public DeskPetAnimator petAnimator;

    [Header("Typewriter")]
    public float typingSpeed = 0.03f;

    Coroutine typingCoroutine;
    bool isWaitingAI = false;




    public void SendMessageFromUI(string text)
    {
        if (isWaitingAI) return; // 正在等待 AI，禁止重复发送

        isWaitingAI = true;

        replyText.text = "……";
        StartCoroutine(
            deepSeekClient.SendMessage(text, OnAIResponse)
        );
    }


    void Start()
    {
        inputField.onSelect.AddListener(OnInputSelected);
        inputField.onDeselect.AddListener(OnInputDeselected);
        inputField.onSubmit.AddListener(OnSubmit);
    }

    void OnSubmit(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

         SendMessageFromUI(text);
    }

    void OnInputSelected(string _)
    {
        petAnimator.PlayListen();
    }

    void OnInputDeselected(string _)
    {
        petAnimator.PlayIdle();
    }


    IEnumerator TypeText(string fullText)
    {
        // Debug.Log("[TypeText] Coroutine Start");
        replyText.text = "";
        dialogBubble.ShowReply();

        foreach (char c in fullText)
        {
            replyText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

    }


    void OnAIResponse(AIResponse response)
    {
        isWaitingAI = false; // AI 已返回，可以下一条
        Debug.Log("[AIResponse] Start: " + response.text + " | Emotion=" + response.emotion);
        petAnimator.PlayAIResponse(response.emotion);

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(response.text));
    }


}
