// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Diagnostics;
using System.Runtime.CompilerServices;

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
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Deconstruct(this Vector2 v, out float x, out float y)
        {
            x = v.X;
            y = v.Y;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Deconstruct(this Vector3 v, out Vector2 xy, out float z)
        {
            xy = new Vector2(v.X, v.Y);
            z = v.Z;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Deconstruct(this Vector3 v, out float x, out float y, out float z)
        {
            x = v.X;
            y = v.Y;
            z = v.Z;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Deconstruct(this Vector4 v, out Vector3 xyz, out float w)
        {
            xyz = new Vector3(v.X, v.Y, v.Z);
            w = v.W;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Deconstruct(this Vector4 v, out float x, out float y, out float z, out float w)
        {
            x = v.X;
            y = v.Y;
            z = v.Z;
            w = v.W;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Deconstruct(this Plane p, out Vector3 normal, out float d)
        {
            normal = p.Normal;
            d = p.D;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Deconstruct(this Plane p, out float x, out float y, out float z, out float d)
        {
            x = p.Normal.X;
            y = p.Normal.Y;
            z = p.Normal.Z;
            d = p.D;
        }        

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Deconstruct(this Matrix3x2 m, out Vector3 column1, out Vector3 column2)
        {
            column1 = new Vector3(m.M11,m.M21,m.M31);
            column2 = new Vector3(m.M12,m.M22,m.M32);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Deconstruct(this Matrix3x2 m, out Vector2 row1, out Vector2 row2, out Vector2 row3)
        {
            row1 = new Vector2(m.M11,m.M12);
            row2 = new Vector2(m.M21,m.M22);
            row3 = new Vector2(m.M31,m.M32);            
        }
    }
}
