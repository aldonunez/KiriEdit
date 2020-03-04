using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace KiriEdit
{
    public class CharacterItem
    {
        //private Project _project;

        private string RootPath { get; }

        public uint CodePoint { get; }

        //public static CharacterItem Make(Project project, uint codePoint)
        //{
        //    return new CharacterItem(project, codePoint);
        //}

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

        private static string MakeCharacterFileName(uint codePoint)
        {
            return string.Format("U_{0:X6}", codePoint);
        }

        private static string GetRootPath(Project project, uint codePoint)
        {
            string name = MakeCharacterFileName(codePoint);
            string charPath = Path.Combine(project.CharactersFolderPath, name);
            return charPath;
        }

        public static void AddStorage(Project project, uint codePoint)
        {
            string rootPath = GetRootPath(project, codePoint);

            // It's OK if it already exists.
            Directory.CreateDirectory(rootPath);

            // TODO: save the character config file
        }

        public static void DeleteStorage(Project project, uint codePoint)
        {
            string rootPath = GetRootPath(project, codePoint);

            // TODO: Delete recursively.

            Directory.Delete(rootPath);
        }
    }
}
