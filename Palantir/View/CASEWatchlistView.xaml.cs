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
using MahApps.Metro.Controls;
using Palantir;

namespace Palantir.View
{
    /// <summary>
    /// CASEWatchlistView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CASEWatchlistView
    {
        private ConfigManager configManager;
        private const string sectionName = "watchlist";
        private string currentGroupName;

        public CASEWatchlistView()
        {
            InitializeComponent();
        }

        public void Init(ConfigManager manager)
        {
            configManager = manager;
            List<string> groupList = configManager.GetGroups(sectionName);
            LbGroupList.ItemsSource = groupList;
            CbGroupList.ItemsSource = groupList;
        }

        public void OpenWithAddToWatchlist(string companyName)
        {
            TbItemName.Text = companyName;
            BnAddItem.IsEnabled = true;
        }

        private void MouseDoubleClick_LbGroupList(object sender, MouseButtonEventArgs e)
        {
            string groupName = (sender as ListBox).SelectedValue.ToString();
            TbGroupName.Text = groupName;
        }

        private void SelectionChanged_CbGroupList(object sender, SelectionChangedEventArgs e)
        {
            string groupName = (sender as ComboBox).SelectedValue.ToString();
            List<string> itemList = configManager.GetItems(sectionName, groupName, "name");
            LbItemList.ItemsSource = itemList;
        }

        private void MouseDoubleClick_LbItemList(object sender, MouseButtonEventArgs e)
        {
            string companyName = (sender as ListBox).SelectedValue.ToString();
            TbItemName.Text = companyName;
            BnAddItem.IsEnabled = false;
        }

        private void Click_BnAddGroup(object sender, RoutedEventArgs e)
        {
            configManager.AddGroup(sectionName, TbGroupName.Text);
            UpdateGroupList();
        }

        private void Click_BnUpdateGroup(object sender, RoutedEventArgs e)
        {
            string groupNameFrom = LbGroupList.SelectedValue.ToString();
            string groupNameTo = TbGroupName.Text;
            configManager.ChangeGroupName(sectionName, groupNameFrom, groupNameTo);
            UpdateGroupList();
        }

        private void Click_BnRemoveGroup(object sender, RoutedEventArgs e)
        {
            string groupName = LbGroupList.SelectedValue.ToString();
            configManager.RemoveGroup(sectionName, groupName);
            UpdateGroupList();
        }

        private void Click_BnSaveGroup(object sender, RoutedEventArgs e)
        {
            configManager.Save();
        }

        private void Click_BnAddItem(object sender, RoutedEventArgs e)
        {
            string groupName = CbGroupList.SelectedValue.ToString();
            configManager.AddUpdate(sectionName, groupName, "name", TbItemName.Text);
            UpdateItemList();
        }

        private void Click_BnRemoveItem(object sender, RoutedEventArgs e)
        {
            string groupName = CbGroupList.SelectedValue.ToString();
            configManager.RemoveItem(sectionName, groupName, "name", TbItemName.Text);
            UpdateItemList();
        }

        private void Click_BnSaveItems(object sender, RoutedEventArgs e)
        {
            configManager.Save();
        }

        private void UpdateGroupList()
        {
            List<string> groupList = configManager.GetGroups(sectionName);
            LbGroupList.ItemsSource = groupList;
        }

        private void UpdateItemList()
        {
            string groupName = CbGroupList.SelectedValue.ToString();
            List<string> itemList = configManager.GetItems(sectionName, groupName, "name");
            LbItemList.ItemsSource = itemList;
        }
    }
}
