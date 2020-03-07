using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using KiriFT.Drawing;

namespace KiriEdit
{
    public partial class CharacterGrid : UserControl
    {
        private const int Columns = 20;
        private const float HeightToWidth = 37f / 32f;
        private const uint MaxUnicodeCodePoint = 0x2FFFF;
        private const uint MinUnicodePoint = '!';

        private CharGridRendererArgs _renderArgs;
        private Font _curFont;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public byte[] ResidencyMap { get; set; }

        [DefaultValue(typeof(Color), "Black")]
        public Color OnCharacterColor { get; set; } = Color.Black;

        [DefaultValue(typeof(Color), "Gray")]
        public Color OffCharacterColor { get; set; } = Color.Gray;

        public CharacterGrid()
        {
            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Up)
            {
                ShiftScrollBar(-vScrollBar.SmallChange);
                return true;
            }
            else if (keyData == Keys.Down)
            {
                ShiftScrollBar(vScrollBar.SmallChange);
                return true;
            }
            else if (keyData == Keys.PageUp)
            {
                ShiftScrollBar(-vScrollBar.LargeChange);
                return true;
            }
            else if (keyData == Keys.PageDown)
            {
                ShiftScrollBar(vScrollBar.LargeChange);
                return true;
            }
            else if (keyData == Keys.Home)
            {
                vScrollBar.Value = vScrollBar.Minimum;
                return true;
            }
            else if (keyData == Keys.End)
            {
                vScrollBar.Value = vScrollBar.Maximum;
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void CharacterGrid_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta != 0)
            {
                int amount = vScrollBar.SmallChange;

                if (e.Delta > 0)
                    amount = -amount;

                ShiftScrollBar(amount);
            }
        }

        private void ShiftScrollBar(int amount)
        {
            int value = vScrollBar.Value + amount;

            if (value < vScrollBar.Minimum)
                value = vScrollBar.Minimum;
            else if (value > vScrollBar.Maximum)
                value = vScrollBar.Maximum;

            vScrollBar.Value = value;
        }

        private void CharacterGrid_GotFocus(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void CharacterGrid_LostFocus(object sender, EventArgs e)
        {
            Invalidate();
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

            _renderArgs.FirstCodePoint = GetPageCodePoint();
            _renderArgs.ResidencyMap = ResidencyMap;
            _renderArgs.ResidencyOffset = GetPageResidencyOffset();
            _renderArgs.OnColor = OnCharacterColor.ToArgb();
            _renderArgs.OffColor = OffCharacterColor.ToArgb();

            IntPtr hdc = e.Graphics.GetHdc();

            try
            {
                _renderArgs.Hdc = hdc;

                UpdateFont();

                CharGridRenderer.Draw(_renderArgs);
            }
            finally
            {
                e.Graphics.ReleaseHdc(hdc);
            }

            Rectangle border = Rectangle.FromLTRB(0, 0, vScrollBar.Left, Height);

            if (Focused)
                ControlPaint.DrawFocusRectangle(e.Graphics, border);
            else
                ControlPaint.DrawBorder(e.Graphics, border, Color.Black, ButtonBorderStyle.Solid);
        }

        private void InitRenderArgs()
        {
            _renderArgs = new CharGridRendererArgs();

            _renderArgs.Columns = Columns;
            _renderArgs.HeightToWidth = HeightToWidth;
            _renderArgs.LastCodePoint = MaxUnicodeCodePoint;
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

        private void UpdateFont()
        {
            Font font = Font;

            if (!object.Equals(_curFont, font))
            {
                if (font != null)
                {
                    _renderArgs.FontFamily = Font.FontFamily.Name;
                    _renderArgs.FontStyle = (int) Font.Style;
                }
                else
                {
                    _renderArgs.FontFamily = null;
                    _renderArgs.FontStyle = 0;
                }

                _curFont = font;
            }
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
