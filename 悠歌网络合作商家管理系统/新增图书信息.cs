using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using YouGe;

namespace 悠歌网络合作商家管理系统
{
    public partial class 新增图书信息 : Form
    {
        YouGeWebApi yg = new YouGeWebApi();
        YouGeWinformApi ygw = new YouGeWinformApi();
        public 新增图书信息()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text ))
            {
                MessageBox.Show("请输入书名");
                textBox1.Focus();
                return;
            }

            if (string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("请输入作者");
                textBox2.Focus();
                return;
            }

            if (string.IsNullOrEmpty(textBox3.Text))
            {
                MessageBox.Show("请输入出版社");
                textBox3.Focus();
                return;
            }

            if (string.IsNullOrEmpty(textBox4.Text))
            {
                MessageBox.Show("请输入定价");
                textBox4.Focus();
                return;
            }

            if (string.IsNullOrEmpty(textBox5.Text))
            {
                MessageBox.Show("请输入ISBN");
                textBox5.Focus();
                return;
            }

            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("isbn", textBox5.Text);
            parameters.Add("fixedPrice", textBox4.Text);
            parameters.Add("name", textBox1.Text);
            parameters.Add("press", textBox3.Text);
            parameters.Add("author", textBox2.Text);
            string gbookid;
            if (yg.InsertNewBookInfo(parameters, out gbookid))
            {
                YouGeWinformApi.Localbookinfo lbi = new YouGeWinformApi.Localbookinfo();
                lbi.author = textBox2.Text;
                lbi.fixedprice = textBox4.Text;
                lbi.guid = gbookid;
                lbi.imgpath = "";
                lbi.isbn = textBox5.Text;
                lbi.name = textBox1.Text;
                lbi.press = textBox3.Text;
                if (ygw.InsertNewBookInfo(lbi, out gbookid))
                {
                    MessageBox.Show("添加成功，重新搜索即可查到");
                    this.Dispose();
                    return;
                }
            }
            MessageBox.Show("添加失败，请在网络状态良好的情况下重试");
            MyOperation.DebugPrint("添加图书信息失败，请在网络状态良好的情况下重试");
        }
    }
}
