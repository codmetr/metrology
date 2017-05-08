using System.Collections.ObjectModel;
using KipTM.Services.ViewModel;

namespace CheckFrame.Services.ViewModel.FillReport
{
    public class ParameterGroup
    {
        /// <summary>
        /// Название группы параметров
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Группа параметров
        /// </summary>
        public ObservableCollection<ParameterRow> Parameters { get; set; } 
    }
}
