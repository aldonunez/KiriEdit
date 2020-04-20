/*
   Copyright 2020 Aldo J. Nunez

   Licensed under the Apache License, Version 2.0.
   See the LICENSE.txt file for details.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using KiriFT.Drawing;
using KiriProj;
using InteropServices = System.Runtime.InteropServices;

namespace KiriEdit
{
    public partial class CharMapView : Form, IView
    {
        [InteropServices.DllImport("getuname.dll", SetLastError = true, CharSet = InteropServices.CharSet.Unicode)]
        private static extern int GetUName(UInt16 wCharCode, StringBuilder lpbuf);

        [InteropServices.DllImport("kernel32.dll")]
        private static extern void SetLastError(int error);

        private const uint FirstCodePoint = '!';
        private const uint LastCodePoint = 0xFFFF;
        private const int CharSetSize = (int) (LastCodePoint - FirstCodePoint + 1);

        private Project _project;
        private List<CharListItem> _charListItems;
        private StringComparer _stringComparer = StringComparer.Ordinal;
        private PrivateFontCollection _fontCollection;
        private StringBuilder _unameBuilder = new StringBuilder(1024);

        // As of Windows 10.0.18363.657, the longest string returned by GetUName is 83 characters for en-US.

        public IShell Shell { get; set; }
        public object ProjectItem { get; set; }

        public Project Project
        {
            get => _project;
            set
            {
                _project = value;
                _project.CharacterItemModified += project_CharacterItemModified;
            }
        }

        public Form Form => this;
        public string DocumentTitle => Text;
        public bool IsDirty => false;
        public HistoryBuffer HistoryBuffer => null;

        public CharMapView()
        {
            InitializeComponent();
        }

        private void project_CharacterItemModified(object sender, CharacterItemModifiedEventArgs args)
        {
            var charItem = args.CharacterItem;

            foreach (var listItem in _charListItems)
            {
                if (listItem.CodePoint == charItem.CodePoint)
                {
                    listItem.Refresh();
                    break;
                }
            }
        }

        private void CharListBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                CharListBox_DoubleClick(sender, e);
            else if (e.KeyCode == Keys.Delete)
                deleteListCharButton_Click(sender, e);
        }

        private void CharListBox_DoubleClick(object sender, EventArgs e)
        {
            if (charListBox.SelectedItems.Count > 0)
            {
                var charListItem = (CharListItem) charListBox.SelectedItems[0];

                if (charListItem != null)
                    Shell.OpenItem(charListItem.CharacterItem);
            }
        }

        private void addListCharButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new InputCharacterForm())
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

        // TODO: Support characters outside of basic multilingual plane.

        private void AddCharacter(uint codePoint)
        {
            var item = Project.Characters.Add(codePoint);

            _charListItems.Add(MakeCharListItem(item));
            SortCharacterList();

            ModifyResidencyMap(charGrid.CharSet, codePoint, ResidencyAction.Add);
            charGrid.Invalidate();
        }

        private void deleteListCharButton_Click(object sender, EventArgs e)
        {
            if (charListBox.SelectedItems.Count > 0)
                DeleteListItem((CharListItem) charListBox.SelectedItems[0]);
        }

        private void DeleteListItem(CharListItem listItem)
        {
            if (!ConfirmDeleteCharacter(listItem))
                return;

            Project.Characters.Delete(listItem.CharacterItem);

            _charListItems.Remove(listItem);
            charListBox.Items.Remove(listItem);
            // No need to sort after deleting an item.

            ModifyResidencyMap(charGrid.CharSet, listItem.CodePoint, ResidencyAction.Remove);
            charGrid.Invalidate();
        }

        private bool ConfirmDeleteCharacter(CharListItem listItem)
        {
            string message = string.Format("'{0}' will be deleted permanently.", listItem.Text);
            DialogResult result = MessageBox.Show(message, ShellForm.AppTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

            if (result == DialogResult.OK)
                return true;

            return false;
        }

        private void charListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            deleteListCharButton.Enabled = charListBox.SelectedItems.Count > 0;

            if (charListBox.SelectedItems.Count > 0)
            {
                var listItem = (CharListItem) charListBox.SelectedItems[0];
                int index = charGrid.CharSet.MapToIndex(listItem.CodePoint);
                charGrid.SelectCharacter(index);
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

            // Add a special first entry to represent sorting by ordinal.

            sortComboBox.Items.Add(new OrdinalCultureItem());
            sortComboBox.Items.AddRange(cultures);
            sortComboBox.DisplayMember = "DisplayName";
            // Leave auto-complete turned off, because it's too slow to load.

            _fontCollection = new PrivateFontCollection();
            _fontCollection.AddFontFile(Project.FontPath);

            var fontFamily = FindFontFamily(_fontCollection);
            if (fontFamily == null)
                throw new ApplicationException();

            charGrid.Font = new Font(fontFamily, 12, (FontStyle) Project.FontStyle);
            fontNameLabel.Text = Project.FontName;

            int CharSetMapSize = SequentialCharSet.GetRecommendedMapSize(CharSetSize);
            int[] residencyMap = new int[CharSetMapSize];
            LoadResidencyMap(residencyMap);

            SequentialCharSet charSet = new SequentialCharSet(
                residencyMap,
                (int) FirstCodePoint,
                (int) LastCodePoint);
            charGrid.CharSet = charSet;
        }

        private FontFamily FindFontFamily(PrivateFontCollection collection)
        {
            foreach (var family in collection.Families)
            {
                if (family.Name == Project.FontFamily)
                    return family;
            }

            return null;
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

            foreach (var item in Project.Characters)
            {
                _charListItems.Add(MakeCharListItem(item));
            }

            SortCharacterList();
        }

        private void SortCharacterList()
        {
            _charListItems.Sort(CompareChars);

            ListViewItem selListItem = null;

            if (charListBox.SelectedItems.Count > 0)
                selListItem = charListBox.SelectedItems[0];

            charListBox.Items.Clear();
            charListBox.Items.AddRange(_charListItems.ToArray());

            if (selListItem != null)
                selListItem.Selected = true;

            charListBox.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            charListBox.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            columnHeader3.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private int CompareChars(CharListItem a, CharListItem b)
        {
            return _stringComparer.Compare(a.String, b.String);
        }

        private static CharListItem MakeCharListItem(CharacterItem item)
        {
            var text = MakeCharListItemText(item);
            return new CharListItem(text, item.CodePoint, item);
        }

        private static string MakeCharListItemText(CharacterItem item)
        {
            var codePoint = item.CodePoint;
            var text = string.Format("U+{0:X6}", codePoint);
            return text;
        }

        public bool Save()
        {
            // Nothing to do.
            return true;
        }

        // Assumes that the map is all zero.
        //
        private void LoadResidencyMap(int[] map)
        {
            foreach (var item in _charListItems)
            {
                int index = (int) (item.CodePoint - FirstCodePoint);
                int row = index / 32;
                int col = index % 32;

                map[row] |= (1 << col);
            }
        }

        private enum ResidencyAction
        {
            Add,
            Remove
        }

        private void ModifyResidencyMap(CharSet charSet, uint codePoint, ResidencyAction action)
        {
            int index = charSet.MapToIndex(codePoint);

            charSet.SetIncluded(index, action == ResidencyAction.Add);
        }

        private void findCharButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new InputCharacterForm())
            {
                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                int index = charGrid.CharSet.MapToIndex(dialog.CodePoint);
                charGrid.SelectCharacter(index);
            }
        }

        private void charGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = charGrid.SelectedIndex;
            if (index < 0)
                return;

            uint codePoint = charGrid.CharSet.MapToCodePoint(charGrid.SelectedIndex);
            string unicodeName = null;

            if (codePoint <= 0xFFFF)
            {
                SetLastError(0);
                int nameLength = GetUName((ushort) codePoint, _unameBuilder);

                if (InteropServices.Marshal.GetLastWin32Error() == 0)
                    unicodeName = _unameBuilder.ToString(0, nameLength);
            }

            if (unicodeName != null)
            {
                charDescriptionLabel.Text = string.Format("U+{0:X4}: {1}", codePoint, unicodeName);
            }
            else
            {
                charDescriptionLabel.Text = string.Format("U+{0:X4}", codePoint);
            }
        }

        private void addCharacterMenuItem_Click(object sender, EventArgs e)
        {
            int index = charGrid.SelectedIndex;
            if (index < 0)
                return;

            uint codePoint = charGrid.CharSet.MapToCodePoint(index);

            AddCharacter(codePoint);
        }

        private void deleteCharacterMenuItem_Click(object sender, EventArgs e)
        {
            CharListItem listItem = FindListItemByCharSetIndex();

            if (listItem != null)
                DeleteListItem(listItem);
        }

        private CharListItem FindListItemByCharSetIndex()
        {
            int index = charGrid.SelectedIndex;
            if (index < 0)
                return null;

            uint codePoint = charGrid.CharSet.MapToCodePoint(index);

            foreach (var listItem in _charListItems)
            {
                if (listItem.CodePoint == codePoint)
                    return listItem;
            }

            return null;
        }

        private void CharGrid_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int index = charGrid.SelectedIndex;
                if (index < 0)
                    return;

                uint codePoint = charGrid.CharSet.MapToCodePoint(index);

                if (Project.Characters.Contains(codePoint))
                {
                    addCharacterMenuItem.Enabled = false;
                    deleteCharacterMenuItem.Enabled = true;
                    editCharacterMenuItem.Enabled = true;
                }
                else
                {
                    addCharacterMenuItem.Enabled = true;
                    deleteCharacterMenuItem.Enabled = false;
                    editCharacterMenuItem.Enabled = false;
                }

                characterContextMenu.Show(charGrid, e.X, e.Y);
            }
        }

        private void CharGrid_DoubleClick(object sender, EventArgs e)
        {
            CharListItem listItem = FindListItemByCharSetIndex();

            if (listItem != null)
            {
                Shell.OpenItem(listItem.CharacterItem);
            }
        }

        private void editCharacterMenuItem_Click(object sender, EventArgs e)
        {
            CharGrid_DoubleClick(sender, e);
        }

        private void checkCompleteButton_Click(object sender, EventArgs e)
        {
            CalculateComplete();

            foreach (var listItem in _charListItems)
            {
                listItem.Refresh();
            }
        }

        private void CalculateComplete()
        {
            foreach (var charItem in Project.Characters)
            {
                FigureDocument masterDoc = charItem.MasterFigureItem.Open();

                var completionTool = new KiriFig.CompletionTool(masterDoc.Figure);

                foreach (var piece in charItem.PieceFigureItems)
                {
                    var document = piece.Open();

                    completionTool.AddComponentFigure(document.Figure);
                }

                if (completionTool.CalculateComplete())
                    charItem.Completion = CompletionState.Complete;
                else
                    charItem.Completion = CompletionState.Incomplete;
            }
        }


        #region Inner classes

        private class CharListItem : ListViewItem
        {
            public uint CodePoint;
            public string String;
            public CharacterItem CharacterItem;

            public CharListItem(string text, uint codePoint, CharacterItem characterItem)
            {
                Text = text;
                CodePoint = codePoint;
                String = CharUtils.GetString(codePoint);
                CharacterItem = characterItem;

                SubItems.Add(String);
                SubItems.Add(GetStatusString());
            }

            public void Refresh()
            {
                SubItems[2].Text = GetStatusString();
            }

            private string GetStatusString()
            {
                switch (CharacterItem.Completion)
                {
                    case CompletionState.Complete:
                        return "Complete";

                    case CompletionState.Incomplete:
                        return CharacterItem.PieceFigureItems.Count.ToString() + " (Incomplete)";

                    default:
                        return CharacterItem.PieceFigureItems.Count.ToString();
                }
            }

            public override string ToString()
            {
                return Text;
            }
        }

        #endregion
    }
}
