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
    public partial class MainWindowEEG : Window
    {
        EEGPlotData[] Data;
        public MainWindowEEG(string FilePath)
        {
            ((App)Application.Current).CloseOpenWindow(true);
            Data = null;
            Data = DataModel.DeserializeXMLFileEEGPD(FilePath);

            this.DataContext = new MainViewModelEEG(Data);
            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e)
        {
            ((App)Application.Current).CloseOpenWindow();
            base.OnClosed(e);
        }

    }

    public class MainViewModelEEG
    {
        public MainViewModelEEG(EEGPlotData[] Data)
        {
            this.MyModel = new PlotModel { Title = "EEGData pre post " };
            this.MyModel.Legends.Add(new Legend(){
                LegendTitle = "Legend",
                LegendPosition = LegendPosition.TopCenter
            });
            this.MyModel.IsLegendVisible = true;
            LineSeries AttRaw = new LineSeries();
            AttRaw.Color = OxyColor.FromRgb(255, 0, 0);
            AttRaw.Title = "Attention RAW";
            AttRaw.Points.AddRange(Data.Select(d => new DataPoint(d.TimeS, d.RawAtt)));
            this.MyModel.Series.Add(AttRaw);
            LineSeries MedRaw = new LineSeries();
            MedRaw.Color = OxyColor.FromRgb(0, 0, 255);
            MedRaw.Title = "Meditation RAW";
            MedRaw.Points.AddRange(Data.Select(d => new DataPoint(d.TimeS, d.RawMed)));
            this.MyModel.Series.Add(MedRaw);
            LineSeries AttFilter = new LineSeries();
            AttFilter.Color = OxyColor.FromRgb(255, 200, 150);
            AttFilter.Title = "Attention Filtered";
            AttFilter.Points.AddRange(Data.Select(d => new DataPoint(d.TimeS, d.GameAtt)));
            this.MyModel.Series.Add(AttFilter);
            LineSeries MedFilter = new LineSeries();
            MedFilter.Color = OxyColor.FromRgb(100, 255, 255);
            MedFilter.Title = "Meditation Filtered";
            MedFilter.Points.AddRange(Data.Select(d => new DataPoint(d.TimeS, d.GameMed)));
            this.MyModel.Series.Add(MedFilter);
        }

        public PlotModel MyModel { get; private set; }

    }
}
