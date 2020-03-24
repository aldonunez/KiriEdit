using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using TryFreetype.Model;

namespace KiriEdit
{
    public delegate void FigureItemModifiedHandler(object sender, FigureItemModifiedEventArgs e);

    public class FigureItemModifiedEventArgs : EventArgs
    {
        public FigureItem FigureItem { get; }

        public FigureItemModifiedEventArgs(FigureItem figureItem)
        {
            FigureItem = figureItem;
        }
    }

    public class CharacterItem
    {
        private const string CharacterFolderSearchPattern = "U_*";
        private const string GenericNameSearchPattern = "piece*.kefig";
        private const string MasterFileName = "master.kefigm";
        private const string FigureSearchPattern = "*.kefig";

        private List<FigureItem> _figureItems = new List<FigureItem>();

        public event FigureItemModifiedHandler FigureItemModified;

        public Project Project { get; }
        public uint CodePoint { get; }
        public string RootPath { get; }
        // TODO: Use MasterFigureItem
        public FigureItem MasterFigureItem { get; }
        public IReadOnlyList<FigureItem> PieceFigureItems => _figureItems;

        private CharacterItem(Project project, uint codePoint, bool enumPieces = false)
        {
            string charRoot = GetRootPath(project, codePoint);
            string masterPath = Path.Combine(charRoot, MasterFileName);

            if (enumPieces)
                FillPieces(charRoot);

            Project = project;
            CodePoint = codePoint;
            RootPath = charRoot;
            // TODO: Use MasterFigureItem
            MasterFigureItem = new FigureItem(masterPath, this);
        }

        private void FillPieces(string rootPath)
        {
            var charRootInfo = new DirectoryInfo(rootPath);

            foreach (var fileInfo in charRootInfo.EnumerateFiles(FigureSearchPattern))
            {
                _figureItems.Add(new FigureItem(fileInfo.FullName, this));
            }
        }

        public static IEnumerable<CharacterItem> EnumerateCharacterItems(Project project)
        {
            var charsFolderInfo = new DirectoryInfo(project.CharactersFolderPath);

            foreach (var dirInfo in charsFolderInfo.EnumerateDirectories(CharacterFolderSearchPattern))
            {
                string substring = dirInfo.Name.Substring(2);
                uint number;

                if (!uint.TryParse(substring, NumberStyles.HexNumber, null, out number))
                    continue;

                var item = new CharacterItem(project, number, true);

                yield return item;
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

        public static CharacterItem Add(Project project, uint codePoint)
        {
            string rootPath = GetRootPath(project, codePoint);

            // TODO: still needed?
            if (Directory.Exists(rootPath))
                return null;

            string figurePath = Path.Combine(rootPath, MasterFileName);

            Figure figure = FigureUtils.MakeMasterFigure(
                project.FontPath,
                project.FaceIndex,
                codePoint);

            var shapes = FigureUtils.CalculateShapes(figure);
            var document = new FigureDocument();

            document.Figure = figure;
            document.Shapes = shapes;

            var characterItem = new CharacterItem(project, codePoint);
            var figureItem = new FigureItem(figurePath, characterItem);

            try
            {
                Directory.CreateDirectory(rootPath);

                figureItem.Save(document);
            }
            catch (Exception)
            {
                Directory.Delete(rootPath, true);
            }

            return characterItem;
        }

        public static void Delete(Project project, uint codePoint)
        {
            string rootPath = GetRootPath(project, codePoint);

            Directory.Delete(rootPath, true);
        }

        public static string FindNextFileName(Project project, uint codePoint)
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

            foreach (var item in _figureItems)
            {
                if (fileName.Equals(item.Name, StringComparison.OrdinalIgnoreCase))
                    throw new ApplicationException();
            }

            // Make the new item with a copy of the master figure.

            var figureItem = new FigureItem(figurePath, this);

            FigureDocument masterDoc = MasterFigureItem.Open();
            FigureDocument pieceDoc = new FigureDocument();

            pieceDoc.Figure = masterDoc.Figure;
            pieceDoc.Shapes = masterDoc.Shapes;
            figureItem.Save(pieceDoc);

            _figureItems.Add(figureItem);
            Project.NotifyItemModified(this);

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
                    _figureItems.RemoveAt(i);
                    Project.NotifyItemModified(this);
                    return;
                }
            }

            throw new ApplicationException();
        }

        internal void NotifyItemModified(FigureItem figureItem)
        {
            if (FigureItemModified != null)
            {
                var args = new FigureItemModifiedEventArgs(figureItem);

                FigureItemModified?.Invoke(this, args);
            }
        }
    }
}
