using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace KipTM.Services.ViewModel.FillReport
{
    public class TemplateReportData : ViewModelBase
    {
        /// <summary>
        /// Название шаблона
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Список именованых параметров
        /// </summary>
        public ObservableCollection<ParameterValuePair> NamedParameters { get; set; }

        /// <summary>
        /// Группированные параметры
        /// </summary>
        public ObservableCollection<ParameterGroup> ParameterGroups { get; set; }

    }
}
