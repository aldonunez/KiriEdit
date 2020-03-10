using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using TryFreetype.Model;

namespace KiriEdit
{
    public class CharacterItem
    {
        private const string CharacterFolderSearchPattern = "U_*";
        private const string GenericNameSearchPattern = "piece*.kefig";
        private const string MasterFileName = "master.kefigm";
        private const string FigureSearchPattern = "*.kefig";

        //public uint CodePoint { get; set; }
        public string RootPath { get; set; }
        // TODO: Use MasterFigureItem
        public FigureItem MasterFigureItem { get; }
        public IList<FigureItem> PieceFigureItems { get; }

        public CharacterItem(Project project, uint codePoint)
        {
            string charRoot = GetRootPath(project, codePoint);
            var charRootInfo = new DirectoryInfo(charRoot);
            var figureItems = new List<FigureItem>();
            string masterPath = Path.Combine(charRoot, MasterFileName);

            foreach (var fileInfo in charRootInfo.EnumerateFiles(FigureSearchPattern))
            {
                figureItems.Add(new FigureItem(fileInfo.FullName));
            }

            //CodePoint = codePoint;
            RootPath = charRoot;
            // TODO: Use MasterFigureItem
            MasterFigureItem = new FigureItem(masterPath);
            PieceFigureItems = figureItems;
        }

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

        public static bool Add(Project project, uint codePoint)
        {
            string rootPath = GetRootPath(project, codePoint);

            if (Directory.Exists(rootPath))
                return false;

            string figurePath = Path.Combine(rootPath, MasterFileName);

            Figure figure = FigureUtils.MakeMasterFigure(
                project.FontPath,
                project.FaceIndex,
                codePoint);

            var document = new FigureDocument();

            document.Figure = figure;

            var figureItem = new FigureItem(figurePath);

            try
            {
                Directory.CreateDirectory(rootPath);

                figureItem.Save(document);
            }
            catch (Exception)
            {
                Directory.Delete(rootPath, true);
            }

            return true;
        }

        public static void Delete(Project project, uint codePoint)
        {
            string rootPath = GetRootPath(project, codePoint);

            Directory.Delete(rootPath, true);
        }

        public static string FindNextGenericName(Project project, uint codePoint)
        {
            string rootPath = GetRootPath(project, codePoint);

            DirectoryInfo rootInfo = new DirectoryInfo(rootPath);
            var numbers = new List<int>();

            foreach (var fileInfo in rootInfo.EnumerateFiles(GenericNameSearchPattern))
            {
                string fileName = fileInfo.Name;
                int lastDot = fileName.LastIndexOf('.');
                string substr = fileName.Substring(5, lastDot - 5);

                if (int.TryParse(substr, NumberStyles.None, null, out int n) && n != 0)
                    numbers.Add(n);
            }

            int number = 1;

            if (numbers.Count > 0)
            {
                numbers.Sort();

                for (int i = 0; i < numbers.Count; i++)
                {
                    if (number != numbers[i])
                        break;

                    number++;
                }
            }

            return string.Format("piece{0}.kefig", number);
        }

        // TODO: RenameItem?

        public FigureItem AddItem(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException();

            string fileName = name + ".kefig";
            string figurePath = Path.Combine(RootPath, fileName);

            // Fail if the item or file exist.

            if (File.Exists(figurePath))
                throw new ApplicationException();

            foreach (var item in PieceFigureItems)
            {
                if (name.Equals(item.Name, StringComparison.OrdinalIgnoreCase))
                    throw new ApplicationException();
            }

            // Make the new item with a copy of the master figure.

            var figureItem = new FigureItem(figurePath);

            FigureDocument masterDoc = MasterFigureItem.Open();
            FigureDocument pieceDoc = new FigureDocument();

            pieceDoc.Figure = masterDoc.Figure;
            figureItem.Save(pieceDoc);

            PieceFigureItems.Add(figureItem);

            return figureItem;
        }

        public void DeleteItem(string name)
        {
            for (int i = 0; i < PieceFigureItems.Count; i++)
            {
                var item = PieceFigureItems[i];

                if (name.Equals(item.Name, StringComparison.OrdinalIgnoreCase))
                {
                    File.Delete(item.Path);
                    PieceFigureItems.RemoveAt(i);
                    break;
                }
            }

            throw new ApplicationException();
        }
    }
}
