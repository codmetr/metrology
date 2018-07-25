using System;

namespace MarkerService
{
    /// <summary>
    /// Указывает что тип - фабрика маркеров
    /// </summary>
    public class MarkerAttribute:Attribute
    {
        private Type _targetType;

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
        public Type TargetType
        {
            get { return _targetType; }
            private set { _targetType = value; }
        }
    }
}
