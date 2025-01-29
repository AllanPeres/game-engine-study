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
using System.Windows.Input;

namespace GameUnitedEditor.GameProject {

    [DataContract(Name = "Game")]
    class Project : ViewModelBase {

        public static string Extension { get; } = ".gameunited";
        public static Project Current => Application.Current.MainWindow.DataContext as Project;
        public static UndoRedo UndoRedo { get; } = new();

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

        public ICommand Undo {  get; set; }
        public ICommand Redo { get; set; }

        public ICommand AddScene { get; private set; }

        public ICommand RemoveScene { get; private set; }

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

        private void AddSceneInternal(String sceneName) {
            Debug.Assert(!string.IsNullOrEmpty(sceneName));
            _scenes.Add(new Scene(this, sceneName));
        }

        private void RemoveSceneInternal(Scene scene) {
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

            AddScene = new RelayCommand<object>(x => {
                AddSceneInternal($"New Scene {_scenes.Count}");
                var newScene = _scenes.Last();
                var sceneIndex = _scenes.Count - 1;
                UndoRedo.Add(new UndoRedoAction(
                    () => RemoveSceneInternal(newScene), 
                    () => _scenes.Insert(sceneIndex, newScene), 
                    $"Add Scene {newScene.Name}"));
            });

            RemoveScene = new RelayCommand<Scene>(x => {
                var sceneIndex = _scenes.IndexOf(x);
                RemoveSceneInternal(x);
                UndoRedo.Add(new UndoRedoAction(
                    () => _scenes.Insert(sceneIndex, x),
                    () => RemoveSceneInternal(x),
                    $"Remove Scene {x.Name}"));
            }, x => !x.IsActive);

            Undo = new RelayCommand<object>(x => UndoRedo.Undo());
            Redo = new RelayCommand<object>(x => UndoRedo.Redo());
        }
    }
}
