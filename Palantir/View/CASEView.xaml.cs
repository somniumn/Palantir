using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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
using System.Configuration;
using Synapse.Eden;
using Synapse.Financials;
using Synapse.Quantitative;
using System.Collections.Specialized;

namespace Palantir.View
{
    /// <summary>
    /// CASEView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CASEView : UserControl
    {
        private QEngine QEng = new QEngine();
        private EdenIF edenIF;
        private string ISIN;
        private AccountingStandard standard = AccountingStandard.Undefined;
        private string _prevText = string.Empty;
        private List<string> companyList = new List<string>();
        private ConfigManager configManager;
        private const string sectionName = "watchlist";

        public CASEView()
        {
            InitializeComponent();
            edenIF = new EdenIF("admin", "98321abc");
            //QEng.bActiveWACC = true;
            ISIN = "KR7005930003";

            // init chile views
            ProfileView.Init(edenIF);
            AnalysisView.Init(edenIF);
            ChartView.Init(edenIF);
            FinancialsView.Init(edenIF);
            NewsView.Init(edenIF);
            DisclosureView.Init(edenIF);
            //ReportView.Init(edenIF);

            DataTable properties = edenIF.GetProperties();

            foreach (DataRow row in properties.Rows)
            {
                companyList.Add(row[1].ToString());
            }

            // initialize
            UpdateContent(ISIN);
            UpdateChildViews(ISIN);

            // init configurations
            configManager = new ConfigManager("configurations.xml");
            configManager.Load();
            configManager.AddSection("watchlist");
            InitWatchlist();
        }

        private void InitWatchlist()
        {
            List<string> groupList = configManager.GetGroups(sectionName);
            if (groupList.Count > 0)
            {
                CbGroupList.ItemsSource = groupList;
                CbGroupList.SelectedIndex = 0;
            }
        }

        public void UpdateContent(string isin)
        {
            double PriceToday = 0;
            double PriceYesterday = 0;
            double PriceChangeValue = 0;
            double PriceChangeRatio = 0;
            DateTime today = edenIF.GetLatestPriceDate(isin);

            PriceYesterday = edenIF.GetPreviousPrice(isin);
            PriceToday = edenIF.GetLatestPrice(isin);
            PriceChangeValue = PriceToday - PriceYesterday;
            PriceChangeRatio = (PriceChangeValue / PriceYesterday) * 100.0;

            TbCompanyName.Text = edenIF.GetProperty(isin, "Name");
            TbCompanyPrice.Text = edenIF.GetLatestPrice(isin).ToString("#,###");
            if (PriceChangeValue > 0)
            {
                TbCompanyPriceChange.Text = "▲" + Math.Abs(PriceChangeValue).ToString("#,##0") + "(+" + Math.Abs(PriceChangeRatio).ToString("#,##0.##") + "%)";
                TbCompanyPriceChange.Foreground = Brushes.IndianRed;
            }
            else if (PriceChangeValue == 0.0)
            {
                TbCompanyPriceChange.Text = PriceChangeValue.ToString("#,##0") + "(" + PriceChangeRatio.ToString("#,##0.##") + "%)";
                TbCompanyPriceChange.Foreground = Brushes.Black;
            }
            else
            {
                TbCompanyPriceChange.Text = "▼" + Math.Abs(PriceChangeValue).ToString("#,##0") + "(-" + Math.Abs(PriceChangeRatio).ToString("#,##0.##") + "%)";
                TbCompanyPriceChange.Foreground = Brushes.RoyalBlue;
            }
            TbCompanyBelongTo.Text = edenIF.GetProperty(isin, "Market").ToString() + ", " + edenIF.GetProperty(isin, "SectorName").ToString();
            TbDate.Text = today.ToShortDateString();
        }

        public void UpdateChildViews(string isin)
        {
            if (edenIF.ContainsFinancials(ISIN, AccountingStandard.IFRS_CON) == true)
            {
                standard = AccountingStandard.IFRS_CON;
            }
            else if (edenIF.ContainsFinancials(ISIN, AccountingStandard.IFRS_SEP) == true)
            {
                standard = AccountingStandard.IFRS_SEP;
            }
            else
            {
                standard = AccountingStandard.Undefined;
            }

            QEng.TrySetCompany(ISIN, standard, false);
            QEng.Pricing();

            ProfileView.UpdateContent(isin, QEng);
            AnalysisView.UpdateContent(isin, QEng);
            ChartView.UpdateContent(isin, QEng);
            FinancialsView.UpdateContent(isin, QEng);
            NewsView.UpdateContent(isin, QEng);
            DisclosureView.UpdateContent(isin, QEng);
            //ReportView.UpdateContent(isin, QEng);
        }

        private void UpdateWatchlist()
        {
            List<string> groupList = configManager.GetGroups(sectionName);
            if (groupList.Count > 0)
            {
                CbGroupList.ItemsSource = groupList;
                CbGroupList.SelectedIndex = 0;
            }
        }

        private void Click_BnSearch(object sender, RoutedEventArgs e)
        {
            ISIN = EdenIF.GetISINByName(TbCompanySearch.Text, edenIF.GetProperties());
            if (ISIN != string.Empty)
            {
                UpdateContent(ISIN);
                UpdateChildViews(ISIN);
            }
        }

        private void PreviewTextInput_CbCompanySearch(object sender, TextCompositionEventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;

            cmb.IsDropDownOpen = true;

            if (!string.IsNullOrEmpty(cmb.Text))
            {
                string fullText = cmb.Text.Insert(GetChildOfType<TextBox>(cmb).CaretIndex, e.Text);
                cmb.ItemsSource = companyList.Where(s => s.IndexOf(fullText, StringComparison.InvariantCultureIgnoreCase) != -1).ToList();
            }
            else if (!string.IsNullOrEmpty(e.Text))
            {
                cmb.ItemsSource = companyList.Where(s => s.IndexOf(e.Text, StringComparison.InvariantCultureIgnoreCase) != -1).ToList();
            }
            else
            {
                cmb.ItemsSource = companyList;
            }
        }

        private void PreviewKeyUp_CbCompanySearch(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                ComboBox cmb = (ComboBox)sender;

                cmb.IsDropDownOpen = true;

                if (!string.IsNullOrEmpty(cmb.Text))
                {
                    cmb.ItemsSource = companyList.Where(s => s.IndexOf(cmb.Text, StringComparison.InvariantCultureIgnoreCase) != -1).ToList();
                }
                else
                {
                    cmb.ItemsSource = companyList;
                }
            }
        }

