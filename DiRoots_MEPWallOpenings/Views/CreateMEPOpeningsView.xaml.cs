using DiRoots_MEPWallOpenings.ViewModels;
using System;
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
        }


        private static CreateMEPOpeningsView? _instance;

        public static CreateMEPOpeningsView Instance
            => _instance ??= new CreateMEPOpeningsView();

    }
}
