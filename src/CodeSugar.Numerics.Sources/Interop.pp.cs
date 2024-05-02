// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;

using VECTOR2 = System.Numerics.Vector2;
using VECTOR3 = System.Numerics.Vector3;
using VECTOR4 = System.Numerics.Vector4;


#nullable disable

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System.Numerics
#else
namespace $rootnamespace$
#endif
{    
    internal static partial class CodeSugarForNumerics
    {
        #if NET6_0_OR_GREATER
        private const DynamicallyAccessedMemberTypes VECTORMEMBERTYPES = DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields;
        #endif

        [System.Diagnostics.DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static T ConvertTo
        <
            #if NET6_0_OR_GREATER
            [DynamicallyAccessedMembers(VECTORMEMBERTYPES)]
            #endif
        T>(this VECTOR2 src) where T:unmanaged { return __Vector2Converter<T>.Convert(src); }

        [System.Diagnostics.DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static VECTOR2 ConvertToVector2
        <
            #if NET6_0_OR_GREATER
            [DynamicallyAccessedMembers(VECTORMEMBERTYPES)]
            #endif
        T>(this T src) where T:unmanaged { return __Vector2Converter<T>.Convert(src); }

        [System.Diagnostics.DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static T ConvertTo
        <
            #if NET6_0_OR_GREATER
            [DynamicallyAccessedMembers(VECTORMEMBERTYPES)]
            #endif
        T>(this VECTOR3 src) where T : unmanaged { return __Vector3Converter<T>.Convert(src); }

        [System.Diagnostics.DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static VECTOR3 ConvertToVector3
        <
            #if NET6_0_OR_GREATER
            [DynamicallyAccessedMembers(VECTORMEMBERTYPES)]
            #endif
        T>(this T src) where T : unmanaged { return __Vector3Converter<T>.Convert(src); }

        [System.Diagnostics.DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static T ConvertTo
        <
            #if NET6_0_OR_GREATER
            [DynamicallyAccessedMembers(VECTORMEMBERTYPES)]
            #endif
        T>(this VECTOR4 src) where T : unmanaged { return __Vector4Converter<T>.Convert(src); }

        [System.Diagnostics.DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static VECTOR4 ConvertToVector4
        <
            #if NET6_0_OR_GREATER
            [DynamicallyAccessedMembers(VECTORMEMBERTYPES)]
            #endif
        T>(this T src) where T : unmanaged { return __Vector4Converter<T>.Convert(src); }


        [System.Diagnostics.DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static T UnsafeConvertToDouble<T>(this VECTOR2 src) where T : unmanaged
        {
            var vv = new _Vector2Double(src);
            return _UnsafeAs<_Vector2Double, T>(ref vv);
        }




        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private readonly struct _Vector2Double
        {
            public _Vector2Double(VECTOR2 v) { X=v.X; Y=v.Y; }

            public readonly Double X;
            public readonly Double Y;
        }


        [System.Diagnostics.DebuggerStepThrough]
        private static class __Vector2Converter
        <
            #if NET6_0_OR_GREATER
            [DynamicallyAccessedMembers(VECTORMEMBERTYPES)]
            #endif
        T> where T : unmanaged
        {
            private const string _V2_XNA_VECTOR2 = "Microsoft.Xna.Framework.Vector2";
            private const string _V2_SIXLABORS_SIZEF = "SixLabors.ImageSharp.SizeF";
            private const string _V2_SIXLABORS_POINTF = "SixLabors.ImageSharp.PointF";            

            static __Vector2Converter()
            {
                if (typeof(T).FullName == _V2_XNA_VECTOR2) return;
                if (typeof(T).FullName == _V2_SIXLABORS_SIZEF) return;
                if (typeof(T).FullName == _V2_SIXLABORS_POINTF) return;
                __Reflection<T>.CheckFloatSequence(2);
            }

            [MethodImpl(AGRESSIVE)]
            public static VECTOR2 Convert(T vector)
            {
                return _UnsafeAs<T, VECTOR2>(ref vector);
            }

            [MethodImpl(AGRESSIVE)]
            public static T Convert(VECTOR2 vector)
            {
                return _UnsafeAs<VECTOR2, T>(ref vector);
            }
        }

        [System.Diagnostics.DebuggerStepThrough]
        private static class __Vector3Converter
        <
            #if NET6_0_OR_GREATER
            [DynamicallyAccessedMembers(VECTORMEMBERTYPES)]
            #endif
        T> where T : unmanaged
        {
            private const string _V3_XNA_VECTOR3 = "Microsoft.Xna.Framework.Vector3";
            
            static __Vector3Converter()
            {
                if (typeof(T).FullName == _V3_XNA_VECTOR3) return;
                __Reflection<T>.CheckFloatSequence(3);
            }

            [MethodImpl(AGRESSIVE)]
            public static VECTOR3 Convert(T vector)
            {
                return _UnsafeAs<T, VECTOR3>(ref vector);
            }

            [MethodImpl(AGRESSIVE)]
            public static T Convert(VECTOR3 vector)
            {
                return _UnsafeAs<VECTOR3, T>(ref vector);
            }
        }

        [System.Diagnostics.DebuggerStepThrough]
        private static class __Vector4Converter
        <
            #if NET6_0_OR_GREATER
            [DynamicallyAccessedMembers(VECTORMEMBERTYPES)]
            #endif
        T> where T : unmanaged
        {
            private const string _V4_XNA_VECTOR4 = "Microsoft.Xna.Framework.Vector4";
            
            static __Vector4Converter()
            {
                if (typeof(T).FullName == _V4_XNA_VECTOR4) return;
                __Reflection<T>.CheckFloatSequence(4);
            }

            [MethodImpl(AGRESSIVE)]
            public static VECTOR4 Convert(T vector)
            {
                return _UnsafeAs<T, VECTOR4>(ref vector);
            }

            [MethodImpl(AGRESSIVE)]
            public static T Convert(VECTOR4 vector)
            {
                return _UnsafeAs<VECTOR4, T>(ref vector);
            }
        }

        [System.Diagnostics.DebuggerStepThrough]
        private static class __Reflection
        <
            #if NET6_0_OR_GREATER
            [DynamicallyAccessedMembers(VECTORMEMBERTYPES)]
            #endif        
        T> where T:unmanaged
        {
            public static void CheckFloatSequence(int count)
            {
                if (GetByteSize() != count * 4) throw new InvalidOperationException($"Must have a length of {count*4} bytes");

                var types = GetFieldTypes();
                if (types.Length != count || types.Any(t => t != typeof(float))) throw new InvalidOperationException($"Expected {count} floats");
            }

            public static int GetByteSize()
            {
                #if NET6_0_OR_GREATER
                return System.Runtime.CompilerServices.Unsafe.SizeOf<T>();
                #else
                throw new NotSupportedException();
                #endif
            }

            public static Type[] GetFieldTypes()
            {
                var t = typeof(T);

                var flags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;

                var elements = t.GetFields(flags).Select(item => item.FieldType);

                // we do not check properties because there can be many "fake" properties

                return elements.ToArray();
            }
        }
    }
}
