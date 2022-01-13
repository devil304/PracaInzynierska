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
    public partial class End : Window
    {
        public End()
        {
            ((App)Application.Current).CloseOpenWindow(true);
            InitializeComponent();
            spaceHint.MouseDown += (_,__) => { SubjectName.Focusable = false; };
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                while (EEGDataExchange.Collecting) { }
                if (SubjectName.Text != "")
                {
                    EEGDataExchange.SaveFile(SubjectName.Text);
                    Close();
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
