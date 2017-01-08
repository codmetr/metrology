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
        object GetReport(TestResult result);
    }
}
