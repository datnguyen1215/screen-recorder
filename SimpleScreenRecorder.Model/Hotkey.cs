using SimpleScreenRecorder.Model.Enums;
using System.Windows.Forms;

namespace SimpleScreenRecorder.Model
{
    public class Hotkey
    {
        public int Id { get; }
        public Keys Key { get; }
        public KeyModifiers Modifiers { get; }

        public Hotkey(int id, Keys key, KeyModifiers modifiers)
        {
            Id = id;
            Key = key;
            Modifiers = modifiers;
        }
    }
}
