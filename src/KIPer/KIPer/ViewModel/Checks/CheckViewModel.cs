﻿using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using KIPer.ViewModel;

namespace KipTM.ViewModel.Checks
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class CheckViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the CheckViewModel class.
        /// </summary>
        public CheckViewModel()
        {
        }

        public ObservableCollection<IParameterResultViewModel> Parameters { get; set; }
    }
}