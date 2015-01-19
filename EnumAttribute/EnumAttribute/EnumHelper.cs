using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace EnumAttribute
{
    public static class EnumHelper
    {
        /// <summary>
        /// Gets first enum with desired value of selected BaseEnumAttribute
        /// </summary>
        /// <typeparam name="E">The enum you searching</typeparam>
        /// <typeparam name="A">The BaseEnumAttribute you are searching by</typeparam>
        /// <param name="value"></param>
        /// <returns>Enum, null if not found</returns>
        public static Enum SelectEnum<E, A>(string value)
            where E : struct, IConvertible, IComparable, IFormattable
            where A : BaseEnumAttribute
        {
            return Enum.GetValues(typeof(E)).Cast<Enum>().FirstOrDefault(e => e.Get<A>() == value);
        }

        /// <summary>
        /// Gets the value of an EnumAttribute
        /// </summary>
        /// <typeparam name="T">The type of the EnumAttribute you want the value of</typeparam>
        /// <param name="enumVal">The enum value</param>
        /// <returns>String value of EnumAttribute</returns>
        public static string Get<T>(this Enum enumVal) where T : BaseEnumAttribute
        {
            var type = enumVal.GetType();
            var member = type.GetMember(enumVal.ToString()).FirstOrDefault();
            if (member != null)
            {
                var attributes = member.GetCustomAttributes(typeof(T), false);

                if (attributes.Any())
                {
                    var attribute = (T)attributes[0];
                    return attribute.Value;
                }
            }
            return string.Empty;
        }
    }
}
