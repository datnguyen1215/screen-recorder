using Accord.Video.FFMPEG;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace SimpleScreenRecorder.Model
{
    public class Recorder
    {
        #region Public events
        public event EventHandler Paused;
        public event EventHandler Started;
        public event EventHandler Stopped;
        #endregion

        #region Public properties
        /// <summary>
        /// Framerate to record. Default is 30.
        /// </summary>
        public int FrameRate { get; set; }
        /// <summary>
        /// Indication whether Recorder has started.
        /// </summary>
        public bool IsStarted { get; private set; }
        /// <summary>
        /// Indication whether it's paused.
        /// </summary>
        public bool IsPaused { get; private set; }
        #endregion

        #region Private properties
        /// <summary>
        /// Currently selected screen.
        /// </summary>
        private CustomScreen _currentScreen { get; set; }
        /// <summary>
        /// Video path to be used.
        /// </summary>
        private string _videoPath { get; set; }
        /// <summary>
        /// Used for writing video.
        /// </summary>
        private VideoFileWriter _videoWriter { get; }
        /// <summary>
        /// Used to make sure framerate is correctly recorded.
        /// </summary>
        private Timer _timer { get; }
        private Stopwatch _stopwatch { get; }
        #endregion

        private Recorder()
        {
            FrameRate = 30;
            _timer = new Timer { Interval = 1000 / FrameRate };
            _timer.Tick += Timer_Tick;
            _videoWriter = new VideoFileWriter();
            _stopwatch = new Stopwatch();
        }

        #region Public methods
        /// <summary>
        /// Start recording the current selected screen.
        /// </summary>
        public void Start()
        {
            if (!IsStarted)
            {
                IsStarted = true;
                _videoPath = Path.Combine(Path.GetTempPath(), DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".mp4");
                Console.WriteLine($"path={_videoPath}");
                _videoWriter.Open(_videoPath, _currentScreen.Width, _currentScreen.Height, FrameRate, VideoCodec.H264, 0);
            }

            _stopwatch.Start();
            _timer.Start();
            IsPaused = false;
            Started?.Invoke(this, new System.EventArgs());
        }

        /// <summary>
        /// Pause recording.
        /// </summary>
        public void Pause()
        {
            IsPaused = true;
            _timer.Stop();
            _stopwatch.Stop();
            Paused?.Invoke(this, new System.EventArgs());
        }

        /// <summary>
        /// Stop recording.
        /// </summary>
        public void Stop()
        {
            _timer.Stop();
            _stopwatch.Stop();
            _stopwatch.Reset();
            _videoWriter.Close();
            _videoPath = null;
            IsStarted = IsPaused = false;
            Stopped?.Invoke(this, new System.EventArgs());
        }

        /// <summary>
        /// Get bitmap of the current selected screen.
        /// </summary>
        public BitmapImage GetBitmapImage() => _currentScreen.GetBitmapImage();

        /// <summary>
        /// Get bitmap of the current selected screen.
        /// </summary>
        public Bitmap GetBitmap() => _currentScreen.GetBitmap();

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
        #endregion

        #region Private methods
        /// <summary>
        /// Triggered when timer has ticked (1000 / FrameRate).
        ///
        /// This function helps making a video with the correct framerate.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, System.EventArgs e)
        {
            using (var bitmap = GetBitmap())
                _videoWriter.WriteVideoFrame(bitmap, _stopwatch.Elapsed);
        }
        #endregion

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
