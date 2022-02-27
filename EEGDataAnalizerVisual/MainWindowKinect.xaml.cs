using EEGDataAnalizer;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Legends;

namespace EEGDataAnalizerVisual
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindowKinect : Window
    {
        KinectPlotData[] Data;
        public MainWindowKinect(string FilePath)
        {
            ((App)Application.Current).CloseOpenWindow(true);
            Data = null;
            Data = DataModel.DeserializeXMLFileKPD(FilePath);

            this.DataContext = new MainViewModelKinect(Data);
            //this.DataContext = new MainViewModelKinectPost(Data);
            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e)
        {
            ((App)Application.Current).CloseOpenWindow();
            base.OnClosed(e);
        }
    }

    public class MainViewModelKinect
    {
        public MainViewModelKinect(KinectPlotData[] Data)
        {
            this.MyModel = new PlotModel { Title = "Kinect data pre" };
            //this.MyModel.PlotMargins = new OxyThickness(2,150,2,5);
            /*this.MyModel.Legends.Add(new Legend()
            {
                LegendTitle = "Legend",
                LegendMaxHeight = 230,
                LegendMargin = 50,
                LegendPosition = LegendPosition.BottomCenter
            });*/
            this.MyModel.IsLegendVisible = true;
            LineSeries RawS1X = new LineSeries();
            RawS1X.Color = OxyColor.FromRgb(255, 0, 0);
            RawS1X.Title = "Sample 1 RAW X";
            RawS1X.Points.AddRange(Data.Select(d => new DataPoint(d.TimeS, d.RawS1.x)));
            this.MyModel.Series.Add(RawS1X);
            LineSeries RawS1Y = new LineSeries();
            RawS1Y.Color = OxyColor.FromRgb(0, 255, 0);
            RawS1Y.Title = "Sample 1 RAW Y";
            RawS1Y.Points.AddRange(Data.Select(d => new DataPoint(d.TimeS, d.RawS1.y)));
            this.MyModel.Series.Add(RawS1Y);
            LineSeries RawS1Z = new LineSeries();
            RawS1Z.Color = OxyColor.FromRgb(0, 0, 255);
            RawS1Z.Title = "Sample 1 RAW Z";
            RawS1Z.Points.AddRange(Data.Select(d => new DataPoint(d.TimeS, d.RawS1.z)));
            this.MyModel.Series.Add(RawS1Z);
            LineSeries RawS2X = new LineSeries();
            RawS2X.Color = OxyColor.FromRgb(255, 150, 0);
            RawS2X.Title = "Sample 2 RAW X";
            RawS2X.Points.AddRange(Data.Select(d => new DataPoint(d.TimeS, d.RawS2.x)));
            this.MyModel.Series.Add(RawS2X);
            LineSeries RawS2Y = new LineSeries();
            RawS2Y.Color = OxyColor.FromRgb(0, 255, 150);
            RawS2Y.Title = "Sample 2 RAW Y";
            RawS2Y.Points.AddRange(Data.Select(d => new DataPoint(d.TimeS, d.RawS2.y)));
            this.MyModel.Series.Add(RawS2Y);
            LineSeries RawS2Z = new LineSeries();
            RawS2Z.Color = OxyColor.FromRgb(0, 150, 255);
            RawS2Z.Title = "Sample 2 RAW Z";
            RawS2Z.Points.AddRange(Data.Select(d => new DataPoint(d.TimeS, d.RawS2.z)));
            this.MyModel.Series.Add(RawS2Z);
            LineSeries RawS3X = new LineSeries();
            RawS3X.Color = OxyColor.FromRgb(255, 150, 150);
            RawS3X.Title = "Sample 3 RAW X";
            RawS3X.Points.AddRange(Data.Select(d => new DataPoint(d.TimeS, d.RawS3.x)));
            this.MyModel.Series.Add(RawS3X);
            LineSeries RawS3Y = new LineSeries();
            RawS3Y.Color = OxyColor.FromRgb(150, 255, 150);
            RawS3Y.Title = "Sample 3 RAW Y";
            RawS3Y.Points.AddRange(Data.Select(d => new DataPoint(d.TimeS, d.RawS3.y)));
            this.MyModel.Series.Add(RawS3Y);
            LineSeries RawS3Z = new LineSeries();
            RawS3Z.Color = OxyColor.FromRgb(150, 150, 255);
            RawS3Z.Title = "Sample 3 RAW Z";
            RawS3Z.Points.AddRange(Data.Select(d => new DataPoint(d.TimeS, d.RawS3.z)));
            this.MyModel.Series.Add(RawS3Z);
        }

        public PlotModel MyModel { get; private set; }

    }

    public class MainViewModelKinectPost
    {
        public MainViewModelKinectPost(KinectPlotData[] Data)
        {
            this.MyModel = new PlotModel { Title = "Kinect data post" };
            //this.MyModel.PlotMargins = new OxyThickness(2,150,2,5);
            /*this.MyModel.Legends.Add(new Legend()
            {
                LegendTitle = "Legend",
                LegendMaxHeight = 230,
                LegendMargin = 50,
                LegendPosition = LegendPosition.BottomCenter
            });*/
            this.MyModel.IsLegendVisible = true;
            LineSeries GameS1X = new LineSeries();
            GameS1X.Color = OxyColor.FromRgb(255, 0, 0);
            GameS1X.Title = "Sample 1  X";
            GameS1X.Points.AddRange(Data.Select(d => new DataPoint(d.TimeS, d.GameS1.x)));
            this.MyModel.Series.Add(GameS1X);
            LineSeries GameS1Y = new LineSeries();
            GameS1Y.Color = OxyColor.FromRgb(0, 255, 0);
            GameS1Y.Title = "Sample 1  Y";
            GameS1Y.Points.AddRange(Data.Select(d => new DataPoint(d.TimeS, d.GameS1.y)));
            this.MyModel.Series.Add(GameS1Y);
            LineSeries GameS1Z = new LineSeries();
            GameS1Z.Color = OxyColor.FromRgb(0, 0, 255);
            GameS1Z.Title = "Sample 1  Z";
            GameS1Z.Points.AddRange(Data.Select(d => new DataPoint(d.TimeS, d.GameS1.z)));
            this.MyModel.Series.Add(GameS1Z);
            LineSeries GameS2X = new LineSeries();
            GameS2X.Color = OxyColor.FromRgb(255, 150, 0);
            GameS2X.Title = "Sample 2  X";
            GameS2X.Points.AddRange(Data.Select(d => new DataPoint(d.TimeS, d.GameS2.x)));
            this.MyModel.Series.Add(GameS2X);
            LineSeries GameS2Y = new LineSeries();
            GameS2Y.Color = OxyColor.FromRgb(0, 255, 150);
            GameS2Y.Title = "Sample 2  Y";
            GameS2Y.Points.AddRange(Data.Select(d => new DataPoint(d.TimeS, d.GameS2.y)));
            this.MyModel.Series.Add(GameS2Y);
            LineSeries GameS2Z = new LineSeries();
            GameS2Z.Color = OxyColor.FromRgb(0, 150, 255);
            GameS2Z.Title = "Sample 2  Z";
            GameS2Z.Points.AddRange(Data.Select(d => new DataPoint(d.TimeS, d.GameS2.z)));
            this.MyModel.Series.Add(GameS2Z);
            LineSeries GameS3X = new LineSeries();
            GameS3X.Color = OxyColor.FromRgb(255, 150, 150);
            GameS3X.Title = "Sample 3  X";
            GameS3X.Points.AddRange(Data.Select(d => new DataPoint(d.TimeS, d.GameS3.x)));
            this.MyModel.Series.Add(GameS3X);
            LineSeries GameS3Y = new LineSeries();
            GameS3Y.Color = OxyColor.FromRgb(150, 255, 150);
            GameS3Y.Title = "Sample 3  Y";
            GameS3Y.Points.AddRange(Data.Select(d => new DataPoint(d.TimeS, d.GameS3.y)));
            this.MyModel.Series.Add(GameS3Y);
            LineSeries GameS3Z = new LineSeries();
            GameS3Z.Color = OxyColor.FromRgb(150, 150, 255);
            GameS3Z.Title = "Sample 3  Z";
            GameS3Z.Points.AddRange(Data.Select(d => new DataPoint(d.TimeS, d.GameS3.z)));
            this.MyModel.Series.Add(GameS3Z);
        }

        public PlotModel MyModel { get; private set; }

    }
}
