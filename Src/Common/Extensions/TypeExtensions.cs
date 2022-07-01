using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace WebApiClean.Common.Extensions
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class TypeExtensions
    {
        private static readonly Dictionary<Type, string> _typesToFriendlyNames = new Dictionary<Type, string>
        {
            { typeof(bool), "bool" },
            { typeof(byte), "byte" },
            { typeof(sbyte), "sbyte" },
            { typeof(char), "char" },
            { typeof(decimal), "decimal" },
            { typeof(double), "double" },
            { typeof(float), "float" },
            { typeof(int), "int" },
            { typeof(uint), "uint" },
            { typeof(long), "long" },
            { typeof(ulong), "ulong" },
            { typeof(object), "object" },
            { typeof(short), "short" },
            { typeof(ushort), "ushort" },
            { typeof(string), "string" }
        };

        public static string GetFriendlyName(this Type type)
        {
            if (type.IsArray)
                return type.GetFriendlyNameOfArrayType();
            if (type.IsGenericType)
                return type.GetFriendlyNameOfGenericType();
            if (type.IsPointer)
                return type.GetFriendlyNameOfPointerType();

            return _typesToFriendlyNames.TryGetValue(type, out var aliasName)
                ? aliasName
                : type.Name;
        }

        private static string GetFriendlyNameOfArrayType(this Type type)
        {
            var sb = new StringBuilder(string.Empty);
            while (type!.IsArray)
            {
                var commas = new string(Enumerable.Repeat(',', type.GetArrayRank() - 1).ToArray());
                sb.Append($"[{commas}]");
                type = type.GetElementType();
            }

            return $"{type.GetFriendlyName()}{sb}";
        }

        private static string GetFriendlyNameOfGenericType(this Type type)
        {
            if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return type.GetGenericArguments().First().GetFriendlyName() + "?";

            var friendlyName = type.Name;
            var indexOfBacktick = friendlyName.IndexOf('`');
            if (indexOfBacktick > 0)
                friendlyName = friendlyName.Remove(indexOfBacktick);

            var typeParameterNames = type
                .GetGenericArguments()
                .Select(typeParameter => typeParameter.GetFriendlyName());
            var joinedTypeParameters = string.Join(", ", typeParameterNames);

            return $"{friendlyName}<{joinedTypeParameters}>";
        }

        private static string GetFriendlyNameOfPointerType(this Type type) =>
            type.GetElementType().GetFriendlyName() + "*";
    }
}
