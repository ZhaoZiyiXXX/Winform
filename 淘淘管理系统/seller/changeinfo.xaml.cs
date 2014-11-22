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
    /// changeinfo.xaml 的交互逻辑
    /// </summary>
    public partial class changeinfo : UserControl
    {
        DBOperation dbo = new DBOperation();
        public changeinfo()
        {
            InitializeComponent();
        }

        private string sellerid = "";

        private void SearchSellerinfo()
        {
            if (string.IsNullOrEmpty(this.tb_selleridsearch.Text))
            {
                MyOperation.MessageShow("请输入查询的书主号");
                this.tb_selleridsearch.Focus();
                return;
            }

            string sql = string.Format("SELECT * FROM tt_sellerinfo WHERE sellerid = '{0}'", tb_selleridsearch.Text);

            DataTable dt = dbo.Selectinfo(sql);
            if (dt.Rows.Count != 1)
            {
                sellerid = "";
                MyOperation.MessageShow("查询失败，请检查书主号是否正确");
                this.tb_selleridsearch.Focus();
                return;
            }
            else
            {
                sellerid = dt.Rows[0]["sellerid"].ToString();
                tb_jiebie.Text = dt.Rows[0]["grade"].ToString();
                tb_mark.Text = dt.Rows[0]["mark"].ToString();
                tb_name.Text = dt.Rows[0]["name"].ToString();
                tb_tel.Text = dt.Rows[0]["phone"].ToString();
            }
        }
        private void button0_Click(object sender, RoutedEventArgs e)
        {
            SearchSellerinfo();
        }
        private void tb_selleridsearch_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    SearchSellerinfo();
                    e.Handled = true;
                }
            }
            catch (Exception e1)
            {
                MyOperation.DebugPrint(e1.Message);

            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tb_name.Text))
            {
                MyOperation.MessageShow("没有输入内容");
                this.tb_name.Focus();
                return;
            }
            string sql = string.Format("UPDATE tt_sellerinfo SET `name` = '{0}' WHERE `sellerid` = '{1}'",tb_name.Text,sellerid);
            if (1 == dbo.AddDelUpdate(sql))
            {
                MyOperation.MessageShow("修改成功！");
            }
            else
            {
                MyOperation.MessageShow("修改失败！");
            }
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tb_tel.Text))
            {
                MyOperation.MessageShow("没有输入内容");
                this.tb_tel.Focus();
                return;
            }
            string sql = string.Format("UPDATE tt_sellerinfo SET `phone` = '{0}' WHERE `sellerid` = '{1}'", tb_tel.Text, sellerid);
            if (1 == dbo.AddDelUpdate(sql))
            {
                MyOperation.MessageShow("修改成功！");
            }
            else
            {
                MyOperation.MessageShow("修改失败！");
            }
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tb_jiebie.Text))
            {
                MyOperation.MessageShow("没有输入内容");
                this.tb_jiebie.Focus();
                return;
            }
            string sql = string.Format("UPDATE tt_sellerinfo SET `grade` = '{0}' WHERE `sellerid` = '{1}'", tb_jiebie.Text, sellerid);
            if (1 == dbo.AddDelUpdate(sql))
            {
                MyOperation.MessageShow("修改成功！");
            }
            else
            {
                MyOperation.MessageShow("修改失败！");
            }
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tb_mark.Text))
            {
                MyOperation.MessageShow("没有输入内容");
                this.tb_mark.Focus();
                return;
            }
            string sql = string.Format("UPDATE tt_sellerinfo SET `mark` = '{0}' WHERE `sellerid` = '{1}'", tb_mark.Text, sellerid);
            if (1 == dbo.AddDelUpdate(sql))
            {
                MyOperation.MessageShow("修改成功！");
            }
            else
            {
                MyOperation.MessageShow("修改失败！");
            }
        }


    }
}
