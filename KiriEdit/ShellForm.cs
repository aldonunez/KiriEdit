using KiriFT;
using System;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace KiriEdit
{
    public partial class ShellForm : Form
    {
        // TODO: put this somewhere else
        public const string AppTitle = "KiriEdit";

        private Project _project;
        private IView _view;

        public ShellForm()
        {
            InitializeComponent();
            EnterNothingMode();
            Text = AppTitle;

            KeyPress += ShellForm_KeyPress;
        }

        private void ShellForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'A' || e.KeyChar == 'a')
            {
                if (_project == null || _view != null)
                    return;

                SetView(new CharMapView());
            }
        }

        private void SetView(IView view)
        {
            view.Project = _project;
            hostPanel.Controls.Add(view.Control);
            view.Control.Dock = DockStyle.Fill;
            _view = view;
        }

        private void ShellForm_FormClosing(object sender, FormClosingEventArgs e)
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

        private void NewProject()
        {
            ProjectSpec projectSpec = null;

            using (var dialog = new NewProjectForm())
            {
                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                projectSpec = dialog.ProjectSpec;
            }

            Project project = Project.Make(projectSpec);

            EnterProjectMode(project);
        }

        private void OpenProject()
        {
            var path = ChooseProject();
            if (path == null)
                return;

            var project = Project.Open(path);

            // TODO: Validate in Project.Open
            //if (!ValidateProject(project))
            //    return;

            EnterProjectMode(project);
        }

        private bool CloseProject()
        {
            if (!ConfirmCloseProject())
                return false;

            EnterNothingMode();
            return true;
        }

        private void EnterNothingMode()
        {
            newProjectMenuItem.Enabled = true;
            openProjectMenuItem.Enabled = true;
            closeProjectMenuItem.Enabled = false;
            saveAllMenuItem.Enabled = false;

            if (_view != null)
            {
                hostPanel.Controls.Clear();
                _view.Control.Dispose();
                _view = null;
            }

            UpdateViewHostingState();
            Text = AppTitle;

            _project = null;
        }

        private void EnterProjectMode(Project project)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            newProjectMenuItem.Enabled = false;
            openProjectMenuItem.Enabled = false;
            closeProjectMenuItem.Enabled = true;
            saveAllMenuItem.Enabled = true;

            UpdateViewHostingState();
            string baseName = project.Name;
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

            _project.Save();
        }

        private string ChooseProject()
        {
            using (var dialog = new OpenFileDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                    return dialog.FileName;
            }

            return null;
        }

        private bool ValidateProject(Project project)
        {
            if (!File.Exists(project.FontPath)
                || !Directory.Exists(project.CharactersFolderPath)
                )
            {
                ShowBadProjectMessage();
                return false;
            }

            using (var lib = new Library())
            {
                Face face = null;

                try
                {
                    face = lib.OpenFace(project.FontPath, project.FaceIndex);
                    if ((face.Flags & FaceFlags.Scalable) != FaceFlags.Scalable)
                    {
                        ShowBadProjectMessage();
                        return false;
                    }
                }
                catch (FreeTypeException)
                {
                    ShowBadProjectMessage();
                    return false;
                }
                finally
                {
                    if (face != null)
                        face.Dispose();
                }
            }

            return true;
        }

        private void ShowBadProjectMessage()
        {
            string message = "The project could not be loaded due to missing files or settings.";
            MessageBox.Show(message, AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    internal interface IApplication
    {
    }

    internal interface IView
    {
        Project Project { get; set; }
        Control Control { get; }
        string DocumentName { get; }
        bool IsDirty { get; }
        void Save();
    }
}
