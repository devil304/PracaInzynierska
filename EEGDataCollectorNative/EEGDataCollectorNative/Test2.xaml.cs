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
using System.Windows.Shapes;

namespace EEGDataCollectorNative
{
    /// <summary>
    /// Interaction logic for Test.xaml
    /// </summary>
    public partial class Test2 : Window
    {
        public Test2()
        {
            ((App)Application.Current).CloseOpenWindow(true);
            InitializeComponent();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                if (!EEGDataExchange.Collecting)
                {
                    spaceHint.Text = "Wciśnij spację aby zakończyć zadanie.";
                    while (!EEGDataExchange.EEGInited) { }
                    EEGDataExchange.StartMeasurement("Relaks1");
                }
                else
                {
                    EEGDataExchange.StopMeasurement(() => {
                        Test3 t3 = new();
                        t3.Show();
                        Close();
                    });
                }
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
