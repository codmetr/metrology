using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Tools
{
    public static class TypeScaner
    {
        /// <summary>
        /// Получить список всех типов всех сборок
        /// </summary>
        public static IEnumerable<Tuple<Assembly, Type>> GetAllTypes()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            return assemblies.SelectMany(GetAllTypes);
        }

        /// <summary>
        /// Получить список всех типов сборки
        /// </summary>
        public static IEnumerable<Tuple<Assembly, Type>> GetAllTypes(Assembly assembly)
        {
            var types = assembly.GetTypes();
            return types.Select(type => new Tuple<Assembly, Type>(assembly, type));
        }
    }
}
