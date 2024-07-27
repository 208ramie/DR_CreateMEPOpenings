using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DiRoots_MEPWallOpenings.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {


        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string Name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        }
    }
}