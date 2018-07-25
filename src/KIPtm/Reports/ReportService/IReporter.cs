using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchiveData.DTO;
using CheckFrame.Checks;

namespace ReportService
{
    /// <summary>
    /// Генератор data source для конкретного типа проверки
    /// </summary>
    public interface IReporter
    {
        /// <summary>
        /// Получить отчет по результатам
        /// </summary>
        /// <param name="resultId">Результаты</param>
        /// <param name="conf"></param>
        /// <param name="result"></param>
        /// <returns>Модель отчета</returns>
        object GetReport(TestResultID resultId, CheckConfigData conf, object result);
    }
}
