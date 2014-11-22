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

namespace 淘淘管理系统.mainmenu
{
    /// <summary>
    /// time.xaml 的交互逻辑
    /// </summary>
    public partial class time : UserControl
    {
        DBOperation dbo = new DBOperation();
        public time()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tb_staffid.Text))
            {
                MyOperation.MessageShow ("请填写员工工号！");
                tb_staffid.Focus();
                return;
            }

            if (string.IsNullOrEmpty(tb_time.Text ))
            {
                MyOperation.MessageShow("请填写工时");
                tb_time.Focus();
                return;
            }

            string sql = string.Format("INSERT INTO tt_time (`id`,`group`,`time`,`staffid`,`mark`,`date`) VALUES (null,'{0}','{1}','{2}','{3}','{4}')", comboBox1.Text, tb_time.Text, tb_staffid.Text, tb_mark.Text,DateTime.Now);
            if (1 == dbo.AddDelUpdate(sql))
            {
                MyOperation.MessageShow("添加成功");
                tb_mark.Text = tb_staffid.Text = tb_time.Text = "";
                return;
            }
            else
            {
                MyOperation.MessageShow("添加失败，请重试");
                return;
            }
        }
    }
}
