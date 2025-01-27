using GameProject;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Editor.GameProject {
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

        public Scene(Project project, string name) {
            Debug.Assert(project != null);
            Project = project;
            Name = name;
        }
    }
}