        private void Pasting_CbCompanySearch(object sender, DataObjectPastingEventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;

            cmb.IsDropDownOpen = true;

            string pastedText = (string)e.DataObject.GetData(typeof(string));
            string fullText = cmb.Text.Insert(GetChildOfType<TextBox>(cmb).CaretIndex, pastedText);

            if (!string.IsNullOrEmpty(fullText))
            {
                cmb.ItemsSource = companyList.Where(s => s.IndexOf(fullText, StringComparison.InvariantCultureIgnoreCase) != -1).ToList();
            }
            else
            {
                cmb.ItemsSource = companyList;
            }
        }

        public static T GetChildOfType<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = (child as T) ?? GetChildOfType<T>(child);
                if (result != null) return result;
            }
            return null;
        }

        private void KeyDown_TbCompanySearch(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                ISIN = EdenIF.GetISINByName(TbCompanySearch.Text, edenIF.GetProperties());
                if (ISIN != string.Empty)
                {
                    UpdateContent(ISIN);
                    UpdateChildViews(ISIN);
                }
            }
        }

        private void Click_BnWatchlist(object sender, RoutedEventArgs e)
        {
            string companyName = TbCompanyName.Text;
            CASEWatchlistView watchlistView = new CASEWatchlistView();
            watchlistView.Init(configManager);
            watchlistView.OpenWithAddToWatchlist(companyName);
            watchlistView.Show();
        }

        private void SelectionChanged_CbGroupList(object sender, SelectionChangedEventArgs e)
        {
            string groupName = (sender as ComboBox).SelectedValue.ToString();
            List<string> companyList = configManager.GetItems(sectionName, groupName, "name");
            LbCompanyList.ItemsSource = companyList;
        }

        private void MouseDoubleClick_LbCompanyList(object sender, MouseButtonEventArgs e)
        {
            string companyName = (sender as ListBox).SelectedValue.ToString();
            ISIN = EdenIF.GetISINByName(companyName, edenIF.GetProperties());
            if (ISIN != string.Empty)
            {
                UpdateContent(ISIN);
                UpdateChildViews(ISIN);
            }
        }

        private void Click_BnEditWatchlist(object sender, RoutedEventArgs e)
        {
            CASEWatchlistView watchlistView = new CASEWatchlistView();
            watchlistView.Init(configManager);
            watchlistView.Show();
        }

        private void Click_BnRefreshWatchlist(object sender, RoutedEventArgs e)
        {
            InitWatchlist();
        }
    }
}
