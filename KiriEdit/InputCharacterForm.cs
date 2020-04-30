/*
   Copyright 2020 Aldo J. Nunez

   Licensed under the Apache License, Version 2.0.
   See the LICENSE.txt file for details.
*/

using System;
using System.Globalization;
using System.Windows.Forms;

namespace KiriEdit
{
    public partial class InputCharacterForm : Form
    {
        private Control _curChangingControl;

        public delegate bool ValidateCharHandler( uint codePoint );

        public event ValidateCharHandler ValidateChar;

        public uint CodePoint { get; private set; }

        public InputCharacterForm()
        {
            InitializeComponent();
        }

        private void charTextBox_TextChanged( object sender, EventArgs e )
        {
            // Validate the input for the OK button.

            int codePointCount = CharUtils.GetCodePointCount( charTextBox.Text );

            okButton.Enabled = codePointCount == 1;

            // Change the other control.

            if ( _curChangingControl == null )
            {
                _curChangingControl = charTextBox;

                try
                {
                    codePointTextBox.Text = "";

                    if ( codePointCount == 1 )
                    {
                        uint codePoint = CharUtils.GetCodePoint( charTextBox.Text );
                        codePointTextBox.Text = string.Format( "U+{0:X4}", codePoint );
                    }
                }
                finally
                {
                    _curChangingControl = null;
                }
            }
        }

        private void codepointTextBox_TextChanged( object sender, EventArgs e )
        {
            // Change the other control.

            if ( _curChangingControl == null )
            {
                _curChangingControl = codePointTextBox;

                try
                {
                    charTextBox.Text = "";

                    string codePointText = codePointTextBox.Text;

                    if ( codePointText.StartsWith( "U+", StringComparison.OrdinalIgnoreCase ) )
                        codePointText = codePointText.Substring( 2 );

                    if ( uint.TryParse( codePointText, NumberStyles.HexNumber, null, out uint codePoint ) )
                    {
                        charTextBox.Text = CharUtils.GetString( codePoint );
                    }
                }
                finally
                {
                    _curChangingControl = null;
                }
            }
        }

        private void okButton_Click( object sender, EventArgs e )
        {
            uint codePoint = CharUtils.GetCodePoint( charTextBox.Text );

            if ( ValidateChar != null && !ValidateChar( codePoint ) )
                return;

            CodePoint = codePoint;
            DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click( object sender, EventArgs e )
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
