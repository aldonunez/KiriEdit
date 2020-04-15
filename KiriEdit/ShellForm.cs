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
    public partial class ShellForm : Form, IShell
    {
        // TODO: put this somewhere else
        public const string AppTitle = "KiriEdit";

        private Project _project;
        private MdiDocumentContainer _documentContainer;
        private ToolStripMenuItem[] _windowListMenuItems;
        private Dictionary<object, IView> _runningDocTable = new Dictionary<object, IView>();

        public ShellForm()
        {
            InitializeComponent();
            Text = AppTitle;

            _windowListMenuItems = new ToolStripMenuItem[]
                {
                    window0MenuItem,
                    window1MenuItem,
                    window2MenuItem,
                    window3MenuItem,
                };

            _documentContainer = new MdiDocumentContainer(this);
            _documentContainer.ViewsChanged += _documentContainer_ViewsChanged;
            _documentContainer.ViewActivate += _documentContainer_ViewActivate;
            _documentContainer.HistoryChanged += _documentContainer_HistoryChanged;

            EnterNothingMode();
        }

        private void _documentContainer_HistoryChanged(object sender, EventArgs e)
        {
            UpdateHistoryMenus();
        }

        private void WindowMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            RebuildWindowListMenu();
        }

        private void _documentContainer_ViewActivate(object sender, EventArgs e)
        {
            UpdateViewHostingState();
            UpdateHistoryMenus();
        }

        private void _documentContainer_ViewsChanged(object sender, ViewsChangedEventArgs e)
        {
            UpdateViewHostingState();

            if (e.View.ProjectItem != null)
            {
                if (e.Action == ViewsChangedAction.Added)
                {
                    _runningDocTable.Add(e.View.ProjectItem, e.View);
                }
                else if (e.Action == ViewsChangedAction.Removed)
                {
                    _runningDocTable.Remove(e.View.ProjectItem);
                }
            }
        }

        private void UpdateHistoryMenus()
        {
            if (_documentContainer.Count == 0 || _documentContainer.CurrentView.HistoryBuffer == null)
            {
                redoMenuItem.Enabled = false;
                undoMenuItem.Enabled = false;
            }
            else
            {
                var history = _documentContainer.CurrentView.HistoryBuffer;

                redoMenuItem.Enabled = history.RedoCount > 0;
                undoMenuItem.Enabled = history.UndoCount > 0;
            }
        }

        private void RebuildWindowListMenu()
        {
            IEnumerator<IView> viewEnumerator = _documentContainer.EnumerateViews();
            int i = 0;

            for (; i < _windowListMenuItems.Length; i++)
            {
                if (!viewEnumerator.MoveNext())
                    break;

                var view = viewEnumerator.Current;

                _windowListMenuItems[i].Tag = view;
                _windowListMenuItems[i].Visible = true;
                _windowListMenuItems[i].Text = string.Format("{0} {1}", i + 1, view.DocumentTitle);
            }

            if (i > 0)
                _windowListMenuItems[0].Checked = true;

            for (; i < _windowListMenuItems.Length; i++)
            {
                _windowListMenuItems[i].Visible = false;
            }
        }

        private void AddView(IView view)
        {
            view.Project = _project;
            view.Shell = this;

            _documentContainer.AddView(view);
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
            SaveAll();
        }

        private bool SaveAll()
        {
            IView[] dirtyViews = _documentContainer.GetDirtyViews();

            return SaveAll(dirtyViews);
        }

        private bool SaveAll(IEnumerable<IView> dirtyViews)
        {
            foreach (var view in dirtyViews)
            {
                if (!view.Save())
                    return false;
            }

            SaveProject();
            return true;
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
            characterMapMenuItem.Enabled = false;

            _documentContainer.Clear();

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
            characterMapMenuItem.Enabled = true;

            UpdateViewHostingState();
            string baseName = project.Name;
            Text = baseName + " - " + AppTitle;

            _project = project;

            AddView(new CharMapView());
        }

        private void UpdateViewHostingState()
        {
            if (_documentContainer.Count == 0)
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

            IView[] dirtyViews = _documentContainer.GetDirtyViews();

            if (_project.IsDirty || dirtyViews.Length > 0)
            {
                var message = "There are unsaved items. Save?";
                var result = MessageBox.Show(message, AppTitle, MessageBoxButtons.YesNoCancel);

                switch (result)
                {
                    case DialogResult.Yes:
                        // Save, then close.
                        if (!SaveAll(dirtyViews))
                            return false;
                        break;

                    case DialogResult.No:
                        // Close without saving.
                        break;

                    default:
                        return false;
                }
            }

            return true;
        }

        private void SaveItem()
        {
            if (_documentContainer.Count > 0)
            {
                _documentContainer.CurrentView.Save();
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
            IView view;

            if (_runningDocTable.TryGetValue(item, out view))
            {
                _documentContainer.Activate(view);
            }
            else
            {
                if (item is CharacterItem)
                    view = new CharEditView();
                else if (item is FigureItem)
                    view = new FigureEditView();
                else
                    throw new ApplicationException();

                view.ProjectItem = item;
                AddView(view);
            }
        }

        private void characterMapMenuItem_Click(object sender, EventArgs e)
        {
            IView view = _documentContainer.FindView(typeof(CharMapView));

            if (view == null)
            {
                AddView(new CharMapView());
            }
            else
            {
                _documentContainer.Activate(view);
            }
        }

        private void closeAllDocumentsMenuItem_Click(object sender, EventArgs e)
        {
            _documentContainer.Clear();
        }

        private void windowXMenuItem_Click(object sender, EventArgs e)
        {
            var menuItem = (ToolStripMenuItem) sender;

            if (menuItem.Tag == null)
                return;

            _documentContainer.Activate((IView) menuItem.Tag);
        }

        private void redoMenuItem_Click(object sender, EventArgs e)
        {
            if (_documentContainer.Count > 0 && _documentContainer.CurrentView.HistoryBuffer != null)
                _documentContainer.CurrentView.HistoryBuffer.Redo();
        }

        private void undoMenuItem_Click(object sender, EventArgs e)
        {
            if (_documentContainer.Count > 0 && _documentContainer.CurrentView.HistoryBuffer != null)
                _documentContainer.CurrentView.HistoryBuffer.Undo();
        }
    }
}
