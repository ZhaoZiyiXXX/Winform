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
            comboBox1.Visible = false;
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
                g_dt_o.Rows[i]["off"] = (string.IsNullOrEmpty(g_dt_o.Rows[i]["off"].ToString()) ? "0.5" : g_dt_o.Rows[i]["off"].ToString());
                dr["off"] = g_dt_o.Rows[i]["off"].ToString();
                g_dt_o.Rows[i]["totalprice"] = (string.IsNullOrEmpty(g_dt_o.Rows[i]["totalprice"].ToString()) ?
                    (Convert.ToDouble(g_dt_o.Rows[i]["price"].ToString()) * Convert.ToDouble(dr["off"].ToString())).ToString() :
                    g_dt_o.Rows[i]["totalprice"].ToString());
                dr["totalprice"] = g_dt_o.Rows[i]["totalprice"].ToString();
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
        public void HandleChoseResult(string bookid)
        {
            DataTable dt = ygw.GetBookinfoByBookid(bookid);
            DataRow dr = g_dt_n.NewRow();
            dr["bookid"] = dt.Rows[0]["id"].ToString();
            dr["name"] = dt.Rows[0]["name"].ToString();
            dr["press"] = dt.Rows[0]["press"].ToString();
            dr["price"] = dt.Rows[0]["price"].ToString();
            dr["type"] = "新书";
            dr["count"] = "1";
            dr["off"] = "0.8";
            dr["totalprice"] = (Convert.ToDouble(dt.Rows[0]["price"].ToString()) * 0.8).ToString();
            g_dt_n.Rows.Add(dr);
            BindingDataTable();
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
                    dt.Columns.Add("off", System.Type.GetType("System.String"));
                    dt.Columns.Add("totalprice", System.Type.GetType("System.String"));
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
                //先判断是否已经添加了相同的条码
                DataTable dt = ygw.SearchBookinfoByIsbn(textBox1.Text);
                if (dt.Rows.Count > 1)
                {
                    图书选择 f = new 图书选择(dt);
                    f.MyEvent += new 图书选择.MyDelegate(HandleChoseResult);//监听b窗体事件
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
             CalcTotalPrice();
        }

        private void CalcTotalPrice()
        {
            double totalprice = 0.0;
            //计算总和
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                totalprice += Convert.ToDouble(dataGridView1.Rows[i].Cells[6].Value.ToString());
            }
            label4.Text = totalprice.ToString();
        }

        private void label4_TextChanged(object sender, EventArgs e)
        {
            textBox2.Text = label4.Text;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            int i = 0;
            for(i = 0;i<g_dt_n.Rows.Count;i++)
            {
                if(!ygw.SoldNewBook(g_dt_n.Rows[i]["bookid"].ToString(),Convert.ToInt32(g_dt_n.Rows[i]["count"].ToString())))
                {
                    MessageBox.Show("零售订单处理失败，请联系管理员查看");
                    return;
                }
            }

            for (i = 0; i < g_dt_o.Rows.Count; i++)
            {
                if (!ygw.SoldOldBook(g_dt_o.Rows[i]["id"].ToString()))
                {
                    MessageBox.Show("零售订单处理失败，请联系管理员查看");
                    return;
                }
            }

            MessageBox.Show("零售订单处理完成");
            g_dt_n.Rows.Clear();
            g_dt_o.Rows.Clear();
            textBox3.Text = textBox4.Text = textBox2.Text = label4.Text = "";
            label5.Text = "书名";
            label6.Text = "出版社";
            label7.Text = "数量";
            BindingDataTable();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                int index = dataGridView1.CurrentRow.Index;
                label5.Text = "书名:" + dataGridView1.Rows[index].Cells[0].Value.ToString();
                label6.Text = "出版社:" + dataGridView1.Rows[index].Cells[1].Value.ToString();
                label7.Text = "定价:" + dataGridView1.Rows[index].Cells[2].Value.ToString();
                textBox3.Text = dataGridView1.Rows[index].Cells[4].Value.ToString();
                textBox4.Text = dataGridView1.Rows[index].Cells[5].Value.ToString();
                if ("新书" == dataGridView1.Rows[index].Cells[3].Value.ToString())
                {
                    textBox3.Enabled = true;
                    comboBox1.Visible = false;
                }
                else
                {
                    textBox3.Enabled = false;
                    comboBox1.Visible = true;
                    comboBox1.Items.Clear();
                    DataRow[] dr = g_dt_o.Select(string.Format("name = '{0}' AND press = '{1}' AND price = '{2}'",
                        dataGridView1.Rows[index].Cells[0].Value.ToString(),
                        dataGridView1.Rows[index].Cells[1].Value.ToString(),
                        dataGridView1.Rows[index].Cells[2].Value.ToString()));
                    for (int i = 0; i < dr.Length; i++)
                    {
                        comboBox1.Items.Add(dr[i]["id"].ToString());
                    }
                    if (dr.Length > 0)
                    {
                        comboBox1.SelectedIndex = 0;
                    }
                        
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("出现异常，请联系管理员处理！" + ex.Message);
                return;
            } 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                int index = dataGridView1.CurrentRow.Index;
                if ("新书" == dataGridView1.Rows[index].Cells[3].Value.ToString())
                {
                    DataRow[] dr = g_dt_n.Select(string.Format("name = '{0}' AND press = '{1}' AND price = '{2}' AND count = '{3}'",
                        dataGridView1.Rows[index].Cells[0].Value.ToString(), 
                        dataGridView1.Rows[index].Cells[1].Value.ToString(),
                        dataGridView1.Rows[index].Cells[2].Value.ToString(), 
                        dataGridView1.Rows[index].Cells[4].Value.ToString()));
                    if (dr.Length  >= 1)
                    {
                        dr[0]["count"] = textBox3.Text;
                        dr[0]["off"] = textBox4.Text;
                        dr[0]["totalprice"] = (Convert.ToDouble(dr[0]["off"].ToString())
                             * Convert.ToInt32(dr[0]["count"].ToString())
                            * Convert.ToDouble(dr[0]["price"].ToString())).ToString();
                    }
                }
                else if ("二手" == dataGridView1.Rows[index].Cells[3].Value.ToString())
                {
                    DataRow[] dr = g_dt_o.Select(string.Format("name = '{0}' AND press = '{1}' AND price = '{2}'",
                        dataGridView1.Rows[index].Cells[0].Value.ToString(),
                        dataGridView1.Rows[index].Cells[1].Value.ToString(),
                        dataGridView1.Rows[index].Cells[2].Value.ToString()));
                    if (dr.Length == 1)
                    {
                        dr[0]["off"] = textBox4.Text;
                        dr[0]["totalprice"] = (Convert.ToDouble(dr[0]["off"].ToString())
                            * Convert.ToDouble(dr[0]["price"].ToString())).ToString();
                    }
                    else if (dr.Length >= 1)
                    {
                        //多个二手的情况下让用户二次确认
                        if(DialogResult.Yes  == MessageBox.Show("您的订单中存在多本相同信息的二手书，请确认是否已经选择了对应的唯一编码","二次确认",MessageBoxButtons.YesNo))
                        {
                            dr = g_dt_o.Select(string.Format("name = '{0}' AND press = '{1}' AND price = '{2}' AND id = '{3}'",
                                dataGridView1.Rows[index].Cells[0].Value.ToString(),
                                dataGridView1.Rows[index].Cells[1].Value.ToString(),
                                dataGridView1.Rows[index].Cells[2].Value.ToString(),
                                comboBox1.SelectedItem.ToString()));
                            if (dr.Length != 1)
                            {
                                MessageBox.Show("选择的ID和图书信息不对应，请仔细确认信息");
                            }
                            else 
                            {
                                dr[0]["off"] = textBox4.Text;
                                dr[0]["totalprice"] = (Convert.ToDouble(dr[0]["off"].ToString())
                                    * Convert.ToDouble(dr[0]["price"].ToString())).ToString();
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("修改图书零售信息时出现系统异常，数组个数：" + dr.Length.ToString());
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("零售出库时无法判断图书类别");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("尚未选择要修改的交易信息！" + ex.Message);
                return;
            }
            finally
            {
                BindingDataTable();
                CalcTotalPrice();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                int index = dataGridView1.CurrentRow.Index;
                if ("新书" == dataGridView1.Rows[index].Cells[3].Value.ToString())
                {
                    DataRow[] dr = g_dt_n.Select(string.Format("name = '{0}' AND press = '{1}' AND price = '{2}' AND count = '{3}'",
                        dataGridView1.Rows[index].Cells[0].Value.ToString(),
                        dataGridView1.Rows[index].Cells[1].Value.ToString(),
                        dataGridView1.Rows[index].Cells[2].Value.ToString(),
                        dataGridView1.Rows[index].Cells[4].Value.ToString()));
                    if (dr.Length >= 1)
                    {
                        g_dt_n.Rows.Remove(dr[0]);
                    }
                    else
                    {
                        MessageBox.Show("删除图书时出现系统异常，数组个数："+dr.Length.ToString());
                        return;
                    }
                }
                else if ("二手" == dataGridView1.Rows[index].Cells[3].Value.ToString())
                {
                    DataRow[] dr = g_dt_o.Select(string.Format("name = '{0}' AND press = '{1}' AND price = '{2}'",
                        dataGridView1.Rows[index].Cells[0].Value.ToString(),
                        dataGridView1.Rows[index].Cells[1].Value.ToString(),
                        dataGridView1.Rows[index].Cells[2].Value.ToString()));
                    if (dr.Length == 1)
                    {
                        g_dt_o.Rows.Remove(dr[0]);
                    }
                    else if(dr.Length > 1)
                    {
                        //多个二手的情况下让用户二次确认
                        if(DialogResult.Yes  == MessageBox.Show("您的订单中存在多本相同信息的二手书，请确认是否已经选择了对应的唯一编码","二次确认",MessageBoxButtons.YesNo))
                        {
                            dr = g_dt_o.Select(string.Format("name = '{0}' AND press = '{1}' AND price = '{2}' AND id = '{3}'",
                                dataGridView1.Rows[index].Cells[0].Value.ToString(),
                                dataGridView1.Rows[index].Cells[1].Value.ToString(),
                                dataGridView1.Rows[index].Cells[2].Value.ToString(),
                                comboBox1.SelectedItem.ToString()));
                            if (dr.Length != 1)
                            {
                                MessageBox.Show("选择的ID和图书信息不对应，请仔细确认信息");
                            }
                            else 
                            {
                                g_dt_o.Rows.Remove(dr[0]);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("删除图书时出现系统异常，数组个数：" + dr.Length.ToString());
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("零售出库时无法判断图书类别");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("尚未选择要修改的交易信息！" + ex.Message);
                return;
            }
            finally
            {
                BindingDataTable();
                CalcTotalPrice();
            }
        }
    }
}
