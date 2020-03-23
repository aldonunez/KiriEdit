using System;
using System.Drawing;
using System.Windows.Forms;

namespace KiriEdit
{
    public partial class FigureEditor : UserControl
    {
        private FigureItem _figureItem;
        private FigureDocument _document;
        private bool _shown;
        private Rectangle _rectangle;

        public FigureItem FigureItem
        {
            get => _figureItem;
            set
            {
                if (value != _figureItem)
                {
                    _figureItem = value;

                    if (_figureItem != null)
                    {
                        _document = _figureItem.Open();

                        if (_shown)
                            RebuildCanvas();
                    }
                    else
                    {
                        _document = null;
                    }
                }
            }
        }

        public FigureEditor()
        {
            InitializeComponent();
        }

        private void FigureEditor_VisibleChanged(object sender, EventArgs e)
        {
            // Only handle this when shown for the first time.

            VisibleChanged -= FigureEditor_VisibleChanged;
            _shown = true;

            RebuildCanvas();
        }

        private void FigureEditor_Resize(object sender, EventArgs e)
        {
            if (_shown)
                RebuildCanvas();
        }

        private void RebuildCanvas()
        {
            if (_document == null || !IsHandleCreated)
                return;

            if (canvas.BackgroundImage != null)
            {
                canvas.BackgroundImage.Dispose();
                canvas.BackgroundImage = null;
            }

            Size picBoxSize = canvas.ClientSize;
            int height = (int) (picBoxSize.Height * 0.80f);
            int width = (int) (height * 32f / 37f);

            Rectangle rect = new Rectangle(
                (int) (picBoxSize.Width - width) / 2,
                (int) (picBoxSize.Height - height) / 2,
                width,
                height);

            Bitmap bitmap = new Bitmap(picBoxSize.Width, picBoxSize.Height);

            DrawCanvas(bitmap, rect);

            canvas.BackgroundImage = bitmap;
            _rectangle = rect;
        }

        private void DrawCanvas(Bitmap bitmap, Rectangle rect)
        {
            using (var graphics = Graphics.FromImage(bitmap))
            {
                using (var painter = new SystemFigurePainter(_document, graphics, rect))
                {
                    for (int i = 0; i < _document.Shapes.Length; i++)
                    {
                        if (_document.Shapes[i].Enabled)
                        {
                            painter.PaintShape(i);
                            painter.Fill();
                        }
                    }

                    painter.PaintFull();
                    painter.Draw();
                }
            }
        }
    }
}
