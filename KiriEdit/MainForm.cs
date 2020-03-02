using KiriFT;
using System;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace KiriEdit
{
    public partial class MainForm : Form
    {
        // TODO: put this somewhere else
        public const string AppTitle = "KiriEdit";

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
            //project.FontFamilyName = "Arial Black";
            //project.FontStyle = FontStyle.Regular;

            return project;
        }

        private void NewProject()
        {
            if (!CloseProject())
                return;

            ProjectSpec projectSpec = null;

            using (var dialog = new NewProjectForm())
            {
                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                projectSpec = dialog.ProjectSpec;
            }

            Project project = MakeProject(projectSpec);

            EnterProjectMode(project);
        }

        private Project MakeProject(ProjectSpec spec)
        {
            // Figure out paths and names.

            string projectFolderPath = Path.Combine(spec.ProjectLocation, spec.ProjectName);
            string projectFileName = spec.ProjectName + ".kiriproj";
            string projectFilePath = Path.Combine(projectFolderPath, projectFileName);
            string fontFileName = Path.GetFileName(spec.FontPath);
            string importedFontPath = Path.Combine(projectFolderPath, fontFileName);

            // Set up the project object.

            var project = new Project();

            project.FontPath = fontFileName;
            project.FaceIndex = spec.FaceIndex;
            project.GlyphListPath = "glyphs.kiriglyf";
            project.FigureFolderPath = "figures";

            // Runtime properties.
            project.Path = projectFilePath;

            // Commit everything to the file system.

            DirectoryInfo dirInfo = null;

            dirInfo = Directory.CreateDirectory(projectFolderPath);
            dirInfo.CreateSubdirectory(project.FigureFolderPath);
            File.Copy(spec.FontPath, importedFontPath);
            File.Create(project.GlyphListPath);

            SaveProject(project);

            return project;
        }

        private void OpenProject()
        {
            if (!CloseProject())
                return;

            var path = ChooseProject();
            if (path == null)
                return;

            var project = LoadProject(path);

            if (!ValidateProject(project))
                return;

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

        private void SaveProject(Project project)
        {
            var writerOptions = new JsonWriterOptions();
            writerOptions.Indented = true;

            var serializerOptions = new JsonSerializerOptions();
            serializerOptions.AllowTrailingCommas = true;

            using (var stream = File.OpenWrite(project.Path))
            using (var writer = new Utf8JsonWriter(stream, writerOptions))
            {
                JsonSerializer.Serialize(writer, project, serializerOptions);
            }
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

        private Project LoadProject(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                var task = JsonSerializer.DeserializeAsync<Project>(stream);
                Project project = task.Result;
                project.Path = path;
                return project;
            }
        }

        private bool ValidateProject(Project project)
        {
            if (!File.Exists(project.FullFontPath)
                || !Directory.Exists(project.FullFiguresFolderPath)
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
                    face = lib.OpenFace(project.FullFontPath, project.FaceIndex);
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
            MessageBox.Show(message, AppTitle);
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
