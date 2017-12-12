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
        /// <param name="resultId">результат проверки</param>
        /// <param name="conf"></param>
        /// <param name="result"></param>
        /// <returns>Data source для отчета</returns>
        public object GetReporter(TestResultID resultId, CheckConfigData conf, object result)
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
                    if (atr.ReportKey != resultId.DeviceType)
                        continue;
                    return reporter.GetReport(resultId, conf, result);
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
