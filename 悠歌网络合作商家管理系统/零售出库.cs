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
            BindingDataTable();
        }

        DataTable g_dt_n = new DataTable();
        DataTable g_dt_o = new DataTable();

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
            dataGridView1.DataSource = g_dt.DefaultView;
        }



        private void button1_Click(object sender, EventArgs e)
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
                        g_dt_o =  dt.Copy();
                        g_dt_o.Rows.Clear();
                    }
                    g_dt_o.ImportRow(dt.Rows[0]);
                }
            }
            else
            {
                //新书

            }
            BindingDataTable();
        }
    }
}
