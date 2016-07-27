using System;

namespace MarkerService
{
    /// <summary>
    /// Указывает что тип - фабрика маркеров
    /// </summary>
    public class MarkerAttribute:Attribute
    {
        /// <summary>
        /// Указывает что тип - фабрика маркеров
        /// </summary>
        /// <param name="targetType">Тип, для которого формируется маркер</param>
        public MarkerAttribute(Type targetType)
        {
            TargetType = targetType;
        }

        /// <summary>
        /// Тип, для которого формируется маркер
        /// </summary>
        public Type TargetType { get; private set; }
    }
}
