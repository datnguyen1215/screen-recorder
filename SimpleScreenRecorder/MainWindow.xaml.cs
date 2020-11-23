using SimpleScreenRecorder.Model;
using System;
using System.Windows;
using System.Windows.Interop;

namespace SimpleScreenRecorder.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            IntPtr hWnd = new WindowInteropHelper(this).EnsureHandle();
            HotkeyManager.Instance.Init(hWnd);
            DispatcherService.Instance.Init();
        }
    }
}
