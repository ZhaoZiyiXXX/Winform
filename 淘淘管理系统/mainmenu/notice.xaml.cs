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

namespace 淘淘管理系统
{
    /// <summary>
    /// page1.xaml 的交互逻辑
    /// </summary>
    public partial class page1 : UserControl
    {
        DBOperation dbo = new DBOperation();
        public page1()
        {
            InitializeComponent();
            Init_data();
        }

        private void Init_data()
        {
            string sql = "SELECT * FROM tt_notice ORDER BY id DESC";
            DataTable dt = dbo.Selectinfo(sql);
            listbox1.ItemsSource = dt.DefaultView;
        }
    }
}
