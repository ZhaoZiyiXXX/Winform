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

namespace 淘淘管理系统.system
{
    /// <summary>
    /// newnotice.xaml 的交互逻辑
    /// </summary>
    public partial class newnotice : UserControl
    {
        DBOperation dbo = new DBOperation();
        public newnotice()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tb_title.Text))
            {
                MyOperation.MessageShow("请填写完整的公告标题");
                tb_title.Focus();
                return;
            }

            if ( string.IsNullOrEmpty(tb_context.Text))
            {
                MyOperation.MessageShow("请填写完整的公告内容");
                tb_context.Focus();
                return;
            }
            string sql = string.Format("INSERT INTO tt_notice (id,title,context) VALUES (null,'{0}','{1}')", tb_title.Text, tb_context.Text);
            if (dbo.AddDelUpdate(sql) == 1)
            {
                MyOperation.MessageShow("添加成功");
                tb_title.Text = "";
                tb_context.Text = "";
            }
            else
            {
                MyOperation.MessageShow("添加失败，请重试");
            }
        }
    }
}
