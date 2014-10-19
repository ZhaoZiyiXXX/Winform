using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using YouGe;

namespace 悠歌网络合作商家管理系统
{
    public partial class 二手书入库 : Form
    {
        DBOperation dbo = new DBOperation();
        YouGeWinformApi ygw = new YouGeWinformApi();
        public 二手书入库()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            SearchByIsbn();
        }

        private void SearchByIsbn()
        {
            DataTable dt = ygw.SearchBookinfoByIsbn(textBox1.Text);
            dataGridView1.DataSource = dt.DefaultView;
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                int index = dataGridView1.CurrentRow.Index; //获取选中行的行号
                label4.Text = "书名：" + dataGridView1.Rows[index].Cells[1].Value.ToString() +
                    "\n\n出版社：" + dataGridView1.Rows[index].Cells[3].Value.ToString() +
                    "\n\n定价：" + dataGridView1.Rows[index].Cells[4].Value.ToString();
            }
            catch
            {
                label4.Text = "选项错误";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("唯一编码不能为空");
                return;
            }
            int index = dataGridView1.CurrentRow.Index; //获取选中行的行号
            if (ygw.insertOldBookInfo(dataGridView1.Rows[index].Cells[0].Value.ToString(), textBox2.Text))
            {
                MessageBox.Show("添加成功");
                textBox2.Text = "";
            }
            else
            {
                MessageBox.Show("添加失败");
            }
            
        }
    }
}
