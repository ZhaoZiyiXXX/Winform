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
        YouGeWebApi yg = new YouGeWebApi();
        MyOperation mo = new MyOperation();
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

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                int index = dataGridView1.CurrentRow.Index; //获取选中行的行号
                label4.Text = "书名：" + dataGridView1.Rows[index].Cells[1].Value.ToString() +
                    "\n\n出版社：" + dataGridView1.Rows[index].Cells[3].Value.ToString() +
                    "\n\n定价：" + dataGridView1.Rows[index].Cells[4].Value.ToString();
                textBox3.Text = (Convert.ToDouble(dataGridView1.Rows[index].Cells[4].Value.ToString()) * 0.45).ToString();
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
            if (ygw.InsertOldBookInfo(dataGridView1.Rows[index].Cells[0].Value.ToString(), textBox2.Text))
            {
                MessageBox.Show("添加成功");
                textBox2.Text = "";
            }
            else
            {
                MessageBox.Show("添加失败");
                return;
            }

            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("sellinfoid", textBox2.Text);
            parameters.Add("price", textBox3.Text);
            backgroundWorker1.RunWorkerAsync(parameters);
        }

        public class ThreadWithState
        {
            private string _price;//售价
            private string _local_book_id;//本地图书ID

            public ThreadWithState(string price,string local_book_id)
            {
                _local_book_id = local_book_id;
                _price = price;
            }

            public void ThreadProc()
            {
                DBOperation dbo = new DBOperation();
                YouGeWebApi yg = new YouGeWebApi();
                YouGeWinformApi ygw = new YouGeWinformApi();
                if (string.IsNullOrEmpty(_local_book_id))
                {
                    throw new Exception("local_book_id is null!");
                }
                逻辑待梳理
                string sql = string.Format("SELECT s.gbookid,s.lbookid FROM oldbooksellinfo AS s WHERE s.lbookid = '{0}'AND s.mallid IS NOT NULL ",
                    _local_book_id);
                DataTable dt = dbo.Selectinfo(sql);
                if (dt.Rows.Count > 0)
                {
                    //说明已经有相同图书ID的交易信息，直接把mallid同步过来
                    MyOperation.DebugPrint("_sellinfoid is Error!", 3);
                    throw new Exception("_sellinfoid is Error!");
                }
                sql = string.Format("");
                IDictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("book_id", dt.Rows[0]["gbookid"].ToString());
                parameters.Add("seller_id", ygw.GetLocalShopId());
                parameters.Add("price", _price);
                string gsellinfoid;//喵校园交易ID
                if (yg.InsertNewSellInfo(parameters, out gsellinfoid))
                {
                    sql = string.Format("UPDATE yg_oldbookdetail SET mallid = '{0}' WHERE guid = '{1}' ", gsellinfoid, _sellinfoid);
                    dbo.AddDelUpdate(sql);
                }
                else
                {
                    MyOperation.DebugPrint("Insert Error!", 3);
                    throw new Exception("Insert Error!");
                }
            }
        }
    }
}
