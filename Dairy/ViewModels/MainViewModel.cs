using System.Collections.Generic;
using System.ComponentModel;

using Dairy.Models;

namespace Dairy.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private List<DairyModel> dairies;

        public event PropertyChangedEventHandler PropertyChanged;

        public List<DairyModel> Dairies { get => this.dairies; set { this.dairies = value; OnPropertyChanged("Dairies"); } }

        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
