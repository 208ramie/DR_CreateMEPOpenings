using DiRoots_MEPWallOpenings.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace DiRoots_MEPWallOpenings.Views
{
    /// <summary>
    /// Interaction logic for CreateMEPOpeningsView.xaml
    /// </summary>
    public partial class CreateMEPOpeningsView : Window
    {
        private CreateMEPOpeningsView()
        {
            InitializeComponent();
            this.Closing += new CancelEventHandler(Window_Closing);
        }


        private static CreateMEPOpeningsView? _instance;

        public static CreateMEPOpeningsView Instance
            => _instance ??= new CreateMEPOpeningsView();


        private void Window_Closing(object sender, CancelEventArgs e)
        {
            // Set the window's visibility to Hidden
            this.Visibility = Visibility.Hidden;

            // Cancel the closing event
            e.Cancel = true;
        }


        private void MergeOpeningsCheckBox_OnChecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            if ((bool)checkbox.IsChecked)
            {
                MinDistanceTextBox.IsEnabled = true;
            }
            else
            {
                MinDistanceTextBox.IsEnabled = false;
            }
            
        }
    }
}
