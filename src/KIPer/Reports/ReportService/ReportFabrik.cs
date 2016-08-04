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

        private ReportFabrik()
        {
            Configure();
        }
        
        public ReportFabrik Configure()
        {
            _container = new Container();
            _container.Configure(x => x.Scan(scaner =>
            {
                scaner.AssembliesFromPath("Reports");
                scaner.AddAllTypesOf<IReporter>();
            }));
            return this;
        }

        public static ReportFabrik Locator{get { return _instance ?? (_instance = (new ReportFabrik()).Configure()); }}

        public object GetReporter( Type tagretType, TestResult result)
        {
            
            
            var reporters = GetReporters();
            foreach (var reporter in reporters)
            {
                var reportAtr =reporter.GetType().GetAttributes(typeof (ReportAttribute));
                foreach (var atrib in reportAtr)
                {
                    var atr = atrib as ReportAttribute;
                    if(atr == null)
                        continue;
                    if (atr.TargetReportKey != tagretType)
                        continue;
                    return reporter.GetReport(result);
                }
            }
            return null;
        }

        private IEnumerable<IReporter> GetReporters()
        {
            return _container.GetAllInstances<IReporter>().Where(el => el.GetType().GetAttributes(typeof(ReportAttribute)).Any());
        }

        #region Implementation of IReportFabrik

        public IReporter GetCustomReporter(Type targetT, TestResult result)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
