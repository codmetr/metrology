﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using KipTM.Archive.DTO;

namespace KipTM.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ArchivesViewModel : ViewModelBase, IArchivesViewModel
    {
        private string _meashuredParameter;

        /// <summary>
        /// Initializes a new instance of the TestsViewModel class.
        /// </summary>
        public ArchivesViewModel()
        {

        }

        /// <summary>
        /// Загрузка базовой конфигурации набора тестов
        /// </summary>
        /// <param name="tests"></param>
        public void LoadTests(ResultsArchive results)
        {
            TestsCollection = new ObservableCollection<ITestResultViewModel>(results.Results.Select(el => new TestResultViewModel(el)));
        }

        /// <summary>
        /// Набор выполненных тестов (процедур поверок/калибровок/аттестаций и пр.) 
        /// </summary>
        public ObservableCollection<ITestResultViewModel> TestsCollection { get; set; }

        
    }
}