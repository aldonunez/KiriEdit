using KiriEdit.Font;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KiriEdit
{
    public partial class MainForm : Form
    {
        private const string AppTitle = "KiriEdit";
        private const int SampleFontSize = 20;

        private Project _project;
        private IView _view;

        public MainForm()
        {
            InitializeComponent();
            EnterNothingMode();
            Text = AppTitle;
            this.FormClosing += MainForm_FormClosing;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!ConfirmCloseProject())
                e.Cancel = true;
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void newProjectMenuItem_Click(object sender, EventArgs e)
        {
            NewProject();
        }

        private void openProjectMenuItem_Click(object sender, EventArgs e)
        {
            OpenProject();
        }

        private void closeProjectMenuItem_Click(object sender, EventArgs e)
        {
            CloseProject();
        }

        private void saveItemMenuItem_Click(object sender, EventArgs e)
        {
            SaveItem();
        }

        private void saveAllMenuItem_Click(object sender, EventArgs e)
        {
            SaveItem();
            SaveProject();
        }

        private Project MakeSampleProject()
        {
            var project = new Project();

            project.Path = @"C:\Temp\sample.kiriproj";
            project.FontFamilyName = "Arial Black";
            project.FontStyle = FontStyle.Regular;

            return project;
        }

        private void NewProject()
        {
            //_manager.CloseProject();

            //System.Drawing.Font font = ChooseFont();

            //_manager.MakeProject(font.Name, (FontStyle) font.Style);

            var project = MakeSampleProject();

            EnterProjectMode(project);
        }

        private void OpenProject()
        {
            var project = MakeSampleProject();

            EnterProjectMode(project);
        }

        private void CloseProject()
        {
            if (!ConfirmCloseProject())
                return;

            EnterNothingMode();
        }

        private void EnterNothingMode()
        {
            closeProjectMenuItem.Enabled = false;
            saveAllMenuItem.Enabled = false;

            UpdateViewHostingState();
            Text = AppTitle;

            _project = null;
        }

        private void EnterProjectMode(Project project)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            closeProjectMenuItem.Enabled = true;
            saveAllMenuItem.Enabled = true;

            UpdateViewHostingState();
            string baseName = Path.GetFileNameWithoutExtension(project.Path);
            Text = baseName + " - " + AppTitle;

            _project = project;
        }

        private void UpdateViewHostingState()
        {
            if (hostPanel.Controls.Count == 0)
            {
                saveItemMenuItem.Enabled = false;
            }
            else
            {
                saveItemMenuItem.Enabled = true;
            }
        }

        private bool ConfirmCloseProject()
        {
            if (_project == null)
                return true;

            if (_project.IsDirty || (_view != null && _view.IsDirty))
            {
                var message = "There are unsaved items. Save?";
                var result = MessageBox.Show(message, AppTitle, MessageBoxButtons.YesNoCancel);

                switch (result)
                {
                    case DialogResult.Yes:
                        // Save, then close.
                        SaveItem();
                        SaveProject();
                        break;

                    case DialogResult.No:
                        // Close without saving.
                        break;

                    case DialogResult.Cancel:
                        return false;
                }
            }

            return true;
        }

        private void SaveItem()
        {
            if (_view == null)
                return;

            _view.Save();
        }

        private void SaveProject()
        {
            if (_project == null)
                return;

            // TODO: Save the project.
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
    }

    internal interface IApplication
    {
    }

    internal interface IView
    {
        Control Control { get; }
        string DocumentName { get; }
        bool IsDirty { get; }
        void Save();
    }
}
