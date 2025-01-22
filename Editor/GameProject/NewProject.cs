using Editor.Utilities;
using GameProject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Editor.GameProject {

    [DataContract]
    public class ProjectTemplate {
        [DataMember]
        public string ProjectType { get; set; }
        [DataMember]
        public string ProjectFile { get; set; }
        [DataMember]
        public List<string> FolderNames { get; set; }
        public byte[] Icon { get; set; }
        public byte[] Screenshot { get; set; }
        public string IconFilePath { get; set; }
        public string ScreenshotFilePath { get; set; }
        public string ProjectFilePath { get; set; }
    }

    class NewProject : ViewModelBase {
        // TODO: get the path from instalation localtion
        private readonly string _templatePath = @"..\..\..\..\Editor\ProjectTemplates";
        private string _projectName = "NewProject";
        public string ProjectName {
            get => _projectName;
            set {
                if (_projectName != value) {
                    _projectName = value;
                    OnPropertyChanged(nameof(ProjectName));
                }
            } 
        }
        private string _projectPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\GameEngined\";
        public string ProjectPath {
            get => _projectPath;
            set {
                if (_projectPath != value) {
                    _projectPath = value;
                    OnPropertyChanged(nameof(ProjectPath));
                }
            }
        }

        private ObservableCollection<ProjectTemplate> _projectTemplates = new();
        public ReadOnlyObservableCollection<ProjectTemplate> ProjectTemplates { get; }

        public NewProject() {
            ProjectTemplates = new(_projectTemplates);
            try {
                var templateFiles = Directory.GetFiles(_templatePath, "template.xml", SearchOption.AllDirectories);
                Debug.Assert(templateFiles.Any());
                foreach (var file in templateFiles) {
                    var template = Serializer.FromFile<ProjectTemplate>(file);
                    template.IconFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "icon.png"));
                    template.Icon = File.ReadAllBytes(template.IconFilePath);
                    template.ScreenshotFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "screenshot.png"));
                    template.Screenshot = File.ReadAllBytes(template.ScreenshotFilePath);
                    template.ProjectFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), template.ProjectFile));
                    
                    _projectTemplates.Add(template);
                }
            } catch(Exception e) {
                Debug.WriteLine(e.Message);
                // TODO: log errors on end of this
            }
        }
    }
}
