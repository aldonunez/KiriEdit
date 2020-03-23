using System;
using System.Drawing;
using System.Windows.Forms;

namespace KiriEdit
{
    public partial class FigureEditView : Form, IView
    {
        private FigureItem _figureItem;
        private string _title;

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

        public string DocumentTitle => Text;

        public bool IsDirty => _figureItem.IsDirty;

        public object ProjectItem
        {
            get => _figureItem;
            set
            {
                if (!(value is FigureItem))
                    throw new ArgumentException();

                _figureItem = (FigureItem) value;
                _title = _figureItem.Name;

                UpdateTitle();
            }
        }

        public bool Save()
        {
            _figureItem.Save(figureEditor.Document);

            UpdateTitle();

            return true;
        }

        private void FigureEditor_Modified(object sender, EventArgs e)
        {
            _figureItem.IsDirty = true;

            LoadProgressPicture();
            UpdateTitle();
        }

        private void UpdateTitle()
        {
            Text = _title;

            if (_figureItem.IsDirty)
                Text += "*";
        }

        private void FigureEditView_Load(object sender, EventArgs e)
        {
            figureEditor.Document = _figureItem.Open();

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
            using (var painter = new SystemFigurePainter(masterDoc))
            {
                painter.SetTransform(graphics, rect);
                painter.PaintFull();
                painter.Fill(graphics);
            }

            masterPictureBox.Image = bitmap;
        }

        private void LoadProgressPicture()
        {
            if (progressPictureBox.BackgroundImage != null)
            {
                progressPictureBox.BackgroundImage.Dispose();
                progressPictureBox.BackgroundImage = null;
            }

            Size picBoxSize = masterPictureBox.ClientSize;
            int height = (int) (picBoxSize.Height * 0.80f);
            int width = (int) (height * 32f / 37f);

            Rectangle rect = new Rectangle(0, 0, width, height);

            Bitmap bitmap = new Bitmap(width, height);

            using (var graphics = Graphics.FromImage(bitmap))
            {
                foreach (var pieceItem in _figureItem.Parent.PieceFigureItems)
                {
                    PaintPiece(pieceItem, graphics, rect);
                }
            }

            progressPictureBox.BackgroundImage = bitmap;
            progressPictureBox.Invalidate();
        }

        private void PaintPiece(FigureItem pieceItem, Graphics graphics, Rectangle rect)
        {
            FigureDocument pieceDoc;
            Brush brush;

            // Draw this view's figure item differently.
            if (pieceItem == _figureItem)
            {
                brush = Brushes.Red;
                pieceDoc = figureEditor.Document;
            }
            else
            {
                brush = Brushes.Black;
                pieceDoc = pieceItem.Open();
            }

            using (var painter = new SystemFigurePainter(pieceDoc))
            {
                painter.SetTransform(graphics, rect);

                for (int i = 0; i < pieceDoc.Shapes.Length; i++)
                {
                    if (pieceDoc.Shapes[i].Enabled)
                    {
                        painter.PaintShape(i);
                        painter.Fill(graphics, brush);
                    }
                }

                painter.PaintFull();
                painter.Draw(graphics);
            }
        }
    }
}
