using System;
using System.Text;
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
            int codePointCount = CharUtils.GetCodePointCount(charTextBox.Text);

            okButton.Enabled = codePointCount == 1;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            int codePointCount = CharUtils.GetCodePointCount(charTextBox.Text);
            uint codePoint = CharUtils.GetCodePoint(charTextBox.Text);

            if (ValidateChar != null && !ValidateChar(codePoint))
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
