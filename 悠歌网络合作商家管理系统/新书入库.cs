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
    public partial class 新书入库 : Form
    {
        DBOperation dbo = new DBOperation();
        DataTable gdt = new DataTable();//全局变量，存储图书订单信息
        
        public 新书入库()
        {
            InitializeComponent();
            InitDataTable();
            dataGridView2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void InitDataTable(){
            //Add columns for DataTable
            gdt.Columns.Add("id", System.Type.GetType("System.Int32"));
            gdt.Columns.Add("name", System.Type.GetType("System.String"));
            gdt.Columns.Add("press", System.Type.GetType("System.String"));
            gdt.Columns.Add("isbn", System.Type.GetType("System.String"));
            gdt.Columns.Add("price", System.Type.GetType("System.String"));
            gdt.Columns.Add("count", System.Type.GetType("System.Int32"));
            dataGridView2.DataSource = gdt.DefaultView;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SearchByIsbn();
        }

        private void SearchByIsbn()
        {
            string sql = string.Format("SELECT id,name,author,press,price,isbn FROM yg_bookinfo WHERE ISBN = '{0}'", textBox1.Text);
            DataTable dt = dbo.Selectinfo(sql);
            dataGridView1.DataSource = dt.DefaultView;
        }

        private void 新书入库_Load(object sender, EventArgs e)
        {
            label5.Text = MyOperation.GetNow();
            cb_binding();
        }

        private void label7_Click(object sender, EventArgs e)
        {
            新增进货渠道 f1 = new 新增进货渠道();
            f1.MyEvent += new 新增进货渠道.MyDelegate(cb_binding);//监听b窗体事件
            f1.Show();
        }

        private void cb_binding()
        {
            string sql = "SELECT * FROM yg_jinhuoqudao";
            DataTable dt = dbo.Selectinfo(sql);
            comboBox1.Items.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                comboBox1.Items.Add(dt.Rows[i]["name"].ToString());
            }
            comboBox1.SelectedIndex = 0;
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                int index = dataGridView1.CurrentRow.Index; //获取选中行的行号
                label2.Text = "书名：" + dataGridView1.Rows[index].Cells[1].Value.ToString() +
                    "\n\n出版社：" + dataGridView1.Rows[index].Cells[3].Value.ToString() +
                    "\n\n定价：" + dataGridView1.Rows[index].Cells[4].Value.ToString();
            }
            catch
            {
                label2.Text = "选项错误";
            }

        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox3.Text))
            {
                MessageBox.Show("请输入数量！");
                textBox3.Focus();
                return;
            }
            try
            {
                int index = dataGridView1.CurrentRow.Index;
                DataRow dr = gdt.NewRow();
                dr["id"] = Convert.ToInt32(dataGridView1.Rows[index].Cells[0].Value.ToString());
                dr["name"] = dataGridView1.Rows[index].Cells[1].Value.ToString();
                dr["press"] = dataGridView1.Rows[index].Cells[3].Value.ToString();
                dr["isbn"] = dataGridView1.Rows[index].Cells[5].Value.ToString();
                dr["price"] = dataGridView1.Rows[index].Cells[4].Value.ToString();
                dr["count"] = Convert.ToInt32(textBox3.Text);
                gdt.Rows.Add(dr);
                dataGridView2.DataSource = gdt.DefaultView;
            }
            catch(Exception ex)
            {
                MessageBox.Show("添加失败，请重试！" + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                复制新图书信息 f = new 复制新图书信息();
                int index = dataGridView1.CurrentRow.Index;
                f.SetBookid(Convert.ToInt32(dataGridView1.Rows[index].Cells[0].Value.ToString()));
                f.MyEvent += new 复制新图书信息.MyDelegate(SearchByIsbn);//监听b窗体事件
                f.Show();
            }
            catch(Exception ex)
            {
                MessageBox.Show("打开新窗体失败！" + ex.Message);
            }

        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            //数字0～9所对应的keychar为48～57，小数点是46，Backspace是8，小数点是46。
            if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8)
                e.Handled = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }
    }
}
