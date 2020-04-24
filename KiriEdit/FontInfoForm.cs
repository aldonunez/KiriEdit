/*
   Copyright 2020 Aldo J. Nunez

   Licensed under the Apache License, Version 2.0.
   See the LICENSE.txt file for details.
*/

using System.Windows.Forms;

namespace KiriEdit
{
    public partial class FontInfoForm : Form
    {
        public FontInfo FontInfo { get; set; }

        public FontInfoForm()
        {
            InitializeComponent();
        }

        private void FontInfoForm_Load(object sender, System.EventArgs e)
        {
            typefaceTextBox.Text = string.Format("{0} ({1})", FontInfo.FamilyName, FontInfo.StyleName);

            copyrightTextBox.Text = FontInfo.Copyright;
            licenseTextBox.Text = FontInfo.License;
            versionTextBox.Text = FontInfo.Version;

            // Put spaces around the URL in order to make it easy to select text.
            licenseUrlRichTextBox.Text = " " + FontInfo.LicenseUrl + " ";
        }

        private void licenseUrlRichTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }
    }
}
