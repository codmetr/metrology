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
        /// <param name="result">результат проверки</param>
        /// <param name="conf"></param>
        /// <returns>Data source для отчета</returns>
        object GetReporter(TestResult result, CheckConfigData conf);
    }
}