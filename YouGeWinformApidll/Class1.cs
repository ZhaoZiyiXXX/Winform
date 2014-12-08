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
        /// <summary>
        /// 本地图书信息类，与本地bookinfo表对应
        /// </summary>
        public struct Localbookinfo
        {
            /// <summary>
            /// 本地id
            /// </summary>
            public string id;
            /// <summary>
            /// 主数据库全局id
            /// </summary>
            public string guid;
            public string name;
            public string author;
            public string press;
            public string fixedprice;
            public string isbn;
            public string imgpath;
        }

        public bool SoldOldBook(string guid)
        {
            DBOperation dbo = new DBOperation();
            string sql = string.Format("UPDATE yg_oldbookdetail SET status = 1 ,outtime = '{0}' WHERE guid = '{1}'", GetNow(), guid);
            if (1 == dbo.AddDelUpdate(sql))
            {
                return true;
            }
            else
            {
                return false;
            }   
        }

        public bool SoldNewBook(string bookid,int count)
        {
            DBOperation dbo = new DBOperation();
            string sql = string.Format("UPDATE yg_bookstock SET count = count - {0} WHERE bookid = '{1}'",count,bookid);
            if (1 == dbo.AddDelUpdate(sql))
            {
                return true;
            }
            else
            {
                return false;
            }           
        }
        public bool InsertNewBookInfo(Localbookinfo bi, out string id) 
        {
            id = null;
            string sql = string.Format("INSERT INTO yg_bookinfo (`gbookid`,`name`,`author`,`press`,`price`,`ISBN`,`imgpath`) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}')",
               bi.guid,bi.name,bi.author,bi.press,bi.fixedprice,bi.isbn,bi.imgpath);
            DBOperation dbo = new DBOperation();
            if (1 != dbo.AddDelUpdate(sql))
            {
                return false;
            }
            sql = string.Format("SELECT id FROM yg_bookinfo WHERE gbookid = '{0}'",bi.guid);
            DataTable dt = dbo.Selectinfo(sql);
            if (dt.Rows.Count > 0)
            {
                id = dt.Rows[0]["id"].ToString();
                return true;
            }
            return false;
        }

        public DataTable GetOldBookInfoById(string id)
        {
            DBOperation dbo = new DBOperation();
            string sql = string.Format("SELECT * FROM OldBookSellInfo WHERE id = '{0}'",id ); 
            return dbo.Selectinfo(sql);
        }

        public bool InsertOldBookInfo(string bookid,string newid,string price)
        {
            string sql = string.Format("INSERT INTO yg_oldbookdetail (guid,bookid,status,intime,price) VALUES ('{0}','{1}','{2}','{3}','{4}')",
                newid, bookid, "0", GetNow(),price);
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
        public DataTable GetBookinfoByBookid(string id)
        {
            DBOperation dbo = new DBOperation();
            string sql = "SELECT id,name,author,press,isbn,price FROM yg_bookinfo WHERE id = '" + id + "'";
            return dbo.Selectinfo(sql);
        }

        public string GetLocalShopName()
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

        public string GetLocalShopId()
        {
            DBOperation dbo = new DBOperation();
            string sql = "SELECT * FROM yg_local_shopinfo";
            DataTable dt = dbo.Selectinfo(sql);
            if (dt.Rows.Count == 1)
            {
                return dt.Rows[0]["shopid"].ToString();
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

        public bool InsertOrderDetail(OrderDetail od)
        {
            string sql = string.Format("INSERT INTO yg_orderdetail (orderid,bookid,count,off) VALUES ('{0}','{1}','{2}','{3}')",
            od.orderid,od.bookid,od.count,od.off);
            DBOperation dbo = new DBOperation();
            if (1 == dbo.AddDelUpdate(sql))
            {
                sql = string.Format("SELECT * FROM yg_bookstock WHERE bookid = '{0}'",od.bookid );
                DataTable dt = dbo.Selectinfo(sql);
                if (dt.Rows.Count == 0)
                {
                    sql = string.Format("INSERT INTO  yg_bookstock  (bookid , count) VALUES ('{0}','{1}')",od.bookid,od.count );
                    if (1 == dbo.AddDelUpdate(sql))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (dt.Rows.Count == 1)
                {
                    sql = string.Format("UPDATE  yg_bookstock  SET count  = count + {0} WHERE bookid = '{1}'",od.count ,dt.Rows[0]["bookid"].ToString());
                    if (1 == dbo.AddDelUpdate(sql))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    this.DebugPrint("出现了两本或更多相同bookid的库存信息");
                    return false;
                }
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

        public bool InsertNewOrder(OrderInfo oi)
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
        public bool InsertNewJinhuoqudao(string name)
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
                sql = "DROP TABLE IF EXISTS `yg_bookinfo`;";
                if (0 != dbo.AddDelUpdate(sql))
                {
                    this.DebugPrint("激活失败！errorcode : 3");
                    return false;
                }
                sql = "CREATE TABLE `yg_bookinfo` (`id` int(10) unsigned NOT NULL AUTO_INCREMENT," +
                "`gbookid` varchar(32) NOT NULL COMMENT '喵校园全局bookid',`name` varchar(255) NOT NULL COMMENT '书名'," +
                "`author` varchar(255) DEFAULT NULL COMMENT '作者',`press` varchar(50) DEFAULT NULL COMMENT '出版社'," +
                "`price` varchar(10) NOT NULL COMMENT '定价',`ISBN` varchar(15) NOT NULL COMMENT 'ISBN'," +
                "`imgpath` varchar(100) DEFAULT NULL COMMENT '图片路径',PRIMARY KEY (`id`)) ENGINE=InnoDB "+
                "AUTO_INCREMENT=1018 DEFAULT CHARSET=utf8;";
                if (0 != dbo.AddDelUpdate(sql))
                {
                    this.DebugPrint("激活失败！errorcode : 4");
                    return false;
                }

                //创建新书库存表
                sql = "DROP TABLE IF EXISTS `yg_bookstock`;";
                if (0 != dbo.AddDelUpdate(sql))
                {
                    this.DebugPrint("激活失败！errorcode : 5");
                    return false;
                }
                sql = "CREATE TABLE `yg_bookstock` (`bookid` int(11) NOT NULL,`count` int(11) NOT NULL,"+
                    " PRIMARY KEY (`bookid`)) ENGINE=InnoDB DEFAULT CHARSET=utf8;";
                if (0 != dbo.AddDelUpdate(sql))
                {
                    this.DebugPrint("激活失败！errorcode : 6");
                    return false;
                }
 
                //创建进货渠道表
                sql = "DROP TABLE IF EXISTS `yg_jinhuoqudao`;";
                if (0 != dbo.AddDelUpdate(sql))
                {
                    this.DebugPrint("激活失败！errorcode : 7");
                    return false;
                }
                sql = "CREATE TABLE `yg_jinhuoqudao` (`id` int(11) unsigned NOT NULL AUTO_INCREMENT," +
                    "`name` varchar(20) NOT NULL COMMENT '进货渠道名称', PRIMARY KEY (`id`)) ENGINE=InnoDB "+
                    "AUTO_INCREMENT=5 DEFAULT CHARSET=utf8;";
                if (0 != dbo.AddDelUpdate(sql))
                {
                    this.DebugPrint("激活失败！errorcode : 8");
                    return false;
                }


                //创建二手书详情表
                sql = "DROP TABLE IF EXISTS `yg_oldbookdetail`;";
                if (0 != dbo.AddDelUpdate(sql))
                {
                    this.DebugPrint("激活失败！errorcode : 9");
                    return false;
                }
                sql = "CREATE TABLE `yg_oldbookdetail` (`guid` varchar(20) NOT NULL COMMENT '图书唯一编码'," +
                    "`bookid` varchar(10) NOT NULL,`status` varchar(1) NOT NULL DEFAULT '0' COMMENT '状态：0，未出售；1，已出售'," +
                    "`intime` varchar(20) NOT NULL COMMENT '入库时间',`outtime` varchar(20) DEFAULT NULL COMMENT '出售时间'," +
                    "`mallid` varchar(24) DEFAULT NULL,`price` float DEFAULT NULL,PRIMARY KEY (`guid`)) ENGINE=InnoDB DEFAULT CHARSET=utf8;";
                if (0 != dbo.AddDelUpdate(sql))
                {
                    this.DebugPrint("激活失败！errorcode : 10");
                    return false;
                }

                //创建订单信息表
                sql = "DROP TABLE IF EXISTS `yg_orderinfo`;";
                if (0 != dbo.AddDelUpdate(sql))
                {
                    this.DebugPrint("激活失败！errorcode : 11");
                    return false;
                }
                sql = "CREATE TABLE `yg_orderinfo` (`id` int(11) unsigned NOT NULL AUTO_INCREMENT COMMENT '序号'," +
                    "`datetime` varchar(30) NOT NULL COMMENT '时间',`jinhuoqudao` int(11) NOT NULL COMMENT '进货渠道ID'," +
                    "`name` varchar(40) DEFAULT NULL COMMENT '订单名称',`price` float NOT NULL,PRIMARY KEY (`id`)" +
                    ") ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8;";
                if (0 != dbo.AddDelUpdate(sql))
                {
                    this.DebugPrint("激活失败！errorcode : 12");
                    return false;
                }

                //创建订单详情表
                sql = "DROP TABLE IF EXISTS `yg_orderdetail`;";
                if (0 != dbo.AddDelUpdate(sql))
                {
                    this.DebugPrint("激活失败！errorcode : 13");
                    return false;
                }
                sql = "CREATE TABLE `yg_orderdetail` (`id` int(11) unsigned NOT NULL AUTO_INCREMENT COMMENT '序号'," +
                    " `orderid` int(11) NOT NULL,`bookid` int(11) NOT NULL,`count` int(11) NOT NULL,`off` float NOT NULL " +
                    "COMMENT '折扣',PRIMARY KEY (`id`)) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8;";
                if (0 != dbo.AddDelUpdate(sql))
                {
                    this.DebugPrint("激活失败！errorcode : 14");
                    return false;
                }

                //创建财务出账表
                sql = "DROP TABLE IF EXISTS `yg_outbookmoney`;";
                if (0 != dbo.AddDelUpdate(sql))
                {
                    this.DebugPrint("激活失败！errorcode : 15");
                    return false;
                }
                sql = "CREATE TABLE `yg_outbookmoney` (`id` int(10) unsigned NOT NULL AUTO_INCREMENT,`outtime` varchar(15) NOT NULL," +
                    "`price` float NOT NULL,PRIMARY KEY (`id`)) ENGINE=InnoDB DEFAULT CHARSET=utf8;";
                if (0 != dbo.AddDelUpdate(sql))
                {
                    this.DebugPrint("激活失败！errorcode : 16");
                    return false;
                }


                //创建根据ID获取订单详情的视图VIEW
                sql = "DROP VIEW IF EXISTS `oldbooksellinfo`;";
                if (0 != dbo.AddDelUpdate(sql))
                {
                    this.DebugPrint("激活失败！errorcode : 17");
                    return false;
                }
                sql = "CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `oldbooksellinfo`"+
                    " AS select `yg_oldbookdetail`.`guid` AS `id`,`yg_bookinfo`.`gbookid` AS `gbookid`,`yg_bookinfo`.`id` "+
                    "AS `lbookid`,`yg_bookinfo`.`name` AS `name`,`yg_bookinfo`.`author` AS `author`,`yg_bookinfo`.`press` "+
                    "AS `press`,`yg_bookinfo`.`price` AS `price`,`yg_bookinfo`.`ISBN` AS `ISBN`,`yg_bookinfo`.`imgpath` AS "+
                    "`imgpath`,`yg_oldbookdetail`.`status` AS `status`,`yg_oldbookdetail`.`intime` AS `intime`,"+
                    "`yg_oldbookdetail`.`outtime` AS `outtime`,`yg_oldbookdetail`.`mallid` AS `mallid` from (`yg_bookinfo` "+
                    "join `yg_oldbookdetail`) where (`yg_bookinfo`.`id` = `yg_oldbookdetail`.`bookid`) ;";
                if (0 != dbo.AddDelUpdate(sql))
                {
                    this.DebugPrint("激活失败！errorcode : 18");
                    return false;
                }
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
            //if (File.Exists("winformdebug.config"))
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
