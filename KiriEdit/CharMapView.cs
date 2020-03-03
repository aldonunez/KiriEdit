using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;

namespace KiriEdit
{
    public partial class CharMapView : UserControl
    {
        private List<CharListItem> _charListItems;

        public Project Project { get; set; }

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

            CharacterItem.Make(Project, codePoint);

            charListBox.Items.Add(MakeCharListItem(codePoint));
            SortCharacterList();

            Project.Characters.Add(codePoint);
        }

        private void deleteListCharButton_Click(object sender, EventArgs e)
        {
            var listItem = (CharListItem) charListBox.SelectedItem;

            _charListItems.Remove(listItem);
            SortCharacterList();

            Project.Characters.Remove(listItem.CodePoint);
        }

        private void charListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            deleteListCharButton.Enabled = charListBox.SelectedIndex >= 0;
        }

        private void CharMapView_Load(object sender, EventArgs e)
        {
            InitSortedCharacterList();
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
            // TODO: sort items according to language

            charListBox.Items.Clear();
            charListBox.Items.AddRange(_charListItems.ToArray());
        }

        private static CharListItem MakeCharListItem(uint codePoint)
        {
            var text = string.Format("U+{0:X4} - {1}", codePoint, CharUtils.GetString(codePoint));
            return new CharListItem(text, codePoint);
        }

        #region Inner classes

        private class CharListItem
        {
            public string Text;
            public uint CodePoint;

            public CharListItem(string text, uint codePoint)
            {
                Text = text;
                CodePoint = codePoint;
            }

            public override string ToString()
            {
                return Text;
            }
        }

        #endregion
    }
}
