using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace SimpleScreenRecorder.Model
{
    public class Recorder
    {
        private CustomScreen _currentScreen { get; set; }

        /// <summary>
        /// Get bitmap of the current selected screen.
        /// </summary>
        public BitmapImage GetBitmapImage() => _currentScreen.GetBitmapImage();

        /// <summary>
        /// Triggered when there's a screen change.
        /// </summary>
        /// <param name="screen">New selected screen.</param>
        public void OnScreenSelect(CustomScreen screen) => _currentScreen = screen;

        /// <summary>
        /// Get all available screens.
        /// </summary>
        public List<CustomScreen> GetScreens()
        {
            var screens = new List<CustomScreen>();
            foreach (var s in Screen.AllScreens)
                screens.Add(new CustomScreen(s));

            return screens;
        }

        #region Singleton implementation
        private static Recorder instance = null;
        private static readonly object padlock = new object();
        public static Recorder Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                        instance = new Recorder();
                    return instance;
                }
            }
        }
        #endregion
    }
}
