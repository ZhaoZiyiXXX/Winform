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
    public partial class 复制新图书信息 : Form
    {
        private int bookid;
        DBOperation dbo = new DBOperation();
        YouGeWinformApi ygw = new YouGeWinformApi();
        public 复制新图书信息()
        {
            InitializeComponent();
        }

        private void 复制新图书信息_Load(object sender, EventArgs e)
        {
            try
            {
                if(bookid<=0)
                {
                    MessageBox.Show("系统参数异常，请重新选择！");
                    this.Dispose();
                }

                DataTable dt = ygw.getBookinfoByBookid(bookid.ToString());
                if (dt.Rows.Count != 1)
                {
                    MessageBox.Show("系统参数异常，请重新选择！");
                    this.Dispose();
                }
                else
                {
                    label9.Text = dt.Rows[0]["name"].ToString();
                    label10.Text = dt.Rows[0]["author"].ToString();
                    label8.Text = dt.Rows[0]["press"].ToString();
                    label7.Text = dt.Rows[0]["isbn"].ToString();
                    textBox1.Text = dt.Rows[0]["price"].ToString();
                }
            }
            catch
            {
                this.Dispose();
            }
        }

        public void SetBookid(int id)
        {
            this.bookid = id;
        }

        //定义委托
        public delegate void MyDelegate();
        //定义事件
        public event MyDelegate MyEvent;

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            //数字0～9所对应的keychar为48～57，小数点是46，Backspace是8，小数点是46。
            if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                e.Handled = true;

            //小数点的处理。
            if ((int)e.KeyChar == 46)                           //小数点
            {
                if (textBox1.Text.Length <= 0)
                    e.Handled = true;   //小数点不能在第一位
                else
                {
                    float f;
                    float oldf;
                    bool b1 = false, b2 = false;
                    b1 = float.TryParse(textBox1.Text, out oldf);
                    b2 = float.TryParse(textBox1.Text + e.KeyChar.ToString(), out f);
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
    }
}
