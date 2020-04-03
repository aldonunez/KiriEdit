using KiriProj;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Point = TryFreetype.Model.Point;

namespace KiriEdit
{
    public partial class FigureEditor : UserControl
    {
        private const float CircleRadius = 4;
        private const float CirclePenWidth = 1;

        private FigureDocument _document;
        private bool _shown;
        private Rectangle _rectangle;
        private Bitmap _shapeMask;

        private float _curControlScaleSingle;
        private SizeF _curControlScaleSize;
        private Matrix _worldToScreenMatrix;

        public event EventHandler Modified;

        public FigureDocument Document
        {
            get => _document;
            set
            {
                if (value != _document)
                {
                    _document = value;

                    if (_shown)
                        RebuildCanvas();
                }
            }
        }

        public FigureEditor()
        {
            InitializeComponent();
        }

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            base.ScaleControl(factor, specified);

            _curControlScaleSingle = Math.Min(factor.Width, factor.Height);
            _curControlScaleSize = factor;
        }

        private void OnModified()
        {
            Modified?.Invoke(this, EventArgs.Empty);
        }

        private void Canvas_MouseClick(object sender, MouseEventArgs e)
        {
            Color color = _shapeMask.GetPixel(e.X, e.Y);

            if (color.B == 0)
                return;

            int index = color.B - 1;

            _document.Figure.Shapes[index].Enabled = !_document.Figure.Shapes[index].Enabled;

            DrawCanvas();
            canvas.Invalidate();

            OnModified();
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

            if (canvas.Image != null)
            {
                canvas.Image.Dispose();
                canvas.Image = null;
            }

            if (_shapeMask != null)
            {
                _shapeMask.Dispose();
                _shapeMask = null;
            }

            Size picBoxSize = canvas.ClientSize;
            int height = (int) (picBoxSize.Height * 0.95f);
            int width = height;

            Rectangle rect = DrawingUtils.CenterFigure(_document.Figure, new Size(width, height));

            rect.X = (picBoxSize.Width - rect.Width) / 2;
            rect.Y = (picBoxSize.Height - rect.Height) / 2;

            _rectangle = rect;
            _shapeMask = new Bitmap(picBoxSize.Width, picBoxSize.Height);
            canvas.BackgroundImage = new Bitmap(picBoxSize.Width, picBoxSize.Height);
            canvas.Image = new Bitmap(picBoxSize.Width, picBoxSize.Height);

            _worldToScreenMatrix = SystemFigurePainter.BuildTransform(_document.Figure, _rectangle);

            DrawCanvas();
        }

        private void DrawCanvas()
        {
            DrawBackgroundShapes();
            DrawOverlay();
        }

        private void DrawBackgroundShapes()
        {
            using (var graphics = Graphics.FromImage(canvas.BackgroundImage))
            using (var maskGraphics = Graphics.FromImage(_shapeMask))
            {
                graphics.Clear(Color.White);
                maskGraphics.Clear(Color.Black);

                using (var painter = new SystemFigurePainter(_document))
                {
                    graphics.Transform = _worldToScreenMatrix;
                    maskGraphics.Transform = _worldToScreenMatrix;

                    for (int i = 0; i < _document.Figure.Shapes.Count; i++)
                    {
                        Brush fillBrush;
                        Color maskColor = Color.FromArgb(0, 0, i + 1);

                        if (_document.Figure.Shapes[i].Enabled)
                            fillBrush = Brushes.Black;
                        else
                            fillBrush = Brushes.LightGray;

                        painter.PaintShape(i);
                        painter.Fill(graphics, fillBrush);

                        using (var brush = new SolidBrush(maskColor))
                        {
                            painter.Fill(maskGraphics, brush);
                        }
                    }

                    painter.PaintFull();
                    painter.Draw(graphics);
                }
            }
        }

        private void DrawOverlay()
        {
            using (var graphics = Graphics.FromImage(canvas.Image))
            {
                graphics.Clear(Color.Transparent);

                // Overlay elements are drawn using screen coordinates, to have better control
                // of their looks.

                graphics.ResetTransform();

                DrawPoints(graphics);
            }
        }

        private void DrawPoints(Graphics graphics)
        {
            PointF[] pointFs = new PointF[1];

            float circleRadius = CircleRadius * _curControlScaleSingle;
            float penWidth = (float) Math.Round(CirclePenWidth * _curControlScaleSingle);

            using (var pen = new Pen(Color.Black, penWidth))
            {
                foreach (var pointGroup in _document.Figure.PointGroups)
                {
                    Point p = pointGroup.Points[0];

                    pointFs[0] = new PointF(p.X, p.Y);
                    _worldToScreenMatrix.TransformPoints(pointFs);

                    if (pointGroup.IsFixed)
                    {
                        graphics.DrawRectangle(
                            pen,
                            pointFs[0].X - circleRadius,
                            pointFs[0].Y - circleRadius,
                            circleRadius * 2,
                            circleRadius * 2);
                    }
                    else
                    {
                        graphics.DrawEllipse(
                            pen,
                            pointFs[0].X - circleRadius,
                            pointFs[0].Y - circleRadius,
                            circleRadius * 2,
                            circleRadius * 2);
                    }
                }
            }
        }
    }
}
