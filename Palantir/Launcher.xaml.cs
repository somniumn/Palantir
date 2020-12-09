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

namespace Palantir
{
    /// <summary>
    /// Launcher.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Launcher : Window
    {
        public bool CloseMainWindow { get; private set; }

        public Launcher()
        {
            InitializeComponent();
            CloseMainWindow = false;
        }

        private void Click_BnLogin(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void Click_BnClose(object sender, RoutedEventArgs e)
        {
            CloseMainWindow = true;
            this.DialogResult = false;
            this.Close();
        }
    }
}
