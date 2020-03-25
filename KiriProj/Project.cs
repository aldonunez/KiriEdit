using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text.Json;
using KiriFT;

namespace KiriEdit
{
    public delegate void CharacterItemModifiedHandler(object sender, CharacterItemModifiedEventArgs e);

    public class CharacterItemModifiedEventArgs : EventArgs
    {
        public CharacterItem CharacterItem { get; }

        public CharacterItemModifiedEventArgs(CharacterItem characterItem)
        {
            CharacterItem = characterItem;
        }
    }

    public class Project
    {
        private ProjectFile ProjectFile { get; set; }

        private string _fullFontPath;
        private string _fullCharactersFolderPath;

        public event CharacterItemModifiedHandler CharacterItemModified;

        public bool IsDirty { get => ProjectFile.IsDirty; }
        public string RootPath { get; private set; }
        public string Name { get; private set; }
        public int FaceIndex { get => ProjectFile.FaceIndex; }
        public string FontPath { get => _fullFontPath; }
        public string FontFamily { get => ProjectFile.FontFamily; }
        public string FontName { get => ProjectFile.FontName; }
        public int FontStyle { get => ProjectFile.FontStyle; }
        public string CharactersFolderPath { get => _fullCharactersFolderPath; }

        public CharacterItemCollection Characters { get; }

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
            projectFile.CharactersFolderPath = "characters";

            FillFontInfo(projectFile, spec);

            // Runtime properties.
            projectFile.Path = projectFilePath;

            // Commit everything to the file system.

            DirectoryInfo dirInfo = null;

            dirInfo = Directory.CreateDirectory(projectFolderPath);
            dirInfo.CreateSubdirectory(projectFile.CharactersFolderPath);
            File.Copy(spec.FontPath, importedFontPath);

            Project project = new Project(projectFile);

            project.Save();

            return project;
        }

        private static void FillFontInfo(ProjectFile projectFile, ProjectSpec spec)
        {
            var openParams = OpenParams.IgnoreTypographicFamily;

            using (var lib = new Library())
            using (var face = lib.OpenFace(spec.FontPath, spec.FaceIndex, openParams))
            {
                uint count = face.GetSfntNameCount();

                for (uint i = 0; i < count; i++)
                {
                    // TODO: literal number
                    var sfntName = face.GetSfntName(i);
                    if (sfntName.NameId == 4)
                    {
                        projectFile.FontName = sfntName.String;
                        break;
                    }
                }

                projectFile.FontFamily = face.FamilyName;
                projectFile.FontStyle = Face.ParseLegacyStyle(face.StyleName);
            }
        }

        private Project(ProjectFile projectFile)
        {
            Characters = new CharacterItemCollection(this);

            ProjectFile = projectFile;

            Name = Path.GetFileNameWithoutExtension(projectFile.Path);
            RootPath = Path.GetDirectoryName(projectFile.Path);

            _fullFontPath = GetFullItemPath(projectFile.FontPath);
            _fullCharactersFolderPath = GetFullItemPath(projectFile.CharactersFolderPath);
        }

        private string GetFullItemPath(string relativeItemPath)
        {
            return Path.Combine(RootPath, relativeItemPath);
        }

        public static Project Open(string path)
        {
            ProjectFile projectFile = LoadProjectFile(path);

            Project project = new Project(projectFile);

            foreach (var item in CharacterItem.EnumerateCharacterItems(project))
            {
                project.Characters.Add(item);
            }

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

        internal void NotifyItemModified(CharacterItem item)
        {
            if (CharacterItemModified != null)
            {
                var args = new CharacterItemModifiedEventArgs(item);

                CharacterItemModified?.Invoke(this, args);
            }
        }


        #region Inner classes

        //--------------------------------------------------------------------

        public class CharacterItemCollection : ItemSet<uint, CharacterItem>
        {
            private Project _project;

            public CharacterItemCollection(Project project)
            {
                _project = project;
            }

            public CharacterItem Add(uint codePoint)
            {
                var item = CharacterItem.Add(_project, codePoint);
                AddInternal(codePoint, item);
                return item;
            }

            public void Add(CharacterItem item)
            {
                AddInternal(item.CodePoint, item);
            }

            public void Delete(CharacterItem item)
            {
                if (RemoveInternal(item.CodePoint))
                {
                    item.Delete();
                }
            }
        }

        public class ItemSet<K, V> : INotifyCollectionChanged, IEnumerable<V>
        {
            private Dictionary<K, V> _set = new Dictionary<K, V>();

            public event NotifyCollectionChangedEventHandler CollectionChanged;

            public int Count { get => _set.Count; }

            protected void AddInternal(K key, V value)
            {
                _set.Add(key, value);
                OnCollectionChanged(NotifyCollectionChangedAction.Add, value);
            }

            protected bool RemoveInternal(K key)
            {
                if (_set.TryGetValue(key, out V value))
                {
                    _set.Remove(key);
                    OnCollectionChanged(NotifyCollectionChangedAction.Remove, value);
                    return true;
                }

                return false;
            }

            public bool Contains(K key)
            {
                return _set.ContainsKey(key);
            }

            public IEnumerator<V> GetEnumerator()
            {
                return _set.Values.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _set.GetEnumerator();
            }

            private void OnCollectionChanged(NotifyCollectionChangedAction action, V value)
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
