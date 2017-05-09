﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiveData.DTO;
using CheckFrame.ViewModel.Archive;
using KipTM.Checks;
using KipTM.Model;
using KipTM.ViewModel;
using MarkerService;
using MarkerService.Filler;

namespace KipTM.Archive.ViewModel
{
    public class TestResultViewModelFactory
    {
        private TestResult _result;
        private CheckConfig _checkConf;
        private IMarkerFactory<IParameterResultViewModel> _resulMaker;
        private IPropertiesLibrary _properties;
        private IFillerFactory<IParameterResultViewModel> _filler;
        private IDataAccessor _accessor;

        public TestResultViewModelFactory(TestResult result, CheckConfig checkConf,
            IMarkerFactory<IParameterResultViewModel> resulMaker, IPropertiesLibrary properties,
            IFillerFactory<IParameterResultViewModel> filler, IDataAccessor accessor)
        {
            _result = result;
            _checkConf = checkConf;
            _resulMaker = resulMaker;
            _properties = properties;
            _filler = filler;
            _accessor = accessor;
        }

        public TestResultViewModel GetTestResult()
        {
            return new TestResultViewModel(_result,
                _resulMaker.GetMarkers(_checkConf.CustomSettings.GetType(), _checkConf.CustomSettings), _filler, _accessor);
        }

    }
}
