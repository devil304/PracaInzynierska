using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using thinkgear_testapp_csharp_64;

namespace EEGDataCollectorNative
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Thread EEG;

        public MainWindow()
        {
            ((App)Application.Current).CloseOpenWindow(true);
            InitializeComponent();
            EEGThread EEGT = new EEGThread();
            EEG = new Thread(EEGT.Start);
            EEG.Start();/*
            while (!EEGDataExchange.EEGInited) { }
            EEGDataExchange.StartMeasurement("Testowa");
            Thread.Sleep(10000);
            EEGDataExchange.StopMeasurement(()=> { EEGDataExchange.SaveFile("Testowy"); });*/
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if(e.Key == Key.Space)
            {
                Test1 t1 = new();
                t1.Show();
                Close();
            }
            base.OnKeyDown(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            ((App)Application.Current).CloseOpenWindow();
            base.OnClosed(e);
        }
    }
} 
