using KiriProj;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace KiriEdit
{
    public partial class CharEditView : Form, IView
    {
        private CharacterItem _characterItem;
        private FigureDocument _masterDoc;

        public CharEditView()
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
        public bool IsDirty => false;

        public object ProjectItem
        {
            get => _characterItem;
            set
            {
                if (!(value is CharacterItem))
                    throw new ArgumentException();

                _characterItem = (CharacterItem) value;
                _characterItem.FigureItemModified += CharacterItem_FigureItemModified;
                _characterItem.Deleted += CharacterItem_Deleted;

                Text = string.Format(
                    "U+{0:X6}  {1}",
                    _characterItem.CodePoint,
                    CharUtils.GetString(_characterItem.CodePoint));
            }
        }

        private void CharacterItem_Deleted(object sender, EventArgs e)
        {
            Close();
        }

        private void CharacterItem_FigureItemModified(object sender, FigureItemModifiedEventArgs args)
        {
            LoadProgressPicture();
            ReplacePieceImage(args.FigureItem);
        }

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            base.ScaleControl(factor, specified);

            int side = (int) Math.Round(32 * factor.Height);
            piecesImageList.ImageSize = new Size(side, side);
        }

        public bool Save()
        {
            // There's nothing to save, because all file changes are immediate.
            return true;
        }

        private void FigureEditView_Load(object sender, EventArgs e)
        {
            _masterDoc = _characterItem.MasterFigureItem.Open();

            foreach (var pieceItem in _characterItem.PieceFigureItems)
            {
                LoadPiece(pieceItem);
            }

            LoadProgressPicture();
        }

        private void LoadProgressPicture()
        {
            Image oldImage = progressPictureBox.BackgroundImage;

            if (oldImage != null)
            {
                progressPictureBox.BackgroundImage = null;
                oldImage.Dispose();
            }

            Size picBoxSize = masterPictureBox.ClientSize;
            int height = (int) (picBoxSize.Height * 0.80f);
            int width = (int) (height * 32f / 37f);

            Rectangle rect = new Rectangle(0, 0, width, height);

            Bitmap bitmap = new Bitmap(width, height);

            using (var graphics = Graphics.FromImage(bitmap))
            {
                foreach (var pieceItem in _characterItem.PieceFigureItems)
                {
                    PaintPiece(pieceItem, graphics, rect);
                }
            }

            progressPictureBox.BackgroundImage = bitmap;
            progressPictureBox.Invalidate();
        }

        private void PaintPiece(FigureItem pieceItem, Graphics graphics, Rectangle rect)
        {
            var pieceDoc = pieceItem.Open();

            using (var painter = new SystemFigurePainter(pieceDoc))
            {
                painter.SetTransform(graphics, rect);

                for (int i = 0; i < pieceDoc.Shapes.Length; i++)
                {
                    if (pieceDoc.Shapes[i].Enabled)
                    {
                        painter.PaintShape(i);
                        painter.Fill(graphics);
                    }
                }

                painter.PaintFull();
                painter.Draw(graphics);
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

            using (var painter = new SystemFigurePainter(_masterDoc))
            {
                painter.SetTransform(e.Graphics, rect);
                painter.PaintFull();
                painter.Fill(e.Graphics);
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

        private void ReplacePieceImage(FigureItem figureItem)
        {
            var pieceDoc = figureItem.Open();

            var rect = new Rectangle(Point.Empty, piecesImageList.ImageSize);

            Bitmap bitmap = new Bitmap(rect.Width, rect.Height);

            try
            {
                using (var g = Graphics.FromImage(bitmap))
                using (var painter = new SystemFigurePainter(pieceDoc))
                {
                    painter.SetTransform(g, rect);

                    for (int i = 0; i < pieceDoc.Shapes.Length; i++)
                    {
                        if (pieceDoc.Shapes[i].Enabled)
                        {
                            painter.PaintShape(i);
                            painter.Fill(g);
                        }
                    }

                    painter.PaintFull();
                    painter.Draw(g);
                }

                int imageIndex = piecesImageList.Images.IndexOfKey(figureItem.Name);

                if (imageIndex < 0)
                    piecesImageList.Images.Add(figureItem.Name, bitmap);
                else
                    piecesImageList.Images[imageIndex] = bitmap;

                bitmap = null;
            }
            catch
            {
                if (bitmap != null)
                    bitmap.Dispose();
                throw;
            }
        }

        private void LoadPiece(FigureItem figureItem)
        {
            string name = figureItem.Name;

            var listItem = piecesListView.Items.Add(name, name, name);

            listItem.Tag = figureItem;

            ReplacePieceImage(figureItem);
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

        private void CharEditView_Shown(object sender, EventArgs e)
        {
            piecesListView.Focus();
        }

        private void piecesListView_ItemActivate(object sender, EventArgs e)
        {
            var figureItem = (FigureItem) piecesListView.SelectedItems[0].Tag;

            Shell.OpenItem(figureItem);
        }
    }
}
