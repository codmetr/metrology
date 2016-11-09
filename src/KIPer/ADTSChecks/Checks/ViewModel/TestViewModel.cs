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
    public class TestViewModel : CheckBaseViewModel
    {

        /// <summary>
        /// Initializes a new instance of the ADTSCalibrationViewModel class.
        /// </summary>
        public TestViewModel(
            Test methodic, IPropertyPool propertyPool,
            IDeviceManager deviceManager, TestResult resultPool, ADTSParameters customConf):
            base(methodic, propertyPool, deviceManager, resultPool, customConf)
        {
            Title = "Поверка ADTS";
            _stateViewModel.TitleSteps = "Поверяемые точки";
        }
    }
}