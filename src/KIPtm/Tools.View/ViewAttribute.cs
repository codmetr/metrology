using System;
using System.Collections.Generic;
using System.Linq;

namespace Tools.View
{
    /// <summary>
    /// Установка соответствия типа представления заданному типу
    /// </summary>
    public class ViewAttribute:Attribute
    {
        /// <summary>
        /// Установка соответствия типа представления заданному типу
        /// </summary>
        /// <param name="modelType">Тип модели</param>
        public ViewAttribute(Type modelType)
        {
            ModelType = modelType;
        }

        /// <summary>
        /// Тип модели
        /// </summary>
        public Type ModelType { get; private set; }

        /// <summary>
        /// Кеш-таблица атрибутов представлений
        /// </summary>
        private static readonly Dictionary<Type, ViewAttribute> AttributeCash = new Dictionary<Type, ViewAttribute>();

        /// <summary>
        /// Проверка соотвествия типа представления типу модели по кешированному 
        /// </summary>
        /// <param name="typeModel">анализируемый тип</param>
        /// <param name="typeView">Потенциальное представление</param>
        /// <returns>Соответствует ли тип модели данному представлению</returns>
        public static bool CheckViewModel(Type typeModel, Type typeView)
        {
            var atributes = typeView.GetCustomAttributes(typeof (ViewAttribute), false);
            if (atributes.Length == 0)
                return false;
            return atributes.Any(el =>
            {
                if(!(el is ViewAttribute))
                    return false;
                var viewAttribute = el as ViewAttribute;
                return viewAttribute.ModelType == typeModel;
            });
        }

        /// <summary>
        /// Проверка соотвествия типа представления типу модели по кешированному 
        /// </summary>
        /// <remarks>Анализирует только проанализированные в методе <see cref="CheckView"/> типы, по составленной кеш-таблице, что дает существеное ускорение обработки.</remarks>
        /// <param name="typeModel">анализируемый тип</param>
        /// <param name="typeView">Потенциальное представление</param>
        /// <returns>Соответствует ли тип модели данному представлению</returns>
        public static bool CheckViewModelCashOnly(Type typeModel, Type typeView)
        {
            if(!AttributeCash.ContainsKey(typeView))
                return false;

            return AttributeCash[typeView].ModelType == typeModel;
        }

        /// <summary>
        /// Проверка наличия метки типа представления
        /// </summary>
        /// <remarks>Так же, сотавляет кеш-таблицу атрибутов <see cref="ViewAttribute"/> по задданым типам представления</remarks>
        /// <param name="typeView">анализируемый тип</param>
        /// <returns></returns>
        public static bool CheckView(Type typeView)
        {
            var atributes = typeView.GetCustomAttributes(typeof (ViewAttribute), false);
            if (atributes.Length == 0)
                return false;
            if(!AttributeCash.ContainsKey(typeView))
                AttributeCash.Add(typeView, atributes.FirstOrDefault() as ViewAttribute);
            else
                AttributeCash[typeView] = atributes.FirstOrDefault() as ViewAttribute;
            return true;
        }
    }
}
