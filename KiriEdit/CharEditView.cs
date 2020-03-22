using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace KiriEdit
{
    public partial class FigureEditView : Form, IView
    {
        private CharacterItem _characterItem;
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
            get => _characterItem;
            set
            {
                if (!(value is CharacterItem))
                    throw new ArgumentException();

                _characterItem = (CharacterItem) value;
            }
        }

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            base.ScaleControl(factor, specified);

            int side = (int) Math.Round(32 * factor.Height);
            piecesImageList.ImageSize = new Size(side, side);
        }

        public bool Save()
        {
            // TODO:
            return true;
        }

        private void FigureEditView_Load(object sender, EventArgs e)
        {
            _document = _characterItem.MasterFigureItem.Open();

            foreach (var pieceItem in _characterItem.PieceFigureItems)
            {
                LoadPiece(pieceItem);
            }
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

            using (var painter = new SystemFigurePainter(_document, e.Graphics, rect, FigurePainterSection.Full))
            {
                painter.Fill();
            }
        }

        private void addPieceButton_Click(object sender, EventArgs e)
        {
            AddPiece();
        }

        private void deletePieceButton_Click(object sender, EventArgs e)
        {
            if (piecesListView.SelectedItems.Count > 0)
                DeletePiece(piecesListView.SelectedItems[0]);
        }

        private void AddPiece()
        {
            // TODO: make this an instance method
            string fileName = CharacterItem.FindNextFileName(Project, _characterItem.CodePoint);
            string name = Path.GetFileNameWithoutExtension(fileName);

            FigureItem figureItem = _characterItem.AddItem(name);

            LoadPiece(figureItem);
        }

        private void LoadPiece(FigureItem figureItem)
        {
            var pieceDoc = figureItem.Open();

            var rect = new Rectangle(Point.Empty, piecesImageList.ImageSize);

            Bitmap bitmap = new Bitmap(rect.Width, rect.Height);

            try
            {
                using (var g = Graphics.FromImage(bitmap))
                {
                    using (var painter = new SystemFigurePainter(pieceDoc, g, rect, FigurePainterSection.Enabled))
                    {
                        painter.Fill();
                        painter.Draw();
                    }
                    using (var painter = new SystemFigurePainter(pieceDoc, g, rect, FigurePainterSection.Disabled))
                    {
                        painter.Draw();
                    }
                }

                piecesImageList.Images.Add(figureItem.Name, bitmap);
                bitmap.Save(@"C:\Temp\x.png");
                bitmap = null;
            }
            catch
            {
                if (bitmap != null)
                    bitmap.Dispose();
                throw;
            }

            string name = figureItem.Name;

            var listItem = piecesListView.Items.Add(name, name, name);

            listItem.Tag = figureItem;
        }

        private void DeletePiece(ListViewItem listViewItem)
        {
            var figureItem = (FigureItem) listViewItem.Tag;

            if (!ConfirmDeletePiece(figureItem))
                return;

            _characterItem.DeleteItem(figureItem.Name);

            piecesListView.Items.Remove(listViewItem);
            piecesImageList.Images.RemoveByKey(figureItem.Name);
        }

        private bool ConfirmDeletePiece(FigureItem figureItem)
        {
            string message = string.Format("'{0}' will be deleted permanently.", figureItem.Name);
            DialogResult result = MessageBox.Show(message, ShellForm.AppTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

            if (result == DialogResult.OK)
                return true;

            return false;
        }
    }
}
