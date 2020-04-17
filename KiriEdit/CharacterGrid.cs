/*
   Copyright 2020 Aldo J. Nunez

   Licensed under the Apache License, Version 2.0.
   See the LICENSE.txt file for details.
*/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using KiriFT.Drawing;

namespace KiriEdit
{
    public partial class CharacterGrid : UserControl
    {
        private const float HeightToWidth = 37f / 32f;
        private const float SelectionBoxRatio = 1.3f;

        private CharGridRendererArgs _renderArgs;
        private Font _curFont;
        private CharSet _charSet = new SequentialCharSet(null, 0, 0xFFFF);
        private int _columns = 10;
        private int _curIndex = -1;
        private bool _mouseDown;

        public event EventHandler SelectedIndexChanged;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CharSet CharSet
        {
            get => _charSet;
            set
            {
                if (value != _charSet)
                {
                    if (value == null)
                        throw new ArgumentNullException("value");

                    _charSet = value;
                    SelectedIndex = -1;
                    UpdateRenderArgs();
                }
            }
        }

        [DefaultValue(10)]
        public int Columns
        {
            get => _columns;
            set
            {
                if (value != _columns)
                {
                    if (value < CharGridRenderer.MinimumColumns || value > CharGridRenderer.MaximumColumns)
                        throw new ArgumentOutOfRangeException("value");

                    _columns = value;
                    UpdateRenderArgs();
                }
            }
        }

        [DefaultValue(typeof(Color), "Black")]
        public Color OnCharacterColor { get; set; } = Color.Black;

        [DefaultValue(typeof(Color), "Gray")]
        public Color OffCharacterColor { get; set; } = Color.Gray;

        [Browsable(false)]
        public int SelectedIndex
        {
            get => _curIndex;
            private set
            {
                if (value != _curIndex)
                {
                    _curIndex = value;
                    OnSelectedIndexChanged();
                }
            }
        }

        public CharacterGrid()
        {
            DoubleBuffered = true;
            InitializeComponent();
        }

        private void CharacterGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                _mouseDown = true;

            SelectByMouse(e);
        }

        private void CharacterGrid_MouseUp(object sender, MouseEventArgs e)
        {
            _mouseDown = false;
        }

