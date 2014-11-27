using System;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using YouGe;
using System.Threading;

namespace 淘淘管理系统.book
{
    /// <summary>
    /// NewSellInfo.xaml 的交互逻辑
    /// </summary>
    public partial class NewSellInfo : UserControl
    {
        System.ComponentModel.BackgroundWorker backgroundWorker1;
        DBOperation dbo = new DBOperation();
        MyOperation mo = new MyOperation();
        YouGeWebApi ygw = new YouGeWebApi();
        public NewSellInfo()
        {
            InitializeComponent();
            DisplayAll(2);

        }

        //搜索书主信息按钮
        private void button3_Click(object sender, RoutedEventArgs e)
        {
            lb_sellername.Content = "";
            if (string.IsNullOrEmpty(tb_sellerid.Text ))
            {
                MyOperation.MessageShow("请输入书主号");
                tb_sellerid.Focus();
                return;
            }

            string sql = string.Format("SELECT name FROM tt_sellerinfo WHERE sellerid = '{0}'",tb_sellerid.Text );
            DataTable dt = dbo.Selectinfo(sql);
            if (dt.Rows.Count == 1)
            {
                lb_sellername.Content = dt.Rows[0]["name"].ToString();
                tb_sellinfoid.Text =  get_bookid(tb_sellerid.Text);
            }
            else
            {
                MyOperation.MessageShow("书主号错误");
                tb_sellerid.Focus();
                return;
            }
        }


        private string get_bookid(string selleid)
        {
            string sql = string.Format("SELECT COUNT(id) FROM tt_sellinfo WHERE sellerid = '{0}'", selleid);
            DataTable dt = dbo.Selectinfo(sql);
            int count = int.Parse( dt.Rows[0][0].ToString()) + 1;
            if (count < 10)
                return selleid + "00" + count.ToString();
            else if (count < 100)
                return selleid + "0" + count.ToString();
            else if (count < 1000)
                return selleid + count.ToString();
            else
                MyOperation.MessageShow("用户寄售数字已经超过999，请工作人员处理");
            return "";
        }

        private string get_bookid2(string selleid)
        {
            string sql = string.Format("SELECT MAX(id) FROM tt_sellinfo WHERE sellerid = '{0}'", selleid);
            
            DataTable dt = dbo.Selectinfo(sql);
            if (dt.Rows[0][0].ToString() == "")
            {
                return selleid + "001";
            }

            int count = int.Parse(dt.Rows[0][0].ToString()) + 1;  
            return selleid + count.ToString();

        }

        //搜索ISBN号按钮
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            search_book();
        }

        private void search_book()
        {
            lbb_author.Content = lbb_ISBN.Content = lbb_name.Content = lbb_press.Content = lbb_price.Content = "";
            if (string.IsNullOrEmpty(tb_isbn.Text))
            {
                MyOperation.MessageShow("请输入图书的ISBN号");
                tb_isbn.Focus();
                return;
            }

            string sql = string.Format("SELECT * FROM tt_bookinfo WHERE isbn LIKE '%{0}%'", tb_isbn.Text);
            DataTable dt = dbo.Selectinfo(sql);
            //修改为只看第一个符合ISBN号的图书，这样可以规避ISBN号重复的问题
            if (dt.Rows.Count >= 1)
            {
                bookinfo boi = new bookinfo();
                boi.bookid = dt.Rows[0]["id"].ToString();
                boi.author = dt.Rows[0]["author"].ToString();
                boi.isbn = dt.Rows[0]["isbn"].ToString();
                boi.name = dt.Rows[0]["name"].ToString();
                boi.press = dt.Rows[0]["press"].ToString();
                boi.price = dt.Rows[0]["price"].ToString();
                boi.imgpath = dt.Rows[0]["imgpath"].ToString();
                tb_price.Text = (MyOperation.string2float(dt.Rows[0]["price"].ToString())*0.3).ToString("0.0");
                DisplayBookinfo(boi);
                return;
            }
            //else if (dt.Rows.Count > 1)
            //{
            //    MyOperation.MessageShow("存在2本以上相同ISBN号的图书，请确认ISBN号或者联系系统管理员");
            //    tb_isbn.Focus();
            //    return;
            //}
            else
            {
                string url = string.Format("http://api.jige.olege.com/book?q={0}&type=ISBN", tb_isbn.Text);
                string html = GetHtml(url);
                bookinfo bi;
                if (PraseHtml(html, out bi))
                {
                    bi.bookid = InsertNewBookInfo(bi);
                    if ("" == bi.bookid)
                    {
                        return;
                    }
                    DisplayBookinfo(bi);
                }
            }
        }

