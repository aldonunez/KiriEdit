using KiriEdit.Font;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KiriEdit
{
    public partial class Form1 : Form
    {
        private int SampleFontSize = 20;

        public Form1()
        {
            InitializeComponent();

            VisibleChanged += Form1_VisibleChanged;
        }

        private void Form1_VisibleChanged(object sender, EventArgs e)
        {
            var findTask = FontFinder.FindFontsAsync();

            using (FontDialog dialog = new FontDialog())
            {
                dialog.AllowSimulations = false;
                dialog.ShowEffects = false;
                dialog.MinSize = SampleFontSize;
                dialog.MaxSize = SampleFontSize;
                dialog.FontMustExist = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    findTask.Wait();
                    var family = findTask.Result[dialog.Font.Name];
                }
            }
        }
    }
}
