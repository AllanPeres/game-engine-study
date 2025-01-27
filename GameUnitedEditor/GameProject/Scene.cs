using GameProject;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Runtime.Serialization;

namespace GameUnitedEditor.GameProject {
    [DataContract]
    internal class Scene : ViewModelBase {

        private string _name;
        [DataMember]
        public string Name {
            get { return _name; }
            set {
                if (_name != value) {
                    _name = value;
                    OnPropertyChanged(Name);
                }
            }
        }
        [DataMember]
        public Project Project { get; private set; }
        [DataMember]
        public bool _isActive;
        private bool IsActive {
            get => _isActive;
            set {
                if (_isActive != value) {
                    _isActive = value;
                    OnPropertyChanged(nameof(IsActive));
                }
            }
        }

        public Scene(Project project, string name) {
            Debug.Assert(project != null);
            Project = project;
            Name = name;
        }
    }
}