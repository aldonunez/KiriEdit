using System;
using System.Windows.Forms;

namespace KiriEdit
{
    public partial class NewProjectForm : Form
    {
        private const string FontFilter = 
            "TrueType files (*.ttf;*.ttc)|*.ttf;*.ttc|OpenType files (*.otf;*.otc)|*.otf;*.otc";

        public NewProjectForm()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            // TODO: prepare info for new project.

            DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void projPathButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog())
            {
                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                projPathTextBox.Text = dialog.FileName;
            }
        }

        private void projPathTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateOKButton();
        }

        private void projNameTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateOKButton();
        }

        private void fontPathButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.CheckFileExists = true;
                dialog.Filter = FontFilter;

                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                fontPathTextBox.Text = dialog.FileName;
            }
        }

        private void fontPathTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateOKButton();
        }

        private void UpdateOKButton()
        {
            if (fontPathTextBox.TextLength > 0
                && projNameTextBox.TextLength > 0
                && projPathTextBox.TextLength > 0)
            {
                okButton.Enabled = true;
            }
            else
            {
                okButton.Enabled = false;
            }
        }
    }
}
