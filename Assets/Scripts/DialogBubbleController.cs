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

    private Coroutine hideInputCoroutine;
    private Coroutine hideReplyCoroutine;

    void Start()
    {
        inputPanel.SetActive(false);
        replyPanel.SetActive(false);

        sendButton.onClick.AddListener(OnSendClicked);

        // 当输入框被选中或输入时取消隐藏
        inputField.onSelect.AddListener(_ => CancelHideInput());
        inputField.onDeselect.AddListener(_ => StartHideInputIfEmpty());
        inputField.onValueChanged.AddListener(_ => {
            if (!string.IsNullOrWhiteSpace(inputField.text))
                CancelHideInput();
        });
        inputField.onSubmit.AddListener(_ => OnSendClicked());
    }

    // ================= 输入框 =================

    public void ShowInput()
    {
        inputPanel.SetActive(true);
        inputField.text = "";
        // 不自动聚焦光标
        CancelHideInput();

        // 如果输入框为空，启动倒计时自动隐藏
        StartHideInputIfEmpty();
    }

    void StartHideInputIfEmpty()
    {
        CancelHideInput();
        hideInputCoroutine = StartCoroutine(HideInputIfEmptyDelay());
    }

    void CancelHideInput()
    {
        if (hideInputCoroutine != null)
        {
            StopCoroutine(hideInputCoroutine);
            hideInputCoroutine = null;
        }
    }

    IEnumerator HideInputIfEmptyDelay()
    {
        float elapsed = 0f;
        while (elapsed < inputAutoHideTime)
        {
            // 如果用户输入了内容，则停止倒计时
            if (!string.IsNullOrWhiteSpace(inputField.text))
                yield break;

            elapsed += Time.deltaTime;
            yield return null; // 等待下一帧
        }

        // 时间到且没有输入内容，隐藏输入框
        inputPanel.SetActive(false);
        hideInputCoroutine = null;
    }

    void OnSendClicked()
    {
        string text = inputField.text;

        if (string.IsNullOrWhiteSpace(text))
            return;

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
