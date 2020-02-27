using KiriEdit.Font;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KiriEdit
{
    public partial class Form1 : Form
    {
        private const string AppTitle = "";
        private const int SampleFontSize = 20;

        private Manager _manager;

        public Form1()
        {
            InitializeComponent();

            _manager = new Manager();
        }

        private System.Drawing.Font ChooseFont()
        {
            using (FontDialog dialog = new FontDialog())
            {
                dialog.AllowSimulations = false;
                dialog.ShowEffects = false;
                dialog.MinSize = SampleFontSize;
                dialog.MaxSize = SampleFontSize;
                dialog.FontMustExist = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    return dialog.Font;
                }
            }

            return null;
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void newProjectMenuItem_Click(object sender, EventArgs e)
        {
            _manager.CloseProject();

            System.Drawing.Font font = ChooseFont();

            _manager.MakeProject(font.Name, (FontStyle) font.Style);
        }
    }
}
