using SimpleScreenRecorder.Model;
using System.Windows;

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

            DispatcherService.Instance.Init();
        }
    }
}
