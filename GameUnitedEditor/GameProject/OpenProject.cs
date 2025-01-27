using GameUnitedEditor.Utilities;
using GameProject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace GameUnitedEditor.GameProject {

    [DataContract]
    public class ProjectData {
        [DataMember]
        public string ProjectName { get; set; }
        [DataMember]
        public string ProjectPath { get; set; }
        [DataMember]
        public DateTime LastOpening { get; set; }
        public string FullPath { get => $"{ProjectPath}{ProjectName}{Project.Extension}"; }
        public byte[] Icon { get; set; }
        public byte[] Screenshot { get; set; }
    }

    [DataContract]
    public class ProjectDataList {
        [DataMember]
        public List<ProjectData> Projects  { get; set; }
    }

    class OpenProject {

        private static readonly string _applicationDataPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\GameUnited";
        private static readonly string _projectDataPath;
        private static readonly ObservableCollection<ProjectData> _projects = new();
        public static ReadOnlyObservableCollection<ProjectData> Projects { get; }

        static OpenProject() {
            try {
                if (!Directory.Exists(_applicationDataPath)) Directory.CreateDirectory(_applicationDataPath);
                _projectDataPath = $@"{_applicationDataPath}ProjectData.xml";
                Projects = new ReadOnlyObservableCollection<ProjectData>(_projects);
                ReadProjectData();
            } catch (Exception e) {
                Debug.WriteLine(e.Message);
            }
        }

        public static Project Open(ProjectData data) {
            ReadProjectData();
            var project = _projects.FirstOrDefault(x => x.FullPath == data.FullPath);
            if (project == null) {
                project = data;
                _projects.Add(project);
            }
            project.LastOpening = DateTime.Now;
            WriteProjectData();

            return Project.Load(project.FullPath);

        }

        private static void ReadProjectData() {
            if (File.Exists(_projectDataPath)) {
                var projects = Serializer.FromFile<ProjectDataList>(_projectDataPath).Projects.OrderByDescending(x => x.LastOpening);
                _projects.Clear();
                foreach (var project in projects) {
                    if (File.Exists(project.FullPath)) {
                        project.Icon = File.ReadAllBytes($@"{project.ProjectPath}\.GameUnited\Icon.png");
                        project.Screenshot = File.ReadAllBytes($@"{project.ProjectPath}\.GameUnited\Screenshot.png");
                        _projects.Add(project);
                    }
                }
            }
        }

        private static void WriteProjectData() {
            var projects = _projects.OrderBy(x => x.LastOpening).ToList();
            Serializer.ToFile(new ProjectDataList() { Projects = projects }, _projectDataPath);
        }
    }
}
