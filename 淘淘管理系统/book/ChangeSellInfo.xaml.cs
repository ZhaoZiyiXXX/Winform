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
using YouGe;

namespace 淘淘管理系统.book
{
    /// <summary>
    /// ChangeSellInfo.xaml 的交互逻辑
    /// </summary>
    public partial class ChangeSellInfo : UserControl
    {
        DBOperation dbo = new DBOperation();
        YouGeWebApi ygw = new YouGeWebApi();
        System.ComponentModel.BackgroundWorker backgroundWorker1;
        public ChangeSellInfo()
        {
            InitializeComponent();
            sp1.Visibility = Visibility.Hidden ;
        }

        void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                string[] Args = e.ToString().Split('|');
                string sql = string.Format("SELECT mallid FROM tt_sellinfo WHERE sellinfoid = '{0}'", Args[0]);
                DataTable dt = dbo.Selectinfo(sql);
                if (dt.Rows.Count == 1)
                {
                    IDictionary<string, string> parameters = new Dictionary<string, string>();
                    parameters.Add("price", Args[1]);
                    parameters.Add("id", dt.Rows[0]["mallid"].ToString());
                    string tmp;
                    ygw.InsertNewSellInfo(parameters, out tmp);
                }
            }
            catch(Exception ex)
            {
                MyOperation.DebugPrint(ex.Message,3);
            }
        }

        private string sellinfoid = "";

        private void SearchSellinfo()
        {
            lb_bookinfo.Content = "";
            if (string.IsNullOrEmpty(tb_sellidsearch.Text))
            {
                MyOperation.MessageShow("请先输入图书编号");
                tb_sellidsearch.Focus();
                return;
            }
            string sql = string.Format("SELECT tb.`name` AS `书名`,tb.press AS `出版社`,tb.price AS `定价`,tb.ISBN AS ISBN,st.`name` AS `收书人`," +
                "se.`issold` AS `出售`,se.`price` AS `售价` FROM tt_bookinfo AS tb , tt_sellinfo AS se ,tt_staffinfo AS st WHERE tb.id = se.bookid " +
                " AND st.staffid = se.buyer AND se.sellinfoid = {0}", tb_sellidsearch.Text);
            DataTable dt = dbo.Selectinfo(sql);
            if (dt.Rows.Count == 1)
            {
                sellinfoid = tb_sellidsearch.Text;
                lb_bookinfo.Content = "图书唯一ID：" + tb_sellidsearch.Text;
                lb_bookinfo.Content += "\r\n书名：" + dt.Rows[0]["书名"].ToString();
                lb_bookinfo.Content += "\r\n出版社：" + dt.Rows[0]["出版社"].ToString();
                lb_bookinfo.Content += "\r\n定价：" + dt.Rows[0]["定价"].ToString();
                lb_bookinfo.Content += "\r\n售价：" + dt.Rows[0]["售价"].ToString();
                lb_bookinfo.Content += "\r\n收书人：" + dt.Rows[0]["收书人"].ToString();
                lb_bookinfo.Content += "\r\n是否出售：" + ((dt.Rows[0]["出售"].ToString() == "0") ? "否" : "是");
                sp1.Visibility = Visibility.Visible ;
            }
            else
            {
                sellinfoid = "";
                MyOperation.MessageShow(string.Format("没有查到图书唯一ID为【{0}】的交易", tb_sellidsearch.Text));
                tb_sellidsearch.Focus();
                return;
            }
        }
        private void tb_sellidsearch_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    SearchSellinfo();
                    e.Handled = true;
                }
            }
            catch (Exception e1)
            {
                MyOperation.DebugPrint(e1.Message);

            }
        }
        private void button0_Click(object sender, RoutedEventArgs e)
        {
            SearchSellinfo();
        }

        private void ChangePrice()
        {
            if (string.IsNullOrEmpty(tb_price.Text))
            {
                MyOperation.MessageShow("请输入新的价格");
                tb_price.Focus();
                return;
            }

            if (sellinfoid == "")
            {
                MyOperation.MessageShow("请先在上方查询出要修改的交易信息");
                tb_sellidsearch.Focus();
                return;
            }
            string sql = string.Format("UPDATE tt_sellinfo SET price = '{0}' WHERE sellinfoid = '{1}'", tb_price.Text, sellinfoid);
            if (1 == dbo.AddDelUpdate(sql))
            {
                MyOperation.MessageShow("修改成功！");
                lb_bookinfo.Content = "";
                if (string.IsNullOrEmpty(tb_sellidsearch.Text))
                {
                    MyOperation.MessageShow("请先输入图书编号");
                    tb_sellidsearch.Focus();
                    return;
                }
                sql = string.Format("SELECT tb.`name` AS `书名`,tb.press AS `出版社`,tb.price AS `定价`,tb.ISBN AS ISBN,st.`name` AS `收书人`," +
                    "se.`issold` AS `出售`,se.`price` AS `售价` FROM tt_bookinfo AS tb , tt_sellinfo AS se ,tt_staffinfo AS st WHERE tb.id = se.bookid " +
                    " AND st.staffid = se.buyer AND se.sellinfoid = {0}", sellinfoid);
                DataTable dt = dbo.Selectinfo(sql);
                if (dt.Rows.Count == 1)
                {
                    lb_bookinfo.Content = "图书唯一ID：" + sellinfoid;
                    lb_bookinfo.Content += "\r\n书名：" + dt.Rows[0]["书名"].ToString();
                    lb_bookinfo.Content += "\r\n出版社：" + dt.Rows[0]["出版社"].ToString();
                    lb_bookinfo.Content += "\r\n定价：" + dt.Rows[0]["定价"].ToString();
                    lb_bookinfo.Content += "\r\n售价：" + dt.Rows[0]["售价"].ToString();
                    lb_bookinfo.Content += "\r\n收书人：" + dt.Rows[0]["收书人"].ToString();
                    lb_bookinfo.Content += "\r\n是否出售：" + ((dt.Rows[0]["出售"].ToString() == "0") ? "否" : "是");
                }
                else
                {
                    tb_sellidsearch.Focus();
                    return;
                }
            }
            else
            {
                MyOperation.MessageShow("修改失败！");
                return;
            }

            //同步修改喵校园主库中的售价信息
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            //backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            string paras = tb_sellidsearch.Text + "|" + tb_price.Text ;
            backgroundWorker1.RunWorkerAsync(paras);
        }
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            ChangePrice();
        }

        private void tb_price_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    ChangePrice();
                    e.Handled = true;
                }
            }
            catch (Exception e1)
            {
                MyOperation.DebugPrint(e1.Message);

            }
        }
    }
}
