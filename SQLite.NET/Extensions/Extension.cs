using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UrbanSoft.Extensions
{
    /// <summary>
    /// Collection of usefull extension methods.
    /// </summary>
    public static class Extension
    {
        /// <summary>
        /// Get the value of an enum's EnumMemberAttribute. If it has none
        /// ToSting() is returned.
        /// </summary>
        /// <param name="enum">The enum to get the value from.</param>
        /// <returns>The enum's EnumMemberAttribute value or ToString if it has
        /// none.</returns>
        public static string ToEnumMemberAttrValue(this Enum @enum)
        {
            var attr = 
                @enum.GetType().GetMember(@enum.ToString()).FirstOrDefault()?.
                    GetCustomAttributes(false).OfType<EnumMemberAttribute>().
                    FirstOrDefault();
            if (attr == null)
                return @enum.ToString();
            return attr.Value;
        }
    }
}
