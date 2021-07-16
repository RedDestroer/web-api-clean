using System;
using System.ComponentModel;

namespace WebApiClean.Common.Extensions
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class TypeExtensions
    {
        public static string GetFriendlyName(this Type type)
        {
            string friendlyName = type.Name;
            if (type.IsGenericType)
            {
                int backtick = friendlyName.IndexOf('`');
                if (backtick > 0)
                    friendlyName = friendlyName.Remove(backtick);

                friendlyName += "<";
                var typeParameters = type.GetGenericArguments();
                for (int i = 0; i < typeParameters.Length; ++i)
                {
                    string typeParamName = GetFriendlyName(typeParameters[i]);
                    friendlyName += i == 0 ? typeParamName : "," + typeParamName;
                }

                friendlyName += ">";
            }
            else if (type.IsNested && type.ReflectedType != null)
            {
                friendlyName = $"{type.ReflectedType.Name}+{friendlyName}";
            }

            return friendlyName;
        }
    }
}
