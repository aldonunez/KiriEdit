﻿using System.IO;
using System.Text.Json;

namespace KiriEdit
{
    // TODO: Make a MasterFigureItem

    public class FigureItem
    {
        public string Name { get; }
        public string Path { get; }
        public bool IsDirty { get; set; }

        public FigureItem(string path)
        {
            string baseName = System.IO.Path.GetFileNameWithoutExtension(path);

            Name = baseName;
            Path = path;
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

            IsDirty = false;
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
    }
}