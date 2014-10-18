using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YouGe;

namespace 悠歌网络内部办公系统
{
    public partial class 注册页面 : Form
    {
        public 注册页面()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tb_username.Text))
            {
                MessageBox.Show("请输入用户名");
                tb_username.Focus();
                return;
            }

            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("请输入密码");
                textBox1.Focus();
                return;
            }

            if (string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("请再次输入密码");
                textBox2.Focus();
                return;
            }

            if (string.IsNullOrEmpty(textBox3.Text))
            {
                MessageBox.Show("请输入姓名");
                textBox3.Focus();
                return;
            }

            if (string.IsNullOrEmpty(textBox4.Text))
            {
                MessageBox.Show("请输入邮箱");
                textBox4.Focus();
                return;
            }

            if (string.IsNullOrEmpty(textBox5.Text))
            {
                MessageBox.Show("请输入手机");
                textBox5.Focus();
                return;
            }

            if(textBox1.Text != textBox2.Text )
            {
                MessageBox.Show("两次输入的密码不一致");
                textBox2.Focus();
                return;
            }
            //开始注册

            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("type","new");
            parameters.Add("username", tb_username.Text);
            parameters.Add("password", textBox2.Text);
            parameters.Add("realname", textBox3.Text);
            parameters.Add("email", textBox4.Text);
            parameters.Add("tel", textBox5.Text);
            YouGeWebApi yg = new YouGeWebApi();
            if (0 == yg.Register(parameters))
            {
                MessageBox.Show("注册成功！请登陆");
                this.Dispose();
            }
            else if (2 == yg.Register(parameters))
            {
                MessageBox.Show("用户名重复，请更换用户名！");
            }
            else
            {
                MessageBox.Show("注册失败！");
            }
        }

        private void 注册页面_Load(object sender, EventArgs e)
        {
            //tb_username.Text = "zhaoziyi";
            //textBox1.Text = "19880425";
            //textBox2.Text = "19880425";
            //textBox3.Text = "赵子逸";
            //textBox4.Text = "13818815102@qq.com";
            //textBox5.Text = "13818815102";
        }
    }
}
