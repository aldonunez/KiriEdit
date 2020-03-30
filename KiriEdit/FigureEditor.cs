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
        private const float CircleRadius = 5;

        private FigureDocument _document;
        private bool _shown;
        private Rectangle _rectangle;
        private Bitmap _shapeMask;
        private SizeF _curControlScale;

        private float _screenToWorldScale;
        private SizeF _wcRadius;
        private Matrix _worldToScreenMatrix;
        private Matrix _screenToWorldMatrix;


        private bool _trackingLine;
        private Point _startPoint;

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

            canvas.MouseDown += Canvas_MouseDown;
            canvas.MouseMove += Canvas_MouseMove;
            //MouseUp += FigureEditor_MouseUp;
        }

        //private void FigureEditor_MouseUp(object sender, MouseEventArgs e)
        //{
        //}

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_trackingLine)
                return;

            PointF[] pointFs = new PointF[1] { new PointF(_startPoint.X, _startPoint.Y) };

            _worldToScreenMatrix.TransformPoints(pointFs);

            using (var g = Graphics.FromImage(canvas.Image))
            {
                g.Clear(Color.Transparent);

                using (var pen = new Pen(Color.Red, 4 * _curControlScale.Width))
                {
                    pen.DashStyle = DashStyle.Dot;

                    g.DrawLine(pen, pointFs[0].X, pointFs[0].Y, e.X, e.Y);
                }
            }

            canvas.Invalidate();
        }

        private PointGroup FindPointGroup(int x, int y)
        {
            PointF[] pointFs = new PointF[1];

            foreach (var pointGroup in _document.Figure.PointGroups)
            {
                var p = pointGroup.Points[0];

                pointFs[0] = new PointF(p.X, p.Y);

                _worldToScreenMatrix.TransformPoints(pointFs);

                float dX = pointFs[0].X - x;
                float dY = pointFs[0].Y - y;

                double distToPoint = Math.Sqrt(dX * dX + dY * dY);

                if (distToPoint <= CircleRadius)
                {
                    return pointGroup;
                }
            }

            return null;
        }

        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            _trackingLine = false;

            PointGroup pointGroup = FindPointGroup(e.X, e.Y);

            if (pointGroup != null)
            {
                _trackingLine = true;
                _startPoint = pointGroup.Points[0];
            }
        }

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            base.ScaleControl(factor, specified);

            _curControlScale = factor;
        }

        private void OnModified()
        {
            Modified?.Invoke(this, EventArgs.Empty);
        }

        private void Canvas_MouseClick(object sender, MouseEventArgs e)
        {
            // TODO: Or replace MouseClick with MouseUp?
            if (_trackingLine)
            {
                _trackingLine = false;

                PointGroup pointGroup = FindPointGroup(e.X, e.Y);

                if (pointGroup != null && pointGroup != _startPoint.Group)
                {
                    //_document.Figure.AddCut(

                    // TODO: add a cut between the two poing groups
                    // TODO: recalculate shapes
                }

                DrawCanvas();
                canvas.Invalidate();
            }
            else
            {
                Color color = _shapeMask.GetPixel(e.X, e.Y);

                if (color.B == 0)
                    return;

                int index = color.B - 1;

                _document.Shapes[index].Enabled = !_document.Shapes[index].Enabled;

                DrawCanvas();
                canvas.Invalidate();

                OnModified();
            }
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

            _screenToWorldScale = (float) _document.Figure.Height / _rectangle.Height;

            _worldToScreenMatrix = SystemFigurePainter.BuildTransform(_document.Figure, _rectangle);
            _screenToWorldMatrix = _worldToScreenMatrix.Clone();
            _screenToWorldMatrix.Invert();

            _wcRadius = new SizeF(
                CircleRadius * _curControlScale.Width * _screenToWorldScale,
                CircleRadius * _curControlScale.Height * _screenToWorldScale);

            DrawCanvas();
        }

        private void DrawCanvas()
        {
            using (var graphics = Graphics.FromImage(canvas.BackgroundImage))
            using (var maskGraphics = Graphics.FromImage(_shapeMask))
            {
                graphics.Clear(Color.White);
                maskGraphics.Clear(Color.Black);

                using (var painter = new SystemFigurePainter(_document))
                {
                    painter.SetTransform(graphics, _rectangle);
                    painter.SetTransform(maskGraphics, _rectangle);

                    for (int i = 0; i < _document.Shapes.Length; i++)
                    {
                        Brush fillBrush;
                        Color maskColor = Color.FromArgb(0, 0, i + 1);

                        if (_document.Shapes[i].Enabled)
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

            using (var graphics = Graphics.FromImage(canvas.Image))
            {
                // TODO: We're only using SystemFigurePainter for SetTransform. Maybe make a static version of the method?
                using (var painter = new SystemFigurePainter(_document))
                {
                    graphics.Clear(Color.Transparent);

                    //graphics.DrawRectangle(Pens.Purple, 100, 100, 100, 100);
                    using (var pen = new Pen(Brushes.Red, 4 * _curControlScale.Width))
                    {
                        pen.DashStyle = DashStyle.Dot;
                        pen.DashCap = DashCap.Flat;
                        graphics.DrawLine(pen, PointF.Empty, new PointF(canvas.Image.Width, canvas.Image.Height));
                    }

                    painter.SetTransform(graphics, _rectangle);

                    foreach (var pointGroup in _document.Figure.PointGroups)
                    {
                        Point p = pointGroup.Points[0];

                        graphics.DrawEllipse(
                            Pens.Black,
                            p.X - _wcRadius.Width,
                            p.Y - _wcRadius.Height,
                            _wcRadius.Width * 2,
                            _wcRadius.Height * 2);
                    }

#if false
                    using (var brush = new SolidBrush(Color.Red))
                    using (var pen = new Pen(brush, 4 * _curControlScale.Width / scale))
                    {
                        pen.DashStyle = DashStyle.Dot;
                        pen.DashCap = DashCap.Flat;

                        graphics.DrawLine(
                            pen,
                            _document.Figure.PointGroups[0].Points[0].X,
                            _document.Figure.PointGroups[0].Points[0].Y,
                            _document.Figure.PointGroups[2].Points[0].X,
                            _document.Figure.PointGroups[2].Points[0].Y
                            );
                    }
#else
                    using (var brush = new SolidBrush(Color.Red))
                    using (var pen = new Pen(brush, 4 * _curControlScale.Width))
                    {
                        pen.DashStyle = DashStyle.Dot;
                        pen.DashCap = DashCap.Flat;

                        PointF[] points = new PointF[2]
                            {
                                new PointF {
                                    X = _document.Figure.PointGroups[0].Points[0].X,
                                    Y = _document.Figure.PointGroups[0].Points[0].Y },

                                new PointF {
                                    X = _document.Figure.PointGroups[2].Points[0].X,
                                    Y = _document.Figure.PointGroups[2].Points[0].Y },
                            };

                        graphics.Transform.TransformPoints(points);

                        //var container = graphics.BeginContainer();
                        graphics.ResetTransform();
                        graphics.DrawLine(
                            pen,
                            points[0],
                            points[1]);
                        //graphics.DrawLine(pen, PointF.Empty, new PointF(canvas.Image.Width, canvas.Image.Height));
                        //graphics.EndContainer(container);
                    }
#endif
                }
            }
        }
    }
}
