using System;
using System.Windows.Forms;
using TryFreetype.Model;
using TryFreetype;
using System.IO;

namespace KiriEdit
{
    public partial class CharMapView : UserControl
    {
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

                AddMasterFigure(dialog.CodePoint);
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

        private void AddMasterFigure(uint codePoint)
        {
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

            Character character = new Character(codePoint);

            Project.Characters.Add(character);
        }

        private void deleteListCharButton_Click(object sender, EventArgs e)
        {

        }
    }
}
