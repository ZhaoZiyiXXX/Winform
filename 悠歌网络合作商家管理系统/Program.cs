using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MySql.Data ;
using System.Data;

namespace 悠歌网络合作商家管理系统
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                string sql = "SHOW TABLES LIKE '%yg_local_shopinfo%'";
                DBOperation dbo = new DBOperation();
                DataTable dt = dbo.Selectinfo(sql);
                if (1 != dt.Rows.Count)
                {
                    Application.Run(new 登录页面());
                }
                else
                {
                    Application.Run(new 主页面());
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("程序启动异常，Msg=" + e.Message);
            }
        }
    }
}
