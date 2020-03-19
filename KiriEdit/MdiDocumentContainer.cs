using System;
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

    internal class ViewsChangedEventArgs
    {
        public IView View { get; }
        public ViewsChangedAction Action { get; }

        public ViewsChangedEventArgs(IView view, ViewsChangedAction action)
        {
            View = view;
            Action = action;
        }
    }

    internal delegate void ViewsChangedEventHandler(object sender, ViewsChangedEventArgs e);

    internal class MdiDocumentContainer
    {
        private Form _form;
        private List<IView> _views = new List<IView>();

        internal int Count => _views.Count;
        internal IView CurrentView => (IView) _form.ActiveMdiChild;

        public event ViewsChangedEventHandler ViewsChanged;
        public event EventHandler ViewActivate;

        public MdiDocumentContainer(Form form)
        {
            _form = form;
            _form.IsMdiContainer = true;
            _form.MdiChildActivate += _form_MdiChildActivate;
        }

        private void _form_MdiChildActivate(object sender, EventArgs e)
        {
            IView activeView = (IView) _form.ActiveMdiChild;

            if (activeView != null && _views.Count > 0 && activeView != _views[0])
            {
                int oldIndex = _views.LastIndexOf(activeView);

                if (oldIndex >= 0)
                {
                    _views.RemoveAt(oldIndex);
                    _views.Insert(0, activeView);
                }
            }

            ViewActivate?.Invoke(this, EventArgs.Empty);
        }

        internal IView[] GetViews()
        {
            return _views.ToArray();
        }

        internal void AddView(IView view)
        {
            _views.Add(view);

            try
            {
                view.Form.MdiParent = _form;
                view.Form.WindowState = FormWindowState.Maximized;
                view.Form.FormClosed += Form_FormClosed;
                view.Form.Show();
            }
            catch
            {
                _views.Remove(view);
                throw;
            }

            ViewsChanged?.Invoke(this, new ViewsChangedEventArgs(view, ViewsChangedAction.Added));
        }

        private void Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form form = (Form) sender;
            form.FormClosed -= Form_FormClosed;

            IView view = (IView) sender;
            _views.Remove(view);

            ViewsChanged?.Invoke(this, new ViewsChangedEventArgs(view, ViewsChangedAction.Removed));
        }

        internal IEnumerator<IView> EnumerateViews()
        {
            return _views.GetEnumerator();
        }

        public void Clear()
        {
            IView[] views = _views.ToArray();

            foreach (var view in views)
            {
                view.Form.Close();
            }

            _views.Clear();
        }

        internal IView FindView(Type viewType)
        {
            return _views.FirstOrDefault(view => view.GetType() == viewType);
        }

        internal IView[] GetDirtyViews()
        {
            return _views.Where(view => view.IsDirty).ToArray();
        }

        internal void Activate(IView menuItem)
        {
            var viewForm = (Form) menuItem;
            viewForm.Activate();
        }
    }
}
