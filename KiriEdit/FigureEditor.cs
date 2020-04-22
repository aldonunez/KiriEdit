/*
   Copyright 2020 Aldo J. Nunez

   Licensed under the Apache License, Version 2.0.
   See the LICENSE.txt file for details.
*/

using KiriFig;
using KiriFig.Model;
using KiriProj;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Point = KiriFig.Model.Point;

namespace KiriEdit
{
    public partial class FigureEditor : UserControl
    {
        private const float CircleRadius = 4;
        private const float CirclePenWidth = 1;
        private const float LinePenWidth = 4;
        private const float LineBoundingWidth = LinePenWidth + 4;
        private const float SnapMargin = 10;

        private FigureDocument _document;
        private FigureContext _context;
        private bool _shown;
        private Rectangle _rectangle;
        private Bitmap _shapeMask;
        private Tool _tool;

        private float _curControlScaleSingle;
        private SizeF _curControlScaleSize;
        private Matrix _worldToScreenMatrix;
        private Matrix _screenToWorldMatrix;
        private float _screenToWorldScale;

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

                    UpdateFigureContext();
                }
            }
        }

        public HistoryBuffer History { get; set; }

        public FigureEditor()
        {
            InitializeComponent();

            _tool = new LineTool(this);
        }

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            base.ScaleControl(factor, specified);

            _curControlScaleSingle = Math.Min(factor.Width, factor.Height);
            _curControlScaleSize = factor;
        }

        private void UpdateFigureContext()
        {
            if (_document == null)
                return;

            _context = new FigureContext(this);
        }

        private void AddHistory(HistoryCommand command)
        {
            if (History != null)
                History.Add(command);
        }

        private void OnModified()
        {
            Modified?.Invoke(this, EventArgs.Empty);
        }

        private void Canvas_MouseClick(object sender, MouseEventArgs e)
        {
            _tool.OnMouseClick(sender, e);
        }

        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            _tool.OnMouseDown(sender, e);
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            _tool.OnMouseMove(sender, e);
        }

        private void TryClickShape(object sender, MouseEventArgs e)
        {
            Color color = _shapeMask.GetPixel(e.X, e.Y);

            if (color.B == 0)
                return;

            int index = color.B - 1;

            var cmd = new EnableShapeCommand(_context, index, !_document.Figure.Shapes[index].Enabled);

            cmd.Apply();
            AddHistory(cmd);
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

        private void RedrawOverlay()
        {
            DrawOverlay();
            canvas.Invalidate();
        }

        private void RedrawCanvas()
        {
            DrawCanvas();
            canvas.Invalidate();
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

                _tool.Draw(graphics);
            }
        }

        private void DrawPoints(Graphics graphics, PointGroup hilitGroup1, PointGroup hilitGroup2)
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

                    if (pointGroup == hilitGroup1 || pointGroup == hilitGroup2)
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

        private void DrawCuts(Graphics graphics, Cut hilitCut)
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

                    if (cut == hilitCut)
                        pen.Color = Color.Red;
                    else
                        pen.Color = Color.LightPink;

                    graphics.DrawLine(pen, pointFs[0], pointFs[1]);
                }
            }
        }

        private void lineButton_Click(object sender, EventArgs e)
        {
            _tool = new LineTool(this);
            OnGroupButtonClick(sender, e);
        }

        private void pointButton_Click(object sender, EventArgs e)
        {
            _tool = new PointTool(this);
            OnGroupButtonClick(sender, e);
        }

        private void OnGroupButtonClick(object sender, EventArgs e)
        {
            foreach (var item in editorToolStrip.Items)
            {
                if (item != sender && item is ToolStripButton button)
                {
                    button.Checked = false;
                }
            }
        }

        private float ScreenToWorld(float f)
        {
            return f * _curControlScaleSingle * _screenToWorldScale;
        }


        #region Inner classes

        private abstract class Tool
        {
            public abstract void OnMouseClick(object sender, MouseEventArgs e);
            public abstract void OnMouseDown(object sender, MouseEventArgs e);
            public abstract void OnMouseMove(object sender, MouseEventArgs e);
            public abstract void Draw(Graphics graphics);
        }


        private class LineTool : Tool
        {
            private FigureEditor _parent;

            private bool _trackingLine;
            private PointGroup _lineStartGroup;
            private PointGroup _lineEndGroup;
            private PointF _lineStart;
            private PointF _lineEnd;
            private Point _candidatePoint1;
            private Point _candidatePoint2;
            private Cut _candidateCut;

            public LineTool(FigureEditor parent)
            {
                _parent = parent;
            }

            public override void OnMouseClick(object sender, MouseEventArgs e)
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
                        _parent.TryClickShape(sender, e);
                }
            }

            private void DeleteLine(Cut cut)
            {
                _candidateCut = null;

                var cmd = new DeleteCutCommand(_parent._context, cut);

                cmd.Apply();
                _parent.AddHistory(cmd);
            }

            private void TryCommitLine(object sender, MouseEventArgs e)
            {
                _trackingLine = false;

                if (_candidatePoint1 != null && _candidatePoint2 != null)
                {
                    var cmd = new AddCutCommand(_parent._context, _candidatePoint1.Group, _candidatePoint2.Group);

                    cmd.Apply();
                    _parent.AddHistory(cmd);

                    _lineStartGroup = null;
                    _lineEndGroup = null;
                    _candidatePoint1 = null;
                    _candidatePoint2 = null;
                }
                else
                {
                    _parent.RedrawOverlay();
                }
            }

            private Cut FindCutSc(int x, int y)
            {
                float halfWidth = (LineBoundingWidth * _parent._curControlScaleSingle) / 2;

                PointF[] pointFs = new PointF[2];

                foreach (var cut in _parent._document.Figure.Cuts)
                {
                    pointFs[0] = new PointF(cut.PairedEdge1.P1.X, cut.PairedEdge1.P1.Y);
                    pointFs[1] = new PointF(cut.PairedEdge1.P2.X, cut.PairedEdge1.P2.Y);

                    _parent._worldToScreenMatrix.TransformPoints(pointFs);

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

                    // Is the mouse in this box?

                    if (rotatedRef.Y >= -halfWidth && rotatedRef.Y <= halfWidth
                        && rotatedRef.X >= 0 && rotatedRef.X <= rotatedP2.X)
                        return cut;
                }

                return null;
            }

            private void TryCapturePointsForCut(int x, int y)
            {
                PointGroup pointGroup = _parent.FindPointGroupSc(x, y);

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

            public override void OnMouseDown(object sender, MouseEventArgs e)
            {
                _trackingLine = false;

                PointGroup pointGroup = _parent.FindPointGroupSc(e.X, e.Y);

                if (pointGroup != null)
                {
                    _trackingLine = true;
                    _lineStartGroup = pointGroup;
                    _lineEndGroup = null;
                    _lineStart = new PointF(e.X, e.Y);
                }
            }

            public override void OnMouseMove(object sender, MouseEventArgs e)
            {
                if (_trackingLine)
                {
                    _candidateCut = null;

                    _lineEnd = new PointF(e.X, e.Y);

                    TryCapturePointsForCut(e.X, e.Y);

                    _parent.RedrawOverlay();
                }
                else
                {
                    Cut cut = FindCutSc(e.X, e.Y);

                    if (cut != _candidateCut)
                    {
                        _candidateCut = cut;

                        _parent.RedrawOverlay();
                    }
                }
            }

            public override void Draw(Graphics graphics)
            {
                _parent.DrawPoints(graphics, _lineStartGroup, _lineEndGroup);
                _parent.DrawCuts(graphics, _candidateCut);
                DrawLine(graphics);
            }

            private void DrawLine(Graphics graphics)
            {
                if (!_trackingLine)
                    return;

                using (var pen = new Pen(Color.Red, LinePenWidth * _parent._curControlScaleSingle))
                {
                    pen.DashStyle = DashStyle.Dot;

                    graphics.DrawLine(pen, _lineStart, _lineEnd);
                }
            }
        }


        private class PointTool : Tool
        {
            private FigureEditor _parent;

            private PointF _candidatePoint;
            private Edge _candidateEdge;
            private PointGroup _hilitGroup;
            private Point _snapPoint;

            public PointTool(FigureEditor parent)
            {
                _parent = parent;
            }

            public override void Draw(Graphics graphics)
            {
                _parent.DrawPoints(graphics, _hilitGroup, null);
                _parent.DrawCuts(graphics, null);
                DrawPoint(graphics);
            }

            public override void OnMouseClick(object sender, MouseEventArgs e)
            {
                if (_candidateEdge != null)
                {
                    var cmd = new AddPointCommand(
                        _parent._context,
                        (int) _candidatePoint.X,
                        (int) _candidatePoint.Y,
                        _candidateEdge);

                    cmd.Apply();
                    _parent.AddHistory(cmd);

                    _candidateEdge = null;
                }
                else if (_hilitGroup != null)
                {
                    var cmd = new DeletePointCommand(_parent._context, _hilitGroup);

                    cmd.Apply();
                    _parent.AddHistory(cmd);

                    _hilitGroup = null;
                }
            }

            public override void OnMouseDown(object sender, MouseEventArgs e)
            {
                // Nothing to do.
            }

            private struct EdgeSearchResult
            {
                public float Distance;
                public Edge Edge;
                public PointF Point;
                public double T;

                public EdgeSearchResult(float distance, Edge edge, PointF point, double t)
                {
                    Distance = distance;
                    Edge = edge;
                    Point = point;
                    T = t;
                }
            }

            public override void OnMouseMove(object sender, MouseEventArgs e)
            {
                if (e.Button != MouseButtons.None)
                    return;

                var pointGroup = _parent.FindPointGroupSc(e.X, e.Y);

                if (pointGroup != null)
                {
                    _candidateEdge = null;
                    MoveOverPoint(pointGroup);
                }
                else
                {
                    _hilitGroup = null;
                    MoveOverEdge(e.X, e.Y);
                }

                _parent.RedrawOverlay();
            }

            private void MoveOverPoint(PointGroup pointGroup)
            {
                if (!pointGroup.IsFixed && pointGroup.Points.Count == 1)
                {
                    _hilitGroup = pointGroup;
                }
            }

            private void MoveOverEdge(int x, int y)
            {
                EdgeSearchResult result = FindNearestEdgeSc(x, y);

                // Only show a point along an edge, if it's near enough to it.

                float visibleDistance = 10 * _parent._curControlScaleSingle * _parent._screenToWorldScale;

                if (result.Edge != null && result.Distance <= visibleDistance)
                {
                    _snapPoint = null;

                    if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                    {
                        var (t, point) = FindPointToSnapTo(result.Edge, result.T);

                        if (point != null)
                        {
                            PointD p = result.Edge.Calculate(t);

                            result.Point = new PointF((float) p.X, (float) p.Y);
                            _snapPoint = point;
                        }
                    }

                    _candidateEdge = result.Edge;
                    _candidatePoint = result.Point;
                }
                else
                {
                    _candidateEdge = null;
                }
            }

            private (double, Point) FindPointToSnapTo(Edge edge, double referenceT)
            {
                double leastT = 2;
                Point point = null;

                BBox bbox = edge.GetBBox();

                int margin = (int) _parent.ScreenToWorld(SnapMargin);

                // Look at each point that doesn't align with the edge's endpoints.

                foreach (var pointGroup in _parent.Document.Figure.PointGroups)
                {
                    foreach (var p in pointGroup.Points)
                    {
                        if (p.X == edge.P1.X
                            || p.Y == edge.P1.Y
                            || p.X == edge.P2.X
                            || p.Y == edge.P2.Y)
                            continue;

                        // If a horizontal or vertical ray from a point crosses the edge, then consider it.
                        // Either ray might cross the edge.

                        double t1 = double.NaN;
                        double t2 = double.NaN;
                        double t;

                        if (p.X - margin >= bbox.Left && p.X + margin <= bbox.Right)
                        {
                            t1 = edge.GetIntersection(referenceT, p.X, Axis.X);
                        }

                        if (p.Y - margin >= bbox.Bottom && p.Y + margin <= bbox.Top)
                        {
                            t2 = edge.GetIntersection(referenceT, p.Y, Axis.Y);
                        }

                        // See which of the two crossings is nearer. Keep in mind that they might not exist.

                        if (double.IsNaN(t1))
                            t = t2;
                        else if (double.IsNaN(t2))
                            t = t1;
                        else if (Math.Abs(referenceT - t1) < Math.Abs(referenceT - t2))
                            t = t1;
                        else
                            t = t2;

                        // Of all the points, look for the one that's nearest the reference t value.

                        if (!double.IsNaN(t) && Math.Abs(referenceT - t) < Math.Abs(referenceT - leastT))
                        {
                            leastT = t;
                            point = p;
                        }
                    }
                }

                return (leastT, point);
            }

            private EdgeSearchResult FindNearestEdgeSc(int x, int y)
            {
                // Change cursor point and padding amount to world coordinates.

                PointF[] pointFs = new PointF[1] { new PointF(x, y) };

                _parent._screenToWorldMatrix.TransformPoints(pointFs);

                var p = new System.Drawing.Point((int) pointFs[0].X, (int) pointFs[0].Y);

                int padding = (int) (20 * _parent._curControlScaleSingle * _parent._screenToWorldScale);

                // Check every edge whose bounding box the mouse cursor is in.
                // Find the nearest point to the mouse cursor among these edges.

                EdgeSearchResult result;

                result.Distance = float.PositiveInfinity;
                result.Edge = null;
                result.Point = new PointF();
                result.T = -1;

                foreach (var group in _parent._document.Figure.PointGroups)
                {
                    foreach (var point in group.Points)
                    {
                        Edge edge = point.OutgoingEdge;

                        if (edge.Unbreakable)
                            continue;

                        BBox box = edge.GetBBox();

                        box.Inflate(padding, padding);

                        if (box.Contains(p.X, p.Y))
                        {
                            var (t, optProjection) = edge.GetProjectedPoint(p.X, p.Y);

                            if (t >= 0 && t <= 1)
                            {
                                PointF projection = new PointF(
                                    (float) optProjection.X,
                                    (float) optProjection.Y);

                                float dX = p.X - projection.X;
                                float dY = p.Y - projection.Y;

                                float distance = (float) Math.Sqrt(dX * dX + dY * dY);

                                if (distance < result.Distance)
                                {
                                    result.Distance = distance;
                                    result.Edge = edge;
                                    result.Point = projection;
                                    result.T = t;
                                }
                            }
                        }
                    }
                }

                return result;
            }

            private void DrawPoint(Graphics graphics)
            {
                if (_candidateEdge == null)
                    return;

                PointF[] pointFs = new PointF[1];

                float circleRadius = CircleRadius * _parent._curControlScaleSingle;
                float penWidth = (float) Math.Round(CirclePenWidth * _parent._curControlScaleSingle);

                using (var pen = new Pen(Color.Black, penWidth))
                {
                    pointFs[0] = _candidatePoint;
                    _parent._worldToScreenMatrix.TransformPoints(pointFs);

                    graphics.DrawEllipse(
                        pen,
                        pointFs[0].X - circleRadius,
                        pointFs[0].Y - circleRadius,
                        circleRadius * 2,
                        circleRadius * 2);

                    if (_snapPoint != null)
                    {
                        PointF begin = pointFs[0];

                        pointFs[0] = new PointF(_snapPoint.X, _snapPoint.Y);
                        _parent._worldToScreenMatrix.TransformPoints(pointFs);

                        pen.Color = Color.Yellow;

                        graphics.DrawLine(
                            pen,
                            begin.X,
                            begin.Y,
                            pointFs[0].X,
                            pointFs[0].Y);
                    }
                }
            }
        }


        private class FigureContext
        {
            private FigureEditor _figureEditor;
            private int _nextId;

            private Dictionary<int, object> _idToObj = new Dictionary<int, object>();
            private Dictionary<object, int> _objToId = new Dictionary<object, int>();

            public Figure Figure => _figureEditor._document.Figure;

            public FigureContext(FigureEditor figureEditor)
            {
                _figureEditor = figureEditor;
            }

            public void OnModifiedShapes()
            {
                _figureEditor.RebuildCanvas();
                _figureEditor.OnModified();
            }

            public void OnEnabledChanged()
            {
                _figureEditor.RedrawCanvas();
                _figureEditor.OnModified();
            }

            private int GetId<T>(T obj)
            {
                if (!_objToId.ContainsKey(obj))
                    return -1;

                return _objToId[obj];
            }

            public T Get<T>(int id)
            {
                return (T) _idToObj[id];
            }

            public int Add<T>(int id, T obj)
            {
                if (id < 0)
                    id = _nextId++;

                _idToObj.Add(id, obj);
                _objToId.Add(obj, id);

                return id;
            }

            public void Remove<T>(int id)
            {
                T obj = (T) _idToObj[id];

                _idToObj.Remove(id);
                _objToId.Remove(obj);
            }

            public int Import<T>(T obj)
            {
                int id = GetId(obj);

                if (id < 0)
                    id = Add(-1, obj);

                return id;
            }
        }


        private abstract class AddDeleteCutCommandBase : HistoryCommand
        {
            protected FigureContext _context;
            protected int _pointGroup1, _pointGroup2;
            protected int _cut = -1;

            public AddDeleteCutCommandBase(FigureContext context, PointGroup pointGroup1, PointGroup pointGroup2)
            {
                _context = context;
                _pointGroup1 = _context.Import(pointGroup1);
                _pointGroup2 = _context.Import(pointGroup2);
            }

            public AddDeleteCutCommandBase(FigureContext context, Cut cut)
            {
                _context = context;
                _cut = _context.Import(cut);
                _pointGroup1 = _context.Import(cut.PairedEdge1.P1.Group);
                _pointGroup2 = _context.Import(cut.PairedEdge1.P2.Group);
            }

            public void Add()
            {
                PointGroup pg1 = _context.Get<PointGroup>(_pointGroup1);
                PointGroup pg2 = _context.Get<PointGroup>(_pointGroup2);

                var (p1, p2) = Figure.FindPointsForCut(pg1, pg2);

                Cut cut = _context.Figure.AddCut(p1, p2);

                _cut = _context.Add(_cut, cut);
                _context.OnModifiedShapes();
            }

            public void Delete()
            {
                Cut cut = _context.Get<Cut>(_cut);

                _context.Remove<Cut>(_cut);
                _context.Figure.DeleteCut(cut);
                _context.OnModifiedShapes();
            }
        }


        private class AddCutCommand : AddDeleteCutCommandBase
        {
            public AddCutCommand(FigureContext context, PointGroup pointGroup1, PointGroup pointGroup2) :
                base(context, pointGroup1, pointGroup2)
            {
            }

            public override void Apply()
            {
                base.Add();
            }

            public override void Unapply()
            {
                base.Delete();
            }
        }


        private class DeleteCutCommand : AddDeleteCutCommandBase
        {
            public DeleteCutCommand(FigureContext context, Cut cut) :
                base(context, cut)
            {
            }

            public override void Apply()
            {
                base.Delete();
            }

            public override void Unapply()
            {
                base.Add();
            }
        }


        private abstract class AddDeletePointCommandBase : HistoryCommand
        {
            private FigureContext _context;
            private int _x, _y;
            private int _pointGroup = -1;
            private int _edgeSingle = -1;
            private int _edgeDouble1 = -1;
            private int _edgeDouble2 = -1;

            public AddDeletePointCommandBase(FigureContext context, int x, int y, Edge edge)
            {
                _context = context;
                _x = x;
                _y = y;
                _edgeSingle = _context.Import(edge);
            }

            public AddDeletePointCommandBase(FigureContext context, PointGroup pointGroup)
            {
                _context = context;
                _x = pointGroup.Points[0].X;
                _y = pointGroup.Points[0].Y;
                _pointGroup = _context.Import(pointGroup);
                _edgeDouble1 = _context.Import(pointGroup.Points[0].IncomingEdge);
                _edgeDouble2 = _context.Import(pointGroup.Points[0].OutgoingEdge);
            }

            public void Add()
            {
                Edge edge = _context.Get<Edge>(_edgeSingle);

                var p = _context.Figure.AddDiscardablePoint(new Point(_x, _y), edge);

                _context.Remove<Edge>(_edgeSingle);
                _pointGroup = _context.Add(_pointGroup, p.Group);
                _edgeDouble1 = _context.Add(_edgeDouble1, p.IncomingEdge);
                _edgeDouble2 = _context.Add(_edgeDouble2, p.OutgoingEdge);

                _context.OnModifiedShapes();
            }

            public void Delete()
            {
                PointGroup pg = _context.Get<PointGroup>(_pointGroup);

                var edge = _context.Figure.DeleteDiscardablePoint(pg);

                _context.Remove<PointGroup>(_pointGroup);
                _context.Remove<Edge>(_edgeDouble1);
                _context.Remove<Edge>(_edgeDouble2);
                _edgeSingle = _context.Add(_edgeSingle, edge);

                _context.OnModifiedShapes();
            }
        }


        private class AddPointCommand : AddDeletePointCommandBase
        {
            public AddPointCommand(FigureContext context, int x, int y, Edge edge) :
                base(context, x, y, edge)
            {
            }

            public override void Apply()
            {
                base.Add();
            }

            public override void Unapply()
            {
                base.Delete();
            }
        }


        private class DeletePointCommand : AddDeletePointCommandBase
        {
            public DeletePointCommand(FigureContext context, PointGroup pointGroup) :
                base(context, pointGroup)
            {
            }

            public override void Apply()
            {
                base.Delete();
            }

            public override void Unapply()
            {
                base.Add();
            }
        }


        private class EnableShapeCommand : HistoryCommand
        {
            private readonly FigureContext _context;
            private int _shapeIndex;
            private bool _enable;

            public EnableShapeCommand(FigureContext figureContext, int shapeIndex, bool enable)
            {
                _context = figureContext;
                _shapeIndex = shapeIndex;
                _enable = enable;
            }

            public override void Apply()
            {
                _context.Figure.Shapes[_shapeIndex].Enabled = _enable;
                _context.OnEnabledChanged();
            }

            public override void Unapply()
            {
                _context.Figure.Shapes[_shapeIndex].Enabled = !_enable;
                _context.OnEnabledChanged();
            }
        }

        #endregion
    }
}
