using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

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

        public static IntPtr GetAdrenalineWindowHandle()
        {
            var hwnd = FindWindow(null, "ADRENA-LINE");

            return hwnd;
        }

        public static Bitmap GetAdrenalineBitmap()
        {
            var hwnd = GetAdrenalineWindowHandle();

            var screenshotHelper = new ScreenshotHelper();
            return screenshotHelper.CaptureWindow(hwnd);
        }
    }
}
