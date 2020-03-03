using System.Text.Json.Serialization;

namespace KiriEdit
{
    public class ProjectFile
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
    }

    public class ProjectSpec
    {
        public string ProjectName;
        public string ProjectLocation;
        public string FontPath;
        public int FaceIndex;
    }

    public class Character
    {
        public uint CodePoint { get; }

        public Character(uint codePoint)
        {
            CodePoint = codePoint;
        }
    }
}
