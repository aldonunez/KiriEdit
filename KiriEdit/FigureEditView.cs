using System;
using System.Drawing;
using System.Windows.Forms;

namespace KiriEdit
{
    public partial class FigureEditView : Form, IView
    {
        private FigureItem _figureItem;
        private FigureDocument _document;

        public FigureEditView()
        {
            // The left pane of the SplitContainer has a docked panel that holds all of the other
            // controls, not counting the ToolStrip.
            //
            // I had to do this, because anchoring individual controls inside the pane didn't work.
            // The right anchor didn't do anything, and the bottom anchor made the ListView's
            // height collapse.

            InitializeComponent();
        }

        public IShell Shell { get; set; }
        public Project Project { get; set; }

        public Form Form { get => this; }

        public string DocumentName { get; set; }

        public bool IsDirty { get; set; }
        public object ProjectItem
        {
            get => _figureItem;
            set
            {
                if (!(value is FigureItem))
                    throw new ArgumentException();

                _figureItem = (FigureItem) value;
            }
        }

        public bool Save()
        {
            // TODO:
            return true;
        }

        private void FigureEditView_Load(object sender, EventArgs e)
        {
            _document = _figureItem.Open();

            LoadMasterPicture();
            LoadProgressPicture();
        }

        private void LoadMasterPicture()
        {
            FigureDocument masterDoc = _figureItem.Parent.MasterFigureItem.Open();

            Size picBoxSize = masterPictureBox.ClientSize;
            int height = (int) (picBoxSize.Height * 0.80f);
            int width = (int) (height * 32f / 37f);

            Rectangle rect = new Rectangle(0, 0, width, height);

            Bitmap bitmap = new Bitmap(width, height);

            using (var graphics = Graphics.FromImage(bitmap))
            using (var painter = new SystemFigurePainter(masterDoc, graphics, rect, FigurePainterSection.Full))
            {
                painter.Fill();
            }

            masterPictureBox.Image = bitmap;
        }

        private void LoadProgressPicture()
        {
            Size picBoxSize = masterPictureBox.ClientSize;
            int height = (int) (picBoxSize.Height * 0.80f);
            int width = (int) (height * 32f / 37f);

            Rectangle rect = new Rectangle(0, 0, width, height);

            Bitmap bitmap = new Bitmap(width, height);

            using (var graphics = Graphics.FromImage(bitmap))
            {
                foreach (var pieceItem in _figureItem.Parent.PieceFigureItems)
                {
                    if (pieceItem != _figureItem)
                        PaintPiece(pieceItem, graphics, rect);
                }
            }

            progressPictureBox.BackgroundImage = bitmap;
        }

        private void PaintPiece(FigureItem pieceItem, Graphics graphics, Rectangle rect)
        {
            var pieceDoc = pieceItem.Open();

            using (var painter = new SystemFigurePainter(pieceDoc, graphics, rect, FigurePainterSection.Enabled))
            {
                painter.Fill();
                painter.Draw();
            }
            using (var painter = new SystemFigurePainter(pieceDoc, graphics, rect, FigurePainterSection.Disabled))
            {
                painter.Draw();
            }
        }

        private void progressPictureBox_Paint(object sender, PaintEventArgs e)
        {
            Size picBoxSize = masterPictureBox.ClientSize;
            int height = (int) (picBoxSize.Height * 0.80f);
            int width = (int) (height * 32f / 37f);

            Rectangle rect = new Rectangle(
                (int) (picBoxSize.Width - width) / 2,
                (int) (picBoxSize.Height - height) / 2,
                width,
                height);

            using (var painter = new SystemFigurePainter(_document, e.Graphics, rect, FigurePainterSection.Enabled))
            {
                painter.Fill();
                painter.Draw();
            }
            using (var painter = new SystemFigurePainter(_document, e.Graphics, rect, FigurePainterSection.Disabled))
            {
                painter.Draw();
            }
        }
    }
}