        private string InsertNewBookInfo(bookinfo bi)
        {
            if (null == bi)
            {
                return "";
            }

            try
            {
                string sql = string.Format("INSERT INTO tt_bookinfo (id,gbookid,name,author,press,price,ISBN,imgpath) VALUES (null,'{0}','{1}','{2}','{3}','{4}','{5}','{6}')",
                    bi.gbookid,bi.name,bi.author,bi.press,bi.price,bi.isbn,bi.imgpath  );
                if (dbo.AddDelUpdate(sql) == 1)
                {
                    sql = "SELECT id FROM tt_bookinfo WHERE isbn ='" + bi.isbn + "'";
                    DataTable dt = dbo.Selectinfo(sql);
                    return dt.Rows[0]["id"].ToString();
                }
                else
                {
                    MyOperation.MessageShow("同步教材信息至本地时发生错误，请联系管理员处理");
                    return "";
                }
            }
            catch (Exception e)
            {
                MyOperation.DebugPrint("InsertNewBookInfo出现catch异常:" + e.Message, 3);
            }
            return "";
        }
        private void DisplayBookinfo(bookinfo bi)
        {
            if (null == bi)
            {
                return;
            }
            try
            {
                lbb_bookid.Content = "内部编号：" + bi.bookid;
                lbb_author.Content = "作者：" + bi.author;
                lbb_ISBN.Content = "ISBN：" + bi.isbn;
                lbb_name.Content = "书名：" + bi.name;
                lbb_press.Content = "出版社：" + bi.press;
                lbb_price.Content = "定价：" + bi.price;
                //====================标准image source添加方法=====================
                // Create source
                BitmapImage myBitmapImage = new BitmapImage();

                // BitmapImage.UriSource must be in a BeginInit/EndInit block
                myBitmapImage.BeginInit();
                myBitmapImage.UriSource = new Uri(@bi.imgpath , UriKind.Absolute);

                // To save significant application memory, set the DecodePixelWidth or  
                // DecodePixelHeight of the BitmapImage value of the image source to the desired 
                // height or width of the rendered image. If you don't do this, the application will 
                // cache the image as though it were rendered as its normal size rather then just 
                // the size that is displayed.
                // Note: In order to preserve aspect ratio, set DecodePixelWidth
                // or DecodePixelHeight but not both.
                myBitmapImage.DecodePixelWidth = 250;
                myBitmapImage.EndInit();
                //set image source
                bookimage.Source = myBitmapImage;
                DisplayAll(1);
            }
            catch (UriFormatException e)
            {
                MyOperation.DebugPrint("DisplayBookinfo未能解析URL:" + e.Message, 1);
                bookimage.Source = new BitmapImage(new Uri(@"/images/defaultbookimg.jpg", UriKind.RelativeOrAbsolute));
                DisplayAll(1);
            }
            catch (Exception e)
            {
                MyOperation.DebugPrint("DisplayBookinfo出现catch异常:" + e.Message, 3);
                DisplayAll(2);
                MyOperation.MessageShow("系统异常，请联系管理员处理");
            }
        }

        private void DisplayAll(int flag)
        {
            switch (flag)
            {
                case 0:
                    this.sp1.Visibility = Visibility.Collapsed  ;
                    this.sp2.Visibility = Visibility.Visible;
                    tb5.Text = tb_isbn.Text;
                    break;
                case 1:
                    this.sp1.Visibility = Visibility.Visible;
                    this.sp2.Visibility = Visibility.Collapsed;
                    break;
                case 2:
                    this.sp1.Visibility = Visibility.Collapsed;
                    this.sp2.Visibility = Visibility.Collapsed;
                    
                    break;
                default:
                    break;
            }
        }

        private class bookinfo
        {
            public string bookid;//淘淘本地bookid
            public string gbookid;//喵校园全局bookid
            public string name;
            public string author;
            public string imgpath;
            public string press;
            public string price;
            public string isbn;
        }

        private Boolean PraseHtml(string html, out bookinfo bookinfo)
        {
            bookinfo = new bookinfo();
            if ("" == html||"{\"result\":0,\"data\":null}" == html)
            {
                MyOperation.DebugPrint("没有找到对应的图书信息", 1);
                DisplayAll(0);
                return false ;
            }
            try
            {
                JObject jo1 = (JObject)JsonConvert.DeserializeObject(html);
                bookinfo.gbookid = jo1["data"]["id"].ToString();
                bookinfo.name = jo1["data"]["name"].ToString();
                bookinfo.author = jo1["data"]["author"].ToString();
                bookinfo.imgpath = jo1["data"]["imgpath"].ToString();
                bookinfo.press = jo1["data"]["press"].ToString();
                bookinfo.price = jo1["data"]["fixedPrice"].ToString();
                bookinfo.isbn = jo1["data"]["isbn"].ToString();
                DisplayAll(1);
                return true;
            }
            catch (Exception e)
            {
                MyOperation.DebugPrint("PraseHtml:出现catch异常:" + e.Message);
                DisplayAll(0);
                return false;
            }
        }

