using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AdrenalineSolver
{
    public static class PinvokeHelpers
    {
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern long GetWindowRect(int hWnd, ref Rectangle lpRect);

        [DllImport("User32.Dll")]
        public static extern Int32 PostMessage(int hWnd, int msg, int wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(int hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);


        const int WM_KEYDOWN = 0x0100;

        // LEFT ARROW key
        const int VK_LEFT = 0x25;

        // UP ARROW key
        const int VK_UP = 0x26;

        // RIGHT ARROW key
        const int VK_RIGHT = 0x27;

        // DOWN ARROW key
        const int VK_DOWN = 0x28;


        public static IntPtr GetAdrenalineWindowHandle()
        {
            // todo: cache this or s/t
            var hwnd = FindWindow(null, "ADRENA-LINE");
            return hwnd;
        }

        public static Bitmap GetAdrenalineBitmap()
        {
            var hwnd = GetAdrenalineWindowHandle();
            var screenshotHelper = new ScreenshotHelper();

            // numbers here are just numbers I got from messing around in paint to find a decent crop size
            return screenshotHelper.CaptureWindow(hwnd, 118, 101, 97, 103);
        }

        public static async Task SendKeyPress(WindowsVirtualKey key)
        {
            SetGameWindowFocus();
            await SendKeypressForALittleWhile(key);
        }

        public static void SetGameWindowFocus()
        {
            SetForegroundWindow(GetAdrenalineWindowHandle());
        }

        private static async Task SendKeypressForALittleWhile(WindowsVirtualKey key)
        {
            var hwnd = GetAdrenalineWindowHandle();

            // absolutely insane usage of async here john, fix it up
            var t = Task.Delay(20);
            while (!t.IsCompleted)
            {
                SendMessage(hwnd.ToInt32(), WM_KEYDOWN, (int)key, 1);
            }

            await t;
        }
    }

    public enum WindowsVirtualKey : int
    {
        // LEFT ARROW key
        Left = 0x25,

        // UP ARROW key
        Up = 0x26,

        // RIGHT ARROW key
        Right = 0x27,

        // DOWN ARROW key
        Down = 0x28,
    }
}
