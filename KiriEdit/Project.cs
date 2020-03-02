using KiriEdit.Font;

namespace KiriEdit
{
    public class Project
    {
        public string FontPath;
        public int FaceIndex;
        public string GlyphListPath;
        public string FigureFolderPath;

        // Runtime properties.
        public string Path;
        public bool IsDirty;
    }

    public class ProjectSpec
    {
        public string ProjectName;
        public string ProjectLocation;
        public string FontPath;
        public int FaceIndex;
    }
}
