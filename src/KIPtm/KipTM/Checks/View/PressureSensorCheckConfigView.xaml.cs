﻿using System.Windows.Controls;
using PressureSensorCheck.Workflow;
using Tools.View;

namespace KipTM.Checks.View
{
    /// <summary>
    /// Interaction logic for PressureSensorCheckConfig.xaml
    /// </summary>
    [View(typeof(PressureSensorCheckConfigVm))]
    public partial class PressureSensorCheckConfigView : UserControl
    {
        public PressureSensorCheckConfigView()
        {
            InitializeComponent();
        }
    }
}