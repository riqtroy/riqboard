using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RIQBoards
{
    class RIQBoardHook
    {
        public delegate int RIQBoardHookProc(
            int code,
            int wParam,
            ref RIQBoardHookStruct lParam);
        private static RIQBoardHookProc callBackDelegate;
        public struct RIQBoardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }
        const int WH_KEYBOARD_LL = 13;
        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;
        const int WM_SYSKEYDOWN = 0x104;
        const int WM_SYSKEYUP = 0x105;

        IntPtr exHook = IntPtr.Zero;
        public event KeyEventHandler Down;
        public event KeyEventHandler Up;
        public RIQBoardHook()
        {
            Hook();
        }
        ~RIQBoardHook()
        {
            UnHook();
        }
        public void Hook()
        {
            IntPtr hInstance = LoadLibrary("User32");
            callBackDelegate = new RIQBoardHookProc(HookProc);
            exHook = SetWindowsHookEx(WH_KEYBOARD_LL, callBackDelegate, hInstance, 0);
        }
        public void UnHook()
        {
            UnhookWindowsHookEx(exHook);
        }
        public int HookProc(int code, int wParam, ref RIQBoardHookStruct lParam)
        {
            if (code >= 0)
            {
                Keys key = (Keys)lParam.vkCode;
                KeyEventArgs push = new KeyEventArgs(key);
                if ((wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN) && Down != null)
                {
                    Down(this, push);
                }
                else if ((wParam == WM_KEYUP || wParam == WM_SYSKEYUP) && Up != null)
                {
                    Up(this, push);
                }
                if (push.Handled)
                    return 1;
            }
            return CallNextHookEx(exHook, code, wParam, ref lParam);
        }
        #region dllimport
        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(
            int idHook,
            RIQBoardHookProc callback,
            IntPtr hInstance,
            uint threadId);
        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(
            IntPtr hInstance);
        [DllImport("user32.dll")]
        static extern int CallNextHookEx(
            IntPtr idHook,
            int nCode,
            int wParam,
            ref RIQBoardHookStruct lParam);
        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(
            string lpFileName);
        #endregion
    }
}
