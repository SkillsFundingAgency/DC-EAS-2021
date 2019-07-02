using System;
using System.ComponentModel;
using System.Reflection;

namespace ESFA.DC.EAS1819.Common.Extensions
{
    public static class EnumDescriptionExtension
    {
        public static string ToStringEnums(this Enum en)
        {
            Type type = en.GetType();

            MemberInfo[] memInfo = type.GetMember(en.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }

            return en.ToString();
        }
    }
}