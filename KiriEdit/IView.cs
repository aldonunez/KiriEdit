using KiriProj;
using System.Windows.Forms;

namespace KiriEdit
{
    internal interface IView
    {
        IShell Shell { get; set; }
        Project Project { get; set; }
        object ProjectItem { get; set; }

        Form Form { get; }
        string DocumentTitle { get; }
        bool IsDirty { get; }

        bool Save();
    }
}
