using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KipTM.ViewModel.Checks
{
    public class MethodViewModelAttribute:Attribute
    {
        private readonly Type _modelType;

        /// <summary>
        /// Указание какой модели репдназначен данный ViewModel
        /// </summary>
        /// <param name="modelType"></param>
        public MethodViewModelAttribute(Type modelType)
        {
            _modelType = modelType;
        }

        public Type ModelType{get { return _modelType; }}
    }
}
