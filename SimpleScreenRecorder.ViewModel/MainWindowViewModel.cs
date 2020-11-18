using SimpleScreenRecorder.Model;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows;

namespace SimpleScreenRecorder.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        Dispatcher _dispatcher { get; }
        public ICommand SelectDisplayCommand { get; }
        public ICommand ToggleRecordCommand { get; }

        private Timer _screenCaptureTimer { get; }
        private Recorder _recorder { get; }

        public MainWindowViewModel()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;

            _recorder = new Recorder();
            Screens = _recorder.GetScreens();

            SelectDisplayCommand = new RelayCommand(o => OnDisplaySelected(o));
            ToggleRecordCommand = new RelayCommand(o => OnToggleRecord(o));

            ToggleRecordButtonText = "Start";

            _screenCaptureTimer = new Timer { Interval = 1000 };
            _screenCaptureTimer.Elapsed += ScreenCaptureTimer_Elapsed;
        }

        private void ScreenCaptureTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _dispatcher.Invoke(() =>
            {
                ScreenBitmap = _recorder.GetBitmapImage();
            });
        }

        private void OnDisplaySelected(object args)
        {
            _screenCaptureTimer.Start();
            _recorder.OnScreenSelect(SelectedScreen);
        }

        private void OnToggleRecord(object args)
        {
            Console.WriteLine("Toggle record");
        }

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

        private string _toggleRecordButtonText { get; set; }
        public string ToggleRecordButtonText
        {
            get => _toggleRecordButtonText;
            set
            {
                _toggleRecordButtonText = value;
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
    }
}
