using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLiteArchive
{
    public static class TypeHelperEx
    {
        public static List<Type> FillNoSerialisedTypes(this List<Type> types, object targetObject, Type targetType)
        {
            if (!targetType.IsSerializable)
            {
                types.Add(targetType);
                //return types;
            }
            foreach (var proptype in targetType.GetProperties())
            {
                var prop = proptype.GetValue(targetObject, null);
                if (proptype.PropertyType == typeof(object))
                {
                    types = types.FillNoSerialisedTypes(prop, prop.GetType());
                    continue;
                }
                else
                {
                    types = types.FillNoSerialisedTypes(prop, proptype.PropertyType);
                    continue;
                }
            }
            return types;
        }
        public static List<Type> FillNoObjectTypes(this List<Type> types, Type targetType)
        {
            if (types.Contains(targetType))
                return types;
            if (targetType != typeof(object))
            {
                types.Add(targetType);
            }
            else
            {
                return types;
            }
            foreach (var propertyInfo in targetType.GetProperties())
            {
                types = types.FillNoObjectTypes(propertyInfo.PropertyType);
            }
            return types;
        }
        public static List<Type> FillNoObjectTypes(this List<Type> types, object targetObject, Type targetType)
        {
            if (types.Contains(targetType))
                return types;
            if (targetType != typeof(object))
            {
                types.Add(targetType);
            }
            else
            {
                return types;
            }
            var properties = targetType.GetProperties();
            foreach (var proptype in targetType.GetProperties())
            {
                var indexes = proptype.GetIndexParameters();
                if (indexes.Any())
                {
                    var propVal = targetObject == null ? null : proptype.GetValue(targetObject, indexes);
                    if (proptype.PropertyType != typeof(object))
                        types = types.FillNoObjectTypes(propVal, proptype.PropertyType);
                    else if (propVal != null)
                        types = types.FillNoObjectTypes(propVal, propVal.GetType());
                    foreach (var index in indexes)
                    {
                        var propVal = targetObject == null ? null : proptype.GetValue(targetObject, new object[]{index});
                        if (proptype.PropertyType != typeof(object))
                            types = types.FillNoObjectTypes(propVal, proptype.PropertyType);
                        else if (propVal != null)
                            types = types.FillNoObjectTypes(propVal, propVal.GetType());
                    }
                    continue;
                }
                var propVal = targetObject == null ? null : proptype.GetValue(targetObject, null);
                if (proptype.PropertyType != typeof(object))
                    types = types.FillNoObjectTypes(propVal, proptype.PropertyType);
                else if (propVal != null)
                    types = types.FillNoObjectTypes(propVal, propVal.GetType());
            }
            return types;
        }

        public static List<Type> FillNoInListTypes(this List<Type> types, object targetObject, Type targetType, List<Type> excludeTypes)
        {
            if (types.Contains(targetType))
                return types;
            if (!excludeTypes.Contains(targetType))
                types.Add(targetType);
            //if (targetObject == null)
            //    return types;
            foreach (var proptype in targetType.GetProperties())
            {
                var propVal = proptype.GetValue(targetObject, null);
                if (proptype.PropertyType != typeof(object))
                    types = types.FillNoInListTypes(propVal, proptype.PropertyType, excludeTypes);
                else if (propVal!=null)
                    types = types.FillNoInListTypes(propVal, propVal.GetType(), excludeTypes);
            }
            return types;
        }

    }
}
