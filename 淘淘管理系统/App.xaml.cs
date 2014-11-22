using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace 淘淘管理系统
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private static string _login_username = "";
        private static string _login_group = "";
        private static string _login_staffid = "";

        public static string login_username
        {
            get { return _login_username; }
            set { _login_username = value; }
        }

        public static string login_group
        {
            get { return _login_group; }
            set { _login_group = value; }
        }

        public static string login_staffid
        {
            get { return _login_staffid; }
            set { _login_staffid = value; }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
             _login_username = "";
             _login_group = "";
             _login_staffid = "";
        }
    }
}
