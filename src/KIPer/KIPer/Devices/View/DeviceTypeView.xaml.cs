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
using KipTM.ViewModel.DeviceTypes;
using Tools;

namespace KipTM.View
{
    /// <summary>
    /// Interaction logic for DeviceTypeView.xaml
    /// </summary>
    [View(typeof(DeviceTypeViewModel))]
    public partial class DeviceTypeView : UserControl
    {
        public DeviceTypeView()
        {
            InitializeComponent();
        }
    }
}