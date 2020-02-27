using KiriEdit.Font;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiriEdit
{
    internal class Manager
    {
        public Project Project { get; private set; }

        public bool IsProjectOpen { get => Project != null; }

        public void CloseProject()
        {
            throw new NotImplementedException();
        }

        public void MakeProject(string familyName, FontStyle fontStyle)
        {
            if (IsProjectOpen)
                throw new ApplicationException();

            var project = new Project();

            project.FontFamilyName = familyName;
            project.FontStyle = fontStyle;

            ValidateProject(project);

            Project = project;
        }

        public void OpenProject(string projectPath)
        {
            Project project = null;

            // TODO: load

            ValidateProject(project);
        }

        public void SaveProject(string projectPath)
        {
            throw new NotImplementedException();
        }

        private void ValidateProject(Project project)
        {
            throw new NotImplementedException();
        }

        private void FindFont()
        {
            FontFace face = null;
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

                    var fontCollection = findTask.Result;
                    FontFamily family;

                    if (fontCollection.TryGetValue(dialog.Font.Name, out family))
                    {
                        face = family.GetFace((FontStyle) dialog.Font.Style);
                    }

                    if (face == null)
                    {
                        string message = string.Format(
                            "The font {0} ({1}) is not supported.",
                            dialog.Font.Name, dialog.Font.Style);

                        MessageBox.Show(message, AppTitle);
                    }
                }
            }
        }
    }
}
