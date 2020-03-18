using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace KiriEdit
{
    internal class StackDocumentContainer : Component
    {
        private bool _disposed;
        private Panel _hostPanel;
        private Stack<IView> _views = new Stack<IView>();

        public int Count => _views.Count;

        public IView CurrentView => (_views.Count > 0) ? _views.Peek() : null;

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (_hostPanel != null)
                {
                    // We don't own the host panel. The form does.
                    // Disconnect from it.
                    _hostPanel.Disposed -= hostPanel_Disposed;
                    _hostPanel = null;
                }
            }

            // No native resourcecs to free.

            _disposed = true;

            base.Dispose(disposing);
        }

        private void hostPanel_Disposed(object sender, EventArgs e)
        {
            if (_hostPanel != null)
            {
                // Disconnect from it.
                _hostPanel.Disposed -= hostPanel_Disposed;
                _hostPanel = null;
            }
        }

        public Control MakeControl()
        {
            if (_hostPanel != null)
                throw new ApplicationException();

            _hostPanel = new Panel();
            _hostPanel.Disposed += hostPanel_Disposed;

            return _hostPanel;
        }

        public void AddView(IView view)
        {
            IView prevView = null;

            if (_views.Count > 0)
                prevView = _views.Peek();

            _views.Push(view);

            // Add the new control.
            _hostPanel.Controls.Add(view.Control);
            view.Control.Dock = DockStyle.Fill;

            // Hide the old control.
            if (prevView != null)
                prevView.Control.Visible = false;
        }

        public void Clear()
        {
            if (_hostPanel != null)
                _hostPanel.Controls.Clear();

            _views.Clear();
        }

        public IView FindView(Type viewType)
        {
            return _views.FirstOrDefault(view => view.GetType() == viewType);
        }

        public IView[] GetDirtyViews()
        {
            return _views.Where(view => view.IsDirty).ToArray();
        }
    }
}
