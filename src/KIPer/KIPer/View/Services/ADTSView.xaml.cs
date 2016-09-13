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
using KipTM.ViewModel.Services;
using Tools;

namespace KipTM.View.Services
{
    /// <summary>
    /// Interaction logic for ADTSView.xaml
    /// </summary>
    [View(typeof(ADTSViewModel))]
    public partial class ADTSView : UserControl
    {
        public ADTSView()
        {
            InitializeComponent();
        }
    }
}
