using KiriFT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace KiriEdit
{
    public partial class NewProjectForm : Form
    {
        private class FontListItem
        {
            private string _itemText;

            public int FaceIndex { get; }
            public string Copyright { get; }

            public FontListItem(Face face)
            {
                FaceIndex = face.FaceIndex;
                _itemText = string.Format("{0} ({1})", face.FamilyName, face.StyleName);

                uint count = face.GetSfntNameCount();

                for (uint i = 0; i < count; i++)
                {
                    SfntName sfntName = face.GetSfntName(i);

                    // TODO: what about the language ID?
                    if (sfntName.NameId == 0)
                    {
                        Copyright = sfntName.String;
                        break;
                    }
                }
            }

            public override string ToString()
            {
                return _itemText;
            }
        }

        private const string FontFilter = 
            "TrueType files (*.ttf;*.ttc)|*.ttf;*.ttc|OpenType files (*.otf;*.otc)|*.otf;*.otc";

        private Library _library = new Library();

        public ProjectSpec ProjectSpec { get; private set; }

        public NewProjectForm()
        {
            InitializeComponent();
        }

        private void FaceIndexComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Includes the empty entry (0)
            if (faceIndexComboBox.SelectedIndex < 1)
            {
                fontFamilyTextBox.Text = "";
                copyrightTextBox.Text = "";
            }
            else
            {
                var item = (FontListItem) faceIndexComboBox.SelectedItem;
                fontFamilyTextBox.Text = item.ToString();
                copyrightTextBox.Text = item.Copyright;
            }
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

        private int GetFaceCount(string path)
        {
            Face face = null;

            try
            {
                face = _library.OpenFace(path, -1);
                return face.FaceCount;
            }
            catch (FreeTypeException)
            {
                return 0;
            }
            finally
            {
                if (face != null)
                    face.Dispose();
            }
        }

        private FontListItem[] GetFaces(string path)
        {
            int faceCount = GetFaceCount(path);
            var items = new List<FontListItem>();

            for (int i = 0; i < faceCount; i++)
            {
                try
                {
                    using (var face = _library.OpenFace(path, i))
                    {
                        if (RawValidateFont(face))
                            items.Add(new FontListItem(face));
                    }
                }
                catch (FreeTypeException)
                {
                    // Ignore the exception, sowe can get the next face.
                }
            }

            return items.ToArray();
        }

        private bool RawValidateFont(Face face)
        {
            return (face.Flags & FaceFlags.Scalable) == FaceFlags.Scalable;
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
                dialog.Title = "Font Location";
                dialog.CheckFileExists = true;
                dialog.Filter = FontFilter;

                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                FontListItem[] items = GetFaces(dialog.FileName);

                if (items.Length == 0)
                {
                    string message = "There are no supported typefaces in the font.";
                    MessageBox.Show(message, MainForm.AppTitle);
                }
                else
                {
                    fontPathTextBox.Text = dialog.FileName;
                    UpdateFaceList(items);

                    if (items.Length > 1)
                    {
                        string message = "The font contains more than one typeface. Choose a typeface.";
                        MessageBox.Show(message, MainForm.AppTitle);
                        faceIndexComboBox.SelectedIndex = 0;
                    }
                    else
                    {
                        faceIndexComboBox.SelectedIndex = 1;
                    }
                }
            }
        }

        private void UpdateFaceList(FontListItem[] items)
        {
            faceIndexComboBox.Enabled = true;
            faceIndexComboBox.Items.Clear();
            faceIndexComboBox.Items.Add("");
            faceIndexComboBox.Items.AddRange(items);

            if (items.Length <= 1)
                faceIndexComboBox.Enabled = false;
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
