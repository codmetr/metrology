using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using ADTS;
using ArchiveData.DTO;
using ArchiveData.DTO.Params;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using KipTM.Archive;
using KipTM.Model;
using KipTM.Model.Channels;
using KipTM.Model.Checks;
using KipTM.Model.Devices;
using KipTM.Model.TransportChannels;
using KipTM.Settings;
using KipTM.View.Checks;
using KipTM.ViewModel;
using KipTM.ViewModel.Channels;
using KipTM.ViewModel.Services;

namespace KipTM.ViewModel.Checks
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    [MethodViewModelAttribute(typeof(ADTSCheckMethod))]
    public class ADTSCalibrationViewModel : ADTSBaseViewModel
    {
        /// <summary>
        /// Initializes a new instance of the ADTSCalibrationViewModel class.
        /// </summary>
        public ADTSCalibrationViewModel(
            ADTSCheckMethod methodic, IPropertyPool propertyPool,
            IDeviceManager deviceManager, TestResult resultPool) :
                base(methodic, propertyPool, deviceManager, resultPool)
        {
            Title = "Калибровка ADTS";
            _stateViewModel.TitleSteps = "Калибруемые точки";
        }
    }
}