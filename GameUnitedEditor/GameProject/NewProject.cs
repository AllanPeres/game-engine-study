using GameUnitedEditor.Utilities;
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

namespace GameUnitedEditor.GameProject {

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
        private readonly string _templatePath = @"..\..\..\..\GameUnitedEditor\ProjectTemplates";
        private string _projectName = "NewProject";
        public string ProjectName {
            get => _projectName;
            set {
                if (_projectName != value) {
                    _projectName = value;
                    ValidateProjectPath();
                    OnPropertyChanged(nameof(ProjectName));
                }
            } 
        }
        private string _projectPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\GameUnited\";
        public string ProjectPath {
            get => _projectPath;
            set {
                if (_projectPath != value) {
                    _projectPath = value;
                    ValidateProjectPath();
                    OnPropertyChanged(nameof(ProjectPath));
                }
            }
        }

        private bool _isValid;
        public bool IsValid {
            get => _isValid;
            set {
                if (_isValid != value) {
                    _isValid = value;
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        private string _errorMsg;
        public string ErrorMsg {
            get => _errorMsg;
            set {
                if (_errorMsg != value) {
                    _errorMsg = value;
                    OnPropertyChanged(nameof(ErrorMsg));
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
                ValidateProjectPath();
            } catch (Exception e) {
                Debug.WriteLine(e.Message);
                // TODO: log errors on end of this
            }
        }

        private bool ValidateProjectPath() {
            var path = ProjectPath;
            if (!Path.EndsInDirectorySeparator(path)) path += @"\";
            path += $@"{ProjectName}\";
            IsValid = false;
            if (string.IsNullOrWhiteSpace(ProjectName.Trim())) {
                ErrorMsg = "Type in a project name.";
            } 
            else if (ProjectName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1) {
                ErrorMsg = "Invalid character(s) used in project name.";
            } 
            else if (string.IsNullOrWhiteSpace(path.Trim())) {
                ErrorMsg = "Type in a path name.";
            } 
            else if (path.IndexOfAny(Path.GetInvalidPathChars()) != -1) {
                ErrorMsg = "Invalid character(s) used in project path.";
            } 
            else if (Directory.Exists(path) && Directory.EnumerateFileSystemEntries(path).Any()) {
                ErrorMsg = "Selected path is not empty, Please select a new path.";
            } 
            else {
                ErrorMsg = "";
                IsValid = true;
            }
            return IsValid;
        }

        public string CreateProject(ProjectTemplate template) {
            ValidateProjectPath();
            if (!IsValid) {
                return string.Empty;
            }

            if (!Path.EndsInDirectorySeparator(ProjectPath)) ProjectPath += @"\";
            var path = $@"{ProjectPath}{ProjectName}\";

            try {
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                foreach (var folder in template.FolderNames) {
                    Directory.CreateDirectory(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path), folder)));
                }
                var dirInfo = new DirectoryInfo(path + @".GameUnited\");
                dirInfo.Attributes |= FileAttributes.Hidden;
                File.Copy(template.IconFilePath, Path.GetFullPath(Path.Combine(dirInfo.FullName, "Icon.png")));
                File.Copy(template.ScreenshotFilePath, Path.GetFullPath(Path.Combine(dirInfo.FullName, "Screenshot.png")));

                var projectXml = File.ReadAllText(template.ProjectFilePath);
                projectXml = string.Format(projectXml, ProjectName, ProjectPath);
                var fullPath = Path.GetFullPath(Path.Combine(path, $"{ProjectName}{Project.Extension}"));
                File.WriteAllText(fullPath, projectXml);
                return path;
            } catch (Exception ex) {
                Debug.WriteLine(ex.ToString());
                //TODO log error
            }
            return string.Empty;
        }
    }
}
