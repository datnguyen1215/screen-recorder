using System;
using System.Windows.Input;

namespace SimpleScreenRecorder.ViewModel
{
    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        readonly Action<object> _action;
        readonly Func<bool> _canExecute;

        public RelayCommand(Action<object> action) : this(action, () => true) { }

        public RelayCommand(Action<object> action, Func<bool> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public void Execute(object parameter)
        {
            if (CanExecute(null))
            {
                _action(parameter ?? "Hello World");
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion ICommand Members
    }
}
