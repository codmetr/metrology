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
using KipTM.Checks.ViewModel;
using Tools.View;

namespace KipTM.Checks.View
{
    /// <summary>
    /// Interaction logic for SaveView.xaml
    /// </summary>
    [View(typeof(SaveVM))]
    public partial class SaveView : UserControl
    {
        public SaveView()
        {
            InitializeComponent();
        }
    }
}