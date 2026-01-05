using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Runtime.InteropServices;

public class ClickThrough : MonoBehaviour
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    private const int GWL_EXSTYLE = -20;
    private const int WS_EX_TRANSPARENT = 0x00000020;
    private const int WS_EX_LAYERED = 0x00080000;

    private IntPtr hwnd;
    private int originalExStyle;

    void Start()
    {
        hwnd = GetActiveWindow();
        originalExStyle = GetWindowLong(hwnd, GWL_EXSTYLE);

        SetClickThrough(true);
    }

    void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        bool isBlocking = false;

        // 1️⃣ 检测 UI 点击阻挡
        if (EventSystem.current != null)
        {
            var pointerData = new PointerEventData(EventSystem.current)
            {
                position = mousePos
            };
            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (var r in results)
            {
                if (r.gameObject.GetComponent<UIClickBlocker>() != null)
                {
                    isBlocking = true;
                    break;
                }
            }
        }

        // 2️⃣ 检测 Spine Collider
        if (!isBlocking)
        {
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            Collider2D hit = Physics2D.OverlapPoint(worldPos);
            if (hit != null && hit.GetComponent<ClickBlocker>() != null)
            {
                isBlocking = true;
            }
        }

        SetClickThrough(!isBlocking);
    }

    void SetClickThrough(bool enable)
    {
        int exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
        if (enable)
        {
            exStyle |= WS_EX_LAYERED | WS_EX_TRANSPARENT;
        }
        else
        {
            exStyle = originalExStyle;
        }
        SetWindowLong(hwnd, GWL_EXSTYLE, exStyle);
    }
}
