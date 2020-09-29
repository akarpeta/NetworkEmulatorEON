using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CableCloud
{
    public class DelegateCommand : ICommand
    {
        public Predicate<object> CanExecuteDelegate { get; set; }

        public Action<object> ExecuteDelegate { get; set; }

        protected bool isExecuting;

        public bool IsExecuting
        {
            get
            {
                return isExecuting;
            }
            private set
            {
                isExecuting = value;
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public DelegateCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            isExecuting = false;
            this.CanExecuteDelegate = canExecute;
            this.ExecuteDelegate = execute ?? throw new ArgumentNullException("Execute");
        }

        public void OnCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        public virtual bool CanExecute(object parameter)
        {
            return (CanExecuteDelegate != null) ? (!IsExecuting && CanExecuteDelegate(parameter)) : !IsExecuting;
        }

        public virtual void Execute(object parameter)
        {
            try
            {
                IsExecuting = true;
                OnExecute(parameter);
            }
            finally
            {
                IsExecuting = false;
                OnCanExecuteChanged();
            }
        }
        protected void OnExecute(object parameter)
        {
            ExecuteDelegate(parameter);
        }
    }
}
