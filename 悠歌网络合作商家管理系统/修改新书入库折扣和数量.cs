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
    public partial class 修改新书入库折扣和数量 : Form
    {
        public 修改新书入库折扣和数量(string bookname,string count,string off)
        {
            InitializeComponent();
            label4.Text = bookname;
            textBox1.Text = count;
            textBox2.Text = off;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        
    }
}
