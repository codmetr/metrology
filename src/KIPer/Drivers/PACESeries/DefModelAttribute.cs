using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PACESeries
{
    [AttributeUsage(AttributeTargets.Method)]
    class DefModelAttribute:Attribute
    {
        public readonly Model Model;

        /// <summary>
        /// Описывает с какими моделями доступна функция
        /// </summary>
        /// <param name="model"></param>
        public DefModelAttribute(Model model)
        {
            Model = model;
        }
    }
}
