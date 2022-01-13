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

namespace EEGDataAnalizerVisual
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int i = 0;
        MeasuredDataConteiner[] Data;
        public MainWindow()
        {
            Data = null;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "EEG Data files(*.eegm)|*.eegm";
            if (openFileDialog.ShowDialog() == true)
            {
                Data = DataModel.DeserializeXMLFile(openFileDialog.FileName);
            }

            this.DataContext = new MainViewModel(Data[i]);
            InitializeComponent();
        }

        private void Prev(object sender, RoutedEventArgs e)
        {
            if (i > 0)
                i--;
            this.DataContext = new MainViewModel(Data[i]);
        }

        private void Next(object sender, RoutedEventArgs e)
        {
            if (i < Data.Length-1)
                i++;
            this.DataContext = new MainViewModel(Data[i]);
        }
    }

    public class MainViewModel
    {
        public MainViewModel(MeasuredDataConteiner Data)
        {
            this.MyModel = new PlotModel { Title = "EEGData: " + Data.Name };
            this.MyModel.IsLegendVisible = true;
            LineSeries alpha1 = new LineSeries();
            alpha1.Color = OxyColor.FromRgb(235, 52, 52);
            alpha1.Title = "Alpha1";
            alpha1.Points.AddRange(Data.Data.Select(d => new DataPoint(TimeSpan.FromTicks(d.Time.Ticks).TotalMilliseconds - TimeSpan.FromTicks(Data.Data[0].Time.Ticks).TotalMilliseconds, d.Alpha1)));
            this.MyModel.Series.Add(alpha1);
            LineSeries alpha2 = new LineSeries();
            alpha2.Color = OxyColor.FromRgb(252, 101, 101);
            alpha2.Title = "Alpha2";
            alpha2.Points.AddRange(Data.Data.Select(d => new DataPoint(TimeSpan.FromTicks(d.Time.Ticks).TotalMilliseconds - TimeSpan.FromTicks(Data.Data[0].Time.Ticks).TotalMilliseconds, d.Alpha2)));
            this.MyModel.Series.Add(alpha2);
            LineSeries beta1 = new LineSeries();
            beta1.Color = OxyColor.FromRgb(3, 148, 252);
            beta1.Title = "Beta1";
            beta1.Points.AddRange(Data.Data.Select(d => new DataPoint(TimeSpan.FromTicks(d.Time.Ticks).TotalMilliseconds - TimeSpan.FromTicks(Data.Data[0].Time.Ticks).TotalMilliseconds, d.Beta1)));
            this.MyModel.Series.Add(beta1);
            LineSeries beta2 = new LineSeries();
            beta2.Color = OxyColor.FromRgb(97, 189, 255);
            beta2.Title = "Beta2";
            beta2.Points.AddRange(Data.Data.Select(d => new DataPoint(TimeSpan.FromTicks(d.Time.Ticks).TotalMilliseconds - TimeSpan.FromTicks(Data.Data[0].Time.Ticks).TotalMilliseconds, d.Beta2)));
            this.MyModel.Series.Add(beta2);                                                
            LineSeries Gamma1 = new LineSeries();
            Gamma1.Color = OxyColor.FromRgb(217, 0, 255);
            Gamma1.Title = "Gamma1";                                                       
            Gamma1.Points.AddRange(Data.Data.Select(d => new DataPoint(TimeSpan.FromTicks(d.Time.Ticks).TotalMilliseconds - TimeSpan.FromTicks(Data.Data[0].Time.Ticks).TotalMilliseconds, d.Gamma1)));
            this.MyModel.Series.Add(Gamma1);                                              
            LineSeries Gamma2 = new LineSeries();
            Gamma2.Color = OxyColor.FromRgb(231, 97, 255);
            Gamma2.Title = "Gamma2";                                                      
            Gamma2.Points.AddRange(Data.Data.Select(d => new DataPoint(TimeSpan.FromTicks(d.Time.Ticks).TotalMilliseconds - TimeSpan.FromTicks(Data.Data[0].Time.Ticks).TotalMilliseconds, d.Gamma2)));
            this.MyModel.Series.Add(Gamma2);                                               
            LineSeries Delta = new LineSeries();
            Delta.Color = OxyColor.FromRgb(255, 217, 0);
            Delta.Title = "Delta";                                                         
            Delta.Points.AddRange(Data.Data.Select(d => new DataPoint(TimeSpan.FromTicks(d.Time.Ticks).TotalMilliseconds - TimeSpan.FromTicks(Data.Data[0].Time.Ticks).TotalMilliseconds, d.Delta)));
            this.MyModel.Series.Add(Delta);                                                
            LineSeries Theta = new LineSeries();
            Theta.Color = OxyColor.FromRgb(21, 255, 0);
            Theta.Title = "Theta";                                                         
            Theta.Points.AddRange(Data.Data.Select(d => new DataPoint(TimeSpan.FromTicks(d.Time.Ticks).TotalMilliseconds - TimeSpan.FromTicks(Data.Data[0].Time.Ticks).TotalMilliseconds, d.Theta)));
            this.MyModel.Series.Add(Theta);
        }

        public PlotModel MyModel { get; private set; }
    }
}
