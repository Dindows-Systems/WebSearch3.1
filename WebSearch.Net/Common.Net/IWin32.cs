using System;
using System.Runtime.InteropServices;
using System.Security;

namespace WebSearch.Common.Net
{
    public class IWin32
    {
        // memcpy - copy a block of memory
        [DllImport("ntdll.dll")]
        public static extern IntPtr memcpy(
            IntPtr dst,
            IntPtr src,
            int count);

        [DllImport("ntdll.dll")]
        public static extern unsafe byte* memcpy(
            byte* dst,
            byte* src,
            int count);

        // memset - fill memory with specified values
        [DllImport("ntdll.dll")]
        public static extern IntPtr memset(
            IntPtr dst,
            int filler,
            int count);

        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public RECT(int l, int t, int r, int b)
            {
                left = l; top = t; right = r; bottom = b;
            }
        }

        // lock the user mouse in the specified rectangular
        [System.Runtime.InteropServices.DllImport("user32", EntryPoint = "ClipCursor")]
        public extern static int ClipCursor(ref RECT lpRect);

        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
        public extern static int GetSystemMetrics(int nIndex);
    }
}
