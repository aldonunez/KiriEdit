/*
   Copyright 2020 Aldo J. Nunez

   Licensed under the Apache License, Version 2.0.
   See the LICENSE.txt file for details.
*/

using System;
using System.Windows.Forms;

namespace KiriEdit
{
    internal partial class WindowsForm : Form
    {
        public MdiDocumentContainer DocumentContainer { get; set; }

        public WindowsForm()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void WindowsForm_Load(object sender, EventArgs e)
        {
            foreach (var view in DocumentContainer)
            {
                windowsListBox.Items.Add(new ListItem(view));
            }

            if (windowsListBox.Items.Count > 0)
                windowsListBox.SelectedIndex = 0;
        }

        private void windowsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (windowsListBox.SelectedIndices.Count == 0)
            {
                activateButton.Enabled = false;
                closeWindowButton.Enabled = false;
            }
            else if (windowsListBox.SelectedIndices.Count == 1)
            {
                activateButton.Enabled = true;
                closeWindowButton.Enabled = true;
            }
            else
            {
                activateButton.Enabled = false;
                closeWindowButton.Enabled = true;
            }
        }

        private void activateButton_Click(object sender, EventArgs e)
        {
            DocumentContainer.Activate(((ListItem) windowsListBox.SelectedItem).View);
        }

        private void closeWindowButton_Click(object sender, EventArgs e)
        {
            object[] itemObjects = new object[windowsListBox.SelectedItems.Count];

            windowsListBox.SelectedItems.CopyTo(itemObjects, 0);

            foreach (var obj in itemObjects)
            {
                var view = ((ListItem) obj).View;

                view.Form.Close();
                windowsListBox.Items.Remove(obj);
            }
        }


        #region Inner classes

        private class ListItem
        {
            public IView View { get; set; }

            public ListItem(IView view)
            {
                View = view;
            }

            public override string ToString()
            {
                return View.DocumentTitle;
            }
        }

        #endregion
    }
}
