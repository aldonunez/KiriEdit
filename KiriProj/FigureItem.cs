using System;
using System.IO;
using System.Text.Json;

namespace KiriProj
{
    // TODO: Make a MasterFigureItem

    public class FigureItem
    {
        public string Name { get; }
        public string Path { get; }
        public bool IsDirty { get; set; }

        public CharacterItem Parent { get; private set; }

        public event EventHandler Deleted;

        internal FigureItem(string path, CharacterItem parent)
        {
            string baseName = System.IO.Path.GetFileNameWithoutExtension(path);

            Name = baseName;
            Path = path;
            Parent = parent;
        }

        public void Save(FigureDocument document)
        {
            var writerOptions = new JsonWriterOptions();
            writerOptions.Indented = true;

            var serializerOptions = new JsonSerializerOptions();
            serializerOptions.AllowTrailingCommas = true;

            using (var stream = File.Create(Path))
            using (var writer = new Utf8JsonWriter(stream, writerOptions))
            {
                JsonSerializer.Serialize(writer, document, serializerOptions);
            }

            bool wasDirty = IsDirty;

            IsDirty = false;

            if (wasDirty)
                Parent.NotifyItemModified(this);
        }

        public FigureDocument Open()
        {
            using (var stream = File.OpenRead(Path))
            {
                var task = JsonSerializer.DeserializeAsync<FigureDocument>(stream);
                FigureDocument document = task.Result;
                return document;
            }
        }

        internal void Delete()
        {
            File.Delete(Path);

            Deleted?.Invoke(this, EventArgs.Empty);
        }

        internal static FigureItem Make(CharacterItem characterItem, string name)
        {
            string fileName = name + ".kefig";
            string figurePath = System.IO.Path.Combine(characterItem.RootPath, fileName);

            // Fail if the item or file exist.

            if (File.Exists(figurePath))
                throw new ApplicationException();

            // Make the new item with a copy of the master figure.

            var figureItem = new FigureItem(figurePath, characterItem);

            FigureDocument pieceDoc = characterItem.MasterFigureItem.Open();

            figureItem.Save(pieceDoc);

            return figureItem;
        }
    }
}
