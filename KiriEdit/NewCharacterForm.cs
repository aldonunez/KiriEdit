using System;
using System.Text;
using System.Windows.Forms;

namespace KiriEdit
{
    public partial class NewCharacterForm : Form
    {
        private static byte[] _codePointBytes = new byte[4];

        public delegate bool ValidateCharHandler(uint codePoint);

        public event ValidateCharHandler ValidateChar;

        public uint CodePoint { get; private set; }

        public NewCharacterForm()
        {
            InitializeComponent();
        }

        private void charTextBox_TextChanged(object sender, EventArgs e)
        {
            int codePointCount = GetCodePointCount(charTextBox.Text);

            okButton.Enabled = codePointCount == 1;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            int codePointCount = GetCodePointCount(charTextBox.Text);
            uint codePoint = GetCodePoint(charTextBox.Text);

            if (ValidateChar != null && !ValidateChar(codePoint))
                return;

            CodePoint = codePoint;
            DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private static int GetCodePointCount(string s)
        {
            int byteCount = Encoding.UTF32.GetByteCount(s);
            return byteCount / 4;
        }

        private static uint GetCodePoint(string s)
        {
            Encoding.UTF32.GetBytes(s, 0, s.Length, _codePointBytes, 0);

            uint codePoint =
                (uint) _codePointBytes[0] << 0 |
                (uint) _codePointBytes[1] << 8 |
                (uint) _codePointBytes[2] << 16 |
                (uint) _codePointBytes[3] << 24;

            return codePoint;
        }
    }
}
