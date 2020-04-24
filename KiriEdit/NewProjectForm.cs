/*
   Copyright 2020 Aldo J. Nunez

   Licensed under the Apache License, Version 2.0.
   See the LICENSE.txt file for details.
*/

using KiriFT;
using KiriProj;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace KiriEdit
{
    public partial class NewProjectForm : Form
    {
        private const string FontFilter =
            "TrueType and OpenType files (*.ttf;*.ttc;*.otf;*.otc)|*.ttf;*.ttc;*.otf;*.otc"
            + "|TrueType files (*.ttf;*.ttc)|*.ttf;*.ttc"
            + "|OpenType files (*.otf;*.otc)|*.otf;*.otc"
            ;

        private Library _library = new Library();

        public ProjectSpec ProjectSpec { get; private set; }

        public NewProjectForm()
        {
            InitializeComponent();
        }

        private void FaceIndexComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Includes the empty entry (0).
            if (typefaceComboBox.SelectedIndex < 1)
            {
                copyrightTextBox.Text = "";
                infoLinkLabel.Enabled = false;
            }
            else
            {
                var item = (FontListItem) typefaceComboBox.SelectedItem;
                copyrightTextBox.Text = item.FontInfo.Copyright;
                infoLinkLabel.Enabled = true;
                infoLinkLabel.LinkVisited = item.SawInfo;
            }

            UpdateOKButton();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (!ValidateOK())
                return;

            ProjectSpec = PrepareProjectSpec();

            DialogResult = DialogResult.OK;
        }

        private ProjectSpec PrepareProjectSpec()
        {
            var projectSpec = new ProjectSpec();

            projectSpec.ProjectName = projNameTextBox.Text;
            projectSpec.ProjectLocation = projPathTextBox.Text;
            projectSpec.FontPath = fontPathTextBox.Text;
            projectSpec.FaceIndex = ((FontListItem) typefaceComboBox.SelectedItem).FaceIndex;

            return projectSpec;
        }

        private bool ValidateOK()
        {
            string fullProjFolder = Path.Combine(projPathTextBox.Text, projNameTextBox.Text);

            if (Directory.Exists(fullProjFolder))
            {
                string message = "The project directory already exists. Choose a different name or location for your project.";

                MessageBox.Show(message, ShellForm.AppTitle);
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
                    // Ignore the exception, so we can get the next face.
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
                    MessageBox.Show(message, ShellForm.AppTitle);
                }
                else
                {
                    fontPathTextBox.Text = dialog.FileName;
                    UpdateFaceList(items);

                    if (items.Length > 1)
                    {
                        string message = "The font contains more than one typeface. Choose a typeface.";
                        MessageBox.Show(message, ShellForm.AppTitle);
                        typefaceComboBox.SelectedIndex = 0;
                    }
                    else
                    {
                        typefaceComboBox.SelectedIndex = 1;
                    }
                }
            }
        }

        private void UpdateFaceList(FontListItem[] items)
        {
            typefaceComboBox.Enabled = true;
            typefaceComboBox.Items.Clear();
            typefaceComboBox.Items.Add("");
            typefaceComboBox.Items.AddRange(items);

            if (items.Length <= 1)
                typefaceComboBox.Enabled = false;
        }

        private void fontPathTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateOKButton();
        }

        private void UpdateOKButton()
        {
            if (fontPathTextBox.TextLength > 0
                && typefaceComboBox.SelectedIndex > 0
                && projNameTextBox.TextLength > 0
                && projPathTextBox.TextLength > 0
                && Path.IsPathRooted(projPathTextBox.Text)
                )
            {
                okButton.Enabled = true;
            }
            else
            {
                okButton.Enabled = false;
            }
        }

        private void infoLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var listItem = (FontListItem) typefaceComboBox.SelectedItem;

            listItem.SawInfo = true;
            infoLinkLabel.LinkVisited = true;

            using (var form = new FontInfoForm())
            {
                form.FontInfo = listItem.FontInfo;
                form.ShowDialog();
            }
        }


        #region Inner classes

        private class FontListItem
        {
            private string _itemText;

            public int FaceIndex { get; }
            public FontInfo FontInfo { get; }
            public bool SawInfo { get; set; }

            public FontListItem(Face face)
            {
                FaceIndex = face.FaceIndex;
                _itemText = string.Format("{0} ({1})", face.FamilyName, face.StyleName);

                FontInfo = new FontInfo(face);
            }

            public override string ToString()
            {
                return _itemText;
            }
        }

        #endregion
    }
}
