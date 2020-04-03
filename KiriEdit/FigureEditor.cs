using KiriProj;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TryFreetype.Model;
using Point = TryFreetype.Model.Point;

namespace KiriEdit
{
    public partial class FigureEditor : UserControl
    {
        private const float CircleRadius = 4;
        private const float CirclePenWidth = 1;
        private const float LinePenWidth = 4;

        private FigureDocument _document;
        private bool _shown;
        private Rectangle _rectangle;
        private Bitmap _shapeMask;

        private float _curControlScaleSingle;
        private SizeF _curControlScaleSize;
        private Matrix _worldToScreenMatrix;
        private Matrix _screenToWorldMatrix;
        private float _screenToWorldScale;

        private bool _trackingLine;
        private PointGroup _lineStartGroup;
        private PointF _lineStart;
        private PointF _lineEnd;

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
            if (_trackingLine)
            {
                TryCommitLine(sender, e);
            }
            else
            {
                TryClickShape(sender, e);
            }
        }

        private void TryCommitLine(object sender, MouseEventArgs e)
        {
            _trackingLine = false;

            PointGroup pointGroup = FindPointGroupSc(e.X, e.Y);

            if (pointGroup != null && pointGroup != _lineStartGroup)
            {
                var (p1, p2) = Figure.FindPointsForCut(_lineStartGroup, pointGroup);

                _document.Figure.AddCut(p1, p2);

                RebuildCanvas();
                OnModified();
            }
            else
            {
                DrawCanvas();
            }

            canvas.Invalidate();
        }

        private void TryClickShape(object sender, MouseEventArgs e)
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

        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            _trackingLine = false;

            PointGroup pointGroup = FindPointGroupSc(e.X, e.Y);

            if (pointGroup != null)
            {
                _trackingLine = true;
                _lineStartGroup = pointGroup;
                _lineStart = new PointF(e.X, e.Y);
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_trackingLine)
            {
                _lineEnd = new PointF(e.X, e.Y);
                DrawCanvas();
                canvas.Invalidate();
            }
        }

        // Find a point group given a point in screen coordinates.

        private PointGroup FindPointGroupSc(int x, int y)
        {
            PointF[] pointFs = new PointF[1] { new PointF(x, y) };

            _screenToWorldMatrix.TransformPoints(pointFs);

            return FindPointGroupWc((int) pointFs[0].X, (int) pointFs[0].Y);
        }

        // Find a point group given a point in world coordinates.

        private PointGroup FindPointGroupWc(int x, int y)
        {
            float wcCircleRadius = CircleRadius * _curControlScaleSingle * _screenToWorldScale;

            foreach (var pointGroup in _document.Figure.PointGroups)
            {
                var p = pointGroup.Points[0];

                double distance = DrawingUtils.GetLineLength(x, y, p.X, p.Y);

                if (distance <= wcCircleRadius)
                    return pointGroup;
            }

            return null;
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
            _screenToWorldMatrix = _worldToScreenMatrix.Clone();
            _screenToWorldMatrix.Invert();
            _screenToWorldScale = (float) _document.Figure.Height / _rectangle.Height;

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
                DrawLine(graphics);
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

        private void DrawLine(Graphics graphics)
        {
            if (!_trackingLine)
                return;

            using (var pen = new Pen(Color.Red, LinePenWidth * _curControlScaleSingle))
            {
                pen.DashStyle = DashStyle.Dot;

                graphics.DrawLine(pen, _lineStart, _lineEnd);
            }
        }
    }
}
