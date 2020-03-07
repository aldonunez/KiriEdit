using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.Windows.Forms;

namespace KiriEdit
{
    public partial class CharMapView : UserControl, IView
    {
        private List<CharListItem> _charListItems;
        private StringComparer _stringComparer = StringComparer.Ordinal;
        private PrivateFontCollection _fontCollection;

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
                MessageBox.Show(message, ShellForm.AppTitle);
                return false;
            }

            return true;
        }

        private void AddCharacter(uint codePoint)
        {
            _charListItems.Add(MakeCharListItem(codePoint));
            SortCharacterList();

            ModifyResidencyMap(charGrid.ResidencyMap, codePoint, ResidencyAction.Add);
            charGrid.Refresh();

            Project.Characters.Add(codePoint);
        }

        private void deleteListCharButton_Click(object sender, EventArgs e)
        {
            var listItem = (CharListItem) charListBox.SelectedItem;

            if (!ConfirmDeleteCharacter(listItem))
                return;

            _charListItems.Remove(listItem);
            charListBox.Items.Remove(listItem);
            // No need to sort after deleting an item.

            ModifyResidencyMap(charGrid.ResidencyMap, listItem.CodePoint, ResidencyAction.Remove);
            charGrid.Refresh();

            Project.Characters.Delete(listItem.CodePoint);
        }

        private bool ConfirmDeleteCharacter(CharListItem listItem)
        {
            string message = string.Format("'{0}' will be deleted permanently.", listItem.Text);
            DialogResult result = MessageBox.Show(message, ShellForm.AppTitle, MessageBoxButtons.OKCancel);

            if (result == DialogResult.OK)
                return true;

            return false;
        }

        private void charListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            deleteListCharButton.Enabled = charListBox.SelectedIndex >= 0;

            if (charListBox.SelectedIndex >= 0)
            {
                var listItem = (CharListItem) charListBox.SelectedItem;
                charGrid.ScrollTo(listItem.CodePoint);
            }
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
            // Leave auto-complete turned off, because it's too slow to load.

            _fontCollection = new PrivateFontCollection();
            _fontCollection.AddFontFile(Project.FontPath);
            // TODO: what about the face index and the style?

            charGrid.Font = new Font(_fontCollection.Families[0], 12);

            byte[] residencyMap = new byte[0x2000];
            LoadResidencyMap(residencyMap);
            charGrid.ResidencyMap = residencyMap;
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

        // Assumes that the map is all zero.
        //
        private void LoadResidencyMap(byte[] map)
        {
            foreach (var item in _charListItems)
            {
                ModifyResidencyMap(map, item.CodePoint, ResidencyAction.Add);
            }
        }

        private enum ResidencyAction
        {
            Add,
            Remove
        }

        private void ModifyResidencyMap(byte[] map, uint codePoint, ResidencyAction action)
        {
            int value = (int) codePoint - '!';

            if (value >= map.Length)
                return;

            int row = value / 20;
            int col = value % 20;

            int mapRow = row;
            int mapCol = col / 8;
            int mapBit = col % 8;

            int byteOffset = (mapRow * 3) + mapCol;

            if (action == ResidencyAction.Add)
                map[byteOffset] |= (byte) (1 << mapBit);
            else
                map[byteOffset] &= (byte) ~(1 << mapBit);
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