        public string GetHtml(string URL)
        {
            try
            {
                WebRequest request = WebRequest.Create(URL);
                request.Credentials = CredentialCache.DefaultCredentials;
                WebResponse response = request.GetResponse();
                string htmlContent = new StreamReader(response.GetResponseStream(), Encoding.Default).ReadToEnd();
                return htmlContent;
            }
            catch
            {
                return "";
            }
        }

        //添加出售信息按钮
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tb_isbn.Text))
            {
                MyOperation.MessageShow("请输入图书的ISBN号");
                tb_isbn.Focus();
                return;
            }

            if (string.IsNullOrEmpty(tb_sellerid.Text))
            {
                MyOperation.MessageShow("请输入书主号");
                tb_sellerid.Focus();
                return;
            }

            if (string.IsNullOrEmpty(tb_sellinfoid.Text ))
            {
                MyOperation.MessageShow("请输入图书唯一ID");
                tb_sellinfoid.Focus();
                return;
            }

            if (string.IsNullOrEmpty(tb_price.Text))
            {
                MyOperation.MessageShow("请输入出售价格");
                tb_price.Focus();
                return;
            }

            string sql = string.Format("SELECT id,gbookid FROM tt_bookinfo WHERE ISBN = '{0}'",tb_isbn.Text);
            DataTable dt = dbo.Selectinfo(sql);
            if (dt.Rows.Count == 0)
            {
                MyOperation.MessageShow("未找到该ISBN号的书籍，请首先在上方搜索对应信息");
                MyOperation.DebugPrint("未找到该ISBN号的书籍 " + sql, 3);
                return;
            }

            sql = string.Format("SELECT sellinfoid FROM tt_sellinfo WHERE sellinfoid = '{0}'", tb_sellinfoid.Text);
            DataTable dt1 = dbo.Selectinfo(sql);
            if (dt1.Rows.Count != 0)
            {
                MyOperation.MessageShow("已经存在该图书唯一ID【"+tb_sellinfoid.Text+"】，请更换后重试");
                return;
            }
            sql = string.Format("INSERT INTO tt_sellinfo (sellinfoid,sellerid,bookid,price,buyer,buytime,issold) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}')",
                tb_sellinfoid.Text,tb_sellerid.Text,dt.Rows[0]["id"].ToString(),tb_price.Text,App.login_staffid ,DateTime.Now.ToString() ,"0");
            if (1 == dbo.AddDelUpdate(sql))
            {
                MyOperation.MessageShow("添加成功");
                
            }
            else
            {
                MyOperation.MessageShow("添加失败");
                MyOperation.DebugPrint("添加交易信息时发生错误", 3);
                return;
            }

            //开始同步新的出售信息到喵校园主库
            ThreadWithState tws = new ThreadWithState(
                dt.Rows[0]["gbookid"].ToString(),
                tb_price.Text,
                dt.Rows[0]["id"].ToString()
            );

            Thread t = new Thread(new ThreadStart(tws.ThreadProc));
            t.IsBackground = true;
            t.Start();
            //原来在添加成功的分支中，现在移到线程建立完毕
            tb_sellinfoid.Text = tb_price.Text = "";
            tb_isbn.Focus();
        }

        public class ThreadWithState
        {
            private string _book_id;//喵校园全局ID
            private string _price;//售价
            private string _local_book_id;//淘淘本地图书ID

            public ThreadWithState(string book_id, string price,string local_book_id)
            {
                _local_book_id = local_book_id;
                _book_id = book_id;
                _price = price;
            }

            public void ThreadProc()
            {
                DBOperation dbo = new DBOperation();
                YouGeWebApi ygw = new YouGeWebApi();
                if (string.IsNullOrEmpty(_book_id))
                {
                    throw new Exception("book_id is null!");
                }
                if (string.IsNullOrEmpty(_price))
                {
                    throw new Exception("price is null!");
                }
                //判断本地是否有相同book_id和price的交易记录，如果有就全部同步成一个mallid,不要再上报给喵校园主库
                string sql = string.Format("SELECT tt_bookinfo.id,tt_sellinfo.mallid FROM tt_bookinfo ,tt_sellinfo WHERE tt_bookinfo.id " +
                    "= tt_sellinfo.bookid AND tt_bookinfo.gbookid ='{0}' AND ABS(tt_sellinfo.price- {1}) < 1e-5  AND tt_sellinfo.mallid IS NOT NULL",
                    _book_id, _price);
                DataTable dt = dbo.Selectinfo(sql);
                //如果有，则直接全部更新一次mallid，不用上报给喵校园主库
                if (dt.Rows.Count > 0)
                {
                    sql = string.Format("UPDATE tt_sellinfo SET mallid = '{0}' WHERE bookid = '{1}' AND ABS(price- {2}) < 1e-5", 
                        dt.Rows[0]["mallid"].ToString(), dt.Rows[0]["id"].ToString(), _price);
                    dbo.AddDelUpdate(sql);
                    return;
                }
                //如果没有，则添加到喵校园主库，返回交易ID后，同步到每一条符合条件的交易中
                string sellinfoid;//喵校园交易ID
                IDictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("book_id", _book_id);
                parameters.Add("price",_price);
                parameters.Add("seller_id", Properties.Settings.Default.sellerid);
                if (ygw.InsertNewSellInfo(parameters, out sellinfoid))
                {
                    sql = string.Format("UPDATE tt_sellinfo SET mallid = '{0}' WHERE bookid = '{1}' AND ABS(price- {2}) < 1e-5",
                        sellinfoid, _local_book_id, _price);
                    dbo.AddDelUpdate(sql);
                    MessageBox.Show("执行完成");
                    return;
                }
                else
                {
                    MyOperation.DebugPrint("Insert Error!", 3);
                    throw new Exception("Insert Error!");
                }
            }
        }

        private void tb_isbn_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    search_book();
                    e.Handled = true;
                }
            }
            catch (Exception e1)
            {
                MyOperation.MessageShow(e1.Message);
            }
        }

        public static HttpWebResponse CreatePostHttpResponse(string url, IDictionary<string, string> parameters, int? timeout, string userAgent, Encoding requestEncoding, CookieCollection cookies)
        {
            const string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";  
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            if (requestEncoding == null)
            {
                throw new ArgumentNullException("requestEncoding");
            }
            HttpWebRequest request = null;
            //仅支持http请求
            request = WebRequest.Create(url) as HttpWebRequest;

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            else
            {
                request.UserAgent = DefaultUserAgent;
            }

            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            //如果需要POST数据  
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                    }
                    i++;
                }
                byte[] data = requestEncoding.GetBytes(buffer.ToString());
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            return request.GetResponse() as HttpWebResponse;
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tb1.Text))
            {
                MyOperation.MessageShow("必须输入书名");
                tb1.Focus();
                return;
            }
            if (string.IsNullOrEmpty(tb2.Text))
            {
                MyOperation.MessageShow("必须输入作者");
                tb2.Focus();
                return;
            }
            if (string.IsNullOrEmpty(tb3.Text))
            {
                MyOperation.MessageShow("必须输入出版社");
                tb3.Focus();
                return;
            }
            if (string.IsNullOrEmpty(tb5.Text))
            {
                MyOperation.MessageShow("必须输入ISBN");
                tb5.Focus();
                return;
            }
            if (string.IsNullOrEmpty(tb6.Text))
            {
                MyOperation.MessageShow("必须输入定价");
                tb6.Focus();
                return;
            }
            string url = "http://api.jige.olege.com/book";
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("isbn", tb5.Text );
            parameters.Add("fixedPrice", tb6.Text );
            parameters.Add("name", tb1.Text );
            parameters.Add("press", tb3.Text );
            parameters.Add("author", tb2.Text ); 
            Encoding encoding = Encoding.GetEncoding("UTF-8");
            HttpWebResponse response = CreatePostHttpResponse(url, parameters, null, null, encoding, null);
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
            Char[] read = new Char[256];
            // Reads 256 characters at a time.    
            int count = reader.Read(read, 0, 256);
            String str = "";
            while (count > 0)
            {
                // Dumps the 256 characters on a string and displays the string to the console.
                str = new String(read, 0, count);
                Console.Write(str);
                count = reader.Read(read, 0, 256);
            }
            MyOperation.DebugPrint("新增图书信息" + tb5.Text + tb6.Text + tb1.Text + tb3.Text + tb2.Text);
            MyOperation.DebugPrint("新增图书信息返回的JSON中data.id为:" + str, 3);

            try
            {
                JObject jo1 = (JObject)JsonConvert.DeserializeObject(str);
                bookinfo bi = new bookinfo() ;
                bi.author = tb2.Text ;
                bi.gbookid = jo1["data"].ToString();
                bi.imgpath = "";
                bi.isbn = tb5.Text ;
                bi.name = tb1.Text ;
                bi.press = tb3.Text ;
                bi.price = tb6.Text ;
                bi.bookid = InsertNewBookInfo(bi);
                if ("" == bi.bookid)
                {
                    return;
                }
                MyOperation.MessageShow("添加成功！");
                DisplayBookinfo(bi);
            }
            catch (Exception ex)
            {
                MyOperation.DebugPrint("新增图书信息button4_Click:出现catch异常:" + ex.Message, 3);
                MyOperation.MessageShow("新增图书出现异常，请尝试重新搜索ISBN");
            }
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            tb_price.Text = (MyOperation.string2float(lbb_price.Content.ToString()) * 0.3).ToString("0.0");
        }
    }
}
