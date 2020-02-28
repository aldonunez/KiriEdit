using KiriEdit.Font;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiriEdit
{
#if false
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
    }
#endif
}
