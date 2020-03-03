using System.Text.Json.Serialization;

namespace KiriEdit
{
    public class ProjectFile
    {
        public string FontPath { get; set; }
        public int FaceIndex { get; set; }
        public string CharactersFolderPath { get; set; }

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
}
