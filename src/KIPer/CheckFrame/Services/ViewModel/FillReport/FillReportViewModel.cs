using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using KipTM.Services.ViewModel.FillReport;

namespace KipTM.Services.ViewModel
{
    public class FillReportViewModel : INotifyPropertyChanged
    {
        private IFormulaDescriptor _selectedFormula;
        private TemplateReportData _selectedTemplate;

        /// <summary>
        /// Набор доступных шаблонов
        /// </summary>
        public ObservableCollection<TemplateReportData> TemplatesReports { get; set; }

        /// <summary>
        /// Выбранный шаблон
        /// </summary>
        public TemplateReportData SelectedTemplate
        {
            get { return _selectedTemplate; }
            set
            {
                _selectedTemplate = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Список доступных формул
        /// </summary>
        public ObservableCollection<IFormulaDescriptor> Formulas { get; set; }

        /// <summary>
        /// Выбранная формула
        /// </summary>
        public IFormulaDescriptor SelectedFormula
        {
            get { return _selectedFormula; }
            set
            {
                _selectedFormula = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
