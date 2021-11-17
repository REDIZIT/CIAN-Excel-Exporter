using System;
using UnityEngine;
using System.Runtime.InteropServices;

public class HideWindow
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    const int SW_SHOW = 5;

    public static void Show()
    {
        var hwnd = GetActiveWindow();
        ShowWindow(hwnd, 0);
        //ShowWindow(hwnd, 5);
    }
}