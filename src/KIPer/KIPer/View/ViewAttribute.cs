using System;
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

        public static bool CheckViewModel(Type typeModel, Type typeView)
        {
            var atributes = typeView.GetCustomAttributes(typeof (ViewAttribute), false);
            if (atributes.Length == 0)
                return false;
            return atributes.Any(el =>
            {
                var viewAttribute = el as ViewAttribute;
                return viewAttribute != null && viewAttribute.ModelType == typeModel;
            });
        }

        public static bool CheckView(Type typeView)
        {
            var atributes = typeView.GetCustomAttributes(typeof (ViewAttribute), false);
            return atributes.Length != 0;
        }
    }
}
