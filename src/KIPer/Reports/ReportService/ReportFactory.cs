using System;
using System.Collections.Generic;
using System.Linq;
using ArchiveData.DTO;
using CheckFrame.Checks;
using StructureMap;
using Tools;

namespace ReportService
{
    /// <summary>
    /// Консолидированная фабрика отчетов
    /// </summary>
    public class ReportFactory : IReportFactory
    {
        private readonly IEnumerable<IReporter> _reporters; 

        /// <summary>
        /// Консолидированная фабрика отчетов
        /// </summary>
        /// <param name="reporters">Набор поддерживаемых типов отчетов</param>
        public ReportFactory(IEnumerable<IReporter> reporters)
        {
            _reporters = reporters;
        }

        #region Implementation of IReportFactory

        /// <summary>
        /// Получить отчет по типу проверки
        /// </summary>
        /// <param name="result">результат проверки</param>
        /// <param name="conf"></param>
        /// <returns>Data source для отчета</returns>
        public object GetReporter(TestResult result, CheckConfigData conf)
        {
            var reporters = GetReporters();
            foreach (var reporter in reporters)
            {
                var reportAtr = reporter.GetType().GetAttributes(typeof(ReportAttribute));
                foreach (var atrib in reportAtr)
                {
                    var atr = atrib as ReportAttribute;
                    if (atr == null)
                        continue;
                    if (atr.ReportKey != result.CheckType)
                        continue;
                    return reporter.GetReport(result, conf);
                }
            }
            return null;
        }

        #endregion

        private IEnumerable<IReporter> GetReporters()
        {
            return _reporters;
        }
    }
}
