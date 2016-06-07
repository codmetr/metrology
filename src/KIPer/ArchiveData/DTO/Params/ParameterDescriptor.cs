
using System;

namespace ArchiveData.DTO.Params
{
    public class ParameterDescriptor
    {
        private readonly string _name;
        private readonly object _point;
        private readonly ParameterType _pType;

        public ParameterDescriptor(string name, object point, ParameterType pType)
        {
            _name = name;
            _point = point;
            _pType = pType;
        }

        public string Name
        {
            get { return _name; }
        }

        public object Point
        {
            get { return _point; }
        }

        public ParameterType PType
        {
            get { return _pType; }
        }

        public string LocalizedString
        {
            get { return GetLocalizedString(); }
        }

        private string GetLocalizedString()
        {
            switch (PType)
            {
                case ParameterType.RealValue:
                    return string.Format("[{0} мБар] значение:", Point);
                    break;
                case ParameterType.Error:
                    return string.Format("[{0} мБар] ошибка:", Point);
                    break;
                case ParameterType.IsCorrect:
                    return string.Format("[{0} мБар] результат:", Point);
                    break;
                case ParameterType.Tolerance:
                    return string.Format("[{0} мБар] допуск:", Point);
                    break;
                case ParameterType.Metadata:
                    return string.Format("Информация:", Point);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #region Overrides of Object

        /// <summary>
        /// Возвращает объект <see cref="T:System.String"/>, который представляет текущий объект <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// Объект <see cref="T:System.String"/>, представляющий текущий объект <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("<Name=\"{0}\", Point=\"{1}\", PType=\"{2}\"/>", _name, _point, _pType);
        }

        #endregion
    }
}
