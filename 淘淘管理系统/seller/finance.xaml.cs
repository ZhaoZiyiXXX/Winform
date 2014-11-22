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
using System.ComponentModel;

namespace 淘淘管理系统.seller
{
    /// <summary>
    /// finance.xaml 的交互逻辑
    /// </summary>
    public partial class finance : UserControl
    {
        DBOperation dbo = new DBOperation();
        public finance()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text)) 
            {
                MyOperation.MessageShow("请输入要查询的书主ID");
                textBox1.Focus();
                return;
            }

            string sql = string.Format("SELECT ts.`name` AS `name`,tt.count AS count,tt.date AS date FROM tt_sellerinfo " +
                "AS ts ,tt_tikuan AS tt WHERE ts.sellerid = tt.sellerid AND ts.sellerid = '{0}'",textBox1.Text );
            DataTable dt = dbo.Selectinfo(sql);
            if (dt.Rows.Count == 0)
            {
                MyOperation.MessageShow("没有查询到书主【{0}】的提款信息");
                return;
            }
            else
            {
                dataGrid.ItemsSource = dt.DefaultView;
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(textBox2.Text))
            {
                MyOperation.MessageShow("请输入要查询的书主ID");
                textBox2.Focus();
                return;
            }

            displayye(textBox2.Text);

        }

        private void displayye(string sellerid)
        {
            string sellername = "";
            string sql = string.Format("SELECT `name` FROM tt_sellerinfo WHERE sellerid = '{0}'", sellerid);
            DataTable dt = dbo.Selectinfo(sql);
            if (dt.Rows.Count == 0)
            {
                MyOperation.MessageShow("查询书主【{0}】信息出错");
                return;
            }
            else
            {
                sellername = dt.Rows[0]["name"].ToString();
            }
            string ye = get_seller_count(sellerid);
            textBox3.Text = ye;
            lb_fanance.Content = sellername + ":" +ye;
        }

        private string get_seller_count(string sellerid)
        {
            if (string.IsNullOrEmpty(sellerid))
            {
                return "未知错误";
            }
            string number1 = "";
            string number2 = "";
            string sql = string.Format("SELECT SUM(`price`) FROM tt_sellinfo WHERE sellerid = '{0}' AND issold ='1'  GROUP BY NULL", textBox2.Text);
            DataTable dt = dbo.Selectinfo(sql);
            if (dt.Rows.Count <= 0)
            {
                return "0";
            }
            else
            {
                //求出已售出的总额
                number1 = dt.Rows[0][0].ToString();
            }

            sql = string.Format("SELECT SUM(`count`) FROM tt_tikuan WHERE sellerid = '{0}' GROUP BY NULL", textBox2.Text);
            dt = dbo.Selectinfo(sql);
            if (dt.Rows.Count <= 0)
            {
                return float.Parse(number1).ToString("0.0"); ;
            }
            else
            {
                //求出已提现的总额
                number2 = dt.Rows[0][0].ToString();
            }
            return (float.Parse(number1)-float.Parse(number2)).ToString("0.0");
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            string sql = string.Format("INSERT INTO tt_tikuan (sellerid,count,date) VALUES ('{0}','{1}','{2}')", textBox2.Text, textBox3.Text, DateTime.Now);
            dbo.AddDelUpdate(sql);
            displayye(textBox2.Text);
        }
    }
}
