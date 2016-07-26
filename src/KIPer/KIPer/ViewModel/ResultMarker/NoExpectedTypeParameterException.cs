using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KipTM.ViewModel.ResultMarker
{
    /// <summary>
    /// Получен неожиданный тип параметра
    /// </summary>
    public class NoExpectedTypeParameterException : Exception
    {
        /// <summary>
        /// Получен неожиданный тип параметра
        /// </summary>
        /// <param name="expectedType">Ожидаемый тип</param>
        /// <param name="receivedType">Полученный тип</param>
        public NoExpectedTypeParameterException(Type expectedType, Type receivedType)
        {
            ExpectedType = expectedType;
            ReceivedType = receivedType;
        }

        /// <summary>
        /// Ожидаемый тип
        /// </summary>
        public Type ExpectedType { get; private set; }

        /// <summary>
        /// Полученный тип
        /// </summary>
        public Type ReceivedType { get; private set; }
    }
}
