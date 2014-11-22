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
using System.Windows.Shapes;
using System.Data;
using FirstFloor.ModernUI.Windows.Controls;
using MyExcel = Microsoft.Office.Interop.Excel;
using System.Reflection;

namespace 淘淘管理系统
{
    /// <summary>
    /// login.xaml 的交互逻辑
    /// </summary>
    public partial class login : Window
    {
        DBOperation dbo = new DBOperation();
        private const string DefaultUserName = "undefine";

        public login()
        {
            InitializeComponent();
            CheckUserNameSave();
            //导入淘淘原有交易信息的功能
            button3.Visibility = Visibility.Collapsed;
            button3.IsEnabled = false;
            //导入淘淘原有书主信息的功能
            button4.Visibility = Visibility.Collapsed;
            button4.IsEnabled = false;
            
        }

        private void CheckUserNameSave()
        {
            if (DefaultUserName != Properties.Settings.Default.username)
            {
                tb_user.Text = Properties.Settings.Default.username;
                checkBox1.IsChecked = true;
            }
        }

        private void CheckVersion()
        {
            string sql = "select value from tt_keyword where `key` = 'version'";
            DataTable dt = dbo.Selectinfo(sql);
            if (dt.Rows.Count != 1)
            {
                MyOperation.MessageShow("获取更新信息出错，请检查网络连接!");
                App.Current.Shutdown();
            }
            else
            {
                int version = Convert.ToInt32(dt.Rows[0]["value"].ToString());
                if (version > DBOperation.SoftwareVersion)
                {
                    MyOperation.MessageShow("检查到新的软件版本，请更新后使用");
                    //自动打开下载地址的函数没有调试好
                    //sql = "select `value` from tt_keyword where `key` = 'addr'";
                    //DataTable dt2 = dbo.Selectinfo(sql);
                    //System.Diagnostics.Process.Start(dt.Rows[0]["value"].ToString());
                    App.Current.Shutdown();
                }
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            logintest();

        }

        private void logintest()
        {
            if (string.IsNullOrEmpty(this.tb_user.Text) || string.IsNullOrEmpty(this.tb_password.Password))
            {
                return; 
            }
            string password = MyOperation.MD5(tb_password.Password);
            string sql = string.Format("SELECT * FROM `tt_staffinfo` WHERE `staffid` = '{0}' AND `password` = '{1}'",
                tb_user.Text, password);
            DataTable dt = dbo.Selectinfo(sql);
            if (dt.Rows.Count == 1)
            {
                MyOperation.DebugPrint(dt.Rows[0]["name"].ToString() + " Login.");
                if (this.checkBox1.IsChecked == true)
                {
                    Properties.Settings.Default.username = tb_user.Text;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    Properties.Settings.Default.username = DefaultUserName;
                    Properties.Settings.Default.Save();
                }
                App.login_staffid = dt.Rows[0]["staffid"].ToString();
                App.login_username = dt.Rows[0]["name"].ToString();
                App.login_group = dt.Rows[0]["role"].ToString();
                CheckVersion();
                MainWindow mw = new MainWindow();
                mw.Show();
                this.Close();
            }
            else
            {
                ModernDialog.ShowMessage("用户名或者密码错误，请重新输入", "登陆异常", MessageBoxButton.OK);
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            MyOperation.MessageShow("本系统不提供自助注册功能，请联系管理员添加用户");
        }

        private void tb_password_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    logintest();
                    e.Handled = true;
                }
            }
            catch (Exception e1)
            {
                MyOperation.MessageShow(e1.Message);
            }
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            MyExcel.Application app = new MyExcel.Application();
            try
            {
                MyExcel.Workbook obook = app.Workbooks.Open("d:/test.xlsx"); // 添加一个工作簿
                MyExcel.Worksheet osheet = (MyExcel.Worksheet)obook.Worksheets[1];// 获取当前工作表
                int r, c;
                int rCount = osheet.UsedRange.Rows.Count;
                int cCount = osheet.UsedRange.Columns.Count;
                for (r = 2; r < rCount; r++)
                //for (r = 12; r < 20; r++)
                {
                    string sellerid = string2length(osheet.Cells[r, 1].Value.ToString(),8);
                    string sellinfoid = string2length(osheet.Cells[r, 2].Value.ToString(),11);
                    string bookid = "1";
                    string buyer = "admin";
                    string buytime = "2014/8/20 00:00:00";
                    string price = osheet.Cells[r, 4].Value.ToString();
                    string issold = "";
                    string seller = "";
                    string soldtime = "";
                    if(osheet.Cells[r, 5].Value.ToString()=="False"){
                        issold = "0";
                        seller = null;
                        soldtime = null;
                    }else{
                        issold = "1";
                        seller = "admin";
                        soldtime = "2014/8/21 00:00:00";
                    }
                    string sql = string.Format("INSERT INTO tt_sellinfo (sellinfoid,sellerid,bookid,price,seller,buyer,buytime,issold,soldtime) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')",
                        sellinfoid, sellerid, bookid, price, seller, buyer, buytime, issold, soldtime);
                    dbo.AddDelUpdate(sql);
                }
                MessageBox.Show("数据导入完毕");
            }
            catch (Exception ex)
            {
                MyOperation.DebugPrint(ex.Message);
                MyOperation.MessageShow(ex.Message);
            }
            finally
            {
                //留给用户选择是否关闭
                //app.Quit();
                app = null;
            }
        }

        private string string2length(string str, int length)
        {
            if (str.Length > length)
            {
                MessageBox.Show("传入字符长度大于指定长度");
                return "";
            }

            string ling = "";
            for (int i = 0; i < (length - str.Length); i++)
            {
                ling += "0";
            }

            return ling + str ;
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            string sql = "SELECT id,sellerid FROM tt_sellerinfo";
            DataTable dt = dbo.Selectinfo(sql);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string newsellerid = string2length(dt.Rows[i]["sellerid"].ToString(),8);
                sql = "UPDATE tt_sellerinfo SET `sellerid` = '"+newsellerid +"' WHERE id = "+dt.Rows[i]["id"].ToString();
                dbo.AddDelUpdate(sql);
            }
        }
    }
}
