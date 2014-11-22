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

namespace 淘淘管理系统.mainmenu
{
    /// <summary>
    /// jiezhang.xaml 的交互逻辑
    /// </summary>
    public partial class jiezhang : UserControl
    {
        DBOperation dbo = new DBOperation();
        public jiezhang()
        {
            InitializeComponent();
            Display();
        }

        private void Display()
        {
            money1.Content = get_money1();
            money2.Content = get_money2();
            float tmp = float.Parse(money2.Content.ToString()) - float.Parse(money1.Content.ToString());
            lb_thisyue.Content = tmp.ToString();
        }

        private string get_money1()
        {
            string sql = "SELECT money FROM tt_fanance ORDER BY date DESC LIMIT 0,1";
            DataTable dt = dbo.Selectinfo(sql);
            if (dt.Rows.Count == 1)
            {
                return dt.Rows[0]["money"].ToString();
            }
            else
            {
                return "0";
            }
        }

        //可以刷新当前实时余额以及出售数量
        private string get_money2()
        {
            string sql = "SELECT date,money FROM tt_fanance ORDER BY date DESC LIMIT 0,1";
            DataTable dt = dbo.Selectinfo(sql);
            float count_old = 0;
            string datetime = "";
            if (dt.Rows.Count == 1)
            {
                count_old = float.Parse(dt.Rows[0]["money"].ToString());
                datetime = dt.Rows[0]["date"].ToString();
            }
            else
            {
                datetime = "2000/1/1 00:00:00";
            }
            float count1 = 0;
            //计算该班次出售的数量
            sql = string.Format("SELECT count(*) FROM tt_sellinfo WHERE issold = '1' AND soldtime > '{0}'  GROUP BY NULL", datetime);
            dt = dbo.Selectinfo(sql);
            if (dt.Rows.Count == 1)
            {
                lb_thiscount.Content = dt.Rows[0][0].ToString();
            }
            else
            {
                lb_thiscount.Content = "数量异常";
            }
            //先计算出售信息
            sql = string.Format("SELECT SUM(price) FROM tt_sellinfo WHERE issold = '1' AND soldtime > '{0}'  GROUP BY NULL", datetime);
            dt = dbo.Selectinfo(sql);
            if (dt.Rows.Count == 1)
            {
                count1 = float.Parse(dt.Rows[0][0].ToString());
            }

            float count2 = 0;
            //再计算提款信息
            sql = string.Format("SELECT SUM(count) FROM tt_tikuan WHERE date > '{0}'  GROUP BY NULL", datetime);
            dt = dbo.Selectinfo(sql);
            if (dt.Rows.Count == 1)
            {
                count2 = float.Parse(dt.Rows[0][0].ToString());
            }
            MyOperation.DebugPrint("前日剩余："+count_old.ToString()+"今日收入："+count1.ToString()+"今日支出："+count2.ToString());
            return (count_old + count1 - count2).ToString();
        }
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //string ye = get_money2();
            if(string.IsNullOrEmpty(textBox1.Text)){
                MyOperation.MessageShow("请输入当前现金余额");
                return;
            }
            string ye = textBox1.Text;
            string sql = string.Format("INSERT INTO tt_fanance (money,date) VALUES ('{0}','{1}')",ye,DateTime.Now);
            if (1 != dbo.AddDelUpdate(sql))
            {
                MyOperation.MessageShow("结账失败，请联系管理员处理");
            }
            else
            {
                MyOperation.MessageShow("结账成功，可以下班了！");
            }
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.F5)
                {
                    Display();
                    e.Handled = true;
                }
            }
            catch (Exception e1)
            {
                MyOperation.DebugPrint("结账下班界面刷新时出现了异常"+e1.Message);
                MyOperation.MessageShow("结账下班界面刷新时出现了异常" + e1.Message);
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Display();
        }
    }
}
