using GameProject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Editor.GameProject {

    [DataContract(Name = "Game")]
    class Project : ViewModelBase {

        public static string Extension { get; } = ".gameunited";
        [DataMember]
        public string Name { get; private set; }
        [DataMember]
        public string Path { get; private set; }

        public string FullPath => $"{Path}{Name}{Extension}";
        [DataMember(Name = "Scenes")]
        private ObservableCollection<Scene> _scenes = new();
        public ReadOnlyCollection<Scene> Scenes { get; }

        public Project(string name, string path) {
            Name = name;
            Path = path;
            _scenes.Add(new Scene(this, "Default Scene"));
        }

    }
}
