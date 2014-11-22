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

namespace 淘淘管理系统.seller
{
    /// <summary>
    /// AddNewSeller.xaml 的交互逻辑
    /// </summary>
    public partial class AddNewSeller : UserControl
    {
        DBOperation dbo = new DBOperation();
        public AddNewSeller()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tb_sellerid.Text)||tb_sellerid.Text.Length !=8)
            {
                MyOperation.MessageShow("请填写正确的书主号，长度8位");
                tb_sellerid.Focus();
                return;
            }
            else
            {
                string sqls = string.Format("SELECT * FROM tt_sellerinfo WHERE sellerid = '{0}'", tb_sellerid.Text);
                DataTable dt = dbo.Selectinfo(sqls);
                if (dt.Rows.Count > 0)
                {
                    MyOperation.MessageShow("该书主号已经存在");
                    tb_sellerid.Focus();
                    return;
                }
            }

            if (string.IsNullOrEmpty(tb_name.Text))
            {
                MyOperation.MessageShow("请填写完整的姓名");
                tb_name.Focus();
                return;
            }

            if (string.IsNullOrEmpty(tb_phone.Text))
            {
                MyOperation.MessageShow("请填写完整的手机");
                tb_phone.Focus();
                return;
            }

            string sql = string.Format("INSERT INTO tt_sellerinfo (id,sellerid,name,phone,grade,mark) VALUES (null,'{0}','{1}','{2}','{3}','{4}')",
                tb_sellerid.Text, tb_name.Text, tb_phone.Text,tb_grade.Text, tb_mark.Text);
            if (dbo.AddDelUpdate(sql) == 1)
            {
                MyOperation.MessageShow("添加成功");
                tb_sellerid.Text = tb_name.Text = tb_phone.Text =  tb_mark.Text =  tb_grade.Text = "";
            }
            else
            {
                MyOperation.MessageShow("添加失败，请重试");
            }
        }
    }
}
