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
    public partial class 零售出库 : Form
    {
        YouGeWinformApi ygw = new YouGeWinformApi();
        public 零售出库()
        {
            InitializeComponent();
            initNewBookTable();
            BindingDataTable();
        }

        DataTable g_dt_n = new DataTable();
        DataTable g_dt_o = new DataTable();

        private void initNewBookTable()
        {
            g_dt_n.Columns.Add("bookid", System.Type.GetType("System.String"));
            g_dt_n.Columns.Add("name", System.Type.GetType("System.String"));
            g_dt_n.Columns.Add("press", System.Type.GetType("System.String"));
            g_dt_n.Columns.Add("price", System.Type.GetType("System.String"));
            g_dt_n.Columns.Add("type", System.Type.GetType("System.String"));
            g_dt_n.Columns.Add("count", System.Type.GetType("System.String"));
            g_dt_n.Columns.Add("off", System.Type.GetType("System.String"));
            g_dt_n.Columns.Add("totalprice", System.Type.GetType("System.String"));
        }

        private void BindingDataTable()
        {
            DataTable g_dt = new DataTable();
            //Add columns for DataTable
            g_dt.Columns.Add("name", System.Type.GetType("System.String"));
            g_dt.Columns.Add("press", System.Type.GetType("System.String"));
            g_dt.Columns.Add("price", System.Type.GetType("System.String"));
            g_dt.Columns.Add("type", System.Type.GetType("System.String"));
            g_dt.Columns.Add("count", System.Type.GetType("System.String"));
            g_dt.Columns.Add("off", System.Type.GetType("System.String"));
            g_dt.Columns.Add("totalprice", System.Type.GetType("System.String"));
            int i = 0;
            //绑定二手书数据到DataGridView
            for (i = 0; i < g_dt_o.Rows.Count;i++ )
            {
                DataRow dr = g_dt.NewRow();
                dr["name"] = g_dt_o.Rows[i]["name"].ToString();
                dr["press"] = g_dt_o.Rows[i]["press"].ToString();
                dr["price"] = g_dt_o.Rows[i]["price"].ToString();
                dr["type"] = "二手";
                dr["count"] = "1";
                dr["off"] = "0.5";
                dr["totalprice"] = (Convert.ToDouble(g_dt_o.Rows[i]["price"].ToString()) * 0.5).ToString();
                g_dt.Rows.Add(dr);
            }

            //绑定新书数据到DataGridView
            DataTable g_dt_n_tmp = new DataTable();
            g_dt_n_tmp = g_dt_n.Copy();
            g_dt_n_tmp.Columns.Remove("bookid");
            g_dt.Merge(g_dt_n_tmp);

            //显示
            dataGridView1.DataSource = g_dt.DefaultView;
        }



        private void button1_Click(object sender, EventArgs e)
        {
            sellbook();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            //回车自动搜索
            if (e.KeyChar == (char)13)
            {
                sellbook();
            }
        }

        private void sellbook()
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("请输入条码后查询");
                textBox1.Focus();
                return;
            }

            if (0 == string.Compare("YG", textBox1.Text.Substring(0, 2), true))
            {
                //二手书
                //先判断是否已经添加了相同的条码
                for (int i = 0; i < g_dt_o.Rows.Count; i++)
                {
                    if(g_dt_o.Rows[i]["id"].ToString() == textBox1.Text){
                        MessageBox.Show("已经添加了该出售信息，无法多次添加");
                        return;
                    }
                }
                DataTable dt = ygw.GetOldBookInfoById(textBox1.Text);
                if (dt.Rows.Count != 1)
                {
                    MessageBox.Show(string.Format( "查找二手书交易信息出错！共查询到【{0}】条交易信息",dt.Rows.Count.ToString()));
                    return;
                }
                else
                {
                    if (g_dt_o.Rows.Count == 0)
                    {
                        g_dt_o = dt.Copy();
                        g_dt_o.Rows.Clear();
                    }
                    g_dt_o.ImportRow(dt.Rows[0]);
                }
            }
            else
            {
                //TODO:新书
                //先判断是否已经添加了相同的条码
                
                DataTable dt = ygw.SearchBookinfoByIsbn(textBox1.Text);
                if (dt.Rows.Count > 1)
                {
                    图书选择 f = new 图书选择(dt);
                    f.Show();
                }
                else if (dt.Rows.Count == 1)
                {
                    //一个
                    for (int i = 0; i < g_dt_n.Rows.Count; i++)
                    {
                        if (g_dt_n.Rows[i]["bookid"].ToString() == dt.Rows[0]["id"].ToString())
                        {
                            MessageBox.Show("已经添加了该出售信息，无法多次添加");
                            return;
                        }
                    }
                    DataRow dr = g_dt_n.NewRow();
                    dr["bookid"] = dt.Rows[0]["id"].ToString();
                    dr["name"] = dt.Rows[0]["name"].ToString();
                    dr["press"] = dt.Rows[0]["press"].ToString();
                    dr["price"] = dt.Rows[0]["price"].ToString();
                    dr["type"] = "新书";
                    dr["count"] = "1";
                    dr["off"] = "0.8";
                    dr["totalprice"] =( Convert.ToDouble(dt.Rows[0]["price"].ToString())*0.8).ToString();
                    g_dt_n.Rows.Add(dr);
                }
                else
                {
                    MessageBox.Show("没有查询到ISBN为【" + textBox1.Text + "】的图书");
                }
            }
            BindingDataTable();
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            double totalprice = 0.0;
            //计算总和
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                totalprice += Convert.ToDouble(dataGridView1.Rows[i].Cells[6].Value.ToString());
            }

            //显示
            label4.Text = totalprice.ToString();
        }

        private void label4_TextChanged(object sender, EventArgs e)
        {
            textBox2.Text = label4.Text;
        }

    }
}
