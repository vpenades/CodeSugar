using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

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
        public static __INPUTCOMMAND ToInputICommand(this Func<Task> execute)
        {
            return new __AsyncCommand(execute);
        }

        public static __INPUTCOMMAND ToInputICommand(this Func<CancellationToken, Task> execute, out Action cancelAction)
        {
            var cmd = new __AsyncCommand(execute);
            cancelAction = cmd.Cancel;
            return cmd;
        }

        public static __INPUTCOMMAND ToInputICommand(this Func<Task> execute, Func<bool> canExecute, out Action notifyCanExecuteChanged)
        {
            var cmd = new __AsyncCommand(execute, canExecute);
            notifyCanExecuteChanged = cmd.NotifyCanExecuteChanged;
            return cmd;
        }

        public static __INPUTCOMMAND ToInputICommand(this Func<CancellationToken, Task> execute, Func<bool> canExecute, out Action notifyCanExecuteChanged, out Action cancelAction)
        {
            var cmd = new __AsyncCommand(execute, canExecute);
            notifyCanExecuteChanged = cmd.NotifyCanExecuteChanged;
            cancelAction = cmd.Cancel;
            return cmd;
        }

        #endregion

        #region internals

        internal sealed class __AsyncCommand : __INPUTCOMMAND, INotifyPropertyChanged
        {
            #region cached values

            internal static readonly PropertyChangedEventArgs ExecutionTaskChangedEventArgs = new PropertyChangedEventArgs(nameof(ExecutionTask));

            internal static readonly PropertyChangedEventArgs CanBeCanceledChangedEventArgs = new PropertyChangedEventArgs(nameof(CanBeCanceled));

            internal static readonly PropertyChangedEventArgs IsCancellationRequestedChangedEventArgs = new PropertyChangedEventArgs(nameof(IsCancellationRequested));

            internal static readonly PropertyChangedEventArgs IsRunningChangedEventArgs = new PropertyChangedEventArgs(nameof(IsRunning));

            #endregion

            #region lifecycle

            public __AsyncCommand(Func<Task> execute)
            {
                if (execute == null) throw new ArgumentNullException(nameof(execute));

                _Execute = execute;
            }

            public __AsyncCommand(Func<CancellationToken, Task> cancelableExecute)
            {
                if (cancelableExecute == null) throw new ArgumentNullException(nameof(cancelableExecute));

                _CancelableExecute = cancelableExecute;
            }

            public __AsyncCommand(Func<Task> execute, Func<bool> canExecute)
            {
                if (execute == null) throw new ArgumentNullException(nameof(execute));
                if (canExecute == null) throw new ArgumentNullException(nameof(canExecute));

                _Execute = execute;
                _CanExecute = canExecute;
            }

            public __AsyncCommand(Func<CancellationToken, Task> cancelableExecute, Func<bool> canExecute)
            {
                if (cancelableExecute == null) throw new ArgumentNullException(nameof(cancelableExecute));
                if (canExecute == null) throw new ArgumentNullException(nameof(canExecute));

                _CancelableExecute = cancelableExecute;
                _CanExecute = canExecute;
            }

            #endregion

            #region data

            private readonly Func<Task> _Execute;
            private readonly Func<CancellationToken, Task> _CancelableExecute;

            private readonly Func<bool> _CanExecute;

            private CancellationTokenSource _CancellationTokenSource;
            private Task _ExecutionTask;

            #endregion

            #region properties

            public event PropertyChangedEventHandler PropertyChanged;

            public event EventHandler CanExecuteChanged;

            public bool CanBeCanceled => IsRunning && _CancellationTokenSource is { IsCancellationRequested: false };
            public bool IsCancellationRequested => _CancellationTokenSource is { IsCancellationRequested: true };
            public bool IsRunning => ExecutionTask is { IsCompleted: false };
            public Task ExecutionTask => _ExecutionTask;

            #endregion

            #region API

            private Task SetExecutionTask(Task value)
            {
                if (ReferenceEquals(_ExecutionTask, value)) return _ExecutionTask;

                _ExecutionTask = value;

                PropertyChanged?.Invoke(this, ExecutionTaskChangedEventArgs);
                PropertyChanged?.Invoke(this, IsRunningChangedEventArgs);

                bool isAlreadyCompletedOrNull = value?.IsCompleted ?? true;

                if (!(_CancellationTokenSource is null))
                {
                    PropertyChanged?.Invoke(this, CanBeCanceledChangedEventArgs);
                    PropertyChanged?.Invoke(this, IsCancellationRequestedChangedEventArgs);
                }

                if (isAlreadyCompletedOrNull) return _ExecutionTask;

                static async void _monitor(__AsyncCommand self, Task task)
                {
                    await GetAwaitableWithoutEndValidation(task);

                    if (ReferenceEquals(self._ExecutionTask, task))
                    {
                        self.PropertyChanged?.Invoke(self, ExecutionTaskChangedEventArgs);
                        self.PropertyChanged?.Invoke(self, IsRunningChangedEventArgs);

                        if (self._CancellationTokenSource != null)
                        {
                            self.PropertyChanged?.Invoke(self, CanBeCanceledChangedEventArgs);
                        }
                    }
                }

                _monitor(this, value!);

                return _ExecutionTask;
            }
            
            public void NotifyCanExecuteChanged()
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }

            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool CanExecute(object parameter)
            {
                bool canExecute = _CanExecute?.Invoke() != false;

                return canExecute && !(ExecutionTask is { IsCompleted: false });
            }

            
            public void Execute(object parameter)
            {
                var executionTask = ExecuteAsync(parameter);
            }

            
            public Task ExecuteAsync(object parameter)
            {
                if (_Execute != null)
                {
                    return SetExecutionTask(_Execute());
                }

                if (_CancelableExecute != null)
                {                    
                    _CancellationTokenSource?.Cancel();

                    var cancellationTokenSource = _CancellationTokenSource = new CancellationTokenSource();
                    
                    return SetExecutionTask(_CancelableExecute(cancellationTokenSource.Token));
                }

                throw new InvalidOperationException();
            }
            
            public void Cancel()
            {
                if (_CancellationTokenSource is CancellationTokenSource { IsCancellationRequested: false } cancellationTokenSource)
                {
                    cancellationTokenSource.Cancel();

                    PropertyChanged?.Invoke(this, CanBeCanceledChangedEventArgs);
                    PropertyChanged?.Invoke(this, IsCancellationRequestedChangedEventArgs);
                }
            }

            #endregion
        }

        #endregion
    }
}
