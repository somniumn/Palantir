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
using Synapse.Financials;
using Synapse.Quantitative;

namespace Palantir.View
{
    /// <summary>
    /// CASEFinancialsView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CASEFinancialsView : UserControl
    {
        private EdenIF edenIF;
        private Financial Revenue; // nonfinance
        private Financial OperatingProfit; // nonfinance
        private Financial OperatingProfitAndLoss; // finance
        private Financial NetProfit;
        private Financial AssetTotal;
        private Financial LiabilitiesTotal;
        private int FinancialVisualMaxCount = 8;

        public CASEFinancialsView()
        {
            InitializeComponent();
        }

        private void AutoGeneratingColumn_DgFinancials(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(System.DateTime))
                (e.Column as DataGridTextColumn).Binding.StringFormat = "dd/MM/yyyy";
        }

        public void Init(EdenIF eden)
        {
            edenIF = eden;

            Revenue = new Financial("Revenue", "매출액");
            OperatingProfit = new Financial("OperatingProfit", "영업이익");
            OperatingProfitAndLoss = new Financial("OperatingProfitAndLoss", "영업손익");
            NetProfit = new Financial("NetProfit", "당기순이익");
            AssetTotal = new Financial("AssetTotal", "자산총계");
            LiabilitiesTotal = new Financial("LiabilitiesTotal", "부채총계");
        }

        public void UpdateContent(string isin, QEngine qengine)
        {
            DataTable income = new DataTable();
            DataTable balance = new DataTable();
            DataTable transposedIncome = new DataTable();
            DataTable transposedBalance = new DataTable();
            int removeRowCount = 0;
            bool isFinance = false;
            isFinance = edenIF.GetProperty(isin, "Category") == "금융" ? true : false;

            if(isFinance == false)
            {
                Revenue = new Financial("Revenue", "매출액");
                OperatingProfit = new Financial("OperatingProfit", "영업이익");
            }
            else
            {
                OperatingProfitAndLoss = new Financial("OperatingProfitAndLoss", "영업손익");
            }
            NetProfit = new Financial("NetProfit", "당기순이익");
            AssetTotal = new Financial("AssetTotal", "자산총계");
            LiabilitiesTotal = new Financial("LiabilitiesTotal", "부채총계");

            // make income statement
            if (edenIF.ContainsFinancials(isin, AccountingStandard.IFRS_CON) == true)
                income = edenIF.GetIncomeStatement(isin, AccountingStandard.IFRS_CON);
            else if (edenIF.ContainsFinancials(isin, AccountingStandard.IFRS_SEP) == true)
                income = edenIF.GetIncomeStatement(isin, AccountingStandard.IFRS_SEP);
            removeRowCount = income.Rows.Count - FinancialVisualMaxCount;

            if (removeRowCount > 0)
            {
                for (int index = 0; index < removeRowCount; index++)
                    income.Rows.RemoveAt(0);
            }
            transposedIncome = edenIF.GenerateTransposedTable(income);

            foreach (DataColumn column in transposedIncome.Columns)
            {
                DateTime date = new DateTime();
                if (DateTime.TryParse(column.ColumnName, out date) == true)
                    column.ColumnName = Convert.ToDateTime(column.ColumnName).ToShortDateString();
            }

            foreach(DataRow row in transposedIncome.Rows)
            {
                string convertedName = Financial.ConvertFinancialNameTo(row[0].ToString(), Synapse.Globalization.eNation.eKR);
                row[0] = convertedName;

                for(int index = 1; index < row.ItemArray.Count(); index++)
                {
                    row[index] = Convert.ToDouble(row[index]).ToString("#,##0.00");
                }
            }

            DgIncomeStatement.ItemsSource = transposedIncome.DefaultView;

            foreach(DataRow row in income.Rows)
            {
                if (isFinance == false)
                {
                    Revenue.Add(new Synapse.Financials.Figure()
                    {
                        Date = Convert.ToDateTime(row["Date"]),
                        Value = Convert.ToDouble(row["Revenue"]),
                        Currency = Currency.KRW,
                        Unit = Unit.Million,
                        ColumnIndex = 0
                    });
                    OperatingProfit.Add(new Synapse.Financials.Figure()
                    {
                        Date = Convert.ToDateTime(row["Date"]),
                        Value = Convert.ToDouble(row["OperatingProfit"]),
                        Currency = Currency.KRW,
                        Unit = Unit.Million,
                        ColumnIndex = 0
                    });
                }
                else
                {
                    OperatingProfitAndLoss.Add(new Synapse.Financials.Figure()
                    {
                        Date = Convert.ToDateTime(row["Date"]),
                        Value = Convert.ToDouble(row["OperatingProfitAndLoss"]),
                        Currency = Currency.KRW,
                        Unit = Unit.Million,
                        ColumnIndex = 0
                    });
                }
                NetProfit.Add(new Synapse.Financials.Figure()
                {
                    Date = Convert.ToDateTime(row["Date"]),
                    Value = Convert.ToDouble(row["NetProfit"]),
                    Currency = Currency.KRW,
                    Unit = Unit.Million,
                    ColumnIndex = 0
                });
            }

            // make balance sheet
            if (edenIF.ContainsFinancials(isin, AccountingStandard.IFRS_CON) == true)
                balance = edenIF.GetBalanceSheet(isin, AccountingStandard.IFRS_CON);
            else if (edenIF.ContainsFinancials(isin, AccountingStandard.IFRS_SEP) == true)
                balance = edenIF.GetBalanceSheet(isin, AccountingStandard.IFRS_SEP);
            removeRowCount = balance.Rows.Count - FinancialVisualMaxCount;

            if (removeRowCount > 0)
            {
                for (int index = 0; index < removeRowCount; index++)
                    balance.Rows.RemoveAt(0);
            }
            transposedBalance = edenIF.GenerateTransposedTable(balance);

            foreach (DataColumn column in transposedBalance.Columns)
            {
                DateTime date = new DateTime();
                if (DateTime.TryParse(column.ColumnName, out date) == true)
                    column.ColumnName = Convert.ToDateTime(column.ColumnName).ToShortDateString();
            }

            foreach (DataRow row in transposedBalance.Rows)
            {
                string convertedName = Financial.ConvertFinancialNameTo(row[0].ToString(), Synapse.Globalization.eNation.eKR);
                row[0] = convertedName;

                for (int index = 1; index < row.ItemArray.Count(); index++)
                {
                    row[index] = Convert.ToDouble(row[index]).ToString("#,##0.00");
                }
            }

            foreach (DataRow row in balance.Rows)
            {
                AssetTotal.Add(new Synapse.Financials.Figure()
                {
                    Date = Convert.ToDateTime(row["Date"]),
                    Value = Convert.ToDouble(row["AssetTotal"]),
                    Currency = Currency.KRW,
                    Unit = Unit.Million,
                    ColumnIndex = 0
                });
                LiabilitiesTotal.Add(new Synapse.Financials.Figure()
                {
                    Date = Convert.ToDateTime(row["Date"]),
                    Value = Convert.ToDouble(row["LiabilitiesTotal"]),
                    Currency = Currency.KRW,
                    Unit = Unit.Million,
                    ColumnIndex = 0
                });
            }

            DgBalanceSheet.ItemsSource = transposedBalance.DefaultView;

            UpdateChart(isin);
        }

