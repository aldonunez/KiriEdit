using System;
using System.Windows.Forms;

namespace KiriEdit
{
    public partial class NewCharacterForm : Form
    {
        public delegate bool ValidateCharHandler(uint codePoint);

        public event ValidateCharHandler ValidateChar;

        public uint CodePoint { get; private set; }

        public NewCharacterForm()
        {
            InitializeComponent();
        }

        private void charTextBox_TextChanged(object sender, EventArgs e)
        {
            // TODO: make this more accurate for code points with more than 1 char.

            okButton.Enabled = charTextBox.TextLength > 0;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            // TODO: make this more accurate for code points with more than 1 char.

            uint codePoint = charTextBox.Text[0];

            if (ValidateChar != null && !ValidateChar(CodePoint))
                return;

            CodePoint = codePoint;

            DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
