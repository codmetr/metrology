using System;
using ArchiveData.DTO;

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
        /// <returns>Data source для отчета</returns>
        object GetReporter(TestResult result);
    }
}