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
    public partial class 新书入库 : Form
    {
        DBOperation dbo = new DBOperation();
        DataTable gdt = new DataTable();//全局变量，存储图书订单信息
        YouGeWinformApi ygw = new YouGeWinformApi();
        MyOperation mo = new MyOperation();
        public 新书入库()
        {
            InitializeComponent();
            InitDataTable();
            dataGridView2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView2.ClearSelection();
        }

        private void InitDataTable(){
            //Add columns for DataTable
            gdt.Columns.Add("id", System.Type.GetType("System.Int32"));
            gdt.Columns.Add("name", System.Type.GetType("System.String"));
            gdt.Columns.Add("press", System.Type.GetType("System.String"));
            gdt.Columns.Add("isbn", System.Type.GetType("System.String"));
            gdt.Columns.Add("price", System.Type.GetType("System.String"));
            gdt.Columns.Add("count", System.Type.GetType("System.Int32"));
            gdt.Columns.Add("off", System.Type.GetType("System.Double"));
            dataGridView2.DataSource = gdt.DefaultView;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SearchByIsbn();
        }

        private void SearchByIsbn()
        {
            DataTable dt = mo.SearchBookinfoByIsbn(textBox1.Text);
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("没有查询到你要的书籍，请手动录入信息后重新搜索");
                新增图书信息 f = new 新增图书信息();
                f.Show();
                return;
            }
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
            DataTable dt = ygw.GetAllJinhuoqudao();
            comboBox1.DisplayMember = "name";
            comboBox1.ValueMember = "id";
            comboBox1.DataSource = dt;
            comboBox1.SelectedIndex = -1;
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

            if (string.IsNullOrEmpty(textBox5.Text))
            {
                MessageBox.Show("请输入折扣！");
                textBox5.Focus();
                return;
            }

            if (Convert.ToDouble(textBox5.Text) < 0 | Convert.ToDouble(textBox5.Text) > 1)
            {
                MessageBox.Show("折扣范围仅允许0-1之间！");
                textBox5.Focus();
                return;
            }

            try
            {
                int index = dataGridView1.CurrentRow.Index;
                for (int i = 0; i < dataGridView2.RowCount;i++ )
                {
                    if (dataGridView1.Rows[index].Cells[0].Value.ToString() == dataGridView2.Rows[i].Cells[0].Value.ToString())
                    {
                        MessageBox.Show("已经添加了该品种！");
                        dataGridView2.ClearSelection();
                        dataGridView2.Rows[i].Selected = true;
                        return;
                    }
                }
                DataRow dr = gdt.NewRow();
                dr["id"] = Convert.ToInt32(dataGridView1.Rows[index].Cells[0].Value.ToString());
                dr["name"] = dataGridView1.Rows[index].Cells[1].Value.ToString();
                dr["press"] = dataGridView1.Rows[index].Cells[3].Value.ToString();
                dr["isbn"] = dataGridView1.Rows[index].Cells[5].Value.ToString();
                dr["price"] = dataGridView1.Rows[index].Cells[4].Value.ToString();
                dr["count"] = Convert.ToInt32(textBox3.Text);
                dr["off"] = Convert.ToDouble(textBox5.Text );
                gdt.Rows.Add(dr);
                dataGridView2.DataSource = gdt.DefaultView;
                dataGridView2.ClearSelection();
            }
            catch(Exception ex)
            {
                MessageBox.Show("添加失败，请重试！" + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.DisplayedRowCount(true) == 0)
            {
                MessageBox.Show("请先选择有效的图书信息");
                return;
            }
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
            YouGeWinformApi.OrderInfo oi = new YouGeWinformApi.OrderInfo();
            oi.datetime = label5.Text;
            oi.jinhuoqudao = comboBox1.SelectedValue.ToString();
            oi.ordername = textBox2.Text;
            oi.totalprice = textBox4.Text;

            if (!ygw.InsertNewOrder(oi))
            {
                MessageBox.Show("订单生成过程中出现了错误1!");
                return;
            }

            string orderid = ygw.GetOrderIdByCreateTime(label5.Text);
            if (null == orderid)
            {
                MessageBox.Show("订单生成过程中出现了错误2!");
                return;
            }

            for (int i = 0; i < gdt.Rows.Count; i++)
            {
                YouGeWinformApi.OrderDetail od = new YouGeWinformApi.OrderDetail();
                od.off = gdt.Rows[i]["off"].ToString();
                od.orderid = orderid;
                od.count = gdt.Rows[i]["count"].ToString();
                od.bookid = gdt.Rows[i]["id"].ToString();
                if (!ygw.InsertOrderDetail(od))
                {
                    MessageBox.Show("订单生成过程中出现了错误！");
                    return;
                }
            }
            MessageBox.Show("订单添加成功！");
            gdt.Rows.Clear();
            dataGridView2.DataSource = gdt.DefaultView;
            textBox5.Text = textBox4.Text = textBox3.Text = textBox2.Text = "";
            label5.Text = MyOperation.GetNow();
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            //数字0～9所对应的keychar为48～57，小数点是46，Backspace是8，小数点是46。
            if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                e.Handled = true;

            //小数点的处理。
            if ((int)e.KeyChar == 46)                           //小数点
            {
                if (textBox4.Text.Length <= 0)
                    e.Handled = true;   //小数点不能在第一位
                else
                {
                    float f;
                    float oldf;
                    bool b1 = false, b2 = false;
                    b1 = float.TryParse(textBox4.Text, out oldf);
                    b2 = float.TryParse(textBox4.Text + e.KeyChar.ToString(), out f);
                    if (b2 == false)
                    {
                        if (b1 == true)
                            e.Handled = true;
                        else
                            e.Handled = false;
                    }
                }
            }
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            //数字0～9所对应的keychar为48～57，小数点是46，Backspace是8，小数点是46。
            if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                e.Handled = true;

            //小数点的处理。
            if ((int)e.KeyChar == 46)                           //小数点
            {
                float f;
                float oldf;
                bool b1 = false, b2 = false;
                b1 = float.TryParse(textBox5.Text, out oldf);
                b2 = float.TryParse(textBox5.Text + e.KeyChar.ToString(), out f);
                if (b2 == false)
                {
                    if (b1 == true)
                        e.Handled = true;
                    else
                        e.Handled = false;
                }
            }
        }

        private void dataGridView2_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            double  totalprice = 0.0;
            for (int i = 0; i < gdt.Rows.Count; i++)
            {
                totalprice += Convert.ToDouble(gdt.Rows[i]["off"].ToString()) * Convert.ToDouble(gdt.Rows[i]["price"].ToString()) *
                    Convert.ToInt32(gdt.Rows[i]["count"].ToString());
            }
            textBox4.Text = totalprice.ToString();
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            textBox1.SelectAll();
        }

        private void dataGridView2_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            
            try
            {
                int index = dataGridView2.CurrentRow.Index;
                string bookname = dataGridView2.Rows[index].Cells[1].Value.ToString();
                string count = dataGridView2.Rows[index].Cells[6].Value.ToString();
                string off = dataGridView2.Rows[index].Cells[5].Value.ToString();
                修改新书入库折扣和数量 f = new 修改新书入库折扣和数量(bookname,count,off);
                f.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("打开新窗体失败！" + ex.Message);
                return;
            }
        }

        public static void ChangeCountAndOff(string bookid,string newcount,string newoff)
        {
            
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
