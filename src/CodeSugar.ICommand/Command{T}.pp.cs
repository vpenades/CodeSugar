using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        public static __INPUTCOMMAND ToInputICommand<T>(this Action<T> execute)
        {
            return new __Command<T>(execute);
        }

        public static __INPUTCOMMAND ToInputICommand<T>(this Action<T> execute, Predicate<T> canExecute, out Action notifyCanExecuteChanged)
        {
            var cmd = new __Command<T>(execute, canExecute);
            notifyCanExecuteChanged = cmd.NotifyCanExecuteChanged;
            return cmd;
        }

        #endregion

        #region internals

        private sealed class __Command<T> : __INPUTCOMMAND
        {
            #region lifecycle
            public __Command(Action<T> execute)
            {
                if (execute == null) throw new ArgumentNullException(nameof(execute));

                _Execute = execute;
            }

            public __Command(Action<T> execute, Predicate<T> canExecute)
            {
                if (execute == null) throw new ArgumentNullException(nameof(execute));
                if (canExecute == null) throw new ArgumentNullException(nameof(canExecute));

                _Execute = execute;
                _CanExecute = canExecute;
            }

            #endregion

            #region data

            private readonly Action<T> _Execute;

            private readonly Predicate<T> _CanExecute;

            public event EventHandler CanExecuteChanged;

            #endregion

            #region API

            public void NotifyCanExecuteChanged()
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool CanExecute(T parameter)
            {
                return _CanExecute?.Invoke(parameter) != false;
            }
            
            public bool CanExecute(object parameter)
            {
                // Special case a null value for a value type argument type.
                // This ensures that no exceptions are thrown during initialization.
                if (!(parameter is null && default(T) is null))
                {
                    return false;
                }

                if (!TryGetCommandArgument(parameter, out T result))
                {
                    ThrowArgumentExceptionForInvalidCommandArgument(parameter);
                }

                return CanExecute(result);
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Execute(T parameter)
            {
                _Execute(parameter);
            }
            
            public void Execute(object parameter)
            {
                if (!TryGetCommandArgument(parameter, out T result))
                {
                    ThrowArgumentExceptionForInvalidCommandArgument(parameter);
                }

                Execute(result);
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static bool TryGetCommandArgument(object parameter, out T result)
            {
                // If the argument is null and the default value of T is also null, then the
                // argument is valid. T might be a reference type or a nullable value type.
                if (parameter is null && default(T) is null)
                {
                    result = default;

                    return true;
                }

                // Check if the argument is a T value, so either an instance of a type or a derived
                // type of T is a reference type, an interface implementation if T is an interface,
                // or a boxed value type in case T was a value type.
                if (parameter is T argument)
                {
                    result = argument;

                    return true;
                }

                result = default;

                return false;
            }
            
            [DoesNotReturn]
            internal static void ThrowArgumentExceptionForInvalidCommandArgument(object parameter)
            {
                throw new ArgumentException($"Parameter \"{nameof(parameter)}\" (object) cannot be of type {parameter?.GetType()}, as the command type requires an argument of type {typeof(T)}.", nameof(parameter));
            }

            #endregion
        }

        #endregion
    }
}
