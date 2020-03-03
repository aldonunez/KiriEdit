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

        public CharacterCollection Characters { get; } = new CharacterCollection();

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
            projectFile.GlyphListPath = "glyphs.kiriglyf";
            projectFile.FigureFolderPath = "figures";

            // Runtime properties.
            projectFile.Path = projectFilePath;

            // Commit everything to the file system.

            DirectoryInfo dirInfo = null;

            dirInfo = Directory.CreateDirectory(projectFolderPath);
            dirInfo.CreateSubdirectory(projectFile.FigureFolderPath);
            File.Copy(spec.FontPath, importedFontPath);
            File.Create(projectFile.GlyphListPath);

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
            _fullFigureFolderPath = GetFullItemPath(projectFile.FigureFolderPath);
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

        public class CharacterCollection : ItemCollection<uint, Character>
        {
            public void Add(Character character)
            {
                Add(character.CodePoint, character);
            }

            public void Remove(Character character)
            {
                Remove(character.CodePoint);
            }
        }

        public class ItemCollection<TKey, TValue> : INotifyCollectionChanged, IEnumerable<TValue>
        {
            private Dictionary<TKey, TValue> _map = new Dictionary<TKey, TValue>();

            public event NotifyCollectionChangedEventHandler CollectionChanged;

            public int Count { get => _map.Count; }

            public void Add(TKey key, TValue value)
            {
                _map.Add(key, value);
                OnCollectionChanged(NotifyCollectionChangedAction.Add);
            }

            public void Remove(TKey key)
            {
                _map.Remove(key);
                OnCollectionChanged(NotifyCollectionChangedAction.Remove);
            }

            public bool Contains(TKey key)
            {
                return _map.ContainsKey(key);
            }

            public IEnumerator<TValue> GetEnumerator()
            {
                return _map.Values.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _map.Values.GetEnumerator();
            }

            private void OnCollectionChanged(NotifyCollectionChangedAction action)
            {
                if (CollectionChanged != null)
                {
                    var e = new NotifyCollectionChangedEventArgs(action);
                    CollectionChanged(this, e);
                }
            }
        }

        #endregion
    }
}
