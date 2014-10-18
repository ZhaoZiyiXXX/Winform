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
                MessageBox.Show("激活失败！errorcode : 1");
                return;
            }
            else
            {
                progressBar1.Value = 40;
                try
                {
                    //创建商户信息表
                    string sql = "CREATE TABLE `yg_local_shopinfo` (`shopid` VARCHAR( 24 ) NOT NULL primary key," +
                    "`shopname` VARCHAR( 30 ) NOT NULL ,`reserve1` VARCHAR( 100 ) DEFAULT NULL ,`reserve2` VARCHAR( 100 ) " +
                    "DEFAULT  NULL) ENGINE = MYISAM CHARACTER SET utf8 COLLATE utf8_general_ci; ";
                    DBOperation dbo = new DBOperation();
                    if (0 != dbo.AddDelUpdate(sql))
                    {
                        MessageBox.Show("激活失败！errorcode : 3");
                        return;
                    }

                    //写入商户信息
                    progressBar1.Value = 50;
                    sql = string.Format( "INSERT INTO yg_local_shopinfo (shopid,shopname) VALUES ('{0}','{1}')",
                        Shopinfo["data"]["id"].ToString(), Shopinfo["data"]["realname"].ToString());
                    if (1 != dbo.AddDelUpdate(sql))
                    {
                        MessageBox.Show("激活失败！errorcode : 2");
                        return;

                    }


                    //创建图书信息表
                    progressBar1.Value = 60;
                    //创建进货渠道表
                    progressBar1.Value = 70;
                    //创建订单信息表
                    progressBar1.Value = 80;
                    //创建订单详情表
                    progressBar1.Value = 90;
                    //创建根据ID获取订单详情的视图VIEW

                    
                    this.Hide();
                    主页面 formmain = new 主页面();
                    formmain.Show();

                }
                catch(Exception ex)
                {
                    MessageBox.Show("激活失败！errorcode : " + ex.Message );
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
