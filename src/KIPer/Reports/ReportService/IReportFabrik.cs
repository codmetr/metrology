using System;
using ArchiveData.DTO;

namespace ReportService
{
    public interface IReportFabrik
    {
        object GetReporter(TestResult result);
    }
}