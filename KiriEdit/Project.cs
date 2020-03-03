using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text.Json;

namespace KiriEdit
{
    public class Project
    {
        private ProjectFile ProjectFile { get; set; }

        private string _fullFontPath;
        private string _fullFigureFolderPath;

        public bool IsDirty { get => ProjectFile.IsDirty; }
        public string RootPath { get; private set; }
        public string Name { get; private set; }
        public int FaceIndex { get => ProjectFile.FaceIndex; }
        public string FontPath { get => _fullFontPath; }
        public string FiguresFolderPath { get => _fullFigureFolderPath; }

        public CharacterItemCollection Characters { get; } = new CharacterItemCollection();

        public static Project Make(ProjectSpec spec)
        {
            // Figure out paths and names.

            string projectFolderPath = Path.Combine(spec.ProjectLocation, spec.ProjectName);
            string projectFileName = spec.ProjectName + ".kiriproj";
            string projectFilePath = Path.Combine(projectFolderPath, projectFileName);
            string fontFileName = Path.GetFileName(spec.FontPath);
            string importedFontPath = Path.Combine(projectFolderPath, fontFileName);

            // Set up the project object.

            var projectFile = new ProjectFile();

            projectFile.FontPath = fontFileName;
            projectFile.FaceIndex = spec.FaceIndex;
            projectFile.CharacterFolderPath = "characters";

            // Runtime properties.
            projectFile.Path = projectFilePath;

            // Commit everything to the file system.

            DirectoryInfo dirInfo = null;

            dirInfo = Directory.CreateDirectory(projectFolderPath);
            dirInfo.CreateSubdirectory(projectFile.CharacterFolderPath);
            File.Copy(spec.FontPath, importedFontPath);

            Project project = new Project(projectFile);

            project.Save();

            return project;
        }

        private Project(ProjectFile projectFile)
        {
            ProjectFile = projectFile;

            Name = Path.GetFileNameWithoutExtension(projectFile.Path);
            RootPath = Path.GetDirectoryName(projectFile.Path);

            _fullFontPath = GetFullItemPath(projectFile.FontPath);
            _fullFigureFolderPath = GetFullItemPath(projectFile.CharacterFolderPath);
        }

        private string GetFullItemPath(string relativeItemPath)
        {
            return Path.Combine(RootPath, relativeItemPath);
        }

        public static Project Open(string path)
        {
            ProjectFile projectFile = LoadProjectFile(path);

            Project project = new Project(projectFile);

            return project;
        }

        private static ProjectFile LoadProjectFile(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                var task = JsonSerializer.DeserializeAsync<ProjectFile>(stream);
                ProjectFile projectFile = task.Result;
                projectFile.Path = path;
                return projectFile;
            }
        }

        public void Save()
        {
            SaveProjectFile(ProjectFile);
        }

        private static void SaveProjectFile(ProjectFile project)
        {
            var writerOptions = new JsonWriterOptions();
            writerOptions.Indented = true;

            var serializerOptions = new JsonSerializerOptions();
            serializerOptions.AllowTrailingCommas = true;

            using (var stream = File.OpenWrite(project.Path))
            using (var writer = new Utf8JsonWriter(stream, writerOptions))
            {
                JsonSerializer.Serialize(writer, project, serializerOptions);
            }
        }


        #region Inner classes

        //--------------------------------------------------------------------

        public class CharacterItemCollection : ItemSet<uint>
        {
        }

        public class ItemSet<T> : INotifyCollectionChanged, IEnumerable<T>
        {
            private HashSet<T> _set = new HashSet<T>();

            public event NotifyCollectionChangedEventHandler CollectionChanged;

            public int Count { get => _set.Count; }

            public void Add(T value)
            {
                _set.Add(value);
                OnCollectionChanged(NotifyCollectionChangedAction.Add, value);
            }

            public void Remove(T value)
            {
                _set.Remove(value);
                OnCollectionChanged(NotifyCollectionChangedAction.Remove, value);
            }

            public bool Contains(T value)
            {
                return _set.Contains(value);
            }

            public IEnumerator<T> GetEnumerator()
            {
                return _set.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _set.GetEnumerator();
            }

            private void OnCollectionChanged(NotifyCollectionChangedAction action, T value)
            {
                if (CollectionChanged != null)
                {
                    var e = new NotifyCollectionChangedEventArgs(action, value);
                    CollectionChanged(this, e);
                }
            }
        }

        #endregion
    }
}
