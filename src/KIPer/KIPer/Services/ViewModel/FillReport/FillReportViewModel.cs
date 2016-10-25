using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using KipTM.Services.ViewModel.FillReport;

namespace KipTM.Services.ViewModel
{
    public class FillReportViewModel : ViewModelBase
    {
        /// <summary>
        /// Набор доступных шаблонов
        /// </summary>
        public ObservableCollection<TemplateReportData> TemplatesReports { get; set; }

        /// <summary>
        /// Выбранный шаблон
        /// </summary>
        public TemplateReportData SelectedTemplate { get; set; }

        /// <summary>
        /// Список доступных формул
        /// </summary>
        public ObservableCollection<IFormulaDescriptor> Formulas { get; set; }
    }
}
