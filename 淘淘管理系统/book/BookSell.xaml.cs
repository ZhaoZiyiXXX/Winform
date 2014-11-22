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
    /// BookSell.xaml 的交互逻辑
    /// </summary>
    public partial class BookSell : UserControl
    {
        System.ComponentModel.BackgroundWorker backgroundWorker1;
        YouGeWebApi ygw = new YouGeWebApi();
        DBOperation dbo = new DBOperation();
        public BookSell()
        {
            InitializeComponent();
        }

        void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                string sellinfoid = e.ToString();
                string sql = string.Format("SELECT mallid FROM tt_sellinfo WHERE sellinfoid = '{0}'", sellinfoid);
                DataTable dt = dbo.Selectinfo(sql);
                if (dt.Rows.Count == 1 && !string.IsNullOrEmpty(dt.Rows[0]["mallid"].ToString()))
                {
                    IDictionary<string, string> parameters = new Dictionary<string, string>();
                    parameters.Add("status", "1");
                    parameters.Add("id", dt.Rows[0]["mallid"].ToString());
                    ygw.InsertNewSellInfo(parameters, out sellinfoid);
                }
            }
            catch(Exception ex)
            {
                MyOperation.DebugPrint(ex.Message,3);
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            lb_bookinfo.Content = "";
            if (string.IsNullOrEmpty(textBox2.Text))
            {
                MyOperation.MessageShow("请先输入图书编号");
                textBox2.Focus();
                return;
            }
            string sql = string.Format("SELECT tb.`name` AS `书名`,tb.press AS `出版社`,tb.price AS `定价`,tb.ISBN AS ISBN,st.`name` AS `收书人`," +
                "se.`issold` AS `出售`,se.`price` AS `售价` FROM tt_bookinfo AS tb , tt_sellinfo AS se ,tt_staffinfo AS st WHERE tb.id = se.bookid " +
                " AND st.staffid = se.buyer AND se.sellinfoid = '{0}'", textBox2.Text);
            DataTable dt = dbo.Selectinfo(sql);
            if (dt.Rows.Count == 1)
            {
                lb_bookinfo.Content = "图书唯一ID：" + textBox2.Text;
                lb_bookinfo.Content += "\r\n书名：" + dt.Rows[0]["书名"].ToString();
                lb_bookinfo.Content += "\r\n出版社：" + dt.Rows[0]["出版社"].ToString();
                lb_bookinfo.Content += "\r\n定价：" + dt.Rows[0]["定价"].ToString();
                lb_bookinfo.Content += "\r\n售价：" + dt.Rows[0]["售价"].ToString();
                lb_bookinfo.Content += "\r\n收书人：" + dt.Rows[0]["收书人"].ToString();
                lb_bookinfo.Content += "\r\n是否出售：" + ((dt.Rows[0]["出售"].ToString() == "0") ? "否" : "是");
            }
            else
            {
                MyOperation.MessageShow(string.Format("没有查到图书唯一ID为【{0}】的交易", textBox2.Text));
                textBox2.Focus();
                return;
            }
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(textBox2.Text))
            {
                MyOperation.MessageShow("请先输入图书编号");
                textBox2.Focus();
                return;
            }

            string sql = string.Format("UPDATE tt_sellinfo SET issold = 1 ,seller = '{0}',soldtime = '{1}' WHERE sellinfoid = '{2}' AND issold = '0'", "123",DateTime.Now.ToString("G"), textBox2.Text);
            if (dbo.AddDelUpdate(sql) != 1)
            {
                MyOperation.MessageShow(string.Format("没有查到图书唯一ID为【{0}】并且尚未出售的交易，请在上方查询详细信息", textBox2.Text));
                textBox2.Focus();
                return;
            }
            else
            {
                MyOperation.MessageShow("出售成功！");
                textBox2.Focus();
            }

            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            string sellinfoid = textBox2.Text;
            backgroundWorker1.RunWorkerAsync(sellinfoid);
        }
    }
}
