using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Windows;
using YouGe;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace 悠歌网络合作商家管理系统
{
    public class MyOperation
    {
        YouGeWinformApi ygw = new YouGeWinformApi();
        YouGeWebApi yg = new YouGeWebApi();
        public static string GetNow()
        {
            return string.Format("{0:F}", DateTime.Now);
        }

        public DataTable SearchBookinfoByIsbn(string isbn)
        {
            DataTable dt = ygw.SearchBookinfoByIsbn(isbn);
            if (dt.Rows.Count > 0)
            {
                //如果本地已经有图书信息，就直接返回给用户
                return dt;
            }
            //本地没有图书信息，从主数据库取信息
            dt = new DataTable();
            //初始化dt结构
            dt.Columns.Add("id", System.Type.GetType("System.Int32"));
            dt.Columns.Add("name", System.Type.GetType("System.String"));
            dt.Columns.Add("press", System.Type.GetType("System.String"));
            dt.Columns.Add("isbn", System.Type.GetType("System.String"));
            dt.Columns.Add("price", System.Type.GetType("System.String"));
            dt.Columns.Add("author", System.Type.GetType("System.String"));
            try
            {
                JObject jo;
                if (yg.SearchBookinfoByIsbn(isbn, out jo))
                {
                    YouGeWinformApi.Localbookinfo lbi = new YouGeWinformApi.Localbookinfo();
                    lbi.author = jo["data"]["author"].ToString();
                    lbi.fixedprice = jo["data"]["fixedPrice"].ToString();
                    lbi.guid = jo["data"]["id"].ToString();;
                    lbi.imgpath = jo["data"]["imgpath"].ToString();
                    lbi.isbn = jo["data"]["isbn"].ToString();
                    lbi.name = jo["data"]["name"].ToString();
                    lbi.press = jo["data"]["press"].ToString();
                    //如果从主数据库取到了图书信息，那么就写入本地数据库
                    string newid;
                    if (ygw.InsertNewBookInfo(lbi,out newid))
                    {
                        //写入成功之后，将dt变量赋值并返回
                        DataRow dr = dt.NewRow();
                        dr["id"] = Convert.ToInt32(newid);
                        dr["name"] = lbi.name;
                        dr["press"] = lbi.press;
                        dr["isbn"] = lbi.isbn;
                        dr["price"] = lbi.fixedprice;
                        dr["author"] = lbi.author;
                        dt.Rows.Add(dr);
                        return dt;
                    }
                    else
                    {
                        return dt;
                    }
                }
                else
                {
                    return dt;
                }
            }
            catch
            {
                //异常返回不一定是错误，jo对象解析完毕也会走到这里
                return dt;
            }

            
        }
        public static void DebugPrint(string logtext,int flag = 0)
        {
            string head = "";
            //if (File.Exists("debug.config"))
            if (true)//调试初期默认打开log开关
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
                StreamWriter log = new StreamWriter("Debug.log", true);
                log.WriteLine(head + "Time:" + System.DateTime.Now.ToString());
                log.WriteLine(logtext);
                log.Close();
            }
        }

        public static string MD5(string text)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(text));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        /// <summary>
        /// 字符串转换成int型整数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int string2int(string str)
        {
            int number = 0;
            string num = null;
            foreach (char item in str)
            {
                if (item >= 48 && item <= 58) 
                {
                    num += item;
                }
            }
            number = int.Parse(num);
            return number;
        }

        public static float string2float(string str)
        {
            if(string.IsNullOrEmpty(str))
                return 0;

            float number = 0;
            if (-1 == str.IndexOf(".", 0))
            {
                number = string2int(str);
            }
            else
            {
                string[] seperated = str.Split(new char[] { '.' });
                string num = null;
                if (seperated.Length != 2)
                    return 0;
                foreach (char item in seperated[0])
                {
                    if (item >= 48 && item <= 58)
                    {
                        num += item;
                    }
                }
                num += '.';
                foreach (char item in seperated[1])
                {
                    if (item >= 48 && item <= 58)
                    {
                        num += item;
                    }
                }
                number = float.Parse(num);
            }

            return number;
        }
    }
}
