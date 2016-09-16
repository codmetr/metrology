using ADTSChecks.Model.Checks;
using ArchiveData.DTO;
using KipTM.Archive;
using KipTM.Model;
using KipTM.Model.Checks;
using KipTM.ViewModel.Checks;

namespace ADTSChecks.ViewModel.Checks
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ADTSTestViewModel : ADTSBaseViewModel
    {

        /// <summary>
        /// Initializes a new instance of the ADTSCalibrationViewModel class.
        /// </summary>
        public ADTSTestViewModel(
            ADTSTestMethod methodic, IPropertyPool propertyPool,
            IDeviceManager deviceManager, TestResult resultPool, ADTSMethodParameters customConf):
            base(methodic, propertyPool, deviceManager, resultPool, customConf)
        {
            Title = "Поверка ADTS";
            _stateViewModel.TitleSteps = "Поверяемые точки";
        }
    }
}