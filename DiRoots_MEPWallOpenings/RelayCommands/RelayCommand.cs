using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices.ObjectiveC;
using System.Windows.Input;

namespace DiRoots_MEPWallOpenings.RelayCommands
{
    public class RelayCommand(Action<object> exec, Predicate<object> canExec) : ICommand
    {

        #region Properties
        private Action<object> Exec { get; set; } = exec;
        private Predicate<object> CanExec { get; set; } = canExec;
        #endregion

        #region Constructor
        public RelayCommand(Action<object> exec) : this(exec, p => true) { }
        #endregion

        #region Interface methods
        public bool CanExecute(object? parameter) => CanExec(parameter);
        public void Execute(object? parameter) => Exec(parameter);
        #endregion

        #region Event 
        public event EventHandler? CanExecuteChanged;
        public void RaiseCanExecuteChanged()
        {
            // Notify all subscribers that CanExecute might have changed
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}