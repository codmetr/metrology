using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Tools.View
{
    public static class ViewViewmodelMatcher
    {
        /// <summary>
        /// Function 
        /// </summary>
        /// <param name="resouceDictionary"></param>
        /// <param name="findView"></param>
        /// <param name="findViewModel"></param>
        public static void AddMatch(System.Windows.ResourceDictionary resouceDictionary, Func<Type, bool> findView, Func<Type, Type, bool> findViewModel)
        {
            var types = GetAllTyes();
            List<Type> viewTypes = new List<Type>();
            foreach (var type in types)
            {
                if (findView(type))
                {
                    viewTypes.Add(type);
                }
            }
            foreach (var type in types)
            {
                var typeView = viewTypes.FirstOrDefault(el => findViewModel(type, el));
                if (typeView != null)
                {
                    var dataTemplate = FormDataTemplate(type, typeView);
                    resouceDictionary.Add(new DataTemplateKey(type), dataTemplate);
                }
            }
        }

        /// <summary>
        /// Get type by name
        /// </summary>
        /// <param name="typeNeme"></param>
        /// <returns></returns>
        private static Type GetType(string typeNeme)
        {
            Type result = Type.GetType(typeNeme);

            if (result != null)
                return result;

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                result = assembly.GetType(typeNeme);
                if (result != null)
                    break;
            }
            return result;
        }
        /// <summary>
        /// Get all types from all assemblies
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<Type> GetAllTyes()
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(el =>
            {
                IEnumerable<Type> types = new List<Type>();
                try
                {
                    types = el.GetTypes();
                }
                catch (Exception)
                {
                }
                return types;
            });
        }

        private static DataTemplate FormDataTemplate(Type typeModel, Type typeView)
        {
            return new DataTemplate
            {
                DataType = typeModel,
                VisualTree = new FrameworkElementFactory(typeView)
            };
        }


    }
}
