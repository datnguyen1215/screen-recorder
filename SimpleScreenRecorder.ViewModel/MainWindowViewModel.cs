using SimpleScreenRecorder.Model;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Forms;
using SimpleScreenRecorder.Model.Enums;
using ScreenRecorderLib;

namespace SimpleScreenRecorder.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        #region ICommand
        public ICommand SelectDisplayCommand { get; }
        public ICommand StartPauseCommand { get; }
        public ICommand StopCommand { get; }
        #endregion

        private System.Timers.Timer _screenCaptureTimer { get; }

        public MainWindowViewModel()
        {
            Screens = ScreenRecorder.Instance.GetScreens();
            ScreenRecorder.Instance.Paused += Recorder_Paused;
            ScreenRecorder.Instance.Started += Recorder_Started;
            ScreenRecorder.Instance.Stopped += Recorder_Stopped;
            HotkeyManager.Instance.HotkeyPressed += HotkeyManager_HotkeyPressed;
            _screenCaptureTimer = new System.Timers.Timer { Interval = 1000 };
            _screenCaptureTimer.Elapsed += ScreenCaptureTimer_Elapsed;

            SelectDisplayCommand = new RelayCommand(o => OnDisplaySelected(o));
            StartPauseCommand = new RelayCommand(o => OnStartPauseButtonClicked(o));
            StopCommand = new RelayCommand(o => OnStopButtonClicked(o));

            IsStopButtonEnabled = false;
            StartPauseButtonText = "Start";

            // Auto-select the first screen.
            SelectedScreen = Screens[0];
            OnDisplaySelected(this);
        }

        /// <summary>
        /// Triggered when a registered hotkey is pressed.
        ///
        /// Please check HotkeyManager for registered hotkeys.
        /// </summary>
        private void HotkeyManager_HotkeyPressed(object sender, Model.EventArgs.HotkeyEventArgs e)
        {
            switch (e.Hotkey.Key)
            {
                case Keys.F9:
                    // Skip if there are modifier keys.
                    if (e.Hotkey.Modifiers != KeyModifiers.None)
                        break;
                    StartPauseRecorder();
                    break;

                case Keys.F10:
                    // Skip if there are modifier keys.
                    if (e.Hotkey.Modifiers != KeyModifiers.None)
                        break;

                    StopRecorder();
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Triggered when screen capture timer elapsed.
        /// </summary>
        private void ScreenCaptureTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DispatcherService.Instance.BeginInvoke(() =>
            {
                ScreenBitmap = ScreenRecorder.Instance.GetBitmapImage();
            });
        }

        /// <summary>
        /// Triggered when new display is selected.
        /// </summary>
        private void OnDisplaySelected(object args)
        {
            _screenCaptureTimer.Start();
            ScreenRecorder.Instance.OnScreenSelect(SelectedScreen);
            ScreenBitmap = ScreenRecorder.Instance.GetBitmapImage();
        }

        /// <summary>
        /// Start/pause recorder.
        /// </summary>
        private void StartPauseRecorder()
        {
            _screenCaptureTimer.Stop();
            IsStopButtonEnabled = true;

            if (ScreenRecorder.Instance.Status == RecorderStatus.Recording)
                ScreenRecorder.Instance.Pause();
            else
                ScreenRecorder.Instance.Start();
        }

        /// <summary>
        /// Stop recorder.
        /// </summary>
        private void StopRecorder()
        {
            if (ScreenRecorder.Instance.Status != RecorderStatus.Idle)
            {
                ScreenRecorder.Instance.Stop();
                ScreenBitmap = null;
                IsStopButtonEnabled = false;
            }
        }

        /// <summary>
        /// Triggered when Start/Pause button is clicked.
        /// </summary>
        private void OnStartPauseButtonClicked(object args) => StartPauseRecorder();

        /// <summary>
        /// Triggered when Stop button is clicked.
        /// </summary>
        private void OnStopButtonClicked(object o) => StopRecorder();

        private void Recorder_Paused(object sender, EventArgs e) => StartPauseButtonText = "Resume";
        private void Recorder_Stopped(object sender, EventArgs e) => StartPauseButtonText = "Start";
        private void Recorder_Started(object sender, EventArgs e) => StartPauseButtonText = "Pause";

        #region UI Properties
        private BitmapImage _screenBitmap { get; set; }
        public BitmapImage ScreenBitmap
        {
            get => _screenBitmap;
            set
            {
                _screenBitmap = value;
                OnPropertyChanged();
            }
        }

        private bool _isStopButtonEnabled { get; set; }
        public bool IsStopButtonEnabled
        {
            get => _isStopButtonEnabled;
            set
            {
                _isStopButtonEnabled = value;
                OnPropertyChanged();
            }
        }

        private string _startPauseButtonText { get; set; }
        public string StartPauseButtonText
        {
            get => _startPauseButtonText;
            set
            {
                _startPauseButtonText = value;
                OnPropertyChanged();
            }
        }

        private List<CustomScreen> _screens { get; set; }
        public List<CustomScreen> Screens
        {
            get => _screens;
            set
            {
                _screens = value;
                OnPropertyChanged();
            }
        }

        private CustomScreen _selectedScreen { get; set; }
        public CustomScreen SelectedScreen
        {
            get => _selectedScreen;
            set
            {
                _selectedScreen = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }
}
