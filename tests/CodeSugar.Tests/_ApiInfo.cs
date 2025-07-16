using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;



namespace CodeSugar
{
    public class ApiInfo
    {
        public static IEnumerable<ApiInfo> ListMethods(System.Reflection.Assembly assembly)
        {
            foreach (var classType in assembly.GetTypes())
            {
                if (classType.IsNested) continue;

                // Only consider classes (not interfaces, enums, etc.)
                if (classType.IsClass)
                {
                    // Get all public methods (instance and static, declared only in this class)
                    var methods = classType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);

                    foreach (var method in methods)
                    {
                        if (method.IsPrivate) continue;

                        yield return new ApiInfo(classType, method);
                    }
                }
            }
        }

        public ApiInfo(Type classType, MethodInfo method)
        {
            ClassType = classType;
            Method = method;
        }

        public Type ClassType { get; }

        public MethodInfo Method { get; }


        public string ToReturnString() => GetTypeName(Method.ReturnType);

        public string ToBodyString()
        {
            var args = Method.GetParameters().Select(item => ToSourceCodeString(item));

            var methodArgs = string.Join(", ", args);

            var isExtension = Method.IsDefined(typeof(ExtensionAttribute), true);
            if (isExtension) methodArgs = "this " + methodArgs;

            return $"{Method.Name}({methodArgs})";
        }        

        private static string ToSourceCodeString(ParameterInfo p)
        {
            var type = p.ParameterType;
            string modifier = "";

            if (p.GetCustomAttributes(typeof(ParamArrayAttribute), false).Any())
                modifier = "params ";
            else if (p.IsIn && !type.IsByRef)
                modifier = "in ";
            else if (p.IsOut)
                modifier = "out ";
            else if (type.IsByRef)
                modifier = "ref ";

            string typeName = GetTypeName(type);
            if (type.IsByRef)
                typeName = GetTypeName(type.GetElementType());

            string s = $"{modifier}{typeName} {p.Name}";

            if (p.HasDefaultValue)
            {
                var defaultValue = p.DefaultValue ?? "null";
                if (defaultValue is string)
                    defaultValue = $"\"{defaultValue}\"";
                s += $" = {defaultValue}";
            }

            return s;
        }

        // Helper to get C# type names (handles generics and arrays)
        private static string GetTypeName(Type t)
        {
            if (t.IsGenericType)
            {
                var genericTypeName = t.Name.Substring(0, t.Name.IndexOf('`'));
                var genericArgs = string.Join(", ", t.GetGenericArguments().Select(GetTypeName));
                return $"{genericTypeName}<{genericArgs}>";
            }
            if (t.IsArray)
            {
                return $"{GetTypeName(t.GetElementType())}[]";
            }
            return t switch
            {
                _ when t == typeof(int) => "int",
                _ when t == typeof(string) => "string",
                _ when t == typeof(object) => "object",
                _ when t == typeof(bool) => "bool",
                _ when t == typeof(double) => "double",
                _ when t == typeof(float) => "float",
                _ when t == typeof(decimal) => "decimal",
                _ when t == typeof(long) => "long",
                _ when t == typeof(short) => "short",
                _ when t == typeof(byte) => "byte",
                _ when t == typeof(char) => "char",
                _ => t.Name
            };
        }
    }
}
