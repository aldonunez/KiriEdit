using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Text;
using KiriFT.Drawing;

namespace KiriEdit
{
    public partial class CharacterGrid : UserControl
    {
        private const int Columns = 20;
        private const float HeightToWidth = 37f / 32f;
        private const uint MaxUnicodeCodePoint = 0x2FFFF;
        private const uint MinUnicodePoint = '!';

        private PrivateFontCollection _fontCollection;
        private CharGridRendererArgs _renderArgs;

        public string FontPath { get; set; }
        public int FaceIndex { get; set; }

        public byte[] ResidencyMap { get; set; }

        public CharacterGrid()
        {
            InitializeComponent();
        }

        private void CharacterGrid_MouseWheel(object sender, MouseEventArgs e)
        {
            int value = vScrollBar.Value;

            if (e.Delta < 0)
                value += vScrollBar.SmallChange;
            else if (e.Delta > 0)
                value -= vScrollBar.SmallChange;

            if (value < vScrollBar.Minimum)
                value = vScrollBar.Minimum;
            else if (value > vScrollBar.Maximum)
                value = vScrollBar.Maximum;

            vScrollBar.Value = value;
        }

        private void CharacterGrid_GotFocus(object sender, EventArgs e)
        {
            vScrollBar.Focus();
        }

        private void CharacterGrid_Load(object sender, EventArgs e)
        {
            InitRenderArgs();
            UpdateRenderArgs();
        }

        private void CharacterGrid_Resize(object sender, EventArgs e)
        {
            UpdateRenderArgs();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            IntPtr hdc = e.Graphics.GetHdc();

            try
            {
                _renderArgs.Hdc = hdc;
                _renderArgs.FirstCodePoint = GetPageCodePoint();
                _renderArgs.ResidencyMap = ResidencyMap;
                _renderArgs.ResidencyOffset = GetPageResidencyOffset();

                CharGridRenderer.Draw(_renderArgs);
            }
            finally
            {
                e.Graphics.ReleaseHdc(hdc);
            }
        }

        private void InitRenderArgs()
        {
            _fontCollection = new PrivateFontCollection();
            _fontCollection.AddFontFile(FontPath);

            _renderArgs = new CharGridRendererArgs();

            _renderArgs.Columns = Columns;
            _renderArgs.LastCodePoint = MaxUnicodeCodePoint;
            _renderArgs.FontFamily = _fontCollection.Families[0].Name;
            _renderArgs.FontStyle = 0;
            _renderArgs.HeightToWidth = HeightToWidth;
            _renderArgs.OnColor = Color.Black.ToArgb();
            _renderArgs.OffColor = Color.Gray.ToArgb();
        }

        private void UpdateRenderArgs()
        {
            if (_renderArgs == null)
                return;

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

        private int GetPageResidencyOffset()
        {
            int row = vScrollBar.Value;
            return row * 3;
        }

        public void ScrollTo(uint codePoint)
        {
            if (codePoint > MaxUnicodeCodePoint)
                return;

            int row = (int) (codePoint - MinUnicodePoint) / Columns;
            vScrollBar.Value = row;
        }
    }
}
