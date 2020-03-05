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

            IntPtr hdc = e.Graphics.GetHdc();

            try
            {
                KiriFT.Drawing.CharGridRendererArgs args = new KiriFT.Drawing.CharGridRendererArgs();

                args.Columns = 20;
                args.FirstCodePoint = 0x1F000;
                //args.FirstCodePoint = 0x4e2d;
                //args.FirstCodePoint = 'A';
                //args.FontFamily = _fontCollection.Families[0].Name;
                args.FontFamily = "Segoe UI";
                args.FontStyle = 0;
                args.Hdc = hdc;
                args.Height = _contentHeight;
                args.Width = _contentWidth;
                args.HeightToWidth = HeightToWidth;
                args.OnColor = Color.Red.ToArgb();

                KiriFT.Drawing.CharGridRenderer.Draw(args);
            }
            finally
            {
                e.Graphics.ReleaseHdc(hdc);
            }
        }

        private void UpdateFont()
        {
            _contentWidth = vScrollBar.Left;
            _contentHeight = Height;

            _cellWidth = _contentWidth / (float) Columns;
            _cellHeight = _cellWidth * HeightToWidth;
        }
    }
}
