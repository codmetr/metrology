using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchiveData.DTO;

namespace ReportService
{
    public interface IReporter
    {
        object GetReport(TestResult result);
    }
}
