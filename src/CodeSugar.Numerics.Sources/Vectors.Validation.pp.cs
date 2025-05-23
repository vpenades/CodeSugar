﻿// Copyright (c) CodeSugar 2024 Vicente Penades

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
        [Conditional("DEBUG")]
        private static void _AssertFinite(in Vector2 v)
        {
            System.Diagnostics.Debug.Assert(v.IsFinite(), "v is not finite");
        }

        [Conditional("DEBUG")]
        private static void _AssertFinite(in Vector3 v)
        {
            System.Diagnostics.Debug.Assert(v.IsFinite(), "v is not finite");
        }

        [Conditional("DEBUG")]
        private static void _AssertFinite(in Vector4 v)
        {
            System.Diagnostics.Debug.Assert(v.IsFinite(), "v is not finite");
        }



        [System.Diagnostics.DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static bool IsFinite(this float val)
        {            
            return float.IsFinite(val);            
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static bool IsFinite(this in Vector2 v)
        {
            return float.IsFinite(v.X) && float.IsFinite(v.Y);
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static bool IsFinite(this in Vector3 v)
        {
            return float.IsFinite(v.X) && float.IsFinite(v.Y) && float.IsFinite(v.Z);
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static bool IsFinite(this in Vector4 v)
        {
            return float.IsFinite(v.X) && float.IsFinite(v.Y) && float.IsFinite(v.Z) && float.IsFinite(v.W);
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static bool IsFinite(this in Quaternion v)
        {
            return float.IsFinite(v.X) && float.IsFinite(v.Y) && float.IsFinite(v.Z) && float.IsFinite(v.W);
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static bool IsFinite(this in Plane v)
        {
            return IsFinite(v.Normal) && float.IsFinite(v.D);
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static bool IsFinite(this in Matrix3x2 v)
        {
            if (!float.IsFinite(v.M11)) return false;
            if (!float.IsFinite(v.M12)) return false;
            if (!float.IsFinite(v.M21)) return false;
            if (!float.IsFinite(v.M22)) return false;
            if (!float.IsFinite(v.M31)) return false;
            if (!float.IsFinite(v.M32)) return false;
            return true;
        }

        [DebuggerStepThrough]
        [MethodImpl(AGRESSIVE)]
        public static bool IsFinite(this in Matrix4x4 v)
        {
            if (!float.IsFinite(v.M11)) return false;
            if (!float.IsFinite(v.M12)) return false;
            if (!float.IsFinite(v.M13)) return false;
            if (!float.IsFinite(v.M14)) return false;

            if (!float.IsFinite(v.M21)) return false;
            if (!float.IsFinite(v.M22)) return false;
            if (!float.IsFinite(v.M23)) return false;
            if (!float.IsFinite(v.M24)) return false;

            if (!float.IsFinite(v.M31)) return false;
            if (!float.IsFinite(v.M32)) return false;
            if (!float.IsFinite(v.M33)) return false;
            if (!float.IsFinite(v.M34)) return false;

            if (!float.IsFinite(v.M41)) return false;
            if (!float.IsFinite(v.M42)) return false;
            if (!float.IsFinite(v.M43)) return false;
            if (!float.IsFinite(v.M44)) return false;
            
            return true;
        }
 
    }
}