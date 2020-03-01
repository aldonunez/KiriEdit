using KiriEdit.Font;

namespace KiriEdit
{
    public class Project
    {
        public string Path;
        public bool IsDirty;

        public string FontFamilyName;
        public FontStyle FontStyle;

        public FontFace FontFace;
    }

    public class ProjectSpec
    {
        public string ProjectName;
        public string ProjectLocation;
        public string FontPath;
        public int FaceIndex;
    }
}
