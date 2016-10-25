using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KipTM.Services.ViewModel.FillReport
{
    /// <summary>
    /// Описатель формулы рассчета погрешности
    /// </summary>
    public interface IFormulaDescriptor
    {
        /// <summary>
        /// Называние типа погрешности
        /// </summary>
        string Name { get;}

        /// <summary>
        /// Получение величины фактической ошибки
        /// </summary>
        /// <param name="ethalon"></param>
        /// <param name="measured"></param>
        /// <returns></returns>
        double GetError(double ethalon, double measured);

        /// <summary>
        /// Поллучение оценки корректности измеренного значения
        /// </summary>
        /// <param name="ethalon"></param>
        /// <param name="measured"></param>
        /// <returns></returns>
        bool IsCorrect(double ethalon, double measured);

    }
}
