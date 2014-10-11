using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 悠歌网络合作商家管理系统
{
    public partial class 登录页面 : Form
    {
        public 登录页面()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            主页面 formmain = new 主页面();
            formmain.Show();
        }
    }
}
