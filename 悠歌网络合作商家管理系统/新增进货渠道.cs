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
    public partial class 新增进货渠道 : Form
    {
        DBOperation dbo = new DBOperation();
        public 新增进货渠道()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("请输入渠道名称");
                textBox2.Focus();
                return;
            }

            string sql = "INSERT INTO yg_jinhuoqudao (id,name) VALUES (null,'"+textBox2.Text +"')";
            dbo.AddDelUpdate(sql);
            if (MyEvent != null)
                MyEvent();//引发事件
            this.Dispose();
        }

        //定义委托
        public delegate void MyDelegate();
        //定义事件
        public event MyDelegate MyEvent;

        private void 新增进货渠道_Load(object sender, EventArgs e)
        {

        }
  
    }
}
