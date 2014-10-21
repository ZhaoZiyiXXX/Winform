using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using YouGe;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace 悠歌网络内部办公系统
{
    public partial class 登录页面 : Form
    {
        public 登录页面()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            login();            
        }

        private void login()
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

            YouGeWebApi yg = new YouGeWebApi();
            JObject userinfo;
            if (yg.UserAccess(tb_username.Text, textBox1.Text, out userinfo))
            {
                try
                {
                    GlobalVar.name = userinfo["data"]["realname"].ToString();
                    GlobalVar.id = userinfo["data"]["id"].ToString();
                    GlobalVar.username = userinfo["data"]["username"].ToString();
                    GlobalVar.email = userinfo["data"]["email"].ToString();
                    GlobalVar.tel = userinfo["data"]["tel"].ToString();
                    GlobalVar.group = userinfo["data"]["group"].ToString();
                    GlobalVar.lasttime = userinfo["data"]["lasttime"].ToString();
                    this.Hide();
                    主页面 formmain = new 主页面();
                    formmain.Show();
                }
                catch (Exception ex)
                {
                    GlobalVar.ClearAll();
                    MessageBox.Show("UserAccess：解析返回的userinfo时出现了异常" + ex.Message);
                    return;
                }
            }
            else
            {
                MessageBox.Show("用户名或者密码错误");
                return;
            }
        }

        private void 登录页面_Load(object sender, EventArgs e)
        {
            string k;
            try
            {
                k = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }
            catch
            {
                k = "未知版本";
            }

            label4.Text = "版本号：" + k;
            //tb_username.Text = "zhaoziyi";
            //textBox1.Text = "19880425";
        }

        private string AssemblyFileVersion
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);
                if (attributes.Length == 0)
                    return "";
                return ((AssemblyFileVersionAttribute)attributes[0]).Version;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            注册页面 f = new 注册页面();
            f.Show();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            //回车自动登录
            if (e.KeyChar == (char)13)
            {
                login();
            }   
        }
    }
}
