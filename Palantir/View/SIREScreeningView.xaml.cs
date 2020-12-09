using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
    /// SIREScreeningView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SIREScreeningView : UserControl
    {
        enum ScreenigSortingBy
        {
            Score,
            PER,
            PBR,
            ROE,
            DebtRatio,
            MarketCap
        };

        enum ScreenigSortingType
        {
            Descending, // 내림차순
            Ascending   // 오름차순
        };

        private EdenIF edenIF;
        private ScreeningParams screeningParams;
        private ScreenigSortingBy sortingBy = ScreenigSortingBy.Score;
        private ScreenigSortingType sortingType = ScreenigSortingType.Descending;

        public SIREScreeningView()
        {
            InitializeComponent();
        }

        public void Init(EdenIF eden)
        {
            edenIF = eden;
        }

        private void Click_BnRunScreening(object sender, RoutedEventArgs e)
        {
            screeningParams = new ScreeningParams();

            screeningParams.MinPER = Convert.ToDouble(TbPERMin.Text);
            screeningParams.MaxPER = Convert.ToDouble(TbPERMax.Text);
            screeningParams.MinPBR = Convert.ToDouble(TbPBRMin.Text);
            screeningParams.MaxPBR = Convert.ToDouble(TbPBRMax.Text);
            screeningParams.MinROE = Convert.ToDouble(TbROEMin.Text);
            screeningParams.MaxROE = Convert.ToDouble(TbROEMax.Text);
            screeningParams.MinDebtRatio = Convert.ToDouble(TbDebtRatioMin.Text);
            screeningParams.MaxDebtRatio = Convert.ToDouble(TbDebtRatioMax.Text);
            screeningParams.MinAggregateValue = Convert.ToDouble(TbMarketCapMin.Text);
            screeningParams.MaxAggregateValue = Convert.ToDouble(TbMarketCapMax.Text);

            screeningParams.UsePER = CbEnablePER.IsChecked == true ? true : false;
            screeningParams.UsePBR = CbEnablePBR.IsChecked == true ? true : false;
            screeningParams.UseROE = CbEnableROE.IsChecked == true ? true : false;
            screeningParams.UseDebtRatio = CbEnableDebtRatio.IsChecked == true ? true : false;
            screeningParams.UseAggregateValue = CbEnableMarketCap.IsChecked == true ? true : false;
            screeningParams.OnlyNonFinance = CbExceptFinanceCompany.IsChecked == true ? true : false;

            DataTable defaultTable = edenIF.GetScreeningBasis();
            IEnumerable<ScreeningResult> screeningResult = ConvertToScreeningResult(defaultTable, screeningParams);

            screeningResult = screeningResult.OrderByDescending(p => p.Score);

            // convert screening result table
            DGScreeningResult.ItemsSource = ConvertToDataTable(screeningResult).DefaultView;
        }

        private IEnumerable<ScreeningResult> ConvertToScreeningResult(DataTable dt, ScreeningParams Params)
        {
            return dt.AsEnumerable().Where(row =>
            (Convert.ToDouble(row["PER"]) >= Params.MinPER && Convert.ToDouble(row["PER"]) <= Params.MaxPER || !Params.UsePER)
            && (Convert.ToDouble(row["PBR"]) >= Params.MinPBR && Convert.ToDouble(row["PBR"]) <= Params.MaxPBR || !Params.UsePBR)
            && (Convert.ToDouble(row["ROE"]) >= Params.MinROE && Convert.ToDouble(row["ROE"]) <= Params.MaxROE || !Params.UseROE)
            && (Convert.ToDouble(row["DebtRatio"]) >= Params.MinDebtRatio && Convert.ToDouble(row["DebtRatio"]) <= Params.MaxDebtRatio || !Params.UseDebtRatio)
            && (Convert.ToDouble(row["AggregateValue"]) >= (Params.MinAggregateValue * 100000000) && Convert.ToDouble(row["AggregateValue"]) <= (Params.MaxAggregateValue * 100000000) || !Params.UseAggregateValue)
            && (!((Convert.ToString(row["SectorName"]) == "금융지원 서비스업" || Convert.ToString(row["SectorName"]) == "기타 금융업" || Convert.ToString(row["SectorName"]) == "투자기관" || Convert.ToString(row["SectorName"]) == "은행 및 저축기관")) || !Params.OnlyNonFinance)
            ).Select(row =>
            {
                Screener screener = new Screener(Convert.ToDouble(row["PER"]), Convert.ToDouble(row["PBR"]), Convert.ToDouble(row["DebtRatio"]));
                double score = screener.Scoring();

                return new ScreeningResult
                {
                    ISIN = Convert.ToString(row[0]),
                    Name = Convert.ToString(row[1]),
                    Date = Convert.ToDateTime(row["Date"]).ToShortDateString(),
                    Score = score.ToString("0.00", CultureInfo.InvariantCulture),
                    Price = String.Format("{0:#,###}", Convert.ToInt32(row["Price"])),
                    EPS = String.Format("{0:#,###}", Convert.ToInt32(row["EPS"])),
                    PER = Convert.ToDouble(row["PER"]).ToString("0.00", CultureInfo.InvariantCulture),
                    PBR = Convert.ToDouble(row["PBR"]).ToString("0.00", CultureInfo.InvariantCulture),
                    ROE = Convert.ToDouble(row["ROE"]).ToString("0.00", CultureInfo.InvariantCulture),
                    DebtRatio = Convert.ToDouble(row["DebtRatio"]).ToString("0.00", CultureInfo.InvariantCulture),
                    StockNumber = String.Format("{0:#,###}", Convert.ToInt32(row["StockNumber"])),
                    NetProfit = String.Format("{0:#,###}", Convert.ToDouble(row["NetProfit"])),
                    AggregateValue = String.Format("{0:#,###}", Convert.ToDouble(row["AggregateValue"]) / 100000000)
                };
            });
        }

        private DataTable ConvertToDataTable(IEnumerable<ScreeningResult> result)
        {
            DataTable table = new DataTable();
            //int index = 1;

            table.Columns.Add("이름");
            table.Columns.Add("주가", typeof(Int32));
            table.Columns.Add("EPS", typeof(Int32));
            table.Columns.Add("PER", typeof(double));
            table.Columns.Add("PBR", typeof(double));
            table.Columns.Add("ROE", typeof(double));
            table.Columns.Add("부채비율", typeof(double));
            table.Columns.Add("시가총액", typeof(double));
            table.Columns.Add("스코어", typeof(double));

            foreach (ScreeningResult res in result)
            {
                DataRow row = table.NewRow();

                row[0] = res.Name;
                row[1] = res.Price == "" ? 0 : Convert.ToDouble(res.Price);
                row[2] = res.EPS == "" ? 0 : Convert.ToDouble(res.EPS);
                row[3] = res.PER;
                row[4] = res.PBR;
                row[5] = res.ROE;
                row[6] = res.DebtRatio;
                row[7] = res.AggregateValue == "" ? 0 : Convert.ToDouble(res.AggregateValue);
                row[8] = res.Score;

                table.Rows.Add(row);
            }

            return table;
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}
