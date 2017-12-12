using System;
using ArchiveData.DTO;
using CheckFrame.Checks;

namespace ReportService
{
    /// <summary>
    /// Консолидированная фабрика отчетов
    /// </summary>
    public interface IReportFactory
    {
        /// <summary>
        /// Получить отчет по типу проверки
        /// </summary>
        /// <param name="resultId">результат проверки</param>
        /// <param name="conf"></param>
        /// <param name="result"></param>
        /// <returns>Data source для отчета</returns>
        object GetReporter(TestResultID resultId, CheckConfigData conf, object result);
    }
}