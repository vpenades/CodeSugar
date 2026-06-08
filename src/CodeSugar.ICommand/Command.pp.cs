using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

#nullable disable

using __INPUTCOMMAND = System.Windows.Input.ICommand;

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USEWINDOWSINPUTNAMESPACE
namespace System.Windows.Input
#else
namespace $rootnamespace$
#endif
{
    internal static partial class CodeSugarForICommand
    {
        #region API
        public static __INPUTCOMMAND ToInputICommand(this Action execute)
        {
            return new __Command(execute);
        }

        public static __INPUTCOMMAND ToInputICommand(this Action execute, Func<bool> canExecute, out Action notifyCanExecuteChanged)
        {
            var cmd = new __Command(execute, canExecute);
            notifyCanExecuteChanged = cmd.NotifyCanExecuteChanged;
            return cmd;
        }

        #endregion

        #region internals

        private sealed class __Command : __INPUTCOMMAND
        {
            #region lifecycle
            public __Command(Action execute)
            {
                if (execute == null) throw new ArgumentNullException(nameof(execute));

                _Execute = execute;
            }

            public __Command(Action execute, Func<bool> canExecute)
            {
                if (execute == null) throw new ArgumentNullException(nameof(execute));
                if (canExecute == null) throw new ArgumentNullException(nameof(canExecute));

                _Execute = execute;
                _CanExecute = canExecute;
            }

            #endregion

            #region data

            private readonly Action _Execute;

            private readonly Func<bool> _CanExecute;

            public event EventHandler CanExecuteChanged;

            #endregion

            #region API

            public void NotifyCanExecuteChanged()
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool CanExecute(object parameter)
            {
                return _CanExecute?.Invoke() != false;
            }

            public void Execute(object parameter)
            {
                _Execute();
            }

            #endregion
        }

        #endregion
    }
}
