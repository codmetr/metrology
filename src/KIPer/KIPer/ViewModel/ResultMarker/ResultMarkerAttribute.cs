using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KipTM.ViewModel.ResultMarker
{
    /// <summary>
    /// Указывает что тип - фабрика маркеров результатов
    /// </summary>
    public class ResultMarkerAttribute:Attribute
    {
        /// <summary>
        /// Указывает что тип - фабрика маркеров результатов
        /// </summary>
        /// <param name="targetType">Тип, для которого формируется маркер</param>
        public ResultMarkerAttribute(Type targetType)
        {
            TargetType = targetType;
        }

        /// <summary>
        /// Тип, для которого формируется маркер
        /// </summary>
        public Type TargetType { get; private set; }
    }
}
