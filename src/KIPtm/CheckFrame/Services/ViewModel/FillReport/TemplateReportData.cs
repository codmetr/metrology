using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CheckFrame.Services.ViewModel.FillReport;

namespace KipTM.Services.ViewModel.FillReport
{
    public class TemplateReportData : INotifyPropertyChanged
    {
        private string _name;
        private ParameterGroup _selectedGroup;

        /// <summary>
        /// Название шаблона
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Список именованых параметров
        /// </summary>
        public ObservableCollection<ParameterValuePair> NamedParameters { get; set; }

        /// <summary>
        /// Группированные параметры
        /// </summary>
        public ObservableCollection<ParameterGroup> ParameterGroups { get; set; }

        /// <summary>
        /// Выбранная группа параметров
        /// </summary>
        public ParameterGroup SelectedGroup
        {
            get { return _selectedGroup; }
            set
            {
                _selectedGroup = value;
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
