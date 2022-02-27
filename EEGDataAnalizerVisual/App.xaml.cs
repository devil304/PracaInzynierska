using EEGDataAnalizer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace EEGDataAnalizerVisual
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {

        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow = new MainWindow();
            if(MainWindow.IsInitialized)
                MainWindow.Show();
        }

        public void CloseOpenWindow(bool windowOpend = false)
        {
            WindowCount += windowOpend ? 1 : -1;
            if (WindowCount == 0)
                Shutdown();
        }

        int WindowCount = 0;
    }
}
