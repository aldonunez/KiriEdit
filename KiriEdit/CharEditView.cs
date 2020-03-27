﻿using KiriProj;
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

            int side = (int) Math.Round(48 * factor.Height);
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

            LoadMasterPicture();
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
            int height = (int) (picBoxSize.Height * 0.95f);
            int width = height;

            Rectangle rect = DrawingUtils.CenterFigure(_masterDoc.Figure, new Size(width, height));

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

            DrawingUtils.PaintPiece(pieceDoc, graphics, rect);
        }

        private void LoadMasterPicture()
        {
            Size picBoxSize = masterPictureBox.ClientSize;
            int height = (int) (picBoxSize.Height * 0.95f);
            int width = height;

            Rectangle rect = DrawingUtils.CenterFigure(_masterDoc.Figure, new Size(width, height));

            Bitmap bitmap = new Bitmap(width, height);

            using (var graphics = Graphics.FromImage(bitmap))
            using (var painter = new SystemFigurePainter(_masterDoc))
            {
                painter.SetTransform(graphics, rect);
                painter.PaintFull();
                painter.Fill(graphics);
            }

            masterPictureBox.Image = bitmap;
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

            var imageListSize = piecesImageList.ImageSize;

            Rectangle rect = DrawingUtils.CenterFigure(_masterDoc.Figure, imageListSize);

            Bitmap bitmap = new Bitmap(imageListSize.Width, imageListSize.Height);

            try
            {
                using (var g = Graphics.FromImage(bitmap))
                {
                    DrawingUtils.PaintPiece(pieceDoc, g, rect);
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
