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
using FirstFloor.ModernUI.Windows.Controls;
using System.Windows.Threading;
using System.Data;
using YouGe;
using System.Threading;
using System.ComponentModel;

namespace 淘淘管理系统
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        DispatcherTimer timer = new DispatcherTimer();
        DBOperation dbo = new DBOperation();
        YouGeWebApi ygw = new YouGeWebApi();
        public MainWindow()
        {
            InitializeComponent();
            if (string.IsNullOrEmpty(App.login_group) || App.login_group != "admin")
            {
                this.MenuLinkGroups.Remove(system);
                this.MenuLinkGroups.Remove(toExcel);
            }
            BackgroundWorker backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.RunWorkerAsync();
        }
        
        void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            string sql;
            DataTable dt;
            int i;
            IDictionary<string, string> parameters;
            string sellid;
            while(true)
            {
                //线上书主信息导入本地
                sql = "SELECT * FROM tt_sellerinfo";
                dt = dbo.Selectinfo(sql);
                ExcelOperation.dataTableToCsv(dt, "LocalSellerInfo.csv",false);
                //Thread.Sleep(3600 * 1000);//间隔1小时

                //本地对线上同步
                sql = "SELECT DISTINCT s.bookid,b.gbookid,s.price,s.mallid FROM tt_bookinfo AS b ,tt_sellinfo AS s WHERE b.isbn not like '1000000%' AND b.id = s.bookid AND s.issold = 0";
                dt = dbo.Selectinfo(sql);
                for (i = 0; i < dt.Rows.Count; i++)
                {
                    if (!ygw.IsExistSellInfo(dt.Rows[i]["mallid"].ToString(), dt.Rows[i]["gbookid"].ToString(), dt.Rows[i]["price"].ToString()))
                    {
                        parameters = new Dictionary<string, string>();
                        parameters.Add("book_id", dt.Rows[i]["gbookid"].ToString());
                        parameters.Add("seller_id", Properties.Settings.Default.sellerid);
                        parameters.Add("price", dt.Rows[i]["price"].ToString());
                        sellid = null;
                        if (ygw.InsertNewSellInfo(parameters, out sellid))
                        {
                            sql = string.Format("UPDATE tt_sellinfo SET mallid = '{0}' WHERE bookid = '{1}' AND  ABS(price- {2}) < 1e-5", sellid, dt.Rows[i]["bookid"].ToString(), dt.Rows[i]["price"].ToString());
                            dbo.AddDelUpdate(sql);
                        }
                    }
                }
                Thread.Sleep(3600 * 1000 );//间隔1小时

                //线上对本地同步
                Thread.Sleep(3600 * 1000);//执行完毕延时1小时
            }
        }
    }
}
