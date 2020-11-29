using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using ScreenRecorderLib;

namespace SimpleScreenRecorder.Model
{
    public class ScreenRecorder
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
        /// Status of the recorder.
        /// </summary>
        public RecorderStatus Status { get => _recorder != null ? _recorder.Status : RecorderStatus.Idle; }
        #endregion

        #region Private properties
        /// <summary>
        /// Currently selected screen.
        /// </summary>
        private CustomScreen _currentDisplay { get; set; }
        /// <summary>
        /// Video path to be used.
        /// </summary>
        private string _videoPath { get; set; }
        /// <summary>
        /// Library-provided recorder to start/stop recorder.
        /// </summary>
        private Recorder _recorder { get; set; }
        #endregion

        private ScreenRecorder()
        {
            FrameRate = 60;
        }

        #region Public methods
        /// <summary>
        /// Start recording the current selected screen.
        /// </summary>
        public void Start()
        {
            if (_recorder == null)
            {

                _videoPath = Path.Combine(Path.GetTempPath(), DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".mp4");
                var op = new RecorderOptions { VideoOptions = new VideoOptions { Framerate = FrameRate } };
                _recorder = Recorder.CreateRecorder(op);
                _recorder.OnRecordingComplete += Rec_OnRecordingComplete;
                _recorder.OnRecordingFailed += Rec_OnRecordingFailed;
                _recorder.OnStatusChanged += Rec_OnStatusChanged;
                _recorder.Record(_videoPath);
            }
            else
            {
                _recorder.Resume();
            }

            Started?.Invoke(this, new System.EventArgs());
        }

        /// <summary>
        /// Pause recording.
        /// </summary>
        public void Pause()
        {
            _recorder?.Pause();
            Paused?.Invoke(this, new System.EventArgs());
        }

        /// <summary>
        /// Stop recording.
        /// </summary>
        public void Stop()
        {
            _recorder?.Stop();
            _videoPath = null;
            Stopped?.Invoke(this, new System.EventArgs());
        }

        /// <summary>
        /// Get bitmap of the current selected screen.
        /// </summary>
        public BitmapImage GetBitmapImage() => _currentDisplay.GetBitmapImage();

        /// <summary>
        /// Get bitmap of the current selected screen.
        /// </summary>
        public Bitmap GetBitmap() => _currentDisplay.GetBitmap();

        /// <summary>
        /// Triggered when there's a screen change.
        /// </summary>
        /// <param name="screen">New selected screen.</param>
        public void OnScreenSelect(CustomScreen screen)
        {
            _currentDisplay = screen;

            // Prevent selection when _recorder isn't starting yet.
            if (_recorder != null)
            {
                _recorder.Pause();
                var option = new RecorderOptions { DisplayOptions = new DisplayOptions { MonitorDeviceName = _currentDisplay.Name } };
                _recorder.SetOptions(option);
                _recorder.Resume();
            }
        }

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
        private void Rec_OnStatusChanged(object sender, RecordingStatusEventArgs e)
        {
            Console.WriteLine($"RecorderStatus={e.Status}");
        }

        private void Rec_OnRecordingFailed(object sender, RecordingFailedEventArgs e)
        {
            _recorder?.Dispose();
            _recorder = null;
        }

        private void Rec_OnRecordingComplete(object sender, RecordingCompleteEventArgs e)
        {
            _recorder?.Dispose();
            _recorder = null;
        }
        #endregion

        #region Singleton implementation
        private static ScreenRecorder instance = null;
        private static readonly object padlock = new object();
        public static ScreenRecorder Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                        instance = new ScreenRecorder();
                    return instance;
                }
            }
        }
        #endregion
    }
}
