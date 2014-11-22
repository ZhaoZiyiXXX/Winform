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
    /// SellerSearch.xaml 的交互逻辑
    /// </summary>
    public partial class SellerSearch : UserControl
    {
        DBOperation dbo = new DBOperation();
        public SellerSearch()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.textBox1.Text))
            {
                MyOperation.MessageShow("请输入查询的书主信息");
                this.textBox1.Focus();
                return;
            }

            string sql = string.Format("SELECT * FROM tt_sellerinfo WHERE name LIKE '%{0}%' OR sellerid = '{1}'", textBox1.Text, textBox1.Text);

            DataTable dt = dbo.Selectinfo(sql);
            dataGrid.ItemsSource = dt.DefaultView;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.textBox1.Text))
            {
                MyOperation.MessageShow("请输入查询的书主信息");
                this.textBox1.Focus();
                return;
            }

            string sql = string.Format("SELECT * FROM tt_sellerinfo WHERE phone LIKE '%{0}%'", textBox1.Text);

            DataTable dt = dbo.Selectinfo(sql);
            dataGrid.ItemsSource = dt.DefaultView;
        }
    }
}
