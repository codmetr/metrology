using System;
using System.Collections.Generic;
using System.Linq;

namespace KipTM.View
{
    public class ViewAttribute:Attribute
    {
        public ViewAttribute(Type modelType)
        {
            ModelType = modelType;
        }

        public Type ModelType { get; private set; }
        private static Dictionary<Type, ViewAttribute> _attributeCash = new Dictionary<Type, ViewAttribute>(); 
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

        public static bool CheckViewModelCashOnly(Type typeModel, Type typeView)
        {
            if(!_attributeCash.ContainsKey(typeView))
                return false;

            return _attributeCash[typeView].ModelType == typeModel;
        }

        public static bool CheckView(Type typeView)
        {
            var atributes = typeView.GetCustomAttributes(typeof (ViewAttribute), false);
            if (atributes.Length == 0)
                return false;
            if(!_attributeCash.ContainsKey(typeView))
                _attributeCash.Add(typeView, atributes.FirstOrDefault() as ViewAttribute);
            else
                _attributeCash[typeView] = atributes.FirstOrDefault() as ViewAttribute;
            return true;
        }

        public static ViewAttribute GetViewAttribute(Type typeView)
        {
            var atributes = typeView.GetCustomAttributes(typeof (ViewAttribute), false);
            return atributes.FirstOrDefault() as ViewAttribute;
        }
    }
}
