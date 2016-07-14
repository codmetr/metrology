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
using KipTM.ViewModel;

namespace KipTM.View
{
    /// <summary>
    /// Interaction logic for PACE5000View.xaml
    /// </summary>
    [ViewAttribute(typeof(Pace1000ViewModel))]
    public partial class PACE1000View : UserControl
    {


        public PACE1000View()
        {
            InitializeComponent();
        }
    }
}
