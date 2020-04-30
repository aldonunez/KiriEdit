/*
   Copyright 2020 Aldo J. Nunez

   Licensed under the Apache License, Version 2.0.
   See the LICENSE.txt file for details.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace KiriEdit
{
    internal enum ViewsChangedAction
    {
        Added,
        Removed,
    }

    internal class ViewsChangedEventArgs : EventArgs
    {
        public IView View { get; }
        public ViewsChangedAction Action { get; }

        public ViewsChangedEventArgs( IView view, ViewsChangedAction action )
        {
            View = view;
            Action = action;
        }
    }

    internal delegate void ViewsChangedEventHandler( object sender, ViewsChangedEventArgs e );

    internal class MdiDocumentContainer : IEnumerable<IView>
    {
        // Store views in order with most recent at the end. This avoids shuffling all
        // elements every time one is moved.

        private Form _form;
        private List<IView> _views = new List<IView>();

        public int Count => _views.Count;
        public IView CurrentView => (IView) _form.ActiveMdiChild;

        public event ViewsChangedEventHandler ViewsChanged;
        public event EventHandler ViewActivate;
        public event EventHandler HistoryChanged;

        public MdiDocumentContainer( Form form )
        {
            _form = form;
            _form.IsMdiContainer = true;
            _form.MdiChildActivate += _form_MdiChildActivate;
        }

        private void _form_MdiChildActivate( object sender, EventArgs e )
        {
            IView activeView = (IView) _form.ActiveMdiChild;

            if ( activeView != null && _views.Count > 0 && activeView != _views[_views.Count - 1] )
            {
                int oldIndex = _views.LastIndexOf( activeView );

                if ( oldIndex >= 0 )
                {
                    _views.RemoveAt( oldIndex );
                    _views.Add( activeView );
                }
            }

            ViewActivate?.Invoke( this, EventArgs.Empty );
        }

        public void AddView( IView view )
        {
            _views.Add( view );

            try
            {
                view.Form.MdiParent = _form;
                view.Form.WindowState = FormWindowState.Maximized;
                view.Form.FormClosed += Form_FormClosed;
                view.Form.Show();

                if ( view.HistoryBuffer != null )
                    view.HistoryBuffer.HistoryChanged += HistoryBuffer_HistoryChanged;
            }
            catch
            {
                _views.Remove( view );
                throw;
            }

            ViewsChanged?.Invoke( this, new ViewsChangedEventArgs( view, ViewsChangedAction.Added ) );
        }

        private void Form_FormClosed( object sender, FormClosedEventArgs e )
        {
            Form form = (Form) sender;
            form.FormClosed -= Form_FormClosed;

            IView view = (IView) sender;
            _views.Remove( view );

            if ( view.HistoryBuffer != null )
                view.HistoryBuffer.HistoryChanged -= HistoryBuffer_HistoryChanged;

            ViewsChanged?.Invoke( this, new ViewsChangedEventArgs( view, ViewsChangedAction.Removed ) );
        }

        private void HistoryBuffer_HistoryChanged( object sender, EventArgs e )
        {
            HistoryChanged?.Invoke( sender, e );
        }

        // Enumerate views in order with most recent first.

        public IEnumerator<IView> EnumerateViews()
        {
            return new ReverseViewEnumerator( _views );
        }

        public void Clear()
        {
            IView[] views = _views.ToArray();

            foreach ( var view in views )
            {
                view.Form.Close();
            }

            _views.Clear();
        }

        public IView FindView( Type viewType )
        {
            return _views.FirstOrDefault( view => view.GetType() == viewType );
        }

        public IView[] GetDirtyViews()
        {
            return _views.Where( view => view.IsDirty ).ToArray();
        }

        public void Activate( IView view )
        {
            var viewForm = (Form) view;
            viewForm.Activate();
        }

        public IEnumerator<IView> GetEnumerator()
        {
            return EnumerateViews();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return EnumerateViews();
        }


        #region Inner classes

        private struct ReverseViewEnumerator : IEnumerator<IView>
        {
            private List<IView> _views;
            private int _index;

            public IView Current => _views[_index];
            object IEnumerator.Current => _views[_index];

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if ( _index <= 0 )
                    return false;

                _index--;
                return true;
            }

            public void Reset()
            {
                _index = _views.Count;
            }

            public ReverseViewEnumerator( List<IView> views )
            {
                _views = views;
                _index = views.Count;
            }
        }

        #endregion
    }
}
