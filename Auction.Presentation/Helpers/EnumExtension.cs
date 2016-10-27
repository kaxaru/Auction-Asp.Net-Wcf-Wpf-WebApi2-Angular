using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Auction.Presentation.Helpers
{
    public static class EnumExtension
    {
            public static string GetAttribute(this Enum values)
            {
                return values.GetType()
                        .GetMember(values.ToString())
                        .First()
                        .GetCustomAttribute<DisplayAttribute>().GetName();
            }

        public static string GetDescription(this Enum enumValue)
        {
            FieldInfo fi = enumValue.GetType().GetField(enumValue.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&                
                attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return enumValue.ToString();
            }
        }
    }
}