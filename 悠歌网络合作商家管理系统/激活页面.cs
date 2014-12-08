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
using MySql.Data;

namespace 悠歌网络合作商家管理系统
{
    public partial class 登录页面 : Form
    {
        YouGeWebApi yg = new YouGeWebApi();

        YouGeWinformApi ygw = new YouGeWinformApi();
        public 登录页面()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tb_username.Text ))
            {
                MessageBox.Show("请输入激活码");
                tb_username.Focus();
                return;
            }
            
            JObject Shopinfo;
            if (false == yg.ActivateNewShop(tb_username.Text, out Shopinfo))
            {
                MessageBox.Show("激活失败！");
                return;
            }
            else
            {
                progressBar1.Value = 30;
                YouGeWinformApi.Shopinfo si = new YouGeWinformApi.Shopinfo();
                //初始化必须参数赋值
                si.shopId = Shopinfo["data"]["id"].ToString();
                si.shopRealname = Shopinfo["data"]["realname"].ToString();
                if (ygw.InitDatabase(si))
                {
                    MyOperation.DebugPrint("激活成功，创建数据表完成");
                    this.Hide();
                    主页面 formmain = new 主页面();
                    formmain.Show();
                }
                else
                {
                    MessageBox.Show("激活失败！");
                    return;
                }
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
        }
    }
}
