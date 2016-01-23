﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace KIPer.ViewModel
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
        public void LoadTests(IEnumerable<ITestResultViewModel> tests)
        {
            TestsCollection = new ObservableCollection<ITestResultViewModel>(tests);
        }

        /// <summary>
        /// Набор выполненных тестов (процедур поверок/калибровок/аттестаций и пр.) 
        /// </summary>
        public ObservableCollection<ITestResultViewModel> TestsCollection { get; set; }

        
    }
}