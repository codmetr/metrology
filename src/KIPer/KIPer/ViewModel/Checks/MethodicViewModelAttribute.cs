using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KipTM.ViewModel.Checks
{
    public class MethodicViewModelAttribute:Attribute
    {
        private Type _modelType;

        /// <summary>
        /// Указание какой модели репдназначен данный ViewModel
        /// </summary>
        /// <param name="modelType"></param>
        public MethodicViewModelAttribute(Type modelType)
        {
            _modelType = modelType;
        }
    }
}
