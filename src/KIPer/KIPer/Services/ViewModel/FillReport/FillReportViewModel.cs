using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace KipTM.Services.ViewModel
{
    public class FillReportViewModel : ViewModelBase
    {
        public ObservableCollection<string> TemplatesReports { get; set; }

        public ObservableCollection<ParameterValuePair> NamedParameters { get; set; }

        public ObservableCollection<ParameterGroup> ParameterGroups { get; set; }
    }
}
