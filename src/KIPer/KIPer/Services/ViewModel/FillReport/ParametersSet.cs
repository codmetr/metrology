using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KipTM.Services.ViewModel
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
