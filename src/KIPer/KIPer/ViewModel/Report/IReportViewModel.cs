namespace KipTM.ViewModel.Report
{
    public interface IReportViewModel
    {
        /// <summary>
        /// Фактический источник данных для отчета
        /// </summary>
        object ReportSource { get; set; }
    }
}