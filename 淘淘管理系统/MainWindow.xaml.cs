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
        private string taotaoid = "546ef619959e76a9178b456b";
        public MainWindow()
        {
            InitializeComponent();
            if (string.IsNullOrEmpty(App.login_group) || App.login_group != "admin")
            {
                this.MenuLinkGroups.Remove(system);
                this.MenuLinkGroups.Remove(toExcel);
            }
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = TimeSpan.FromSeconds(10);
            //timer.Interval = TimeSpan.FromHours(2);
            timer.Start();
        }
        bool flag = true ;
        void timer_Tick(object sender, EventArgs e)
        {
            if(flag){
            //本地对线上同步
                string sql = "SELECT DISTINCT s.bookid,b.gbookid,s.price,s.mallid FROM tt_bookinfo AS b ,tt_sellinfo AS s WHERE b.id = s.bookid AND s.issold = 0";
                DataTable dt = dbo.Selectinfo(sql);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (!ygw.IsExistSellInfo(dt.Rows[i]["mallid"].ToString(), dt.Rows[i]["gbookid"].ToString(), dt.Rows[i]["price"].ToString()))
                    {
                        IDictionary<string, string> parameters = new Dictionary<string, string>();
                        parameters.Add("book_id", dt.Rows[i]["gbookid"].ToString());
                        parameters.Add("seller_id", taotaoid);
                        parameters.Add("price", dt.Rows[i]["price"].ToString());
                        string sellid;
                        if (ygw.InsertNewSellInfo(parameters, out sellid))
                        {
                            sql = string.Format("UPDATE tt_sellinfo SET mallid = '{0}' WHERE bookid = '{1}' AND price = '{2}'", sellid, dt.Rows[i]["bookid"].ToString(), dt.Rows[i]["price"].ToString());
                            dbo.AddDelUpdate(sql);
                        }
                    }
                }
            }else{
            //线上对本地同步
            }
            flag = !flag;

        }
    }
}
