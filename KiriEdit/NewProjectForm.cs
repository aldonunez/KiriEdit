using KiriFT;
using System;
using System.IO;
using System.Windows.Forms;

namespace KiriEdit
{
    public partial class NewProjectForm : Form
    {
        private const string FontFilter = 
            "TrueType files (*.ttf;*.ttc)|*.ttf;*.ttc|OpenType files (*.otf;*.otc)|*.otf;*.otc";

        private Library _library = new Library();
        private int _faceIndex;

        public ProjectSpec ProjectSpec { get; private set; }

        public NewProjectForm()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (!ValidateOK())
                return;

            ProjectSpec = PrepareProject();

            DialogResult = DialogResult.OK;
        }

        private ProjectSpec PrepareProject()
        {
            var projectSpec = new ProjectSpec();

            projectSpec.ProjectName = projNameTextBox.Text;
            projectSpec.ProjectPath = projPathTextBox.Text;
            projectSpec.FontPath = fontFamilyTextBox.Text;
            // TODO: face index

            return projectSpec;
        }

        private bool ValidateOK()
        {
            string fullProjFolder = Path.Combine(projPathTextBox.Text, projNameTextBox.Text);

            if (Directory.Exists(fullProjFolder))
            {
                string message = "The project directory already exists. Choose a different name or location for your project.";

                MessageBox.Show(message, MainForm.AppTitle);
                return false;
            }

            return true;
        }

        private Face GetFace(string path, int index)
        {
            return _library.OpenFace(path, index);
        }

        private bool ValidateFont(Face face)
        {
            if ((face.Flags & FaceFlags.Scalable) != FaceFlags.Scalable)
            {
                string message = string.Format(
                    "The font \"{0}\" is not supported.",
                    face.FamilyName);

                MessageBox.Show(message, MainForm.AppTitle);
                return false;
            }

            return true;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void projPathButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderSelect.FolderSelectDialog())
            {
                dialog.Title = "Project Location";

                if (!dialog.ShowDialog())
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

                Face face = null;

                try
                {
                    face = GetFace(dialog.FileName, 0);

                    if (face.FaceCount > 1)
                    {
                        string message = "The font file contains more than one face. Choose a face.";
                        MessageBox.Show(message, MainForm.AppTitle);
                        fontPathTextBox.Text = dialog.FileName;
                        _faceIndex = -1;
                        UpdateFaceList(dialog.FileName, face);
                        return;
                    }

                    if (!ValidateFont(face))
                        return;

                    fontPathTextBox.Text = dialog.FileName;
                    UpdateFaceInfo(dialog.FileName, face);
                }
                catch (FreeTypeException)
                {
                    string message = "The font file could not be loaded.";
                    MessageBox.Show(message, MainForm.AppTitle);
                    return;
                }
                finally
                {
                    if (face != null)
                        face.Dispose();
                }
            }
        }

        private void UpdateFaceList(string path, Face firstFace)
        {
            faceIndexComboBox.Items.Clear();
            faceIndexComboBox.Items.Add("");

            int faceCount = firstFace.FaceCount;

            for (int i = 0; i < faceCount; i++)
            {
                string familyName = firstFace.FamilyName;
                string styleName = firstFace.StyleName;

                if (i > 0)
                {
                    using (var face = _library.OpenFace(path, i))
                    {
                        familyName = face.FamilyName;
                        styleName = face.StyleName;
                    }
                }

                string item = string.Format("{0} - {1} ({2})", i, familyName, styleName);
                faceIndexComboBox.Items.Add(item);
            }

            faceIndexComboBox.SelectedIndex = _faceIndex + 1;
        }

        private void UpdateFaceInfo(string path, Face face)
        {
            fontFamilyTextBox.Text = face.FamilyName + " (" + face.StyleName + ")";
            copyrightTextBox.Text = "";

            uint count = face.GetSfntNameCount();

            for (uint i = 0; i < count; i++)
            {
                SfntName sfntName = face.GetSfntName(i);

                // TODO: what about the language ID?
                if (sfntName.NameId == 0)
                {
                    copyrightTextBox.Text = sfntName.String;
                    break;
                }
            }

            _faceIndex = face.FaceIndex;
            UpdateFaceList(path, face);
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

    public class ProjectSpec
    {
        public string ProjectName;
        public string ProjectPath;
        public string FontPath;
        public int FaceIndex;
    }
}
