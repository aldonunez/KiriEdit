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

        private Library _library;
        private Face _face;
        private PrivateFontCollection _fontCollection;
        private CharGridRendererArgs _renderArgs = new CharGridRendererArgs();

        private int _contentWidth;
        private int _contentHeight;

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
            _renderArgs.FontFamily = _fontCollection.Families[0].Name;
            _renderArgs.FontStyle = 0;
            _renderArgs.HeightToWidth = HeightToWidth;
            _renderArgs.OnColor = Color.Black.ToArgb();
            _renderArgs.OffColor = Color.Gray.ToArgb();

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
                _renderArgs.FirstCodePoint = ' ';
                _renderArgs.Hdc = hdc;
                _renderArgs.Height = _contentHeight;
                _renderArgs.Width = _contentWidth;

                CharGridRenderer.Draw(_renderArgs);
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
        }
    }
}
