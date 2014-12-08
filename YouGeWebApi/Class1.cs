using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace YouGe
{
    public class YouGeWebApi
    {
        private string URL_ROOT = "http://api.jige.olege.com";
        //private string URL_ROOT = "http://api.jigedev.olege.com";
        private string URL_STATUS_API = "http://1.ivanapi.sinaapp.com";
        private string URL_CURRENT_WEIXIN_VERSION = "http://1.mallschoolwx.sinaapp.com";
        public string GetUrl()
        {
            return this.URL_ROOT;
        }

        /// <summary>
        /// 根据交易ID，售价以及图书编号确定是否存在这样一笔交易。
        /// 如果图书编号不一致，则将原交易信息关闭并返回False
        /// 如果定价不一致的情况本函数会自动更新成一致并返回true
        /// </summary>
        /// <param name="sellid"></param>
        /// <param name="bookid"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        public bool IsExistSellInfo(string sellid,string bookid,string price)
        {
            try{
                JObject jo;
                if (GetSellInfoById(sellid, out jo))
                {
                    if (bookid != jo["data"]["book_id"].ToString() )
                    {
                        IDictionary<string, string> parameters = new Dictionary<string, string>();
                        parameters.Add("id", sellid);
                        parameters.Add("status", "1");
                        this.UpdateSellInfo(parameters);
                        return false;
                    }
                    else if (price != jo["data"]["price"].ToString())
                    {
                        IDictionary<string, string> parameters = new Dictionary<string, string>();
                        parameters.Add("id", sellid);
                        parameters.Add("price", price);
                        this.UpdateSellInfo(parameters);
                        return true;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }catch(Exception ex){
                this.DebugPrint("IsExistSellInfo Error:" + ex.Message );
                return false;
            }
            
        }
        /// <summary>
        /// 通过交易ID获取交易详情
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bookinfo"></param>
        /// <returns></returns>
        public bool GetSellInfoById(string id,out JObject bookinfo)
        {
            string url = string.Format(this.URL_ROOT + "/sell?id={0}", id);
            string html = GET(url);
            this.DebugPrint("GetSellInfoById返回：" + html);
            try
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(html);
                bookinfo = jo;
                return true;
            }
            catch (Exception e)
            {
                this.DebugPrint("GetSellInfoById出现catch异常：" + e.Message,3);
                bookinfo = null;
                return false;
            }
        }

        /// <summary>
        /// 更新交易信息，传入参数必须有ID一项
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public bool UpdateSellInfo(IDictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey("id"))
            {
                this.DebugPrint("UpdateSellInfo:传入的参数缺失！");
                return false;
            }

            string url = string.Format(this.URL_ROOT + "/sell");
            string html = POST(url, parameters);
            this.DebugPrint(html);
            try
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(html);
                if ("0" == jo["result"].ToString())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                this.DebugPrint("UpdateSellInfo:出现catch异常:" + e.Message);
                return false;
            }
        }
        /// <summary>
        /// 插入一条新的交易信息，必须包含book_id，seller_id，price
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="sellinfoid"></param>
        /// <returns></returns>
        public bool InsertNewSellInfo(IDictionary<string, string> parameters, out string sellinfoid)
        {
            if (!parameters.ContainsKey("book_id") |
                !parameters.ContainsKey("seller_id") |
                !parameters.ContainsKey("price"))
            {
                sellinfoid = null;
                this.DebugPrint("InsertNewSellInfo:传入的参数缺失！");
                return false;
            }

            string url = string.Format(this.URL_ROOT + "/sell");
            string html = POST(url, parameters);
            this.DebugPrint(html);
            try
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(html);
                if ("0" == jo["result"].ToString())
                {
                    sellinfoid = jo["data"].ToString();
                    return true;
                }
                else
                {
                    sellinfoid = null;
                    return false;
                }
            }
            catch (Exception e)
            {
                this.DebugPrint("InsertNewSellInfo:出现catch异常:" + e.Message);
                sellinfoid = null;
                return false;
            } 
        }

        /// <summary>
        /// 新增图书信息，返回添加后的id
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="gbookid"></param>
        /// <returns></returns>
        public bool InsertNewBookInfo(IDictionary<string, string> parameters,out string gbookid)
        {
            if(!parameters.ContainsKey("isbn")|
                !parameters.ContainsKey("fixedPrice")|
                !parameters.ContainsKey("name")|
                !parameters.ContainsKey("press")|
                !parameters.ContainsKey("author"))
            {
                gbookid = null;
                this.DebugPrint("InsertNewBookInfo:传入的参数缺失！");
                return false;
            }

            string url = string.Format(this.URL_ROOT + "/book");
            string html = POST(url, parameters);
            this.DebugPrint(html);
            try
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(html);
                if ("0" == jo["result"].ToString())
                {
                    gbookid = jo["data"].ToString();
                    return true;
                }
                else
                {
                    gbookid = null;
                    return false;
                }
            }
            catch (Exception e)
            {
                this.DebugPrint("InsertNewBookInfo:出现catch异常:" + e.Message);
                gbookid = null;
                return false;
            } 
        }

        /// <summary>
        /// 根据ISBN号搜索图书信息
        /// </summary>
        /// <param name="isbn"></param>
        /// <param name="bookinfo"></param>
        /// <returns></returns>
        public bool SearchBookinfoByIsbn(string isbn, out JObject bookinfo)
        {
            string url = string.Format(this.URL_ROOT + "/book?q={0}&type=ISBN", isbn);
            string html = GET(url);
            this.DebugPrint("getBookInfoByIsbn返回：" + html);
            try
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(html);
                bookinfo = jo;
                return true;
            }
            catch (Exception e)
            {
                this.DebugPrint("getBookInfoByIsbn出现catch异常：" + e.Message);
                bookinfo = null;
                return false;
            }
        }

        /// <summary>
        /// 更新意见信息
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public bool UpdateYijian(IDictionary<string, string> parameters)
        {
            string url = string.Format(this.URL_CURRENT_WEIXIN_VERSION + "/outjson/UpdateYijian.php");
            string html = POST(url, parameters);
            this.DebugPrint(html);
            try
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(html);
                if ("0" == jo["result"].ToString())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                this.DebugPrint("UpdateYijian:出现catch异常:" + e.Message);
                return false;
            } 
        }

        /// <summary>
        /// 获取用户意见列表
        /// </summary>
        /// <param name="YijianInfo"></param>
        /// <returns></returns>
        public bool GetYijian(out JObject YijianInfo)
        {
            string url = string.Format(this.URL_CURRENT_WEIXIN_VERSION + "/outjson/getyijian.php");
            string html = GET(url);
            this.DebugPrint("GetYijian返回：" + html);
            try
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(html);
                YijianInfo = jo;
                return true;
            }
            catch (Exception e)
            {
                this.DebugPrint("GetYijian出现catch异常：" + e.Message);
                YijianInfo = null;
                return false;
            }
        }

        /// <summary>
        /// 新店铺激活
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ShopInfo"></param>
        /// <returns></returns>
        public bool ActivateNewShop(string id,out JObject ShopInfo)
        {
            string url = string.Format(this.URL_ROOT + "/user?id="+id);
            string html = GET(url);
            this.DebugPrint("ActivateNewShop返回："+html);
            try
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(html);
                if (id == jo["data"]["id"].ToString())
                {
                    ShopInfo = jo;
                    return true;
                }
                else
                {
                    ShopInfo = null;
                    return false;
                }
            }
            catch (Exception e)
            {
                this.DebugPrint("ActivateNewShop出现catch异常：" + e.Message);
                ShopInfo = null;
                return false;
            }

        }

        /// <summary>
        /// 重置图书审核状态
        /// </summary>
        /// <returns></returns>
        public bool ResetReviewBookInfo()
        {
            string url = string.Format(this.URL_ROOT + "/work/BookReviewReset");
            string html = POST(url);
            this.DebugPrint(html);
            try
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(html);
                this.DebugPrint(jo["data"]["result"].ToString());
                if ("True" == jo["data"]["result"].ToString())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                this.DebugPrint("ResetReviewBookInfo:出现catch异常:" + e.Message);
                return false;
            }

        }

        /// <summary>
        /// 获取待审核的图书信息
        /// </summary>
        /// <param name="bookinfo"></param>
        /// <returns></returns>
        public bool GetReviewBookInfo(out JObject bookinfo)
        {
            string url = URL_ROOT + "/work/BookReview";
            string html = GET(url);
            if (null == html)
            {
                bookinfo = null;
                return false;
            }
            else
            {
                try
                {
                    JObject jo = (JObject)JsonConvert.DeserializeObject(html);
                    bookinfo = jo;
                    return true;
                }
                catch (Exception e)
                {
                    this.DebugPrint("GetReviewBookInfo:出现catch异常:" + e.Message);
                    bookinfo = null;
                    return false;
                }
            }
        }

        /// <summary>
        /// 提交审核之后的图书信息
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public bool PostReviewBookInfo(IDictionary<string, string> parameters)
        {
            string url = string.Format(this.URL_ROOT + "/work/BookReview");
            string html = POST(url, parameters);
            this.DebugPrint(html);
            try
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(html);
                this.DebugPrint("PostReviewBookInfo:" + jo["result"].ToString());
                if ("0" == jo["result"].ToString())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                this.DebugPrint("PostReviewBookInfo:出现catch异常:" + e.Message);
                return false;
            }
        }

        /// <summary>
        /// 打印LOG函数，当程序统计文件夹下存在apidebug.config文件时，输出log信息到ApiDebug.log文件
        /// </summary>
        /// <param name="logtext"></param>
        /// <param name="flag"></param>
        public void DebugPrint(string logtext, int flag = 0)
        {
            string head = "";
            //if (File.Exists("apidebug.config"))
            if (true)//TODO:调试初期默认打开log开关
            {
                switch (flag)
                {
                    case 1:
                        head = "[Info]";
                        break;
                    case 2:
                        head = "[Warning]";
                        break;
                    case 3:
                        head = "[Error]";
                        break;
                    default:
                        head = "[Debug]";
                        break;
                }
                StreamWriter log = new StreamWriter("ApiDebug.log", true);
                log.WriteLine(head + "Time:" + System.DateTime.Now.ToString());
                log.WriteLine(logtext);
                log.Close();
            }
        }

        /// <summary>
        /// 上报用户信息给状态服务器，建议每2分钟上报一次
        /// </summary>
        /// <returns></returns>
        public bool UpdateUserStatus(string id ,string name)
        {
            string url = string.Format(this.URL_STATUS_API + "/status/post.php");
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("uid", id);
            parameters.Add("name", name);
            string html = POST(url, parameters);
            this.DebugPrint("UpdateUserStatus:" + html);
            return true;
        }
        /// <summary>
        /// 获取当前在线用户列表函数
        /// </summary>
        /// <returns>用户姓名列表</returns>
        public List<string> GetOnlineUser()
        {
            string url = string.Format(this.URL_STATUS_API + "/status/get.php");
            string html = GET(url);
            this.DebugPrint("GetOnlineUser:" + html);
            if (null == html)
            {
                return new List<string>();
            }

            List<string> username = new List<string>();
            try
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(html);
                int i = 0;
                while(!string.IsNullOrEmpty(jo["data"][i]["name"].ToString())){
                    username.Add(jo["data"][i]["name"].ToString());
                    i++;
                }
                return username;
            }
            catch (Exception e)
            {
                this.DebugPrint("GetOnlineUser:出现catch异常:" + e.Message);
                return username;
            }
        }


        /// <summary>
        /// 登陆鉴权函数，会直接将用户信息存在全局变量里
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>true:登陆成功;false：登陆失败</returns>
        public bool UserAccess(string username, string password, out JObject results)
        {
            string url = string.Format(this.URL_ROOT + "/work/user?username={0}&password={1}&type=access", username, password);
            string html = GET(url);
            if (null == html)
            {
                results = null;
                return false;
            }
            try
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(html);
                this.DebugPrint(jo["result"].ToString());
                if ("0" != jo["result"].ToString())
                {
                    results = null;
                    return false;
                }
                else
                {
                    results = jo;
                    return true;
                }
            }
            catch (Exception e)
            {
                this.DebugPrint("UserAccess:出现catch异常:" + e.Message);
                results = null;
                return false;
            }
        }
        /// <summary>
        /// 注册新用户
        /// </summary>
        /// <param name="parameters">注册信息</param>
        /// <returns>0:成功；1:失败；2:用户名重复；</returns>
        public int Register(IDictionary<string, string> parameters)
        {
            string url = string.Format(this.URL_ROOT + "/work/user");
            string html = POST(url, parameters);
            this.DebugPrint(html);
            try
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(html);
                this.DebugPrint("Register" + jo["result"].ToString());
                if ("0" == jo["result"].ToString())
                {
                    return 0;
                }
                else if ("1005" == jo["result"].ToString())
                {
                    return 2;
                }
                else
                {
                    return 1;
                }
            }
            catch (Exception e)
            {
                this.DebugPrint("Register:出现catch异常:" + e.Message);
                return 1;
            }
        }

        # region 基础函数


        private string POST(string url, IDictionary<string, string> parameters = null, string encode = "UTF-8")
        {
            Encoding encoding = Encoding.GetEncoding(encode);
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

            return str;
        }

        private static HttpWebResponse CreatePostHttpResponse(string url, IDictionary<string, string> parameters, int? timeout, string userAgent, Encoding requestEncoding, CookieCollection cookies)
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
        /// <summary>
        /// GET方法
        /// </summary>
        /// <param name="URL"></param>
        /// <returns>出现异常时返回""</returns>
        private static string GET(string URL)
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
                return null;
            }
        }

        #endregion
    }
}
