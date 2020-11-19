using System;
using System.Windows.Threading;

namespace SimpleScreenRecorder.Model
{
    public class DispatcherService
    {
        private Dispatcher _dispatcher { get; set; }

        public void Init() => _dispatcher = Dispatcher.CurrentDispatcher;

        public void Invoke(Action action) => _dispatcher?.Invoke(action);

        public void BeginInvoke(Action action) => _dispatcher?.BeginInvoke(action);

        #region Singleton implementation
        private static DispatcherService instance = null;
        private static readonly object padlock = new object();
        public static DispatcherService Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                        instance = new DispatcherService();
                    return instance;
                }
            }
        }
        #endregion
    }
}
