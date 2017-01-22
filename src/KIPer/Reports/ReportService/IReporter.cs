using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchiveData.DTO;

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
        /// <param name="result">Результаты</param>
        /// <returns>Модель отчета</returns>
        object GetReport(TestResult result);
    }
}
