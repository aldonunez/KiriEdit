﻿/*
   Copyright 2020 Aldo J. Nunez

   Licensed under the Apache License, Version 2.0.
   See the LICENSE.txt file for details.
*/

using KiriProj;
using System;
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
        public HistoryBuffer HistoryBuffer { get; } = new HistoryBuffer();

        public object ProjectItem
        {
            get => _figureItem;
            set
            {
                if ( !(value is FigureItem) )
                    throw new ArgumentException();

                _figureItem = (FigureItem) value;
                _figureItem.Deleted += _figureItem_Deleted;

                _title = string.Format(
                    "{0} : {1}",
                    CharUtils.GetFullCharTitle( _figureItem.Parent.CodePoint ),
                    _figureItem.Name );

                UpdateTitle();
            }
        }

        private void _figureItem_Deleted( object sender, EventArgs e )
        {
            _deleted = true;
            Close();
        }

        public bool Save()
        {
            _figureItem.Save( figureEditor.Document );

            UpdateTitle();

            return true;
        }

        public void CloseView( bool force )
        {
            if ( force )
                _figureItem.IsDirty = false;

            Close();
        }

        private void FigureEditor_Modified( object sender, EventArgs e )
        {
            _figureItem.IsDirty = true;

            LoadProgressPicture( figureEditor.Document );
            UpdateTitle();
        }

        private void UpdateTitle()
        {
            Text = _title;

            if ( _figureItem.IsDirty )
                Text += "*";
        }

        private void FigureEditView_FormClosed( object sender, FormClosedEventArgs e )
        {
            _figureItem.IsDirty = false;
        }

        private void FigureEditView_FormClosing( object sender, FormClosingEventArgs e )
        {
            if ( e.CloseReason == CloseReason.UserClosing && !_deleted )
            {
                if ( !ConfirmClose() )
                    e.Cancel = true;
            }
        }

        private bool ConfirmClose()
        {
            if ( !IsDirty )
                return true;

            string message = string.Format( "Do you want to save '{0}'?", _title );
            var result = MessageBox.Show( message, ShellForm.AppTitle, MessageBoxButtons.YesNoCancel );

            switch ( result )
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

        private void FigureEditView_Load( object sender, EventArgs e )
        {
            figureEditor.Document = _figureItem.Open();
            figureEditor.History = HistoryBuffer;

            LoadMasterPicture( figureEditor.Document );
            LoadProgressPicture( figureEditor.Document );

            figureEditor.Select();
        }

        private void LoadMasterPicture( FigureDocument masterDoc )
        {
            DrawingUtils.LoadMasterPicture( masterPictureBox, masterDoc );
        }

        private void LoadProgressPicture( FigureDocument masterDoc )
        {
            DrawingUtils.LoadProgressPicture(
                progressPictureBox,
                _figureItem.Parent,
                masterDoc,
                _figureItem,
                figureEditor.Document );
        }
    }
}
