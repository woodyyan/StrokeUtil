using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApplication1
{
    public partial class MainWindow
    {
        private const double gap = 3;
        private PathGeometry currentPathGeometry;
        private Point? previousPosition;
        private Path currentPath;
        private bool isMouseDown;

        private void DrawStart(Point currentPosition)
        {
            if (currentPathGeometry == null)
            {
                currentPath = new Path();
                currentPath.Stroke = new SolidColorBrush(Colors.Black);
                currentPath.StrokeThickness = 4;
                currentPath.StrokeEndLineCap = PenLineCap.Round;
                currentPath.StrokeLineJoin = PenLineJoin.Round;
                currentPath.StrokeMiterLimit = 2;
                currentPath.Data = currentPathGeometry = new PathGeometry();
                Grid1.Children.Add(currentPath);
            }

            AddNewPosition(currentPosition, DrawType.Start);
        }

        private void DrawProcessing(Point currentPosition)
        {
            var currentFigure = currentPathGeometry.Figures.LastOrDefault();
            AddNewPosition(currentPosition, DrawType.Segment);
        }

        private void DrawComplete()
        {
            previousPosition = null;
        }

        private void AddNewPosition(Point newPosition, DrawType drawType)
        {
            if (drawType == DrawType.Start)
            {
                currentPathGeometry.Figures.Add(new PathFigure { StartPoint = newPosition });
                previousPosition = newPosition;
            }
            else
            {
                var currentFigure = currentPathGeometry.Figures.LastOrDefault();
                if (currentFigure != null)
                {
                    if (IsValidPosition(newPosition))
                    {
                        currentFigure.Segments.Add(new LineSegment(newPosition, true));
                        previousPosition = newPosition;
                    }
                }
            }
        }

        private bool IsValidPosition(Point currentPosition)
        {
            if (previousPosition == null) return true;
            else return (Math.Abs(currentPosition.X - previousPosition.Value.X) >= gap || Math.Abs(currentPosition.Y - previousPosition.Value.Y) >= gap);
        }

        private Tuple<Point, DrawType> GetNextPoint(PathGeometry sourceGeometry, int index)
        {
            int currentIndex = 0;
            for (int i = 0; i < sourceGeometry.Figures.Count; i++)
            {
                PathFigure figure = sourceGeometry.Figures[i];
                Point p = figure.StartPoint;
                if (currentIndex == index)
                {
                    return new Tuple<Point, DrawType>(p, DrawType.Start);
                }
                currentIndex++;

                for (int j = 0; j < figure.Segments.Count; j++)
                {
                    LineSegment lineSegment = (LineSegment)figure.Segments[j];
                    p = lineSegment.Point;
                    if (currentIndex == index)
                    {
                        return new Tuple<Point, DrawType>(p, DrawType.Segment);
                    }
                    currentIndex++;
                }
            }

            return null;
        }

        private void Clear()
        {
            currentPath.Data = currentPathGeometry = new PathGeometry();
        }

        public enum DrawType
        {
            Start = 0,
            Segment = 1
        }
    }
}
