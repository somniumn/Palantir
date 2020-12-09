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
using System.Windows.Threading;
using MahApps.Metro.Controls;

namespace Palantir
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow
    {
        private DispatcherTimer mStateTimer;
        private double mTimeOutCounter = 0;
        private const double mTimerInterval = 2000;


        public MainWindow()
        {
            InitializeComponent();

            mStateTimer = new DispatcherTimer();
            mStateTimer.Interval = TimeSpan.FromMilliseconds(1000);
            mStateTimer.Tick += StateTimer_Tick;
            mStateTimer.Start();

        }

        private void StateTimer_Tick(object sender, EventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Background, (Action)delegate ()
            {
                //TbCurrentDateTime.Text = DateTime.Now.ToString();

                // test
            });
        }

        private void HamburgerMenuControl_ItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs e)
        {
            HamburgerMenuControl.Content = e.InvokedItem;
        }

        private void Loaded_MetroWindow(object sender, RoutedEventArgs e)
        {
            Launcher launcherWindow = new Launcher();
            //launcherWindow.Closed += (s, arg) => { if (launcherWindow.CloseMainWindow) this.Close(); };

            if(launcherWindow.ShowDialog() == true)
            {

            }
        }
    }
}
