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
    /// SIRETagView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SIRETagView : UserControl
    {
        private EdenIF edenIF;
        private string TagName;

        public SIRETagView()
        {
            InitializeComponent();
        }

        public void Init(EdenIF eden)
        {
            edenIF = eden;

            List<TagCount> tagCounter = edenIF.GetTagList();

            DataTable tagList = new DataTable();
            tagList.Columns.Add("Tag");
            tagList.Columns.Add("Count");

            foreach(TagCount counter in tagCounter)
            {
                DataRow row = tagList.NewRow();
                row[0] = counter.Tag;
                row[1] = counter.Count;
                tagList.Rows.Add(row);
            }
            DGTagList.ItemsSource = tagList.DefaultView;
        }

        private void Click_BnSearchTag(object sender, RoutedEventArgs e)
        {
            TagName = TbTagName.Text;
            DataTable companyList = edenIF.FindCompaniesByTag(TagName);
            DGTagCompanyList.ItemsSource = companyList.DefaultView;
        }

        private void MouseDoubleClick_DGTagList(object sender, MouseButtonEventArgs e)
        {
            var cell = (sender as DataGrid).CurrentCell.Item;
            if (cell.GetType() == typeof(DataRowView))
            {
                TbTagName.Text = (cell as DataRowView).Row.ItemArray[0].ToString();
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}
