using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

public class MouseHandler
{
    [DllImport("user32.dll")]
    static extern void SetCursorPos(int x, int y);

    static bool shakeActive = false;
    static bool trapActive = false;
    static Thread shakeThread;
    static Thread trapThread;

    public static void StartShake()
    {
        if (shakeThread == null || !shakeThread.IsAlive)
        {
            shakeActive = true;
            shakeThread = new Thread(ShakeMouse);
            shakeThread.IsBackground = true;
            shakeThread.Start();
        }
    }

    public static void StopShake()
    {
        shakeActive = false;
    }

    static void ShakeMouse()
    {
        Random r = new Random();
        while (shakeActive)
        {
            int x = Cursor.Position.X + r.Next(-5, 6);
            int y = Cursor.Position.Y + r.Next(-5, 6);
            SetCursorPos(x, y);
            Thread.Sleep(20);
        }
    }

    public static void Trap()
    {
        if (trapThread == null || !trapThread.IsAlive)
        {
            trapActive = true;
            trapThread = new Thread(TrapMouse);
            trapThread.IsBackground = true;
            trapThread.Start();
        }
    }

    public static void Untrap()
    {
        trapActive = false;
    }

    static void TrapMouse()
    {
        int lastX = Cursor.Position.X;
        int lastY = Cursor.Position.Y;

        while (trapActive)
        {
            int currentX = Cursor.Position.X;
            int currentY = Cursor.Position.Y;

            lastX = currentX;
            lastY = currentY;

            Thread.Sleep(1000);

            SetCursorPos(lastX, lastY);
        }
    }
}
