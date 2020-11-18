using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace SimpleScreenRecorder.Model
{
    public class Recorder
    {
        private CustomScreen _currentScreen { get; set; }

        public Recorder()
        {

        }

        public BitmapImage GetBitmapImage()
        {
            return _currentScreen.GetBitmapImage();
        }

        public void OnScreenSelect(CustomScreen screen)
        {
            _currentScreen = screen;
        }

        public List<CustomScreen> GetScreens()
        {
            var screens = new List<CustomScreen>();
            foreach (var s in Screen.AllScreens)
                screens.Add(new CustomScreen(s));

            return screens;
        }
    }
}
