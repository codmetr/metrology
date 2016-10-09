using ADTSChecks.Model.Checks;
using ArchiveData.DTO;
using CheckFrame.Archive;
using CheckFrame.Model;

namespace ADTSChecks.ViewModel.Checks
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ADTSCalibrationViewModel : ADTSBaseViewModel
    {
        /// <summary>
        /// Initializes a new instance of the ADTSCalibrationViewModel class.
        /// </summary>
        public ADTSCalibrationViewModel(
            AdtsCheckMethod methodic, IPropertyPool propertyPool,
            IDeviceManager deviceManager, TestResult resultPool, ADTSMethodParameters customConf) :
                base(methodic, propertyPool, deviceManager, resultPool, customConf)
        {
            Title = "Калибровка ADTS";
            _stateViewModel.TitleSteps = "Калибруемые точки";
        }
    }
}