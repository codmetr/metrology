using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ArchiveData.DTO;
using CheckFrame.Checks;
using ReportService;

namespace KipTM.ViewModel.Report
{
    public class ReportViewModel : INotifyPropertyChanged, IReportViewModel, IDisposable
    {
        private object _reportSource;

        public ReportViewModel(IReportFactory reportFactory, TestResultID resultId, CheckConfigData conf, object result)
        {
            var reporter = reportFactory.GetReporter(resultId, conf, result);
            if (reporter != null)
                ReportSource = reporter;
        }

        /// <summary>
        /// Фактический источник данных для отчета
        /// </summary>
        public object ReportSource
        {
            get { return _reportSource; }
            set { _reportSource = value;
                OnPropertyChanged();
            }
        }

        public virtual void Cleanup()
        {
            var disp = _reportSource as IDisposable;
            disp?.Dispose();
        }

        public void Dispose()
        {
            var reportDispose = ReportSource as IDisposable;
            reportDispose?.Dispose();
            Cleanup();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
