using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace KiriEdit
{
    public class CharacterItem
    {
        private Project _project;

        private string RootPath { get; }

        public uint CodePoint { get; }

        private CharacterItem(Project project, uint codePoint)
        {
            string name = MakeCharacterFileName(codePoint);
            string charPath = Path.Combine(project.CharactersFolderPath, name);

            RootPath = charPath;
            _project = project;
            CodePoint = codePoint;
        }

        public static CharacterItem Make(Project project, uint codePoint)
        {
            return new CharacterItem(project, codePoint);
        }

        private const string CharacterFolderSearchPattern = "U_*";

        public static IEnumerable<uint> EnumerateCharacterItems(Project project)
        {
            var charsFolderInfo = new DirectoryInfo(project.CharactersFolderPath);

            foreach (var dirInfo in charsFolderInfo.EnumerateDirectories(CharacterFolderSearchPattern))
            {
                string substring = dirInfo.Name.Substring(2);
                uint number;

                if (!uint.TryParse(substring, NumberStyles.HexNumber, null, out number))
                    continue;

                yield return number;
            }

            yield break;
        }

        public static string MakeCharacterFileName(uint codePoint)
        {
            return string.Format("U_{0:X6}", codePoint);
        }

        public void Save()
        {
            // It's OK if it already exists.
            Directory.CreateDirectory(RootPath);

            // TODO: save the character config file
        }
    }
}
