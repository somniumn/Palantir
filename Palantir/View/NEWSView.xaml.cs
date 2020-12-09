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
using Synapse.Eden;
using Synapse.Financials;
using Synapse.Quantitative;

namespace Palantir.View
{
    /// <summary>
    /// NEWSView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class NEWSView : UserControl
    {
        private QEngine engine = new QEngine();
        private EdenIF edenIF;

        public NEWSView()
        {
            InitializeComponent();
            edenIF = new EdenIF("admin", "98321abc");

            NEWSGlobalMarketMonitorView.Init(edenIF);
        }
    }
}
