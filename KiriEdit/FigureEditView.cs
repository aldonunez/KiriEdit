using System;
using System.Drawing;
using System.Windows.Forms;

namespace KiriEdit
{
    public partial class FigureEditView : Form, IView
    {
        private CharacterItem _characterItem;
        private FigureDocument _document;

        public FigureEditView()
        {
            InitializeComponent();
        }

        public IShell Shell { get; set; }
        public Project Project { get; set; }

        public Form Form { get => this; }

        public string DocumentName { get; set; }

        public bool IsDirty { get; set; }
        public object ProjectItem
        {
            get => _characterItem;
            set
            {
                if (!(value is CharacterItem))
                    throw new ArgumentException();

                _characterItem = (CharacterItem) value;
            }
        }

        public bool Save()
        {
            // TODO:
            return true;
        }

        private void FigureEditView_Load(object sender, EventArgs e)
        {
            _document = _characterItem.MasterFigureItem.Open();
        }

        private void masterPictureBox_Paint(object sender, PaintEventArgs e)
        {
            Size picBoxSize = masterPictureBox.ClientSize;
            int height = (int) (picBoxSize.Height * 0.80f);
            int width = (int) (height * 32f / 37f);

            Rectangle rect = new Rectangle(
                (int) (picBoxSize.Width - width) / 2,
                (int) (picBoxSize.Height - height) / 2,
                width,
                height);

            using (var painter = new SystemFigurePainter(_document.Figure, e.Graphics, rect))
            {
                painter.Paint();
            }
        }
    }
}
