using ArchiveData.DTO;
using KipTM.Archive;
using KipTM.Model;
using KipTM.Model.Checks;

namespace KipTM.ViewModel.Checks
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
            ADTSCheckMethod methodic, IPropertyPool propertyPool,
            IDeviceManager deviceManager, TestResult resultPool, ADTSMethodParameters customConf) :
                base(methodic, propertyPool, deviceManager, resultPool, customConf)
        {
            Title = "Калибровка ADTS";
            _stateViewModel.TitleSteps = "Калибруемые точки";
        }
    }
}