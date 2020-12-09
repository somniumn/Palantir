using System;
using System.Collections.Generic;
using System.Data;
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
using Synapse.Quantitative;

namespace Palantir.View
{
    /// <summary>
    /// SIREMagicFormulaView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SIREMagicFormulaView : UserControl
    {
        private EdenIF edenIF;

        public SIREMagicFormulaView()
        {
            InitializeComponent();
        }

        public void Init(EdenIF eden)
        {
            edenIF = eden;
            UpdateContent();
        }

        public void UpdateContent()
        {
            DataTable result = edenIF.GetMagicFormulaTableTop50();
            DGMagicFormulaResult.ItemsSource = result.DefaultView;
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}
