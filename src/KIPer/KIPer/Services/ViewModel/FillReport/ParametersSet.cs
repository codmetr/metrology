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
        public string Name { get; set; }

        public ObservableCollection<ParameterRow> Parameters { get; set; } 
    }
}
