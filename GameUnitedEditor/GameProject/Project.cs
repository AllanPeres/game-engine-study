using GameUnitedEditor.Utilities;
using GameProject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GameUnitedEditor.GameProject {

    [DataContract(Name = "Game")]
    class Project : ViewModelBase {

        public static string Extension { get; } = ".gameunited";
        public static Project Current => Application.Current.MainWindow.DataContext as Project;

        [DataMember]
        public string Name { get; private set; } = "New Project";
        [DataMember]
        public string Path { get; private set; }
        [DataMember(Name = "Scenes")]
        private ObservableCollection<Scene> _scenes = new();
        public ReadOnlyObservableCollection<Scene> Scenes { get; private set; }

        private Scene _activeScene;
        public Scene ActiveScene {
            get => _activeScene;
            set {
                if (_activeScene != value) {
                    _activeScene = value;
                    OnPropertyChanged(nameof(ActiveScene));
                }
            }
        }


        public string FullPath => $"{Path}{Name}{Extension}";

        public Project(string name, string path) {
            Name = name;
            Path = path;
            _scenes.Add(new Scene(this, "Default Scene"));
            OnDeserialized(new StreamingContext());
        }

        public static Project Load(string file) {
            Debug.Assert(File.Exists(file));
            return Serializer.FromFile<Project>(file);
        }

        public static void Save(Project project) {
            Serializer.ToFile(project, project.FullPath);
        }

        public void Unload() {

        }

        public void AddScene(String sceneName) {
            Debug.Assert(!string.IsNullOrEmpty(sceneName));
            _scenes.Add(new Scene(this, sceneName));
        }

        public void RemoveScene(Scene scene) {
            Debug.Assert(_scenes.Contains(scene));
            _scenes.Remove(scene);
        }


        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) {
            if (_scenes != null) {
                Scenes = new ReadOnlyObservableCollection<Scene>(_scenes);
                OnPropertyChanged(nameof(Scenes));
            }
            ActiveScene = Scenes.FirstOrDefault(x => x.IsActive);
        }
    }
}
