using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiRoots_MEPWallOpenings.RelayCommands;
using DiRoots_MEPWallOpenings.RevitCommands;

namespace DiRoots_MEPWallOpenings.ViewModels
{
    internal class CreateMEPOpeningsVM : ViewModelBase
    {

        #region ViewModel singleton

        private static CreateMEPOpeningsVM? _instance;

        public static CreateMEPOpeningsVM Instance 
            => _instance ??= new CreateMEPOpeningsVM();

        #endregion


        #region Fields

        private double _servicefOffsetInmm = 50;

        private double _minDisatnceInmmInmm = 100;

        private bool mergeOpenings = false;

        #endregion


        #region Backing properties

        public double ServiceOffsetInmm
        {
            get => _servicefOffsetInmm;
            set
            {
                _servicefOffsetInmm = value;
                OnPropertyChanged();
            }
        }

        public double MinDisatnceInmm
        {
            get => _minDisatnceInmmInmm;
            set
            {
                _minDisatnceInmmInmm = value;
                OnPropertyChanged();
            }
        }


        public bool MergeOpenings
        {
            get => mergeOpenings;
            set
            {
                mergeOpenings = value;
                OnPropertyChanged();
            }
        }

        #endregion


        #region Commands

        public RelayCommand CreateOpeningsCommand { get; set; }

        #endregion


        #region Private Constructor

        public CreateMEPOpeningsVM()
        {
            // Create a method for the relay command
            CreateOpeningsCommand = new RelayCommand(CreateOpenings);
            _instance = this;
        }


        #endregion


        #region Methods

        private void CreateOpenings(object obj)
        {
            // Raise the event defined in the Command
            CreateOpeningsRCommand.MEPOpeningsEvent.Raise();
        }

        #endregion



    }
}
