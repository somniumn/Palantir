using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Controls;
using Synapse.Eden;
using Synapse.Plot;
using Synapse.Quantitative;

namespace Palantir.View
{
    /// <summary>
    /// CASEChartView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CASEChartView : UserControl
    {
        private EdenIF edenIF;

        public CASEChartView()
        {
            InitializeComponent();
        }

        public void Init(EdenIF eden)
        {
            edenIF = eden;
        }

        public void UpdateContent(string isin, QEngine qengine)
        {
            OhlcvDataSeries series;

            PltStock.Init("Stock & Volume");
            series = ConvertToStockData(edenIF.GetSignalData(isin, EdenIF.DateType.Daily), DateTime.Today, 180);
            PltStock.SetData(series);
        }

        private OhlcvDataSeries ConvertToStockData(DataTable dt, DateTime RefDay, int Range)
        {
            OhlcvDataSeries dataSeries = new OhlcvDataSeries();

            DateTime EndDay = RefDay.AddDays(-Range);
            var tempData = dt.AsEnumerable().Where(row => row.Field<DateTime>("Date") > EndDay);

            if (tempData.Count() != 0)
            {
                foreach(DataRow row in tempData)
                {
                    DateTime date = Convert.ToDateTime(row["Date"]);
                    double price = Convert.ToDouble(row["StockPrice"]);
                    double open = Convert.ToDouble(row["OpenPrice"]);
                    double high = Convert.ToDouble(row["HighPrice"]);
                    double low = Convert.ToDouble(row["LowPrice"]);
                    double close = Convert.ToDouble(row["ClosePrice"]);
                    double volume = Convert.ToDouble(row["ExchangeVolume"]);

                    dataSeries.Add(date.ToOADate(), high, low, open, close, volume, 0);
                }
            }

            return dataSeries;
        }
    }
}
