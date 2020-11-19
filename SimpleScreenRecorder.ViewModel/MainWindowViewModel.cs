using SimpleScreenRecorder.Model;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace SimpleScreenRecorder.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        #region ICommand
        public ICommand SelectDisplayCommand { get; }
        public ICommand StartPauseCommand { get; }
        public ICommand StopCommand { get; }
        #endregion

        private Timer _screenCaptureTimer { get; }

        public MainWindowViewModel()
        {
            Screens = Recorder.Instance.GetScreens();
            _screenCaptureTimer = new Timer();
            _screenCaptureTimer.Elapsed += ScreenCaptureTimer_Elapsed;

            SelectDisplayCommand = new RelayCommand(o => OnDisplaySelected(o));
            StartPauseCommand = new RelayCommand(o => OnStartPauseButtonClicked(o));
            StopCommand = new RelayCommand(o => OnStopButtonClicked(o));

            IsStopButtonEnabled = false;

            // Auto-select the first screen.
            SelectedScreen = Screens[0];
            OnDisplaySelected(this);
        }


        /// <summary>
        /// Triggered when screen capture timer elapsed.
        /// </summary>
        private void ScreenCaptureTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DispatcherService.Instance.BeginInvoke(() =>
            {
                ScreenBitmap = Recorder.Instance.GetBitmapImage();
            });
        }

        /// <summary>
        /// Triggered when new display is selected.
        /// </summary>
        private void OnDisplaySelected(object args)
        {
            _screenCaptureTimer.Start();
            Recorder.Instance.OnScreenSelect(SelectedScreen);
        }

        /// <summary>
        /// Triggered when Start/Pause button is clicked.
        /// </summary>
        private void OnStartPauseButtonClicked(object args)
        {
            _screenCaptureTimer.Stop();
            Recorder.Instance.Start();
            IsStopButtonEnabled = true;
        }

        /// <summary>
        /// Triggered when Stop button is clicked.
        /// </summary>
        private void OnStopButtonClicked(object o)
        {
            Recorder.Instance.Stop();
            ScreenBitmap = null;
            IsStopButtonEnabled = false;
        }

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
                StartPauseButtonText = value ? "Pause" : "Start";
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
