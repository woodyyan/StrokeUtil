using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Grid_StylusDown(object sender, StylusDownEventArgs e)
        {
            Point currentPosition = e.GetPosition(Grid1);
            DrawStart(currentPosition);
        }

        private void Grid_StylusMove(object sender, StylusEventArgs e)
        {
            Point currentPosition = e.GetPosition(Grid1);
            DrawProcessing(currentPosition);
        }

        private void Grid_StylusUp(object sender, StylusEventArgs e)
        {
            DrawComplete();
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
            Clear();
        }

        private void Grid1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isMouseDown = true;
            Point currentPosition = e.GetPosition(Grid1);
            DrawStart(currentPosition);
        }

        private void Grid1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                Point currentPosition = e.GetPosition(Grid1);
                DrawProcessing(currentPosition);
            }
        }

        private void Grid1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DrawComplete();
            isMouseDown = false;
        }
    }
}
