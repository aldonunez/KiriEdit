using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace KiriEdit
{
    internal class MdiDocumentContainer
    {
        private Form _form;
        private List<IView> _views = new List<IView>();

        internal int Count => _views.Count;

        internal IView CurrentView => (IView) _form.ActiveMdiChild;

        public MdiDocumentContainer(Form form)
        {
            _form = form;
            _form.IsMdiContainer = true;
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
        }

        private void Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form form = (Form) sender;
            form.FormClosed -= Form_FormClosed;

            IView view = (IView) sender;
            _views.Remove(view);
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
    }
}
