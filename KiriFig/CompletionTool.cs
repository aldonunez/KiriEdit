/*
   Copyright 2020 Aldo J. Nunez

   Licensed under the Apache License, Version 2.0.
   See the LICENSE.txt file for details.
*/

using KiriFig.Model;
using System;
using System.Collections.Generic;

namespace KiriFig
{
    public class CompletionTool
    {
        private struct Interval
        {
            public int Begin;
            public int End;

            public Interval(int begin, int end)
            {
                Begin = begin;
                End = end;
            }
        }

        private Figure _referenceFigure;
        private int _refEdgeCount;
        private List<Interval>[] _edgeIntervals;

        public CompletionTool(Figure referenceFigure)
        {
            _referenceFigure = referenceFigure;

            int maxLabel = -1;

            foreach (var shape in referenceFigure.Shapes)
            {
                var (count, max) = CountEdges(shape.Contours[0]);

                _refEdgeCount += count;

                if (maxLabel < max)
                    maxLabel = max;
            }

            _edgeIntervals = new List<Interval>[maxLabel + 1];
        }

        public void AddComponentFigure(Figure figure)
        {
            foreach (var shape in figure.Shapes)
            {
                if (!shape.Enabled)
                    continue;

                Contour contour = shape.Contours[0];
                Point p = contour.FirstPoint;

                while (true)
                {
                    int label = p.OutgoingEdge.Label;

                    if (label >= 0)
                    {
                        if (_edgeIntervals[label] == null)
                            _edgeIntervals[label] = new List<Interval>();

                        PointGroup pg1 = p.Group;
                        PointGroup pg2 = p.OutgoingEdge.P2.Group;
                        int begin, end;

                        if (pg1.IsFixed)
                            begin = 0;
                        else
                            begin = pg1.NormalT;

                        if (pg2.IsFixed)
                            end = PointGroup.MaxNormalT;
                        else
                            end = pg2.NormalT;

                        _edgeIntervals[label].Add(new Interval(begin, end));
                    }

                    p = p.OutgoingEdge.P2;

                    if (p == contour.FirstPoint)
                        break;
                }
            }
        }

        public bool CalculateComplete()
        {
            int completedEdges = 0;

            foreach (var intervalList in _edgeIntervals)
            {
                if (intervalList == null || intervalList.Count == 0)
                    continue;

                Interval fullInterval = new Interval();
                int countBefore;

                do
                {
                    countBefore = intervalList.Count;

                    for (int i = intervalList.Count - 1; i >= 0; i--)
                    {
                        var interval = intervalList[i];

                        if (AboutLessOrEqual(interval.Begin, fullInterval.End)
                            && AboutGreaterOrEqual(interval.End, fullInterval.Begin))
                        {
                            fullInterval.Begin = Math.Min(interval.Begin, fullInterval.Begin);
                            fullInterval.End = Math.Max(interval.End, fullInterval.End);

                            intervalList.RemoveAt(i);
                        }
                    }
                }
                while (intervalList.Count > 0 && intervalList.Count < countBefore);

                if (fullInterval.Begin == 0 && fullInterval.End == PointGroup.MaxNormalT)
                    completedEdges++;
            }

            return completedEdges == _refEdgeCount;
        }

        private static (int count, int max) CountEdges(Contour contour)
        {
            int count = 0;
            int max = -1;
            Point p = contour.FirstPoint;

            while (true)
            {
                count++;

                if (max < p.OutgoingEdge.Label)
                    max = p.OutgoingEdge.Label;

                p = p.OutgoingEdge.P2;

                if (p == contour.FirstPoint)
                    break;
            }

            return (count, max);
        }

        private static bool AboutGreaterOrEqual(int a, int b)
        {
            return a > b || AboutEqual(a, b);
        }

        private static bool AboutLessOrEqual(int a, int b)
        {
            return a < b || AboutEqual(a, b);
        }

        private static bool AboutEqual(int a, int b)
        {
            return Math.Abs(a - b) < PointGroup.Epsilon;
        }
    }
}
