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

        private void button11_Click(object sender, EventArgs e)
        {
            ProcessStartInfo info = new ProcessStartInfo();
            string path = System.Windows.Forms.Application.StartupPath + "/DBSyncServer.exe";
            if (File.Exists(path))
            {
                info.FileName = path; // 要启动的程序
                info.WindowStyle = ProcessWindowStyle.Normal  ;   //隐藏窗口
                Process pro = Process.Start(info); //启动程序
            }
            else
            {
                MessageBox.Show("未找到可以启动的应用程序");
            }
        }

        private void 主页面_Load(object sender, EventArgs e)
        {

        }
    }
}
