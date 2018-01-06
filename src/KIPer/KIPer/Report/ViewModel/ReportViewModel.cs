﻿using System;
using ArchiveData.DTO;
using CheckFrame.Checks;
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
            var reportDispose = ReportSource as IDisposable;
            if (reportDispose != null)
                reportDispose.Dispose();
            Cleanup();
        }
    }
}
