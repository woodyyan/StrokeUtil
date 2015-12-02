using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const double gap = 3;
        private PathGeometry currentPathGeometry;
        private Point? previousPosition;
        private DispatcherTimer timer;
        private Path currentPath;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Grid_StylusDown(object sender, StylusDownEventArgs e)
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

            Point currentPosition = e.GetPosition(Grid1);
            AddNewPosition(e.GetPosition(Grid1), DrawType.Start);
        }

        private void Grid_StylusMove(object sender, StylusEventArgs e)
        {
            var currentFigure = currentPathGeometry.Figures.LastOrDefault();
            AddNewPosition(e.GetPosition(Grid1), DrawType.Segment);
        }

        private bool Diff(Point currentPosition)
        {
            if (previousPosition == null) return true;
            else return (Math.Abs(currentPosition.X - previousPosition.Value.X) >= gap || Math.Abs(currentPosition.Y - previousPosition.Value.Y) >= gap);
        }

        private void Grid_StylusUp(object sender, StylusEventArgs e)
        {
            previousPosition = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PathGeometry oldPathGeometry = currentPathGeometry;
            currentPath.Data = currentPathGeometry = new PathGeometry();

            if (timer == null)
            {
                int index = 0;

                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(8);
                timer.Tick += delegate
                {
                    var nextPoint = GetNextPoint(oldPathGeometry, index);
                    if (nextPoint != null)
                    {
                        AddNewPosition(nextPoint.Item1, nextPoint.Item2);
                    }
                    else
                    {
                        timer.Stop();
                        timer = null;
                    }
                    index++;
                };

                timer.Start();
            }
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
                    if (Diff(newPosition))
                    {
                        currentFigure.Segments.Add(new LineSegment(newPosition, true));
                        previousPosition = newPosition;
                    }
                }
            }
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

        public enum DrawType
        {
            Start = 0,
            Segment = 1
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = ".txt";
            if (dialog.ShowDialog().Value)
            {
                string toString = currentPathGeometry.ToString();
                System.IO.File.WriteAllText(dialog.FileName, toString);
            }
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            currentPath.Data = currentPathGeometry = new PathGeometry();
        }
    }
}
