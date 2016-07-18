using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ArchiveData.DTO;
using StructureMap;
using Tools;

namespace ReportService
{
    public class ReportFabrik
    {
        private Container _container;
        public void Configure()
        {
            _container = new Container();
            _container.Configure(x => x.Scan(scaner =>
            {
                scaner.AssembliesFromPath("Reports");
                scaner.AddAllTypesOf<IReport>();
            }));

        }

        public IReport GetCustomReporter(TestResult result, string key)
        {
            var reporters = GetReporters();
            return reporters.Where(el => { return el.GetType().GetAttributes(typeof(ReportAttribute)).Any(atr => (atr as ReportAttribute).TargetReportKey == key); }).FirstOrDefault();
        }

        private IEnumerable<IReport> GetReporters()
        {
            return _container.GetAllInstances<IReport>().Where(el => el.GetType().GetAttributes(typeof(ReportAttribute)).Any());
        }
    }
}
