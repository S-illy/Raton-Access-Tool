using System;
using System.Runtime.InteropServices;

public class TaskbarHandler
{
    [DllImport("user32.dll")]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    public static extern int ShowWindow(IntPtr hWnd, int nCmdShow);

    const int SW_HIDE = 0;
    const int SW_SHOW = 5;

    public static void HideTaskbar()
    {
        IntPtr taskbarWnd = FindWindow("Shell_TrayWnd", null);
        ShowWindow(taskbarWnd, SW_HIDE);
    }

    public static void ShowTaskbar()
    {
        IntPtr taskbarWnd = FindWindow("Shell_TrayWnd", null);
        ShowWindow(taskbarWnd, SW_SHOW);
    }
}
