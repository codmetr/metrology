using System;
using ArchiveData.DTO;

namespace ReportService
{
    public interface IReportFabrik
    {
        object GetReporter(Type targetT, TestResult result);
    }
}