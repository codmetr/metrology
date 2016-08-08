using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiveData.DTO;
using GalaSoft.MvvmLight;
using ReportService;

namespace KipTM.ViewModel.Report
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ReportViewModel : ViewModelBase, IReportViewModel, IDisposable
    {
        private object _reportSource;

        public ReportViewModel(IReportFabrik reportFabric, Type getType, TestResult result)
        {
            var reporter = reportFabric.GetReporter(getType, result);
            if (reporter != null)
                ReportSource = reporter;
        }

        /// <summary>
        /// Фактический источник данных для отчета
        /// </summary>
        public object ReportSource
        {
            get { return _reportSource; }
            set { Set(ref _reportSource, value); }
        }

        public override void Cleanup()
        {
            base.Cleanup();
            var disp = _reportSource as IDisposable;
            if(disp!=null)
                disp.Dispose();
        }

        public void Dispose()
        {
            Cleanup();
        }
    }
}
