using System.ComponentModel;
using System.Runtime.Serialization;

namespace GameProject {

    [DataContract(IsReference = true)]
    internal class ViewModelBase : INotifyPropertyChanged {
        
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}