using SimpleScreenRecorder.Model.Enums;
using SimpleScreenRecorder.Model.EventArgs;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Interop;

namespace SimpleScreenRecorder.Model
{
    public class HotkeyManager
    {
        private const int WM_HOTKEY = 0x312;
        public List<Hotkey> Hotkeys { get; }
        public IntPtr WindowHandle { get; private set; }

        public event EventHandler<HotkeyEventArgs> HotkeyPressed;

        private HotkeyManager()
        {
            Hotkeys = new List<Hotkey>();
        }

        ~HotkeyManager()
        {
            foreach (var i in Hotkeys)
                CustomUnregisterHotkey(i.Id, false);

            Hotkeys.Clear();
            Console.WriteLine("Unregistered all hotkeys");
        }

        public void Init(IntPtr hWnd)
        {
            WindowHandle = hWnd;
            HwndSource source = HwndSource.FromHwnd(WindowHandle);
            source.AddHook(new HwndSourceHook(WndProc));
            CustomRegisterHotkey(Keys.F9, KeyModifiers.None);
            CustomRegisterHotkey(Keys.F10, KeyModifiers.None);
        }

        public int CustomRegisterHotkey(Keys key, KeyModifiers modifiers)
        {
            // Create a new hotkey
            var hotkey = new Hotkey(Hotkeys.Count, key, modifiers);
            Hotkeys.Add(hotkey);
            RegisterHotKey(WindowHandle, hotkey.Id, (uint)hotkey.Modifiers, (uint)hotkey.Key);
            return 0;
        }

        public void CustomUnregisterHotkey(int id, bool remove = true)
        {
            var hotkey = Hotkeys.Find(x => x.Id == id);
            if (hotkey != null)
            {
                UnregisterHotKey(WindowHandle, id);

                if (remove) Hotkeys.Remove(hotkey);
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_HOTKEY:
                    var id = wParam.ToInt32();
                    var hk = Hotkeys.Find(x => x.Id == id);
                    if (hk != null)
                        HotkeyPressed.Invoke(this, new HotkeyEventArgs(hk));
                    handled = true;
                    break;

                default:
                    break;
            }

            return IntPtr.Zero;
        }

        #region Singleton implementation
        private static HotkeyManager instance = null;
        private static readonly object padlock = new object();
        public static HotkeyManager Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                        instance = new HotkeyManager();
                    return instance;
                }
            }
        }
        #endregion

        #region User32 methods
        [DllImport("user32", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        #endregion
    }
}
