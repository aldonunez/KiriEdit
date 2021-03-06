﻿/*
   Copyright 2020 Aldo J. Nunez

   Licensed under the Apache License, Version 2.0.
   See the LICENSE.txt file for details.
*/

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
        public HistoryBuffer HistoryBuffer => null;

        public object ProjectItem
        {
            get => _characterItem;
            set
            {
                if ( !(value is CharacterItem) )
                    throw new ArgumentException();

                _characterItem = (CharacterItem) value;
                _characterItem.FigureItemModified += CharacterItem_FigureItemModified;
                _characterItem.Deleted += CharacterItem_Deleted;

                Text = CharUtils.GetFullCharTitle( _characterItem.CodePoint );
            }
        }

        private void CharacterItem_Deleted( object sender, EventArgs e )
        {
            Close();
        }

        private void CharacterItem_FigureItemModified( object sender, FigureItemModifiedEventArgs args )
        {
            LoadProgressPicture();
            ReplacePieceImage( args.FigureItem );
        }

        protected override void ScaleControl( SizeF factor, BoundsSpecified specified )
        {
            base.ScaleControl( factor, specified );

            int side = (int) Math.Round( 48 * factor.Height );
            piecesImageList.ImageSize = new Size( side, side );
        }

        public bool Save()
        {
            // There's nothing to save, because all file changes are immediate.
            return true;
        }

        public void CloseView( bool force )
        {
            Close();
        }

        private void FigureEditView_Load( object sender, EventArgs e )
        {
            _masterDoc = _characterItem.MasterFigureItem.Open();

            foreach ( var pieceItem in _characterItem.PieceFigureItems )
            {
                LoadPiece( pieceItem );
            }

            LoadMasterPicture();
            LoadProgressPicture();
        }

        private void PiecesListView_SelectedIndexChanged( object sender, EventArgs e )
        {
            if ( piecesListView.SelectedIndices.Count == 0 )
            {
                deletePieceButton.Enabled = false;
                copyPieceButton.Enabled = false;
            }
            else
            {
                deletePieceButton.Enabled = true;
                copyPieceButton.Enabled = true;
            }
        }

        private void LoadProgressPicture()
        {
            DrawingUtils.LoadProgressPicture(
                progressPictureBox,
                _characterItem,
                _masterDoc,
                null,
                null );
        }

        private void LoadMasterPicture()
        {
            DrawingUtils.LoadMasterPicture( masterPictureBox, _masterDoc );
        }

        private void addPieceButton_Click( object sender, EventArgs e )
        {
            AddPiece();
        }

        private void deletePieceButton_Click( object sender, EventArgs e )
        {
            if ( piecesListView.SelectedItems.Count > 0 )
                DeletePiece( piecesListView.SelectedItems[0] );
        }

        private void copyPieceButton_Click( object sender, EventArgs e )
        {
            if ( piecesListView.SelectedItems.Count > 0 )
                CopyPiece( piecesListView.SelectedItems[0] );
        }

        private void PiecesListView_KeyUp( object sender, KeyEventArgs e )
        {
            if ( e.KeyCode == Keys.Delete )
                deletePieceButton_Click( sender, e );
        }

        private void AddPiece( FigureItemBase template )
        {
            string fileName = CharacterItem.FindNextFileName( Project, _characterItem.CodePoint );
            string name = Path.GetFileNameWithoutExtension( fileName );

            FigureItem figureItem = _characterItem.AddItem( name, template );

            LoadPiece( figureItem );
            LoadProgressPicture();
        }

        private void AddPiece()
        {
            AddPiece( _characterItem.MasterFigureItem );
        }

        private void CopyPiece( ListViewItem listViewItem )
        {
            var figureItem = (FigureItem) listViewItem.Tag;

            AddPiece( figureItem );
        }

        private void ReplacePieceImage( FigureItem figureItem )
        {
            var pieceDoc = figureItem.Open();

            var imageListSize = piecesImageList.ImageSize;

            Rectangle rect = DrawingUtils.CenterFigure( _masterDoc.Figure, imageListSize );

            Bitmap bitmap = new Bitmap( imageListSize.Width, imageListSize.Height );

            try
            {
                using ( var g = Graphics.FromImage( bitmap ) )
                {
                    DrawingUtils.PaintPiece( pieceDoc, g, rect );
                }

                int imageIndex = piecesImageList.Images.IndexOfKey( figureItem.Name );

                if ( imageIndex < 0 )
                    piecesImageList.Images.Add( figureItem.Name, bitmap );
                else
                    piecesImageList.Images[imageIndex] = bitmap;

                bitmap = null;
            }
            catch
            {
                if ( bitmap != null )
                    bitmap.Dispose();
                throw;
            }
        }

        private void LoadPiece( FigureItem figureItem )
        {
            string name = figureItem.Name;

            var listItem = piecesListView.Items.Add( name, name, name );

            listItem.Tag = figureItem;

            ReplacePieceImage( figureItem );
        }

        private void DeletePiece( ListViewItem listViewItem )
        {
            var figureItem = (FigureItem) listViewItem.Tag;

            if ( !ConfirmDeletePiece( figureItem ) )
                return;

            _characterItem.DeleteItem( figureItem.Name );

            piecesListView.Items.Remove( listViewItem );
            piecesImageList.Images.RemoveByKey( figureItem.Name );

            LoadProgressPicture();
        }

        private bool ConfirmDeletePiece( FigureItem figureItem )
        {
            string message = string.Format( "'{0}' will be deleted permanently.", figureItem.Name );
            DialogResult result = MessageBox.Show( message, ShellForm.AppTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning );

            if ( result == DialogResult.OK )
                return true;

            return false;
        }

        private void CharEditView_Shown( object sender, EventArgs e )
        {
            piecesListView.Focus();
        }

        private void piecesListView_ItemActivate( object sender, EventArgs e )
        {
            var figureItem = (FigureItem) piecesListView.SelectedItems[0].Tag;

            Shell.OpenItem( figureItem );
        }
    }
}
