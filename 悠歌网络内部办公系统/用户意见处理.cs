using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using YouGe;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace 悠歌网络内部办公系统
{
    public partial class 用户意见处理 : Form
    {

        DataTable gdt = new DataTable();//全局变量，存储意见列表
        private void InitDataTable()
        {
            //Add columns for DataTable
            gdt.Columns.Add("id", System.Type.GetType("System.Int32"));
            gdt.Columns.Add("content", System.Type.GetType("System.String"));
            gdt.Columns.Add("time", System.Type.GetType("System.String"));
            gdt.Columns.Add("status", System.Type.GetType("System.String"));
            gdt.Columns.Add("staff", System.Type.GetType("System.String"));
            gdt.Columns.Add("stime", System.Type.GetType("System.String"));
            gdt.Columns.Add("mark", System.Type.GetType("System.String"));
            dataGridView1.DataSource = gdt.DefaultView;
        }

        public 用户意见处理()
        {
            InitializeComponent();
        }

        private void 用户意见处理_Load(object sender, EventArgs e)
        {
            InitDataTable();
            LoadYijian();
            textBox1.Enabled = false;
        }

        private void LoadYijian()
        {
            JObject jo;
            YouGeWebApi yg = new YouGeWebApi();
            try
            {
                if (yg.GetYijian(out jo))
                {
                    int i = 0;
                    gdt.Rows.Clear();
                    while (!string.IsNullOrEmpty(jo["data"][i]["id"].ToString()))
                    {
                        DataRow dr = gdt.NewRow();
                        dr["id"] = Convert.ToInt32(jo["data"][i]["id"].ToString());
                        dr["content"] = jo["data"][i]["content"].ToString();
                        dr["time"] = MyOperation.Unix2Datetime(jo["data"][i]["time"].ToString());
                        dr["status"] = jo["data"][i]["status"].ToString();
                        dr["staff"] = jo["data"][i]["staff"].ToString();
                        if (string.IsNullOrEmpty(jo["data"][i]["stime"].ToString()))
                        {
                            dr["stime"] = jo["data"][i]["stime"].ToString();
                        }
                        else
                        {
                            dr["stime"] = MyOperation.Unix2Datetime(jo["data"][i]["stime"].ToString());
                        }
                        dr["mark"] = jo["data"][i]["mark"].ToString();
                        gdt.Rows.Add(dr);
                        i++;
                    }
                }
                else
                {
                    MessageBox.Show("初始化意见列表失败");
                }
            }
            catch (Exception e)
            {
                MyOperation.DebugPrint("LoadYijian出现catch异常："+e.Message );
            }
            finally
            {
                dataGridView1.DataSource = gdt.DefaultView;
            }

        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                int index = dataGridView1.CurrentRow.Index; //获取选中行的行号
                label2.Text = dataGridView1.Rows[index].Cells[2].Value.ToString();
                label4.Text = dataGridView1.Rows[index].Cells[1].Value.ToString();
                if ("未处理" != dataGridView1.Rows[index].Cells[3].Value.ToString())
                {
                    button2.Enabled = false;
                }
                else
                {
                    button2.Enabled = true;
                }
                textBox1.Text = dataGridView1.Rows[index].Cells[6].Value.ToString();
            }
            catch
            {
                label2.Text = label4.Text = "选项错误";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int index = dataGridView1.CurrentRow.Index; //获取选中行的行号
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("id", dataGridView1.Rows[index].Cells[0].Value.ToString());
            parameters.Add("staff", GlobalVar.name );
            parameters.Add("status", "处理中");
            parameters.Add("stime", MyOperation.Datetime2Unix(DateTime.Now.ToString()));
            YouGeWebApi yg = new YouGeWebApi();
            if (yg.UpdateYijian(parameters))
            {
                MessageBox.Show("接单成功！");
                LoadYijian();
            }
            else
            {
                MessageBox.Show("接单失败！");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("备注不能为空！");
                textBox1.Focus();
                return;
            }
            int index = dataGridView1.CurrentRow.Index; //获取选中行的行号
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("id", dataGridView1.Rows[index].Cells[0].Value.ToString());
            parameters.Add("mark", textBox1.Text);
            YouGeWebApi yg = new YouGeWebApi();
            if (yg.UpdateYijian(parameters))
            {
                MessageBox.Show("添加备注成功！");
                LoadYijian();
            }
            else
            {
                MessageBox.Show("添加备注失败！");
            }
            textBox1.Enabled = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int index = dataGridView1.CurrentRow.Index; //获取选中行的行号
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("id", dataGridView1.Rows[index].Cells[0].Value.ToString());
            parameters.Add("status", "已完成");
            YouGeWebApi yg = new YouGeWebApi();
            if (yg.UpdateYijian(parameters))
            {
                MessageBox.Show("操作成功！");
                LoadYijian();
            }
            else
            {
                MessageBox.Show("操作失败！");
            }
            textBox1.Enabled = false;
        }
    }
}
