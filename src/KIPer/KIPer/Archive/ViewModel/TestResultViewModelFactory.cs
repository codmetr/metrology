using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiveData.DTO;
using CheckFrame.ViewModel.Archive;
using KipTM.Checks;
using KipTM.ViewModel;
using MarkerService;
using MarkerService.Filler;
using SQLiteArchive;

namespace KipTM.Archive.ViewModel
{

    public class TestResultViewModelFactory
    {
        private TestResult _result;
        private CheckConfigDevice _checkConf;
        private IMarkerFactory<IParameterResultViewModel> _resulMaker;
        private IFillerFactory<IParameterResultViewModel> _filler;
        private IObjectiveArchive _archive;


        public TestResultViewModelFactory(TestResult result, CheckConfigDevice checkConf,
            IMarkerFactory<IParameterResultViewModel> resulMaker,
            IFillerFactory<IParameterResultViewModel> filler, IObjectiveArchive archive)
        {
            _result = result;
            _checkConf = checkConf;
            _resulMaker = resulMaker;
            _filler = filler;
            _archive = archive;
        }

        public TestResultViewModel GetTestResult()
        {
            var markers = _resulMaker.GetMarkers(_checkConf.CustomSettings.GetType(), _checkConf.CustomSettings);
            var parameters = new List<IParameterResultViewModel>(markers);
            foreach (var stepResult in _result.Results)
            {
                var filledResult = _filler.FillMarker(stepResult.Result.GetType(), new Tuple<string, string>(stepResult.CheckKey, stepResult.StepKey), stepResult.Result);
                if (filledResult == null)
                    continue;
                var index = parameters.FindIndex((el) => el.PointMeasuring == filledResult.PointMeasuring);
                if (index >= 0)
                {
                    parameters.RemoveAt(index);
                    parameters.Insert(index, filledResult);
                }
            }
            var accessor = new DataAccessor(_result, _checkConf.Data, _checkConf.CustomSettings, _archive);
            return new TestResultViewModel(_result, parameters, accessor);
        }

    }
}
