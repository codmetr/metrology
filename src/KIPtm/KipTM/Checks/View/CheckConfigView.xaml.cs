﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KipTM.ViewModel.Checks;
using KipTM.ViewModel.Checks.Config;
using Tools;
using Tools.View;

namespace KipTM.View.Checks
{
    /// <summary>
    /// Interaction logic for CheckConfigView.xaml
    /// </summary>
    [View(typeof(CheckConfigViewModel))]
    public partial class CheckConfigView : UserControl
    {
        public CheckConfigView()
        {
            InitializeComponent();
        }
    }
}
