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
using Synapse.Quantitative;

namespace Palantir.View
{
    /// <summary>
    /// CASEProfileView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CASEProfileView : UserControl
    {
        private EdenIF edenIF;

        public CASEProfileView()
        {
            InitializeComponent();
        }

        public void Init(EdenIF eden)
        {
            edenIF = eden;
        }

        public void UpdateContent(string isin, QEngine qengine)
        {
            TbSector.Text = edenIF.GetProperty(isin, "SectorName");
            TbMarketCap.Text = String.Format("{0:#,###}", (Convert.ToInt64(edenIF.GetLatestPrice(isin)) * Convert.ToInt64(edenIF.GetProperty(isin, "StockNumber")) / 100000000)) + " 억";
            TbLowHighIn52W.Text = qengine.PricingData.LowPriceOfYear.ToString("#,##0") + " / " + qengine.PricingData.HighPriceOfYear.ToString("#,##0");
            PbStatusIn52W.Value = qengine.PricingData.Price;
            PbStatusIn52W.Minimum = qengine.PricingData.LowPriceOfYear;
            PbStatusIn52W.Maximum = qengine.PricingData.HighPriceOfYear;
            TbFacePrice.Text = String.Format("{0:#,###}", (Convert.ToInt32(edenIF.GetProperty(isin, "FaceValue")))) + " 원";
            TbAddress.Text = edenIF.GetProperty(isin, "Address");
            TbHomepage.Text = edenIF.GetProperty(isin, "HomePageURL");
            TbTags.Text = edenIF.GetProperty(isin, "Tags");
            TbRisk.Text = edenIF.GetProperty(isin, "Risk");
        }
    }
}
