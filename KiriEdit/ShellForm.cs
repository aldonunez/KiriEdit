using KiriFT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace KiriEdit
{
    public partial class ShellForm : Form, IShell
    {
        // TODO: put this somewhere else
        public const string AppTitle = "KiriEdit";

        private Project _project;

        // TODO: rename to _docHost.
        private Control hostPanel;

        public ShellForm()
        {
            InitializeComponent();
            Text = AppTitle;

            hostPanel = documentContainer.MakeControl();
            hostPanel.Dock = DockStyle.Fill;
            this.Controls.Add(hostPanel);

            EnterNothingMode();

            KeyPress += ShellForm_KeyPress;
        }

        private void ShellForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'A' || e.KeyChar == 'a')
            {
                if (_project != null
                    && documentContainer.FindView(typeof(CharMapView)) == null)
                {
                    AddView(new CharMapView());
                }
            }
        }

        private void AddView(IView view)
        {
            view.Project = _project;
            view.Shell = this;

            documentContainer.AddView(view);
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

            documentContainer.Clear();

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

            IView[] dirtyViews = documentContainer.GetDirtyViews();

            if (_project.IsDirty || dirtyViews.Length > 0)
            {
                var message = "There are unsaved items. Save?";
                var result = MessageBox.Show(message, AppTitle, MessageBoxButtons.YesNoCancel);

                switch (result)
                {
                    case DialogResult.Yes:
                        // Save, then close.
                        // TODO:
                        //SaveItem();
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
            if (documentContainer.Count > 0)
            {
                documentContainer.CurrentView.Save();
            }
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

        public void OpenItem(object item)
        {
            var view = new FigureEditView();
            AddView(view);
        }
    }

    internal interface IApplication
    {
    }

    internal interface IView
    {
        IShell Shell { get; set; }
        Project Project { get; set; }
        Control Control { get; }
        string DocumentName { get; }
        bool IsDirty { get; }
        void Save();
    }
}
