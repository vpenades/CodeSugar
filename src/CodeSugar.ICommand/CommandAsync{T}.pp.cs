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
        public static __INPUTCOMMAND ToInputICommand<T>(this Func<T, Task> execute)
        {
            return new __AsyncCommand<T>(execute);
        }

        public static __INPUTCOMMAND ToInputICommand<T>(this Func<T, CancellationToken, Task> execute, out Action cancelAction)
        {
            var cmd = new __AsyncCommand<T>(execute);
            cancelAction = cmd.Cancel;
            return cmd;
        }

        public static __INPUTCOMMAND ToInputICommand<T>(this Func<T, Task> execute, Predicate<T> canExecute, out Action notifyCanExecuteChanged)
        {
            var cmd = new __AsyncCommand<T>(execute, canExecute);
            notifyCanExecuteChanged = cmd.NotifyCanExecuteChanged;
            return cmd;
        }

        public static __INPUTCOMMAND ToInputICommand<T>(this Func<T, CancellationToken, Task> execute, Predicate<T> canExecute, out Action notifyCanExecuteChanged, out Action cancelAction)
        {
            var cmd = new __AsyncCommand<T>(execute, canExecute);
            notifyCanExecuteChanged= cmd.NotifyCanExecuteChanged;
            cancelAction = cmd.Cancel;
            return cmd;
        }

        #endregion

        #region internals

        internal sealed class __AsyncCommand<T> : __INPUTCOMMAND, INotifyPropertyChanged
        {
            #region lifecycle
            public __AsyncCommand(Func<T, Task> execute)
            {
                if (execute == null) throw new ArgumentNullException(nameof(execute));

                _Execute = execute;
            }

            public __AsyncCommand(Func<T, CancellationToken, Task> cancelableExecute)
            {
                if (cancelableExecute == null) throw new ArgumentNullException(nameof(cancelableExecute));

                _CancelableExecute = cancelableExecute;
            }

            public __AsyncCommand(Func<T, Task> execute, Predicate<T> canExecute)
            {
                if (execute == null) throw new ArgumentNullException(nameof(execute));
                if (canExecute == null) throw new ArgumentNullException(nameof(canExecute));

                _Execute = execute;
                _CanExecute = canExecute;
            }

            public __AsyncCommand(Func<T, CancellationToken, Task> cancelableExecute, Predicate<T> canExecute)
            {
                if (cancelableExecute == null) throw new ArgumentNullException(nameof(cancelableExecute));
                if (canExecute == null) throw new ArgumentNullException(nameof(canExecute));

                _CancelableExecute = cancelableExecute;
                _CanExecute = canExecute;
            }

            #endregion

            #region data

            private readonly Func<T, Task> _Execute;            
            private readonly Func<T, CancellationToken, Task> _CancelableExecute;

            private readonly Predicate<T> _CanExecute;

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

                PropertyChanged?.Invoke(this, __AsyncCommand.ExecutionTaskChangedEventArgs);
                PropertyChanged?.Invoke(this, __AsyncCommand.IsRunningChangedEventArgs);

                bool isAlreadyCompletedOrNull = value?.IsCompleted ?? true;

                if (!(_CancellationTokenSource is null))
                {
                    PropertyChanged?.Invoke(this, __AsyncCommand.CanBeCanceledChangedEventArgs);
                    PropertyChanged?.Invoke(this, __AsyncCommand.IsCancellationRequestedChangedEventArgs);
                }

                if (isAlreadyCompletedOrNull) return _ExecutionTask;

                static async void _monitor(__AsyncCommand<T> self, Task task)
                {
                    await GetAwaitableWithoutEndValidation(task);

                    if (ReferenceEquals(self._ExecutionTask, task))
                    {
                        self.PropertyChanged?.Invoke(self, __AsyncCommand.ExecutionTaskChangedEventArgs);
                        self.PropertyChanged?.Invoke(self, __AsyncCommand.IsRunningChangedEventArgs);

                        if (self._CancellationTokenSource != null)
                        {
                            self.PropertyChanged?.Invoke(self, __AsyncCommand.CanBeCanceledChangedEventArgs);
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
            public bool CanExecute(T parameter)
            {
                bool canExecute = _CanExecute?.Invoke(parameter) != false;

                return canExecute && !(ExecutionTask is { IsCompleted: false });
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool CanExecute(object parameter)
            {                
                if (parameter is null && !(default(T) is null))
                {
                    return false;
                }

                if (!__Command<T>.TryGetCommandArgument(parameter, out T result))
                {
                    __Command<T>.ThrowArgumentExceptionForInvalidCommandArgument(parameter);
                }

                return CanExecute(result);
            }

            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Execute(T parameter)
            {
                Task executionTask = ExecuteAsync(parameter);                
            }
            
            public void Execute(object parameter)
            {
                if (!__Command<T>.TryGetCommandArgument(parameter, out T result))
                {
                    __Command<T>.ThrowArgumentExceptionForInvalidCommandArgument(parameter);
                }

                Execute(result);
            }

            public Task ExecuteAsync(object parameter)
            {
                if (!__Command<T>.TryGetCommandArgument(parameter, out T result))
                {
                    __Command<T>.ThrowArgumentExceptionForInvalidCommandArgument(parameter);
                }

                return ExecuteAsync(result);
            }

            public Task ExecuteAsync(T parameter)
            {
                if (_Execute != null)
                {
                    return SetExecutionTask(_Execute(parameter));
                }

                if (_CancelableExecute != null)
                {
                    _CancellationTokenSource?.Cancel();

                    var cancellationTokenSource = _CancellationTokenSource = new CancellationTokenSource();

                    return SetExecutionTask(_CancelableExecute(parameter, cancellationTokenSource.Token));
                }

                throw new InvalidOperationException();
            }            
            
            public void Cancel()
            {
                if (_CancellationTokenSource is CancellationTokenSource { IsCancellationRequested: false } cancellationTokenSource)
                {
                    cancellationTokenSource.Cancel();

                    PropertyChanged?.Invoke(this, __AsyncCommand.CanBeCanceledChangedEventArgs);
                    PropertyChanged?.Invoke(this, __AsyncCommand.IsCancellationRequestedChangedEventArgs);
                }
            }

            #endregion
        }

        #endregion
    }
}
