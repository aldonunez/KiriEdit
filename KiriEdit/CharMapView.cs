﻿using System;
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
                MessageBox.Show(message, ShellForm.AppTitle);
                return false;
            }

            return true;
        }

        private void AddCharacter(uint codePoint)
        {
            _charListItems.Add(MakeCharListItem(codePoint));
            SortCharacterList();

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

            Project.Characters.Delete(listItem.CodePoint);
        }

        private bool ConfirmDeleteCharacter(CharListItem listItem)
        {
            string message = string.Format("'{0}' will be deleted permanently.", listItem.CodePoint);
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
                // TEST:
                var listItem = (CharListItem) charListBox.SelectedItem;
                var charGrid = (CharacterGrid) splitContainer1.Panel2.Controls[0];
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

            // TEST:
            splitContainer1.Panel2.Padding = new Padding(20);
            var grid = new CharacterGrid();
            grid.Dock = DockStyle.Fill;
            grid.FontPath = Project.FontPath;
            grid.FaceIndex = Project.FaceIndex;
            splitContainer1.Panel2.Controls.Add(grid);

            // TEST:
            byte[] residencyMap = new byte[0x10000];
            residencyMap[0] = 0b00001111;
            residencyMap[1] = 0b00001111;
            residencyMap[2] = 0b00001111;

            residencyMap[3] = 0b11110000;
            residencyMap[4] = 0b11110000;
            residencyMap[5] = 0b11110000;
            grid.ResidencyMap = residencyMap;
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
