using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace VidyoConnector.Commands
{
    /// <summary>
    /// Wrapper class for using Commands accoding MVVM practice
    /// </summary>
    public class BindableCommand : ICommand
    {
        private bool _isCanExecute = true;
        public bool IsCanExecute {
            get { return _isCanExecute; }
            set
            {
                if (_isCanExecute == value) return;
                SetProperty(ref _isCanExecute, value);
                if (CanExecuteChanged != null) CanExecuteChanged.Invoke(this, null);
            }
        }

        public event Action<object> ExecuteAction;
        public event EventHandler CanExecuteChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public bool CanExecute(object parameter) { return IsCanExecute; }

        public void Execute(object parameter)
        {
            if (ExecuteAction != null) ExecuteAction.Invoke(parameter);
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}