        public void UpdateChart(string isin)
        {
            bool isFinance = false;
            isFinance = edenIF.GetProperty(isin, "Category") == "금융" ? true : false;

            PltIncome.Clear();
            PltBalance.Clear();

            List<string> seriesName = new List<string>();
            if (isFinance == false)
            {
                seriesName.Add("Revenue");
                seriesName.Add("OperatingProfit");
            }
            else
            {
                seriesName.Add("OperatingProfitAndLoss");
            }
            seriesName.Add("NetProfit");

            List<string> categoryNames = new List<string>();
            List<int> categoryIndex = new List<int>();
            foreach (Synapse.Financials.Figure figure in Revenue.Figures)
            {
                categoryNames.Add(figure.Date.Year.ToString() + "/" + figure.Date.Month.ToString());
                categoryIndex.Add(-1);
            }

            PltIncome.Init("Income Statement", seriesName, categoryNames);

            List<double> values = new List<double>(); // Revenue
            if (isFinance == false)
            {
                foreach (Synapse.Financials.Figure figure in Revenue.Figures)
                {
                    values.Add(figure.Value);
                }
                PltIncome.SetData(0, values, categoryIndex);

                values = new List<double>();
                foreach (Synapse.Financials.Figure figure in OperatingProfit.Figures)
                {
                    values.Add(figure.Value);
                }
                PltIncome.SetData(1, values, categoryIndex);

                values = new List<double>();
                foreach (Synapse.Financials.Figure figure in NetProfit.Figures)
                {
                    values.Add(figure.Value);
                }
                PltIncome.SetData(2, values, categoryIndex);
            }
            else
            {
                foreach (Synapse.Financials.Figure figure in OperatingProfitAndLoss.Figures)
                {
                    values.Add(figure.Value);
                }
                PltIncome.SetData(0, values, categoryIndex);

                values = new List<double>();
                foreach (Synapse.Financials.Figure figure in NetProfit.Figures)
                {
                    values.Add(figure.Value);
                }
                PltIncome.SetData(1, values, categoryIndex);
            }

            seriesName = new List<string>();
            seriesName.Add("AssetTotal");
            seriesName.Add("LiabilitiesTotal");

            // Balance Sheet
            PltBalance.Init("Balance Sheet", seriesName, categoryNames);

            values = new List<double>(); // AssetTotal
            foreach (Synapse.Financials.Figure figure in AssetTotal.Figures)
            {
                values.Add(figure.Value);
            }
            PltBalance.SetData(0, values, categoryIndex);

            values = new List<double>(); // LiabilitiesTotal
            foreach (Synapse.Financials.Figure figure in LiabilitiesTotal.Figures)
            {
                values.Add(figure.Value);
            }
            PltBalance.SetData(1, values, categoryIndex);

        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}
