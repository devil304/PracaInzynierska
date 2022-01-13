using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace EEGDataCollectorNative
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            EEGDataExchange.InitCloseEEG();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow = new MainWindow();
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
