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

namespace 悠歌网络内部办公系统
{
    public partial class 主页面 : Form
    {
        public 主页面()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            图书信息审核 f = new 图书信息审核();
            f.Show();
        }

        private void 主页面_Load(object sender, EventArgs e)
        {
            label4.Text = MyOperation.GetTime(GlobalVar.lasttime);
            label2.Text = GlobalVar.name;
            timer1.Interval = 120*1000;
            timer1.Start();
        }

        private void 主页面_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            YouGeWebApi yg = new YouGeWebApi();
            yg.UpdateUserStatus(GlobalVar.id,GlobalVar.name );
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            YouGeWebApi yg = new YouGeWebApi();
            List<string> username = new List<string>();
            username = yg.GetOnlineUser();
            MyOperation.DebugPrint(username.ToString());
            listBox1.Items.Clear();
            foreach (string n in username)
            {
                listBox1.Items.Add(n);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }
    }
}
