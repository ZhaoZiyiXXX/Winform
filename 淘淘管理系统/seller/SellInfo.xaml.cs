using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;

namespace 淘淘管理系统.seller
{
    /// <summary>
    /// SellInfo.xaml 的交互逻辑
    /// </summary>
    public partial class SellInfo : UserControl
    {
        DBOperation dbo = new DBOperation();
        public SellInfo()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tb_sellerid.Text))
            {
                MyOperation.MessageShow("请填写完整的书主号");
                tb_sellerid.Focus();
                return;
            }
            string sql = string.Format("SELECT s.sellinfoid AS `图书ID`,b.`name` AS `书名`,b.press AS `出版社`,s.price AS `售价`,"+
                "s.issold AS `是否出售` FROM tt_sellinfo AS s , tt_bookinfo AS b WHERE s.bookid = b.id AND "+
                "s.sellerid = '{0}'",tb_sellerid.Text );
            DataTable dt = dbo.Selectinfo(sql);
            int count = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if("0" == dt.Rows[i]["是否出售"].ToString())
                {
                    dt.Rows[i]["是否出售"] = "否";
                }
                else
                {
                    dt.Rows[i]["是否出售"] = "是";
                    count ++;
                }
            }
            dataGrid.ItemsSource = dt.DefaultView;
            lb_info.Content  = string.Format("共寄售{0}本，已出售{1}本",dt.Rows.Count.ToString(),count.ToString());
        }
    }
}
