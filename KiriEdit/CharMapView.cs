using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KiriEdit.Model;
using TryFreetype.Model;
using TryFreetype;
using System.IO;

namespace KiriEdit
{
    public partial class CharMapView : UserControl
    {
        public CharMapView()
        {
            InitializeComponent();
        }

        private void addListCharButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new NewCharacterForm())
            {
                // TODO:
                ProjectManager m = new ProjectManager();


                dialog.ValidateChar += (uint codePoint) => !m.Characters.Contains(codePoint);

                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                AddMasterFigure(dialog.CodePoint);
            }
        }

        private void AddMasterFigure(uint codePoint)
        {
            // TODO:
            ProjectManager m = new ProjectManager();


            string figurePath = "";

            Figure figure = FigureUtils.MakeMasterFigure(
                m.Project.FontPath,
                m.Project.FaceIndex,
                codePoint);

            using (var stream = File.Create(figurePath))
            using (var writer = new StreamWriter(stream))
            {
                FigureSerializer.Serialize(figure, writer);
            }

            Character character = new Character(codePoint);

            m.Characters.Add(character);
        }

        private void deleteListCharButton_Click(object sender, EventArgs e)
        {

        }
    }
}
