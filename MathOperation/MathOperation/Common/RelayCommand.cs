using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace MathOperation.Common
{
    public class RelayCommand : ICommand
    {
        private Action<object> _Execute;
        private Predicate<object> _CanExecute;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action<object> execute) : this(execute, null) { }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _Execute = execute;
            _CanExecute = canExecute;
            RaiseOnCanExecuteChanged();
        }

        public void RaiseOnCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            return _CanExecute != null ? _CanExecute(parameter) : true;
        }

        public void Execute(object parameter)
        {
            if (_Execute != null && CanExecute(parameter))
                _Execute(parameter);
        }
    }
}
