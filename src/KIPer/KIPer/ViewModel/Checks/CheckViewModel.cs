using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using KipTM.Interfaces;
using KipTM.Model.Checks;
using KipTM.ViewModel.Checks;

namespace KipTM.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class CheckViewModel : ViewModelBase
    {
        private IDataService _dataService;
        private readonly IDictionary<string, ICheckMethodic> _checks;
        private object _selectedCheck;
        private ICheckMethodic _selectedCheckType;

        /// <summary>
        /// Initializes a new instance of the CheckViewModel class.
        /// </summary>
        public CheckViewModel(IDataService dataService, IDictionary<string, ICheckMethodic> checks)
        {
            _dataService = dataService;
            _checks = checks;
            if (_checks.Count > 0)
            {
                var selected = _checks.First();
                DeviceTypes = _checks.Keys;
                CheckTypes = _checks.Values;
            }
        }

        public ObservableCollection<IParameterResultViewModel> Parameters { get; set; }

        public IEnumerable<string> DeviceTypes { get; set; }

        public IEnumerable<ICheckMethodic> CheckTypes { get; set; }
 
        public string SelectedDeviceType { get; set; }

        public ICheckMethodic SelectedCheckType
        {
            get { return _selectedCheckType; }
            set
            {
                _selectedCheckType = value;
                Check = GetViewModelFor(_selectedCheckType);
            }
        }

        public string SerialNumber { get; set; }

        public DateTime PreviousCheckTime { get; set; }

        public ICommand Save { get; set; }

        public ICommand Report { get; set; }

        public object Check
        {
            get { return _selectedCheck; }
            set { Set(ref _selectedCheck, value); }
        }

        public object GetViewModelFor(ICheckMethodic methodic)
        {
            if (methodic is ADTSCheckMethodic)
                return new ADTSCalibrationViewModel(methodic as ADTSCheckMethodic);
            return null;
        }
    }
}