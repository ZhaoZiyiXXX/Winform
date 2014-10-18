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
    public partial class 图书信息审核 : Form
    {
        YouGeWebApi yg = new YouGeWebApi();
        public 图书信息审核()
        {
            InitializeComponent();
        }
        
        private void 图书信息审核_Load(object sender, EventArgs e)
        {
            groupBox1.Visible = false;//先不显示历史审核
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            LoadBookInfo();
            if(Convert.ToInt32( GlobalVar.group) < 3){
                button2.Visible = false;
            }
        }
        private string bookid;
        private void LoadBookInfo()
        {
            pictureBox1.ImageLocation = null;
            //button1.Enabled = false;
            JObject bookinfo;
            yg.GetReviewBookInfo(out bookinfo);
            try
            {
                bookid = bookinfo["data"]["id"].ToString();
                tb_bookname.Text = bookinfo["data"]["name"].ToString();
                textBox1.Text = bookinfo["data"]["author"].ToString();
                textBox3.Text = bookinfo["data"]["press"].ToString();
                textBox2.Text = bookinfo["data"]["fixedPrice"].ToString();
                textBox5.Text = bookinfo["data"]["isbn"].ToString();
                textBox4.Text = bookinfo["data"]["imgpath"].ToString();
                pictureBox1.ImageLocation = textBox4.Text;
                Clipboard.SetData(DataFormats.Text, textBox5.Text);
            }
            catch(Exception e)
            {
                bookid = null;
                MessageBox.Show("LoadBookInfo：解析返回的bookinfo时出现了异常"+e.Message );
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tb_bookname.Text))
            {
                MessageBox.Show("请输入书名");
                tb_bookname.Focus();
                return;
            }

            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("请输入作者");
                textBox1.Focus();
                return;
            }

            if (string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("请再次输入出版社");
                textBox2.Focus();
                return;
            }

            if (string.IsNullOrEmpty(textBox3.Text))
            {
                MessageBox.Show("请输入定价");
                textBox3.Focus();
                return;
            }

            if (string.IsNullOrEmpty(textBox4.Text))
            {
                MessageBox.Show("请输入图片路径");
                textBox4.Focus();
                return;
            }

            if (string.IsNullOrEmpty(textBox5.Text))
            {
                MessageBox.Show("请输入ISBN");
                textBox5.Focus();
                return;
            }
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("id", bookid);
            parameters.Add("name", tb_bookname.Text);
            parameters.Add("author", textBox1.Text);
            parameters.Add("press", textBox3.Text);
            parameters.Add("fixedPrice", textBox2.Text);
            parameters.Add("isbn", textBox5.Text);
            parameters.Add("imgpath", textBox4.Text);
            YouGeWebApi yg = new YouGeWebApi();
            if (yg.PostReviewBookInfo(parameters))
            {
                MessageBox.Show("修改成功！");
                LoadBookInfo();
            }
            else
            {
                MessageBox.Show("修改失败！");
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                pictureBox1.ImageLocation = textBox4.Text;
                button1.Enabled = true;
            }
            catch
            {
                MessageBox.Show("非法的图片路径！");
                textBox4.Focus();
                return;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox4.Text = "http://www.mallschool.com/static/images/book/defaultbook.png";
            button1.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            YouGeWebApi yg = new YouGeWebApi();
            if (yg.ResetReviewBookInfo())
            {
                MessageBox.Show("重置成功");
            }
            else
            {
                MessageBox.Show("重置失败");
            }
            
        }
    }
}
