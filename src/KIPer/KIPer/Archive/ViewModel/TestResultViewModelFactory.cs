using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiveData.DTO;
using CheckFrame.ViewModel.Archive;
using Core.Archive.DataTypes;
using KipTM.Checks;
using KipTM.ViewModel;
using MarkerService;
using MarkerService.Filler;
using SQLiteArchive;

namespace KipTM.Archive.ViewModel
{
    public class TestResultViewModelFactory
    {
        private TestResultID _resultId;
        private CheckConfigDevice _checkConf;
        private IMarkerFactory<IParameterResultViewModel> _resulMaker;
        private IFillerFactory<IParameterResultViewModel> _filler;
        private IDataAccessor _archive;


        public TestResultViewModelFactory(TestResultID resultId, CheckConfigDevice checkConf,
            IMarkerFactory<IParameterResultViewModel> resulMaker,
            IFillerFactory<IParameterResultViewModel> filler, IDataAccessor archive)
        {
            _resultId = resultId;
            _checkConf = checkConf;
            _resulMaker = resulMaker;
            _filler = filler;
            _archive = archive;
        }

        public ITestResultViewModel GetTestResult()
        {
            var markers = _resulMaker.GetMarkers(_checkConf.CustomSettings.GetType(), _checkConf.CustomSettings);
            var parameters = new List<IParameterResultViewModel>(markers);
            var results = _archive.Load(_resultId) as IEnumerable<TestStepResult>;
            if(results!=null)
                foreach (var stepResult in results)
                {
                    var filledResult = _filler.FillMarker(stepResult.Result.GetType(),
                        new Tuple<string, string>(stepResult.CheckKey, stepResult.StepKey), stepResult.Result);
                    if (filledResult == null)
                        continue;
                    var index = parameters.FindIndex((el) => el.PointMeasuring == filledResult.PointMeasuring);
                    if (index >= 0)
                    {
                        parameters.RemoveAt(index);
                        parameters.Insert(index, filledResult);
                    }
                }
            return new TestResultViewModel(_resultId, _checkConf.Data, parameters, _archive);
        }

    }
}
