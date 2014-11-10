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
    public partial class 图书选择 : Form
    {
        private DataTable bookinfo;
        public 图书选择(DataTable bookinfo)
        {
            if (null == bookinfo)
            {
                MessageBox.Show("没有传入正确的窗体参数");
                return;
            }
            this.bookinfo = bookinfo;
            InitializeComponent();
        }

        private void 图书选择_Load(object sender, EventArgs e)
        {
            dataGridView1.DataSource = bookinfo.DefaultView;
        }


    }
}
