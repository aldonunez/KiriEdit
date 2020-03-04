using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Text;
using KiriFT;

namespace KiriEdit
{
    public partial class CharacterGrid : UserControl
    {
        private const int Columns = 20;
        private const float HeightToWidth = 37f / 32f;

        private Library _library;
        private Face _face;
        private PrivateFontCollection _fontCollection;

        private int _contentWidth;
        private int _contentHeight;
        private float _cellWidth;
        private float _cellHeight;

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

            UpdateFont();
        }

        private void CharacterGrid_Resize(object sender, EventArgs e)
        {
            UpdateFont();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            int rows = (int) Math.Ceiling(_contentHeight / _cellHeight);

            float x = 0;
            float y = 0;

            for (int i = 0; i < Columns; i++)
            {
                e.Graphics.DrawLine(Pens.Black, x, 0, x, _contentHeight);
                x += _cellWidth;
            }

            e.Graphics.DrawLine(Pens.Black, vScrollBar.Left - 1, 0, vScrollBar.Left - 1, _contentHeight);

            for (int i = 0; i < rows; i++)
            {
                e.Graphics.DrawLine(Pens.Black, 0, y, _contentWidth, y);
                y += _cellHeight;
            }

            e.Graphics.DrawLine(Pens.Black, 0, _contentHeight - 1, _contentWidth, _contentHeight - 1);

            uint codePoint = 32;

            float ycell = 0;
            float xcell = 0;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    _face.SetPixelSizes((uint) _cellWidth, (uint) _cellHeight);
                    //_face.SetPixelSizes(0, 160);
                    _face.LoadChar(codePoint);

                    //var bbox = face.GetBBox();
                    //var metrics = face.GetMetrics();

                    //int width = ((metrics.Width + 63) / 64) * 64;
                    //int height = ((metrics.Height + 63) / 64) * 64;


                    string s = CharUtils.GetString(codePoint);
                    SizeF sizes = e.Graphics.MeasureString(s, Font);

#if false
                    var bbox = _face.GetBBox();

                    var metrics = _face.GetMetrics();
                    float width = ((metrics.Width + 63) / 64f);
                    float height = ((metrics.Height + 63) / 64f);

                    x = xcell + (cellWidth - width) / 2;
                    y = ycell + (cellHeight - height) / 2;

                    //if (s == "A")
                    //    System.Diagnostics.Debugger.Break();
#else
                    x = xcell + (_cellWidth - sizes.Width) / 2 + 1;
                    y = ycell + (_cellHeight - sizes.Height) / 2 + 1;
#endif

                    e.Graphics.DrawString(s, Font, Brushes.Black, x, y);

                    xcell += _cellWidth;
                    codePoint++;
                }

                xcell = 0;
                ycell += _cellHeight;
            }
        }

        private void UpdateFont()
        {
            _contentWidth = vScrollBar.Left;
            _contentHeight = Height;

            _cellWidth = _contentWidth / (float) Columns;
            _cellHeight = _cellWidth * HeightToWidth;

            float emSize = _cellHeight * 0.70f;

            Font = new Font(_fontCollection.Families[0], emSize, GraphicsUnit.Pixel);
        }
    }
}
