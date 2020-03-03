using System.IO;

namespace KiriEdit
{
    public class CharacterItem
    {
        public uint CodePoint { get; }

        public CharacterItem(uint codePoint)
        {
            CodePoint = codePoint;
        }

        public static void Make(Project project, uint codePoint)
        {
            string name = MakeCharacterFileName(codePoint);
            string charPath = Path.Combine(project.FiguresFolderPath, name);

            // It's OK if it already exists.
            Directory.CreateDirectory(charPath);
        }

        public static string MakeCharacterFileName(uint codePoint)
        {
            return string.Format("U_{0:X6}", codePoint);
        }
    }
}
