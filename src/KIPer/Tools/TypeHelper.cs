using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools
{
    public static class TypeHelper
    {
        public static IEnumerable<object> GetAttributes(this Type targetType, Type attribute)
        {
            return targetType.GetCustomAttributes(false).Where(atr=>atr.GetType().IsAssignableFrom(attribute));
        }
    }
}
