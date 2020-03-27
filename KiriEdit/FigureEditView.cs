using KiriProj;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace KiriEdit
{
    public partial class FigureEditView : Form, IView
    {
        private FigureItem _figureItem;
        private string _title;
        private bool _deleted;

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

        public Form Form => this;
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
                _figureItem.Deleted += _figureItem_Deleted;

                _title = string.Format(
                    "U+{0:X6}  {1} : {2}",
                    _figureItem.Parent.CodePoint,
                    CharUtils.GetString(_figureItem.Parent.CodePoint),
                    _figureItem.Name);

                UpdateTitle();
            }
        }

        private void _figureItem_Deleted(object sender, EventArgs e)
        {
            _deleted = true;
            Close();
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

            LoadProgressPicture(figureEditor.Document);
            UpdateTitle();
        }

        private void UpdateTitle()
        {
            Text = _title;

            if (_figureItem.IsDirty)
                Text += "*";
        }

        private void FigureEditView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && !_deleted)
            {
                if (!ConfirmClose())
                    e.Cancel = true;
            }
        }

        private bool ConfirmClose()
        {
            if (!IsDirty)
                return true;

            string message = string.Format("Do you want to save '{0}'?", _title);
            var result = MessageBox.Show(message, ShellForm.AppTitle, MessageBoxButtons.YesNoCancel);

            switch (result)
            {
                case DialogResult.Yes:
                    // Save, then close.
                    return Save();

                case DialogResult.No:
                    // Close without saving.
                    return true;

                default:
                    return false;
            }
        }

        private void FigureEditView_Load(object sender, EventArgs e)
        {
            figureEditor.Document = _figureItem.Open();

            LoadMasterPicture(figureEditor.Document);
            LoadProgressPicture(figureEditor.Document);
        }

        private void LoadMasterPicture(FigureDocument masterDoc)
        {
            Size picBoxSize = masterPictureBox.ClientSize;
            int height = (int) (picBoxSize.Height * 0.95f);
            int width = height;

            Rectangle rect = DrawingUtils.CenterFigure(masterDoc.Figure, new Size(width, height));

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

        private void LoadProgressPicture(FigureDocument masterDoc)
        {
            if (progressPictureBox.BackgroundImage != null)
            {
                progressPictureBox.BackgroundImage.Dispose();
                progressPictureBox.BackgroundImage = null;
            }

            Size picBoxSize = masterPictureBox.ClientSize;
            int height = (int) (picBoxSize.Height * 0.95f);
            int width = height;

            Rectangle rect = DrawingUtils.CenterFigure(masterDoc.Figure, new Size(width, height));

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
            bool standOut;

            // Draw this view's figure item differently.
            if (pieceItem == _figureItem)
            {
                standOut = true;
                pieceDoc = figureEditor.Document;
            }
            else
            {
                standOut = false;
                pieceDoc = pieceItem.Open();
            }

            DrawingUtils.PaintPiece(pieceDoc, graphics, rect, standOut);
        }
    }
}
