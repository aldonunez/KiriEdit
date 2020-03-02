using System.Text.Json.Serialization;

namespace KiriEdit
{
    public class Project
    {
        public string FontPath { get; set; }
        public int FaceIndex { get; set; }
        public string GlyphListPath { get; set; }
        public string FigureFolderPath { get; set; }

        // Runtime properties.
        [JsonIgnore]
        public string Path { get; set; }
        [JsonIgnore]
        public bool IsDirty { get; set; }
        [JsonIgnore]
        public string FullFontPath { get => GetFullItemPath(FontPath); }
        [JsonIgnore]
        public string FullGlyphListPath { get => GetFullItemPath(GlyphListPath); }
        [JsonIgnore]
        public string FullFiguresFolderPath { get => GetFullItemPath(FigureFolderPath); }

        private string GetFullItemPath(string relativeItemPath)
        {
            string projectFolderPath = System.IO.Path.GetDirectoryName(Path);
            string fullItemPath = System.IO.Path.Combine(projectFolderPath, relativeItemPath);
            return fullItemPath;
        }
    }

    public class ProjectSpec
    {
        public string ProjectName;
        public string ProjectLocation;
        public string FontPath;
        public int FaceIndex;
    }
}
