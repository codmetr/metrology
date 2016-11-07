using ADTSData;
using ArchiveData.DTO.Params;

namespace ADTSChecks.Checks.Data
{
    public static class AdtsPointService
    {
        /// <summary>
        /// Заполнить поле по заданному типу в параметре
        /// </summary>
        /// <param name="field">Заполняемое поле</param>
        /// <param name="ptype">Тип заполняемого поля</param>
        /// <param name="value">Значние поля</param>
        /// <returns></returns>
        public static AdtsPointResult SetProperty(this AdtsPointResult field, ParameterDescriptor ptype, object value)
        {
            field.Point = (double)ptype.Point;

            if (ptype.PType == ParameterType.RealValue)
                field.RealValue = (double)value;
            else if (ptype.PType == ParameterType.Error)
                field.Error = (double)value;
            else if (ptype.PType == ParameterType.Tolerance)
                field.Tolerance = (double)value;
            else if (ptype.PType == ParameterType.IsCorrect)
                field.IsCorrect = (bool)value;

            return field;
        }
    }
}
