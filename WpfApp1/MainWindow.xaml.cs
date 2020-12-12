using System;
using System.Collections.Generic;
using System.IO;
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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public List<Color> Clr = new List<Color>()
        {
            Colors.Red,
            Colors.Green,
            Colors.LightBlue,
            Colors.Cyan,
            Colors.Orange,
            Colors.Yellow,
            Colors.DarkViolet,
            Colors.Black,
            Colors.DarkBlue,
            Colors.HotPink,
            Colors.Brown,
        };
        public MainWindow()
        {
            InitializeComponent();
        }

        private void kCentroid(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
            int k = Int32.Parse(kcentroid.Text);
            var data = K_Centroid.Start(k);
            int index = 0;
            List<(int, int)> points = data.Item1;
            List<(int, int)> centers = data.Item3;
            int[] clusters = data.Item2;
            for (int i = 0; i < clusters.Length; i++)
            {
                index = clusters[i];
                if (index >= 11) index %= 11;
                Ellipse point = new Ellipse();
                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                mySolidColorBrush.Color = Clr[index];
                point.Fill = mySolidColorBrush;
                point.StrokeThickness = 0;
                point.Width = 5;
                point.Height = 5;
                Canvas.SetTop(point, 10000 - points[i].Item1);
                Canvas.SetLeft(point, points[i].Item2);
                canvas.Children.Add(point);
            }
            index = 0;
            foreach(var center in centers)
            {
                if (index >= 11) index %= 11;
                Ellipse point = new Ellipse();
                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                mySolidColorBrush.Color = Clr[index];
                point.Fill = mySolidColorBrush;
                point.Stroke = new SolidColorBrush(Colors.DarkGray);
                point.StrokeThickness = 2;
                point.Width = 50;
                point.Height = 50;
                Canvas.SetTop(point, 10000 - center.Item1 - 25);
                Canvas.SetLeft(point, center.Item2 - 25);
                canvas.Children.Add(point);
                index++;
            }
            CreateSaveBitmap(canvas, @"C:\temp\Centroid.bmp");
        }

        private void CreateSaveBitmap(Canvas canvas, string filename)
        {
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
             (int)canvas.Width, (int)canvas.Height,
             96d, 96d, PixelFormats.Pbgra32);
            // needed otherwise the image output is black
            canvas.Measure(new Size((int)canvas.Width, (int)canvas.Height));
            canvas.Arrange(new Rect(new Size((int)canvas.Width, (int)canvas.Height)));

            renderBitmap.Render(canvas);
           

            //JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            //BmpBitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

            using (FileStream file = File.Create(filename))
            {
                encoder.Save(file);
            }
        }

        private void kMedoid(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
            int k = Int32.Parse(kmedoid.Text);
            var data = K_Medoid.Start(k);
            int index = 0;
            List<(int, int)> points = data.Item1;
            List<(int, int)> centers = data.Item3;
            int[] clusters = data.Item2;
            for (int i = 0; i < clusters.Length; i++)
            {
                index = clusters[i];
                if (index >= 11) index %= 11;
                Ellipse point = new Ellipse();
                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                mySolidColorBrush.Color = Clr[index];
                point.Fill = mySolidColorBrush;
                point.StrokeThickness = 0;
                point.Width = 5;
                point.Height = 5;
                Canvas.SetTop(point, 5000 - points[i].Item1);
                Canvas.SetLeft(point, points[i].Item2);
                canvas.Children.Add(point);
            }
            index = 0;
            foreach (var center in centers)
            {
                if (index >= 11) index %= 11;
                Ellipse point = new Ellipse();
                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                mySolidColorBrush.Color = Clr[index];
                point.Fill = mySolidColorBrush;
                point.Stroke = new SolidColorBrush(Colors.DarkGray);
                point.StrokeThickness = 2;
                point.Width = 10;
                point.Height = 10;
                Canvas.SetTop(point, 5000 - center.Item1);
                Canvas.SetLeft(point, center.Item2);
                canvas.Children.Add(point);
                index++;
            }
            CreateSaveBitmap(canvas, @"C:\temp\Medoid.png");
        }

        private void Aggl(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
            var Clusters = Agglomerative.Start();
            Console.WriteLine(Clusters.Count());
            int index = 0;
            foreach(var cluster in Clusters)
            {
                if (index >= 11) index %= 11;
                var color = Clr[index++];
                Console.WriteLine(index - 1 + " " + cluster.Count());
                foreach (var pnt in cluster)
                {
                    Ellipse point = new Ellipse();
                    SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                    mySolidColorBrush.Color = color;
                    point.Fill = mySolidColorBrush;
                    point.StrokeThickness = 0;
                    point.Width = 5;
                    point.Height = 5;
                    Canvas.SetTop(point, 5000 - pnt.Item1);
                    Canvas.SetLeft(point, pnt.Item2);
                    canvas.Children.Add(point);
                }
            }
            CreateSaveBitmap(canvas, @"C:\temp\Aggl.png");
        }

        private void Divise(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
            var Clusters = Divisive.Start();
            Console.WriteLine(Clusters.Count());
            int index = 0;
            foreach (var cluster in Clusters)
            {
                if (index >= 11) index %= 11;
                var color = Clr[index++];
                Console.WriteLine(index - 1 + " " + cluster.Count());
                foreach (var pnt in cluster)
                {
                    Ellipse point = new Ellipse();
                    SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                    mySolidColorBrush.Color = color;
                    point.Fill = mySolidColorBrush;
                    point.StrokeThickness = 0;
                    point.Width = 5;
                    point.Height = 5;
                    Canvas.SetTop(point, 500 - pnt.Item1);
                    Canvas.SetLeft(point, pnt.Item2);
                    canvas.Children.Add(point);
                }
            }
            CreateSaveBitmap(canvas, @"C:\temp\Div.png");
        }
    }
}
