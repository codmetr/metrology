using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace SQLiteArchive
{
    /// <summary>
    /// Методы расширения для поиска типов всех элементов
    /// </summary>
    public static class TypeHelperEx
    {
        /// <summary>
        /// Дополнить список набором не учтенных сериализуемых типов вложенных элементов переданного объекта
        /// </summary>
        /// <param name="types">Имеющийся список типов</param>
        /// <param name="targetObject">Анализирумый объект</param>
        /// <param name="targetType">Целевой тип</param>
        /// <remarks>Сам объект передается на случай, если есть поле типа <see cref="object"/></remarks>
        /// <returns>Дополненный список <paramref name="types"/></returns>
        public static List<Type> AddSerializedTypes(this List<Type> types, object targetObject, Type targetType)
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
                    types = types.AddSerializedTypes(prop, prop.GetType());
                    continue;
                }
                else
                {
                    types = types.AddSerializedTypes(prop, proptype.PropertyType);
                    continue;
                }
            }
            return types;
        }

        /// <summary>
        /// Дополнить список набором типов свойств переданного типа (рекурсивно)
        /// </summary>
        /// <param name="types">Имеющийся список типов</param>
        /// <param name="targetType">Целевой тип</param>
        /// <remarks>В случае если поле типа <see cref="object"/>, оно игнорируется</remarks>
        /// <returns>Дополненный список <paramref name="types"/></returns>
        public static List<Type> AddTypes(this List<Type> types, Type targetType)
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
                types = types.AddTypes(propertyInfo.PropertyType);
            }
            return types;
        }

        /// <summary>
        /// Дополнить список набором типов вложенных элементов переданного объекта, с исключением проанализированных типов
        /// </summary>
        /// <param name="types">Имеющийся список типов</param>
        /// <param name="targetObject">Анализирумый объект</param>
        /// <param name="targetType">Целевой тип</param>
        /// <remarks>Сам объект передается на случай, если есть поле типа <see cref="object"/></remarks>
        /// <returns>Дополненный список <paramref name="types"/></returns>
        public static List<Type> AddTypes(this List<Type> types, object targetObject, Type targetType)
        {
            return AddTypesExcludeParrent(types, new List<Type>(), targetObject, targetType);
        }

        /// <summary>
        /// Дополнить список набором не учтенных типов вложенных элементов переданного объекта,
        /// с исключением проанализированных типов.
        /// </summary>
        /// <param name="types">Имеющийся список типов</param>
        /// <param name="parentTypes">Список проанализированных ранее типов</param>
        /// <param name="targetObject">Анализирумый объект</param>
        /// <param name="targetType">Целевой тип</param>
        /// <remarks>Сам объект передается на случай, если есть поле типа <see cref="object"/></remarks>
        /// <returns>Дополненный список <paramref name="types"/></returns>
        public static List<Type> AddTypesExcludeParrent(List<Type> types, List<Type> parentTypes, object targetObject, Type targetType)
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
                return AddTypesExcludeParrent(types, parentTypes, targetObject, targetType);
            }
            parentTypes.Add(targetType);
            foreach (var proptype in targetType.GetProperties())
            {
                var indexes = proptype.GetIndexParameters();
                if (indexes.Any())
                {
                    types = AddTypesPropertyIndex(types, parentTypes, targetObject, proptype);
                    continue;
                }
                types = AddTypesProperty(types, parentTypes, targetObject, proptype);
            }
            parentTypes.Remove(targetType);
            return types;
        }

        /// <summary>
        /// Дополнить список типами из свойства
        /// </summary>
        /// <param name="types">Имеющийся список типов</param>
        /// <param name="parentTypes">Список проанализированных ранее типов</param>
        /// <param name="targetObject">Анализирумый объект</param>
        /// <param name="proptype">тип свойства</param>
        /// <returns></returns>
        private static List<Type> AddTypesProperty(List<Type> types, List<Type> parentTypes, object targetObject, PropertyInfo proptype)
        {
            var propVal = targetObject == null ? null : proptype.GetValue(targetObject, null);
            if (proptype.PropertyType != typeof(object))
                types = AddTypesExcludeParrent(types, parentTypes, propVal, proptype.PropertyType);
            else if (propVal != null)
                types = AddTypesExcludeParrent(types, parentTypes, propVal, propVal.GetType());

            return types;
        }

        /// <summary>
        /// Дополнить список типами из индексного свойства
        /// </summary>
        /// <param name="types">Имеющийся список типов</param>
        /// <param name="parentTypes">Список проанализированных ранее типов</param>
        /// <param name="targetObject">Анализирумый объект</param>
        /// <param name="proptype">тип свойства</param>
        /// <returns></returns>
        private static List<Type> AddTypesPropertyIndex(List<Type> types, List<Type> parentTypes, object targetObject, PropertyInfo proptype)
        {
            // fill index types
            var indexesTypes = proptype.GetIndexParameters();
            foreach (var parameterInfo in indexesTypes)
            {
                types = AddTypesExcludeParrent(types, parentTypes, null, parameterInfo.ParameterType);
            }

            // fill proerty type
            types = AddTypesExcludeParrent(types, parentTypes, null, proptype.PropertyType);
            if (targetObject == null)
                return types;

            var enumerable = targetObject as IEnumerable;
            if (enumerable != null)
            {
                foreach (object val in enumerable)
                    // fill elements types
                    types = AddTypesExcludeParrent(types, parentTypes, val, val.GetType());
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
