using ADTSChecks.Checks.Data;
using ADTSChecks.Model.Checks;
using ArchiveData.DTO;
using CheckFrame.Model;
using KipTM.Archive;
using KipTM.Model;

namespace ADTSChecks.Checks.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class CalibrationViewModel : CheckBaseViewModel
    {
        /// <summary>
        /// Initializes a new instance of the ADTSCalibrationViewModel class.
        /// </summary>
        public CalibrationViewModel(
            Calibration methodic, IPropertyPool propertyPool,
            IDeviceManager deviceManager, TestResult resultPool, ADTSParameters customConf) :
                base(methodic, propertyPool, deviceManager, resultPool, customConf)
        {
            Title = "Калибровка ADTS";
            _stateViewModel.TitleSteps = "Калибруемые точки";
        }
    }
}