using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;

namespace KiriEdit
{
    public partial class CharMapView : UserControl, IView
    {
        private List<CharListItem> _charListItems;
        private StringComparer _stringComparer = StringComparer.Ordinal;

        public Project Project { get; set; }
        public Control Control => this;
        public string DocumentName => "test";
        public bool IsDirty => false;

        public CharMapView()
        {
            InitializeComponent();
        }

        private void addListCharButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new NewCharacterForm())
            {
                dialog.ValidateChar += ValidateChar;

                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                AddCharacter(dialog.CodePoint);
            }
        }

        private bool ValidateChar(uint codePoint)
        {
            if (Project.Characters.Contains(codePoint))
            {
                string message = "The character is already included. Choose a different character.";
                MessageBox.Show(message, MainForm.AppTitle);
                return false;
            }

            return true;
        }

        private void AddCharacter(uint codePoint)
        {
            // TODO: put this in the character/shape/figure editor, or in CharacterItem.
#if false
            // AddMasterFigure(uint codePoint)

            string figurePath = "";

            Figure figure = FigureUtils.MakeMasterFigure(
                Project.FontPath,
                Project.FaceIndex,
                codePoint);

            using (var stream = File.Create(figurePath))
            using (var writer = new StreamWriter(stream))
            {
                FigureSerializer.Serialize(figure, writer);
            }
#endif

            var item = CharacterItem.Make(Project, codePoint);

            item.Save();

            _charListItems.Add(MakeCharListItem(codePoint));
            SortCharacterList();

            Project.Characters.Add(codePoint);
        }

        private void deleteListCharButton_Click(object sender, EventArgs e)
        {
            var listItem = (CharListItem) charListBox.SelectedItem;

            _charListItems.Remove(listItem);
            charListBox.Items.Remove(listItem);
            // No need to sort after deleting an item.

            Project.Characters.Remove(listItem.CodePoint);
        }

        private void charListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            deleteListCharButton.Enabled = charListBox.SelectedIndex >= 0;
        }

        private void CharMapView_Load(object sender, EventArgs e)
        {
            InitSortedCharacterList();
            charListBox_SelectedIndexChanged(charListBox, EventArgs.Empty);

            var cultures = CultureInfo.GetCultures(CultureTypes.NeutralCultures);

            Array.Sort(
                cultures,
                (a, b) => CultureInfo.CurrentUICulture.CompareInfo.Compare(a.DisplayName, b.DisplayName));

            sortComboBox.Items.Add(new OrdinalCultureItem());
            sortComboBox.Items.AddRange(cultures);
            sortComboBox.DisplayMember = "DisplayName";
        }

        private class OrdinalCultureItem
        {
            public string DisplayName { get => "Ordinal"; }
        }

        private void SortComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sortComboBox.SelectedIndex < 0)
                return;

            var listItem = sortComboBox.SelectedItem;

            if (sortComboBox.SelectedItem is OrdinalCultureItem)
                _stringComparer = StringComparer.Ordinal;
            else
                _stringComparer = ((CultureInfo) listItem).CompareInfo.GetStringComparer(CompareOptions.None);

            SortCharacterList();
        }

        private void InitSortedCharacterList()
        {
            Debug.Assert(_charListItems == null);

            _charListItems = new List<CharListItem>(Project.Characters.Count);

            foreach (uint codePoint in Project.Characters)
            {
                _charListItems.Add(MakeCharListItem(codePoint));
            }

            SortCharacterList();
        }

        private void SortCharacterList()
        {
            _charListItems.Sort(CompareChars);

            object selListItem = charListBox.SelectedItem;

            charListBox.Items.Clear();
            charListBox.Items.AddRange(_charListItems.ToArray());

            charListBox.SelectedItem = selListItem;
        }

        private int CompareChars(CharListItem a, CharListItem b)
        {
            return _stringComparer.Compare(a.String, b.String);
        }

        private static CharListItem MakeCharListItem(uint codePoint)
        {
            var text = string.Format("U+{0:X6} - {1}", codePoint, CharUtils.GetString(codePoint));
            return new CharListItem(text, codePoint);
        }

        public void Save()
        {
            // Nothing to do.
        }

        #region Inner classes

        private class CharListItem
        {
            public string Text;
            public uint CodePoint;
            public string String;

            public CharListItem(string text, uint codePoint)
            {
                Text = text;
                CodePoint = codePoint;
                String = CharUtils.GetString(codePoint);
            }

            public override string ToString()
            {
                return Text;
            }
        }

        #endregion
    }
}
