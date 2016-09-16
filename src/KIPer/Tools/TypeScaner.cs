using System;
using System.Collections.Generic;
using System.IO;
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
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                Assembly assembly;
                try
                {
                    assembly = Assembly.LoadFile(file);
                    if (assemblies.Any(el => el.FullName == assembly.FullName))
                        continue;
                }
                catch
                {
                    continue;
                }
                assemblies.Add(assembly);
            }
            return assemblies.SelectMany(GetAllTypes);
        }

        /// <summary>
        /// Получить список всех типов сборки
        /// </summary>
        public static IEnumerable<Tuple<Assembly, Type>> GetAllTypes(Assembly assembly)
        {
            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch
            {
                return new List<Tuple<Assembly, Type>>();
            }
            return types.Select(type => new Tuple<Assembly, Type>(assembly, type));
        }
    }
}