        private void CharacterGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseDown)
                SelectByMouse(e);
        }

        private void SelectByMouse(MouseEventArgs e)
        {
            var metrics = _renderArgs.GetMetrics();
            int row = (int) (e.Y / metrics.CellHeight);
            int col = (int) (e.X / metrics.CellWidth);
            int relativeIndex = row * Columns + col;
            int index = relativeIndex + GetPageStartRow() * Columns;

            SelectCharacter(index);
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

            _renderArgs.StartRow = GetPageStartRow();
            _renderArgs.OnColor = OnCharacterColor.ToArgb();
            _renderArgs.OffColor = OffCharacterColor.ToArgb();

            UpdateFont();

            IntPtr hdc = e.Graphics.GetHdc();

            try
            {
                _renderArgs.Hdc = hdc;

                CharGridRenderer.Draw(_renderArgs, CharSet);
            }
            finally
            {
                _renderArgs.Hdc = IntPtr.Zero;
                e.Graphics.ReleaseHdc(hdc);
            }

            if (_curIndex >= 0)
                DrawSelectionCell(e.Graphics);

            Rectangle border = Rectangle.FromLTRB(0, 0, vScrollBar.Left, Height);

            if (Focused)
                ControlPaint.DrawFocusRectangle(e.Graphics, border);
            else
                ControlPaint.DrawBorder(e.Graphics, border, Color.Black, ButtonBorderStyle.Solid);
        }

        private void DrawSelectionCell(Graphics graphics)
        {
            CharGridMetrics metrics = _renderArgs.GetMetrics();

            int wholeRows = (int) (_renderArgs.Height / metrics.CellHeight);
            int startIndex = GetPageStartRow() * Columns;
            int endIndex = startIndex + wholeRows * Columns;

            if (_curIndex < startIndex || _curIndex >= endIndex)
                return;

            int relativeIndex = _curIndex - startIndex;
            int row = relativeIndex / Columns;
            int col = relativeIndex % Columns;

            float halfWidth = metrics.CellWidth / 2;
            float halfHeight = metrics.CellHeight / 2;
            float centerX = col * metrics.CellWidth + halfWidth;
            float centerY = row * metrics.CellHeight + halfHeight;

            float x1 = centerX - halfWidth * SelectionBoxRatio;
            float y1 = centerY - halfHeight * SelectionBoxRatio;
            float x2 = centerX + halfWidth * SelectionBoxRatio;
            float y2 = centerY + halfHeight * SelectionBoxRatio;

            if (x1 < 0)
            {
                x2 -= x1;
                x1 = 0;
            }
            else if (x2 > _renderArgs.Width)
            {
                x1 -= (x2 - _renderArgs.Width);
                x2 = _renderArgs.Width;
            }

            if (y1 < 0)
            {
                y2 -= y1;
                y1 = 0;
            }
            else if (y2 > _renderArgs.Height)
            {
                y1 -= (y2 - _renderArgs.Height);
                y2 = _renderArgs.Height;
            }

            float width = x2 - x1;
            float height = y2 - y1;

            graphics.FillRectangle(Brushes.White, x1, y1, width, height);
            graphics.DrawRectangle(Pens.Black, x1, y1, width, height);

            int oldHeight = _renderArgs.Height;
            int oldWidth = _renderArgs.Width;
            _renderArgs.Height = 1;
            _renderArgs.Width = (int) width;
            _renderArgs.Left = (int) x1;
            _renderArgs.Top = (int) y1;
            _renderArgs.Columns = 1;
            _renderArgs.StartRow = _curIndex;

            IntPtr hdc = graphics.GetHdc();

            try
            {
                _renderArgs.Hdc = hdc;

                CharGridRenderer.Draw(_renderArgs, CharSet);
            }
            finally
            {
                _renderArgs.Hdc = IntPtr.Zero;
                graphics.ReleaseHdc(hdc);

                _renderArgs.Height = oldHeight;
                _renderArgs.Width = oldWidth;
                _renderArgs.Left = 0;
                _renderArgs.Top = 0;
                _renderArgs.Columns = Columns;
            }
        }

        private void InitRenderArgs()
        {
            _renderArgs = new CharGridRendererArgs();

            _renderArgs.HeightToWidth = HeightToWidth;
        }

        private void UpdateRenderArgs()
        {
            if (_renderArgs == null)
                return;

            _renderArgs.Columns = Columns;
            _renderArgs.Height = Height;
            _renderArgs.Width = vScrollBar.Left;

            CharGridMetrics metrics = _renderArgs.GetMetrics();
            int wholeRows = (int) (_renderArgs.Height / metrics.CellHeight);

            vScrollBar.Maximum = Math.Max(0, CharSet.Length - 1) / Columns;
            vScrollBar.LargeChange = wholeRows;
        }

        private void UpdateFont()
        {
            Font font = Font;

            if (!object.Equals(_curFont, font))
            {
                if (font != null)
                {
                    _renderArgs.FontFamily = font.FontFamily.Name;
                    _renderArgs.FontStyle = (int) font.Style;
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
            Invalidate();
        }

        private int GetPageStartRow()
        {
            int row = vScrollBar.Value;
            return row;
        }

        public void ScrollTo(int index)
        {
            if (index >= CharSet.Length)
                return;

            CharGridMetrics metrics = _renderArgs.GetMetrics();

            int wholeRows = (int) (_renderArgs.Height / metrics.CellHeight);
            int startIndex = GetPageStartRow() * Columns;
            int endIndex = startIndex + wholeRows * Columns;

            if (_curIndex >= startIndex && _curIndex < endIndex)
                return;

            int row = index / Columns;
            vScrollBar.Value = row;
        }

        public void ScrollCenterTo(int index)
        {
            if (index >= CharSet.Length)
                return;

            CharGridMetrics metrics = _renderArgs.GetMetrics();

            int wholeRows = (int) (_renderArgs.Height / metrics.CellHeight);
            int startIndex = GetPageStartRow() * Columns;
            int endIndex = startIndex + wholeRows * Columns;

            if (_curIndex >= startIndex && _curIndex < endIndex)
                return;

            int selectedRow = index / Columns;
            int row = selectedRow - wholeRows / 2;

            if (row < 0)
                row = 0;

            vScrollBar.Value = row;
        }

        public void SelectCharacter(int index)
        {
            SelectedIndex = index;
            ScrollCenterTo(index);
            Invalidate();
        }

        private void OnSelectedIndexChanged()
        {
            SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
