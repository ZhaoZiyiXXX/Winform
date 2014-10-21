using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using YouGe;

namespace 悠歌网络合作商家管理系统
{
    public partial class 主页面 : Form
    {
        public 主页面()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            新书入库 formmain = new 新书入库();
            formmain.Show();
        }

        private void 主页面_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void 主页面_Load(object sender, EventArgs e)
        {
            YouGeWinformApi ygw = new YouGeWinformApi();
            label2.Text = ygw.GetLocalShopName();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            二手书入库 f = new 二手书入库();
            f.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            零售出库 f = new 零售出库();
            f.Show();
        }
    }
}
