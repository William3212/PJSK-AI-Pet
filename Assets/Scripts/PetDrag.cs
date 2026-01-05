using UnityEngine;

public class PetDrag : MonoBehaviour
{
    private Vector3 offset;
    private Camera mainCamera;
    private bool isDragging = false;

    [Header("Spine / Pet Transform")]
    public Transform target;

    [Header("UI Elements to follow the pet")]
    public RectTransform bubble;      // 气泡
    public RectTransform sendButton;  // 发送按钮
    public RectTransform settingButton; // 设置按钮
    public RectTransform inputField;  // 输入框

    // UI 与宠物的初始偏移
    private Vector3 bubbleOffset;
    private Vector3 sendButtonOffset;
    private Vector3 settingButtonOffset;
    private Vector3 inputFieldOffset;

    void Start()
    {
        mainCamera = Camera.main;

        if (target == null)
        {
            target = transform.parent; // 默认父节点
        }

        // 计算初始偏移
        if (bubble != null) bubbleOffset = bubble.position - target.position;
        if (sendButton != null) sendButtonOffset = sendButton.position - target.position;
        if (settingButton != null) settingButtonOffset = settingButton.position - target.position;
        if (inputField != null) inputFieldOffset = inputField.position - target.position;
    }

    void Update()
    {
        // 保证 ClickArea 跟随目标
        if (target != null)
            transform.position = target.position;

        // UI 跟随宠物移动
        if (bubble != null) bubble.position = target.position + bubbleOffset;
        if (sendButton != null) sendButton.position = target.position + sendButtonOffset;
        if (settingButton != null) settingButton.position = target.position + settingButtonOffset;
        if (inputField != null) inputField.position = target.position + inputFieldOffset;
    }

    void OnMouseDown()
    {
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        offset = target.position - mouseWorldPos;
        isDragging = true;
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        target.position = mouseWorldPos + offset;
        // UI 跟随拖拽已经在 Update 里处理
    }

    void OnMouseUp()
    {
        isDragging = false;
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(mainCamera.transform.position.z);
        return mainCamera.ScreenToWorldPoint(mouseScreenPos);
    }
}
