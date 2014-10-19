using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;

namespace YouGe
{
    public class YouGeWinformApi
    {
        public bool insertOldBookInfo(string bookid,string newid)
        {
            string sql = string.Format("INSERT INTO yg_oldbookdetail (guid,bookid,status,intime) VALUES ('{0}','{1}','{2}','{3}')",
                newid, bookid, "0", GetNow());
            DBOperation dbo = new DBOperation();
            if (1 == dbo.AddDelUpdate(sql))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public DataTable getBookinfoByBookid(string id)
        {
            DBOperation dbo = new DBOperation();
            string sql = "SELECT name,author,press,isbn,price FROM yg_bookinfo WHERE id = '" + id + "'";
            return dbo.Selectinfo(sql);
        }

        public string getLocalShopName()
        {
            DBOperation dbo = new DBOperation();
            string sql = "SELECT * FROM yg_local_shopinfo";
            DataTable dt = dbo.Selectinfo(sql);
            if (dt.Rows.Count == 1)
            {
                return dt.Rows[0]["shopname"].ToString();
            }
            else
            {
                return null;
            }
        }

        public struct OrderDetail
        {
            public string orderid;
            public string bookid;
            public string count;
            public string off;
        }

        public bool insertOrderDetail(OrderDetail od)
        {
            string sql = string.Format("INSERT INTO yg_orderdetail (orderid,bookid,count,off) VALUES ('{0}','{1}','{2}','{3}')",
            od.orderid,od.bookid,od.count,od.off);
            DBOperation dbo = new DBOperation();
            if (1 == dbo.AddDelUpdate(sql))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public struct OrderInfo
        {
            public string datetime;
            public string jinhuoqudao;
            public string ordername;
            public string totalprice;
        }

        public bool insertNewOrder(OrderInfo oi)
        {
            string sql = string.Format("INSERT INTO yg_orderinfo (datetime ,jinhuoqudao,`name`,`price`) VALUES ('{0}','{1}','{2}','{3}')",
                oi.datetime , oi.jinhuoqudao , oi.ordername , oi.totalprice );
            DBOperation dbo = new DBOperation();
            if (1 == dbo.AddDelUpdate(sql))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetOrderIdByCreateTime(string time)
        {
            string sql = string.Format("SELECT id FROM yg_orderinfo WHERE datetime = '{0}'", time);
            DBOperation dbo = new DBOperation();
            DataTable dt = dbo.Selectinfo(sql);
            if (dt.Rows.Count != 1)
            {
                this.DebugPrint("获取订单id错误，取到的结果集行数为："+dt.Rows.Count.ToString());
                return null;
            }
            else
            {
                return dt.Rows[0]["id"].ToString();
            }
        }
        public bool insertNewJinhuoqudao(string name)
        {
            DBOperation dbo = new DBOperation();
            string sql = "INSERT INTO yg_jinhuoqudao (id,name) VALUES (null,'" + name + "')";
            if (1 == dbo.AddDelUpdate(sql))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public DataTable GetAllJinhuoqudao()
        {
            DBOperation dbo = new DBOperation();
            string sql = "SELECT * FROM yg_jinhuoqudao";
            return dbo.Selectinfo(sql);
        }

        public DataTable SearchBookinfoByIsbn(string isbn)
        {
            DBOperation dbo = new DBOperation();
            string sql = string.Format("SELECT id,name,author,press,price,isbn FROM yg_bookinfo WHERE ISBN = '{0}'", isbn);
            DataTable dt = dbo.Selectinfo(sql);
            return dt;
        }
        public struct Shopinfo
        {
            public string shopId;
            public string shopRealname;
        }

        public bool InitDatabase(Shopinfo shopinfo)
        {
            try
            {
                //创建商户信息表
                string sql = "CREATE TABLE `yg_local_shopinfo` (`shopid` VARCHAR( 24 ) NOT NULL primary key," +
                "`shopname` VARCHAR( 30 ) NOT NULL ,`reserve1` VARCHAR( 100 ) DEFAULT NULL ,`reserve2` VARCHAR( 100 ) " +
                "DEFAULT  NULL) ENGINE = MYISAM CHARACTER SET utf8 COLLATE utf8_general_ci; ";
                DBOperation dbo = new DBOperation();
                if (0 != dbo.AddDelUpdate(sql))
                {
                    this.DebugPrint("激活失败！errorcode : 1");
                    return false;
                }

                //写入商户信息
                sql = string.Format("INSERT INTO yg_local_shopinfo (shopid,shopname) VALUES ('{0}','{1}')",
                    shopinfo.shopId, shopinfo.shopRealname);
                if (1 != dbo.AddDelUpdate(sql))
                {
                    this.DebugPrint("激活失败！errorcode : 2");
                    return false;

                }


                //创建图书信息表
 
                //创建进货渠道表

                //创建订单信息表

                //创建订单详情表

                //创建根据ID获取订单详情的视图VIEW
                return true;
            }
            catch (Exception ex)
            {
                this.DebugPrint("激活失败！errorcode : " + ex.Message);
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
            if (File.Exists("winformdebug.config"))
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
                StreamWriter log = new StreamWriter("WinformDebug.log", true);
                log.WriteLine(head + "Time:" + System.DateTime.Now.ToString());
                log.WriteLine(logtext);
                log.Close();
            }
        }

        public static string GetNow()
        {
            return string.Format("{0:F}", DateTime.Now);
        }
    }
}
