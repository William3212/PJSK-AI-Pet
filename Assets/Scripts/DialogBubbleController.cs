using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DialogBubbleController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject inputPanel;
    public GameObject replyPanel;

    [Header("Input")]
    public TMP_InputField inputField;
    public Button sendButton;

    [Header("Reply")]
    public TMP_Text replyText;

    [Header("Timing")]
    public float inputAutoHideTime = 3f;
    public float replyAutoHideTime = 2.5f;

    [Header("Refs")]
    public ChatController chatController;

    Coroutine hideInputCoroutine;
    Coroutine hideReplyCoroutine;

    void Start()
    {
        inputPanel.SetActive(false);
        replyPanel.SetActive(false);

        sendButton.onClick.AddListener(OnSendClicked);

        inputField.onSelect.AddListener(_ => CancelHideInput());
        inputField.onDeselect.AddListener(_ => StartHideInputTimer());
        inputField.onSubmit.AddListener(_ => OnSendClicked());
    }

    // ================= 输入框 =================

    public void ShowInput()
    {
        inputPanel.SetActive(true);
        inputField.text = "";
        inputField.ActivateInputField();
        CancelHideInput();
    }

    void StartHideInputTimer()
    {
        CancelHideInput();
        hideInputCoroutine = StartCoroutine(HideInputDelay());
    }

    void CancelHideInput()
    {
        if (hideInputCoroutine != null)
            StopCoroutine(hideInputCoroutine);
    }

    IEnumerator HideInputDelay()
    {
        yield return new WaitForSeconds(inputAutoHideTime);
        inputPanel.SetActive(false);
    }

    void OnSendClicked()
    {
        if (string.IsNullOrWhiteSpace(inputField.text))
            return;

        string text = inputField.text;

        inputField.text = "";
        inputPanel.SetActive(false);

        chatController.SendMessageFromUI(text);
    }

    // ================= 回复框 =================

    public void ShowReply()
    {
        replyPanel.SetActive(true);

        if (hideReplyCoroutine != null)
            StopCoroutine(hideReplyCoroutine);

        hideReplyCoroutine = StartCoroutine(HideReplyDelay());
    }


    IEnumerator HideReplyDelay()
    {
        yield return new WaitForSeconds(replyAutoHideTime);
        replyPanel.SetActive(false);
    }
}
