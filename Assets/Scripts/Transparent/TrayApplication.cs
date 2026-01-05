using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public class TrayApplication : MonoBehaviour
{
#if UNITY_STANDALONE_WIN
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    private static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern uint SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
        int X, int Y, int cx, int cy, uint uFlags);

    private const int GWL_EXSTYLE = -20;
    private const uint WS_EX_TOOLWINDOW = 0x00000080; // 隐藏任务栏
    private const uint WS_EX_APPWINDOW = 0x00040000;  // 显示任务栏（需要去掉）
    private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_SHOWWINDOW = 0x0040;

    private NotifyIcon trayIcon;
#endif

    void Awake()
    {
        // 失去焦点仍然运行
        UnityEngine.Application.runInBackground = true;

#if UNITY_STANDALONE_WIN
        IntPtr hwnd = GetActiveWindow();

        // 隐藏任务栏图标，显示为工具窗口
        uint exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
        exStyle |= WS_EX_TOOLWINDOW;    // 设置工具窗口
        exStyle &= ~WS_EX_APPWINDOW;    // 去掉普通窗口样式
        SetWindowLong(hwnd, GWL_EXSTYLE, exStyle);

        // 窗口置顶
        SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0,
            SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);

        // 初始化托盘图标
        trayIcon = new NotifyIcon();
        trayIcon.Icon = System.Drawing.SystemIcons.Application; // 可换成桌宠图标
        trayIcon.Text = "pjsk桌宠";
        trayIcon.Visible = true;

        // 托盘右键菜单
        System.Windows.Forms.ContextMenu menu = new System.Windows.Forms.ContextMenu();
        System.Windows.Forms.MenuItem exitItem = new System.Windows.Forms.MenuItem("退出", (s, e) =>
        {
            trayIcon.Visible = false;
            UnityEngine.Application.Quit();
        });
        menu.MenuItems.Add(exitItem);
        trayIcon.ContextMenu = menu;

        // 双击托盘图标重新置顶窗口
        trayIcon.DoubleClick += (s, e) =>
        {
            SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0,
                SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
        };
#endif
    }

    void OnApplicationQuit()
    {
#if UNITY_STANDALONE_WIN
        if (trayIcon != null)
        {
            trayIcon.Visible = false;
            trayIcon.Dispose();
        }
#endif
    }
}
