using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

#nullable disable

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
        
        
        [EditorBrowsable(EditorBrowsableState.Never)]        
        private static __TaskAwaitableWithoutEndValidation GetAwaitableWithoutEndValidation(Task task)
        {
            return new __TaskAwaitableWithoutEndValidation(task);
        }

        
        [EditorBrowsable(EditorBrowsableState.Never)]        
        private readonly struct __TaskAwaitableWithoutEndValidation
        {
            #region lifecycle

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public __TaskAwaitableWithoutEndValidation(Task task)
            {
                this._Task = task;
            }

            #endregion

            #region data

            private readonly Task _Task;

            #endregion

            #region API


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Awaiter GetAwaiter()
            {
                return new Awaiter(this._Task);
            }

            #endregion

            #region nested types

            public readonly struct Awaiter : ICriticalNotifyCompletion
            {
                #region lifecycle
                public Awaiter(Task task)
                {
                    this._Awaiter = task.GetAwaiter();
                }


                #endregion

                #region data

                private readonly TaskAwaiter _Awaiter;

                #endregion

                #region properties

                public bool IsCompleted
                {
                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
                    get => this._Awaiter.IsCompleted;
                }

                #endregion

                #region API


                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void GetResult() { }

                
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void OnCompleted(Action continuation)
                {
                    this._Awaiter.OnCompleted(continuation);
                }

                
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void UnsafeOnCompleted(Action continuation)
                {
                    this._Awaiter.UnsafeOnCompleted(continuation);
                }

                #endregion
            }

            #endregion
        }
    }
    
}
