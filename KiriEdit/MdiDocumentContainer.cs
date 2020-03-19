using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace KiriEdit
{
    internal class MdiDocumentContainer
    {
        private Form _form;
        private Stack<IView> _views = new Stack<IView>();

        internal int Count => _views.Count;

        internal IView CurrentView => (_views.Count > 0) ? _views.Peek() : null;

        public MdiDocumentContainer(Form form)
        {
            _form = form;
            _form.IsMdiContainer = true;
        }

        internal void AddView(IView view)
        {
            _views.Push(view);

            view.Form.MdiParent = _form;
            view.Form.WindowState = FormWindowState.Maximized;
            view.Form.Show();
        }

        public void Clear()
        {
            foreach (var view in _views)
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
    }
}
