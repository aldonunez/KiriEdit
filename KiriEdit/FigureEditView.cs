using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KiriEdit
{
    public partial class FigureEditView : UserControl, IView
    {
        public FigureEditView()
        {
            InitializeComponent();
        }

        public IShell Shell { get; set; }
        public Project Project { get; set; }

        public Control Control { get => this; }

        public string DocumentName { get; set; }

        public bool IsDirty { get; set; }

        public bool Save()
        {
            // TODO:
            return true;
        }
    }
}
