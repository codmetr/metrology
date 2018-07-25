using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckFrame.Checks
{
    /// <summary>
    /// Маркер фабрики визуальной модели
    /// </summary>
    public class ViewModelFactoryAttribute:Attribute
    {
        public ViewModelFactoryAttribute(Type modelType)
        {
            ModelType = modelType;
        }

        public Type ModelType { get; private set; }
    }
}
