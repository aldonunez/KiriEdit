using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Text;
using KiriFT;
using KiriFT.Drawing;

namespace KiriEdit
{
    public partial class CharacterGrid : UserControl
    {
        private const int Columns = 20;
        private const float HeightToWidth = 37f / 32f;
        private const uint MaxUnicodeCodePoint = 0x2FFFF;
        private const uint MinUnicodePoint = '!';

        private Library _library;
        private Face _face;
        private PrivateFontCollection _fontCollection;
        private CharGridRendererArgs _renderArgs = new CharGridRendererArgs();

        public string FontPath { get; set; }
        public int FaceIndex { get; set; }

        public CharacterGrid()
        {
            InitializeComponent();
        }

        private void CharacterGrid_Load(object sender, EventArgs e)
        {
            _library = new Library();
            _face = _library.OpenFace(FontPath, FaceIndex);

            _fontCollection = new PrivateFontCollection();
            _fontCollection.AddFontFile(FontPath);

            _renderArgs.Columns = Columns;
            _renderArgs.LastCodePoint = MaxUnicodeCodePoint;
            _renderArgs.FontFamily = _fontCollection.Families[0].Name;
            _renderArgs.FontStyle = 0;
            _renderArgs.HeightToWidth = HeightToWidth;
            _renderArgs.OnColor = Color.Black.ToArgb();
            _renderArgs.OffColor = Color.Gray.ToArgb();

            UpdateLayout();
        }

        private void CharacterGrid_Resize(object sender, EventArgs e)
        {
            UpdateLayout();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            IntPtr hdc = e.Graphics.GetHdc();

            try
            {
                _renderArgs.Hdc = hdc;
                _renderArgs.FirstCodePoint = GetPageCodePoint();

                CharGridRenderer.Draw(_renderArgs);
            }
            finally
            {
                e.Graphics.ReleaseHdc(hdc);
            }
        }

        private void UpdateLayout()
        {
            _renderArgs.Height = Height;
            _renderArgs.Width = vScrollBar.Left;

            CharGridMetrics metrics = _renderArgs.GetMetrics();
            int wholeRows = (int) (_renderArgs.Height / metrics.CellHeight);

            vScrollBar.Maximum = (int) (MaxUnicodeCodePoint - MinUnicodePoint) / Columns;
            vScrollBar.LargeChange = wholeRows;
        }

        private void vScrollBar_ValueChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        private uint GetPageCodePoint()
        {
            int row = vScrollBar.Value;
            return (uint) (MinUnicodePoint + row * Columns);
        }
    }
}
