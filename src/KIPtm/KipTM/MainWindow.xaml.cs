using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using KipTM.ViewModel;
using Microsoft.Windows.Controls.Ribbon;

namespace KipTM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            ContentControl contentControl = FindVisualChildByName<ContentControl>(Ribbon, "mainItemsPresenterHost");
            contentControl.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ContentControl contentControl = FindVisualChildByName<ContentControl>(Ribbon, "mainItemsPresenterHost");
            contentControl.Visibility = System.Windows.Visibility.Visible;
        }

        private T FindVisualChildByName<T>(DependencyObject parent, string name) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                string controlName = child.GetValue(Control.NameProperty) as string;
                if (controlName == name)
                {
                    return child as T;
                }
                else
                {
                    T result = FindVisualChildByName<T>(child, name);
                    if (result != null)
                        return result;
                }
            }
            return null;
        }
    }
}