using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Collection<Collection<Point>> strokeHistory = new Collection<Collection<Point>>();
        private Collection<Point> currentStroke;
        private Path currentPath;
        private Point? previousPosition;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Grid_StylusDown(object sender, StylusDownEventArgs e)
        {
            currentStroke = new Collection<Point>();
            strokeHistory.Add(currentStroke);

            Point currentPosition = e.GetPosition(Grid1);
            currentStroke.Add(currentPosition);
            previousPosition = currentPosition;

            if (currentPath == null)
            {
                currentPath = new Path();
                currentPath.Stroke = new SolidColorBrush(Colors.Black);
                currentPath.StrokeThickness = 2;
                currentPath.Data = GetGeometry();
                Grid1.Children.Add(currentPath);
            }
        }

        private void Grid_StylusMove(object sender, StylusEventArgs e)
        {
            if (currentStroke != null)
            {
                Point currentPosition = e.GetPosition(Grid1);

                if (Diff(currentPosition))
                {
                    currentStroke.Add(currentPosition);
                    currentPath.Data = GetGeometry();
                    previousPosition = currentPosition;
                }
            }
        }

        private bool Diff(Point currentPosition)
        {
            if (previousPosition == null) return true;
            else return (Math.Abs(currentPosition.X - previousPosition.Value.X) > 4 || Math.Abs(currentPosition.Y - previousPosition.Value.Y) > 4);
        }

        private void Grid_StylusUp(object sender, StylusEventArgs e)
        {
            previousPosition = null;
        }

        private PathGeometry GetGeometry()
        {
            PathGeometry path = new PathGeometry();
            foreach (var line in strokeHistory)
            {
                PathFigure pathFigure = new PathFigure();
                path.Figures.Add(pathFigure);

                for (int i = 0; i < line.Count; i++)
                {
                    if (i == 0)
                    {
                        pathFigure.StartPoint = line[i];
                    }
                    else
                    {
                        pathFigure.Segments.Add(new LineSegment(line[i], true));
                    }
                }

            }

            return path;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
