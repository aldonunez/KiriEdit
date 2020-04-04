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
        private const float LineBoundingWidth = LinePenWidth + 4;

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
        private PointGroup _lineEndGroup;
        private PointF _lineStart;
        private PointF _lineEnd;
        private Point _candidatePoint1;
        private Point _candidatePoint2;
        private Cut _candidateCut;

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
                Cut cut = FindCutSc(e.X, e.Y);

                if (cut != null)
                    DeleteLine(cut);
                else
                    TryClickShape(sender, e);
            }
        }

        private void DeleteLine(Cut cut)
        {
            _candidateCut = null;

            _document.Figure.DeleteCut(cut);

            RebuildCanvas();
            OnModified();
        }

        private void TryCommitLine(object sender, MouseEventArgs e)
        {
            _trackingLine = false;

            if (_candidatePoint1 != null && _candidatePoint2 != null)
            {
                _document.Figure.AddCut(_candidatePoint1, _candidatePoint2);

                _lineStartGroup = null;
                _lineEndGroup = null;
                _candidatePoint1 = null;
                _candidatePoint2 = null;

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
                _lineEndGroup = null;
                _lineStart = new PointF(e.X, e.Y);
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_trackingLine)
            {
                _candidateCut = null;

                _lineEnd = new PointF(e.X, e.Y);

                TryCapturePointsForCut(e.X, e.Y);

                DrawCanvas();
                canvas.Invalidate();
            }
            else
            {
                Cut cut = FindCutSc(e.X, e.Y);

                if (cut != _candidateCut)
                {
                    _candidateCut = cut;

                    DrawCanvas();
                    canvas.Invalidate();
                }
            }
        }

        private Cut FindCutSc(int x, int y)
        {
            float halfWidth = (LineBoundingWidth * _curControlScaleSingle) / 2;

            PointF[] pointFs = new PointF[2];

            foreach (var cut in _document.Figure.Cuts)
            {
                pointFs[0] = new PointF(cut.PairedEdge1.P1.X, cut.PairedEdge1.P1.Y);
                pointFs[1] = new PointF(cut.PairedEdge1.P2.X, cut.PairedEdge1.P2.Y);

                _worldToScreenMatrix.TransformPoints(pointFs);

                // Translate by P1, so P1 is the origin.

                PointF translatedRef = new PointF(x - pointFs[0].X, y - pointFs[0].Y);
                PointF translatedP2 = new PointF(pointFs[1].X - pointFs[0].X, pointFs[1].Y - pointFs[0].Y);

                // Get the cut's angle.

                double angle = Math.Atan2(translatedP2.Y, translatedP2.X);

                // Rotate by negative angle.

                double sin = Math.Sin(-angle);
                double cos = Math.Cos(-angle);

                PointF rotatedRef = new PointF(
                    (float) (translatedRef.X * cos - translatedRef.Y * sin),
                    (float) (translatedRef.X * sin + translatedRef.Y * cos));

                PointF rotatedP2 = new PointF(
                    (float) (translatedP2.X * cos - translatedP2.Y * sin),
                    (float) (translatedP2.X * sin + translatedP2.Y * cos));

                if (   rotatedRef.Y >= -halfWidth && rotatedRef.Y <= halfWidth
                    && rotatedRef.X >=  0 && rotatedRef.X <= rotatedP2.X)
                    return cut;
            }

            return null;
        }

        private void TryCapturePointsForCut(int x, int y)
        {
            PointGroup pointGroup = FindPointGroupSc(x, y);

            if (pointGroup != _lineEndGroup)
            {
                Point p1 = null;
                Point p2 = null;

                if (pointGroup != null && pointGroup != _lineStartGroup)
                {
                    (p1, p2) = Figure.FindPointsForCut(_lineStartGroup, pointGroup);
                }

                if (p1 != null && p2 != null)
                    _lineEndGroup = pointGroup;
                else
                    _lineEndGroup = null;

                _candidatePoint1 = p1;
                _candidatePoint2 = p2;
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
                DrawCuts(graphics);
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

                    if (pointGroup == _lineStartGroup || pointGroup == _lineEndGroup)
                        pen.Color = Color.Red;
                    else
                        pen.Color = Color.Black;

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

        private void DrawCuts(Graphics graphics)
        {
            PointF[] pointFs = new PointF[2];

            using (var pen = new Pen(Color.Turquoise, LinePenWidth * _curControlScaleSingle))
            {
                pen.DashStyle = DashStyle.Dot;

                foreach (var cut in _document.Figure.Cuts)
                {
                    pointFs[0] = new PointF(cut.PairedEdge1.P1.X, cut.PairedEdge1.P1.Y);
                    pointFs[1] = new PointF(cut.PairedEdge1.P2.X, cut.PairedEdge1.P2.Y);

                    _worldToScreenMatrix.TransformPoints(pointFs);

                    if (cut == _candidateCut)
                        pen.Color = Color.Red;
                    else
                        pen.Color = Color.LightPink;

                    graphics.DrawLine(pen, pointFs[0], pointFs[1]);
                }
            }
        }
    }
}
