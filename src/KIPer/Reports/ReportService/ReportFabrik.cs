using System;
using System.Collections.Generic;
using System.Linq;
using ArchiveData.DTO;
using StructureMap;
using Tools;

namespace ReportService
{
    /// <summary>
    /// Получение списка 
    /// </summary>
    public class ReportFabrik : IReportFabrik
    {
        private static ReportFabrik _instance = null;
        private Container _container;
        private IEnumerable<IReporter> _reporters; 

        private ReportFabrik()
        {
            Configure();
        }

        public ReportFabrik(IEnumerable<IReporter> reporters)
        {
            _reporters = reporters;
        }

        public ReportFabrik Configure()
        {
            _container = new Container();
            _container.Configure(x => x.Scan(scaner =>
            {
                scaner.AssembliesFromPath("Libs");
                scaner.AddAllTypesOf<IReporter>();
            }));
            _reporters = _container.GetAllInstances<IReporter>()
                .Where(el => el.GetType().GetAttributes(typeof (ReportAttribute)).Any());
            return this;
        }

        public static ReportFabrik Locator{get { return _instance ?? (_instance = (new ReportFabrik()).Configure()); }}

        public object GetReporter(TestResult result)
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
                    return reporter.GetReport(result);
                }
            }
            return null;
        }

        private IEnumerable<IReporter> GetReporters()
        {
            return _reporters;
        }

        #region Implementation of IReportFabrik

        public IReporter GetCustomReporter(Type targetT, TestResult result)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
