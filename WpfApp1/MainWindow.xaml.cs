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
        public static int Size = 10001;//10001;
        public static int Offset = 100; //100
        public static int NumOfPoints = 20000; //40000
        public static int RandPoints = 20;

        public static List<(int, int)> Data;

        public List<Color> Clr = new List<Color>()
        {
            Colors.Red,
            Colors.LightGreen,
            Colors.LightBlue,
            Colors.Cyan,
            Colors.Orange,
            Colors.Yellow,
            Colors.DarkViolet,
            Colors.Black,
            Colors.DarkBlue,
            Colors.HotPink,
            Colors.Brown,
            Colors.DarkGreen,
            Colors.LavenderBlush
        };
        public MainWindow()
        {
            InitializeComponent();
            CreateData();
        }

        public static void CreateData()
        {
            int[,] map = new int[Size + 1, Size + 1];
            List<(int, int)> data = new List<(int, int)>(NumOfPoints + RandPoints);
            List<(int, int)> initPoints = new List<(int, int)>(RandPoints);
            Random r = new Random();
            int x, y;
            for (int i = 0; i < RandPoints; i++)
            {
                x = r.Next(Size-Offset + 1);
                y = r.Next(Size-Offset + 1);
                while (map[y, x] == 1)
                {
                    x = r.Next(Size-Offset + 1);
                    y = r.Next(Size-Offset + 1);
                }
                map[y, x] = 1;
                (int, int) point;
                point.Item1 = y;
                point.Item2 = x;
                initPoints.Add(point);
                data.Add(point);
            }
            int X_offset, Y_offset, rndInd;
            for (int i = 0; i < NumOfPoints; i++)
            {
                rndInd = r.Next(RandPoints);
                y = initPoints[rndInd].Item1;
                x = initPoints[rndInd].Item2;
                X_offset = r.Next(-Offset, Offset);
                Y_offset = r.Next(-Offset, Offset);
                while (y + Y_offset < 0 || y + Y_offset >= Size || x + X_offset < 0 || x + X_offset >= Size)
                {
                    X_offset = r.Next(-Offset, Offset);
                    Y_offset = r.Next(-Offset, Offset);
                }
                (int, int) point;
                point.Item1 = y + Y_offset;
                point.Item2 = x + X_offset;
                data.Add(point);
            }
            Data = data;
        }

        private void kCentroid(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
            int k = Int32.Parse(kcentroid.Text);
            var data = K_Centroid.Start(k, Data, Size);
            int index = 0;
            List<(int, int)> points = data.Item1;
            List<(int, int)> centers = data.Item3;
            int[] clusters = data.Item2;
            for (int i = 0; i < clusters.Length; i++)
            {
                index = clusters[i];
                if (index >= Clr.Count) index %= Clr.Count;
                Ellipse point = new Ellipse();
                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                mySolidColorBrush.Color = Clr[index];
                point.Fill = mySolidColorBrush;
                point.StrokeThickness = 0;
                point.Width = 5;
                point.Height = 5;
                Canvas.SetTop(point, Size - points[i].Item1);
                Canvas.SetLeft(point, points[i].Item2);
                canvas.Children.Add(point);
            }
            index = 0;
            foreach(var center in centers)
            {
                if (index >= Clr.Count) index %= Clr.Count;
                Ellipse point = new Ellipse();
                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                mySolidColorBrush.Color = Clr[index];
                point.Fill = mySolidColorBrush;
                point.Stroke = new SolidColorBrush(Colors.DarkGray);
                point.StrokeThickness = 2;
                point.Width = 16;
                point.Height = 16;
                Canvas.SetTop(point, Size - center.Item1 - point.Height / 2);
                Canvas.SetLeft(point, center.Item2 - point.Height/2);
                canvas.Children.Add(point);
                index++;
            }
            CreateSaveBitmap(canvas, @"C:\Temp\KCentroid.png");
            //kMedoid(null, null);
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
            //int k = 5;
            var data = K_Medoid.Start(k, Data);
            int index = 0;
            List<(int, int)> points = data.Item1;
            List<(int, int)> centers = data.Item3;
            int[] clusters = data.Item2;
            for (int i = 0; i < clusters.Length; i++)
            {
                index = clusters[i];
                if (index >= Clr.Count) index %= Clr.Count;
                Ellipse point = new Ellipse();
                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                mySolidColorBrush.Color = Clr[index];
                point.Fill = mySolidColorBrush;
                point.StrokeThickness = 0;
                point.Width = 5;
                point.Height = 5;
                Canvas.SetTop(point, Size - points[i].Item1);
                Canvas.SetLeft(point, points[i].Item2);
                canvas.Children.Add(point);
            }
            index = 0;
            foreach (var center in centers)
            {
                if (index >= Clr.Count) index %= Clr.Count;
                Ellipse point = new Ellipse();
                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                mySolidColorBrush.Color = Clr[index];
                point.Fill = mySolidColorBrush;
                point.Stroke = new SolidColorBrush(Colors.DarkGray);
                point.StrokeThickness = 2;
                point.Width = 16;
                point.Height = 16;
                Canvas.SetTop(point, Size - center.Item1);
                Canvas.SetLeft(point, center.Item2 - point.Height/2);
                canvas.Children.Add(point);
                index++;
            }
            CreateSaveBitmap(canvas, @"C:\Temp\KMedoid.png");
        }

        private void Aggl(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
            Agglomerative.Start(Data);
            var Clusters = Agglomerative.Clusters;
            Console.WriteLine(Clusters.Count());
            int index = 0;
            foreach(var cluster in Clusters)
            {
                if (index >= Clr.Count) index %= Clr.Count;
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
                    Canvas.SetTop(point, Size - pnt.Item1);
                    Canvas.SetLeft(point, pnt.Item2);
                    canvas.Children.Add(point);
                }
            }
            CreateSaveBitmap(canvas, @"C:\Temp\Agglomerative.png");
            //Divise(null, null);
        }

        private void Divise(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
            Divisive.Start(Data);
            var Clusters = Divisive.Clusters;
            Console.WriteLine(Clusters.Count());
            int colorInd = 0;
            foreach (var cluster in Clusters)
            {
                if (colorInd >= Clr.Count) colorInd %= Clr.Count;
                var color = Clr[colorInd++];
                Console.WriteLine(colorInd - 1 + " " + cluster.Count());
                foreach (var pnt in cluster)
                {
                    Ellipse point = new Ellipse();
                    SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                    mySolidColorBrush.Color = color;
                    point.Fill = mySolidColorBrush;
                    point.StrokeThickness = 0;
                    point.Width = 5;
                    point.Height = 5;
                    Canvas.SetTop(point, Size - pnt.Item1);
                    Canvas.SetLeft(point, pnt.Item2);
                    canvas.Children.Add(point);
                }
            }
            CreateSaveBitmap(canvas, @"C:\Temp\Divisive.png");
        }
    }
}
