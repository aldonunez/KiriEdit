using KiriEdit.Font;
using System;
using System.ComponentModel;

namespace KiriEdit
{
    internal class FontListLoader
    {
        private FontFamilyCollection _families;
        private BackgroundWorker _worker = new BackgroundWorker();
        private bool _reloadPending;

        public event EventHandler FontListLoaded;

        public FontFamilyCollection FontFamilies => _families;

        public FontListLoader()
        {
            _worker.DoWork += _worker_DoWork;
            _worker.RunWorkerCompleted += _worker_RunWorkerCompleted;
        }

        private void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bool loaded = false;

            if (e.Error != null)
            {
                // For now, nothing to do. But should we tell the user?
            }
            else if (e.Cancelled)
            {
                // Nothing to do.
            }
            else
            {
                _families = (FontFamilyCollection) e.Result;
                loaded = true;
            }

            if (_reloadPending)
            {
                _reloadPending = false;
                _worker.RunWorkerAsync();
            }

            if (loaded)
                FontListLoaded?.Invoke(this, EventArgs.Empty);
        }

        private void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            FontFamilyCollection families = FontFinder.FindFonts();

            e.Result = families;
        }

        public void Reload()
        {
            if (_worker.IsBusy)
            {
                _reloadPending = true;
            }
            else
            {
                _worker.RunWorkerAsync();
            }
        }
    }
}
 