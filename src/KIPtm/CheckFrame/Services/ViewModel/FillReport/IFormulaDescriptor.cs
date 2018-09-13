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
        /// <param name="etalon"></param>
        /// <param name="measured"></param>
        /// <returns></returns>
        double GetError(double etalon, double measured);

        /// <summary>
        /// Поллучение оценки корректности измеренного значения
        /// </summary>
        /// <param name="etalon"></param>
        /// <param name="measured"></param>
        /// <returns></returns>
        bool IsCorrect(double etalon, double measured);

    }
}
