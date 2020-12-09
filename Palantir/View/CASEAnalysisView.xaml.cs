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
    /// CASEAnalysisView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CASEAnalysisView : UserControl
    {
        private EdenIF edenIF;

        public CASEAnalysisView()
        {
            InitializeComponent();
        }

        public void Init(EdenIF eden)
        {
            edenIF = eden;
        }

        public void UpdateContent(string isin, QEngine engine)
        {
            TbPER.Text = engine.PricingData.PER.ToString("#,##0.##");
            TbEPS.Text = engine.PricingData.EPS.ToString("#,###") + "원";
            TbPBR.Text = engine.PricingData.PBR.ToString("#,##0.##");
            TbBPS.Text = engine.PricingData.BPS.ToString("#,###") + "원";
            TbDebtRatio.Text = engine.PricingData.Debtratio.ToString("#,##0.##") + "%";

            TbWACC.Text = engine.PricingData.WACC.ToString("#,##0.##") + " %";
            TbProfitLatest.Text = engine.PricingData.NetProfit.ToString("#,##0") + " 억";
            TbEPV.Text = (engine.PricingData.EPV / 100000000).ToString("#,##0") + " 억";
            TbPriceBasedEPV.Text = engine.PricingData.EPVPrice.ToString("#,##0") + " 원";
            TbNetAsset.Text = engine.PricingData.NetWorth.ToString("#,##0") + " 억";
            TbPriceBasedNetWorth.Text = engine.PricingData.NetWorthPrice.ToString("#,##0") + " 원";
            TbPremium.Text = engine.PricingData.Premium.ToString("#,##0.##") + " %";
            TbSafetyMargin.Text = engine.PricingData.SafetyMargin.ToString("#,##0.##") + " %";
            TbRightPrice.Text = engine.PricingData.RightPrice.ToString("#,###") + " 원";
            TbGuidePrice.Text = engine.PricingData.GuidePrice.ToString("#,###") + " 원";
        }
    }
}
