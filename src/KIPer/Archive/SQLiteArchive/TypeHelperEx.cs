using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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
            return FillNoObjectTypesExcludeParrent(types, new List<Type>(), targetObject, targetType);
        }

        public static List<Type> FillNoObjectTypesExcludeParrent(List<Type> types, List<Type> parentTypes, object targetObject, Type targetType)
        {
            // for exclude stacOverflow
            if (parentTypes.Contains(targetType))
                return types;
            if (targetType != typeof(object))
            {
                if (!types.Contains(targetType))
                    types.Add(targetType);
            }
            else
            {
                if (targetObject == null)
                    return types;
                // try get real type of object
                targetType = targetObject.GetType();
                if (targetType != typeof (object))
                    // if real type is object
                    return types;
                return FillNoObjectTypesExcludeParrent(types, parentTypes, targetObject, targetType);
            }
            parentTypes.Add(targetType);
            foreach (var proptype in targetType.GetProperties())
            {
                var indexes = proptype.GetIndexParameters();
                if (indexes.Any())
                {
                    types = FillNoInListTypesPropertyIndex(types, parentTypes, targetObject, proptype);
                    continue;
                }
                types = FillNoInListTypesProperty(types, parentTypes, targetObject, proptype);
            }
            parentTypes.Remove(targetType);
            return types;
        }

        private static List<Type> FillNoInListTypesProperty(List<Type> types, List<Type> parentTypes, object targetObject, PropertyInfo proptype)
        {
            var propVal = targetObject == null ? null : proptype.GetValue(targetObject, null);
            if (proptype.PropertyType != typeof(object))
                types = FillNoObjectTypesExcludeParrent(types, parentTypes, propVal, proptype.PropertyType);
            else if (propVal != null)
                types = FillNoObjectTypesExcludeParrent(types, parentTypes, propVal, propVal.GetType());

            return types;
        }

        private static List<Type> FillNoInListTypesPropertyIndex(List<Type> types, List<Type> parentTypes, object targetObject, PropertyInfo proptype)
        {
            // fill index types
            var indexesTypes = proptype.GetIndexParameters();
            foreach (var parameterInfo in indexesTypes)
            {
                types = FillNoObjectTypesExcludeParrent(types, parentTypes, null, parameterInfo.ParameterType);
            }

            // fill proerty type
            types = FillNoObjectTypesExcludeParrent(types, parentTypes, null, proptype.PropertyType);
            if (targetObject == null)
                return types;

            var enumerable = targetObject as IEnumerable;
            if (enumerable != null)
            {
                foreach (object val in enumerable)
                    // fill elements types
                    types = FillNoObjectTypesExcludeParrent(types, parentTypes, val, val.GetType());
            }
            return types;
        }

        //public static List<Type> FillNoInListTypes(this List<Type> types, object targetObject, Type targetType, List<Type> excludeTypes)
        //{
        //    if (types.Contains(targetType))
        //        return types;
        //    if (!excludeTypes.Contains(targetType))
        //        types.Add(targetType);
        //    foreach (var proptype in targetType.GetProperties())
        //    {
        //        var propVal = proptype.GetValue(targetObject, null);
        //        if (proptype.PropertyType != typeof(object))
        //            types = types.FillNoInListTypes(propVal, proptype.PropertyType, excludeTypes);
        //        else if (propVal != null)
        //            types = types.FillNoInListTypes(propVal, propVal.GetType(), excludeTypes);
        //    }
        //    return types;
        //}

    }
}
