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
    /// NewStaff.xaml 的交互逻辑
    /// </summary>
    public partial class NewStaff : UserControl
    {
        DBOperation dbo = new DBOperation();
        public NewStaff()
        {
            InitializeComponent();
            InitComboBox();
        }

        private void InitComboBox()
        {
            Dictionary<string, string> mydic = new Dictionary<string, string>()
            {
                {"staff","员工"},
                {"admin","管理员"}
            };
            comboBox1.ItemsSource = mydic;
            comboBox1.SelectedValuePath = "Key";
            comboBox1.DisplayMemberPath = "Value";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tb_name.Text))
            {
                MyOperation.MessageShow("请填写姓名");
                tb_name.Focus();
                return;
            }

            if (string.IsNullOrEmpty(tb_phone.Text))
            {
                MyOperation.MessageShow("请填写手机");
                tb_phone.Focus();
                return;
            }

            if (string.IsNullOrEmpty(tb_sellerid.Text))
            {
                MyOperation.MessageShow("请填写学号");
                tb_sellerid.Focus();
                return;
            }

            if (string.IsNullOrEmpty(tb_email.Text))
            {
                MyOperation.MessageShow("请填写邮箱");
                tb_email.Focus();
                return;
            }

            if (string.IsNullOrEmpty(passwordBox1.Password))
            {
                MyOperation.MessageShow("请填写密码");
                passwordBox1.Focus();
                return;
            }

            if (string.IsNullOrEmpty(passwordBox2.Password))
            {
                MyOperation.MessageShow("请填写确认密码");
                passwordBox2.Focus();
                return;
            }

            if (passwordBox1.Password != passwordBox2.Password)
            {
                MyOperation.MessageShow("两次输入的密码不一致");
                passwordBox2.Focus();
                return;
            }

            string sql = string.Format("INSERT INTO tt_staffinfo (id,staffid,name,phone,password,email,mark,role) VALUES (null,'{0}','{1}','{2}','{3}','{4}','{5}','{6}')",
                tb_sellerid.Text,tb_name.Text,tb_phone.Text,MyOperation.MD5(passwordBox1.Password) , tb_email.Text,tb_mark.Text,comboBox1.SelectedValue.ToString());

            if (1 != dbo.AddDelUpdate(sql))
            {
                MyOperation.MessageShow("添加新用户失败，请重新添加");
                return;
            }
            else
            {
                MyOperation.MessageShow("添加成功");
                MyOperation.DebugPrint("添加了新用户");
                tb_email.Text = tb_mark.Text = tb_name.Text = tb_phone.Text = tb_sellerid.Text = "";
                return;
            }

        }
    }
}
