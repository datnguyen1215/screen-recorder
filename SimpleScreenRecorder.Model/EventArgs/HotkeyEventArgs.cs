namespace SimpleScreenRecorder.Model.EventArgs
{
    public class HotkeyEventArgs : System.EventArgs
    {
        public Hotkey Hotkey { get; set; }

        public HotkeyEventArgs(Hotkey hotkey)
        {
            Hotkey = hotkey;
        }
    }
}
