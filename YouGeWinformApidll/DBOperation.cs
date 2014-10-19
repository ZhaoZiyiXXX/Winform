using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;

namespace YouGe
{
    public class DBOperation
    {
        public const int SoftwareVersion = 1;//软件版本，每次需要修改
        private const string connectstring = "Database='yg_db';Data Source='localhost';User Id='root';Password='1234';pooling=false ;charset=utf8";
        //private const string connectstring = "Database='letsgo';Data Source='202.102.86.233';User Id='letsgo';Password='letsgo246810';pooling=false ;charset=utf8";
        /// <summary>
        /// 得到连接对象
        /// </summary>
        /// <returns></returns>
        public MySqlConnection GetConn()
        {
            MySqlConnection mysqlconn = new MySqlConnection(connectstring);
            return mysqlconn;
        }

        /// <summary>
        /// 查询操作
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable Selectinfo(string sql)
        {
            MySqlConnection mysqlconn = null;
            MySqlDataAdapter sda = null;
            DataTable dt = null;
            try
            {
                mysqlconn = this.GetConn();
                mysqlconn.Open();
                sda = new MySqlDataAdapter(sql, mysqlconn);
                dt = new DataTable();
                sda.Fill(dt);
                mysqlconn.Close();
                return dt;
            }
            catch (Exception ex)
            {
                this.DebugPrint(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 增删改操作
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>执行后的条数</returns>
        public int AddDelUpdate(string sql)
        {
            MySqlConnection conn = null;
            MySqlCommand cmd = null;
            try
            {
                conn = this.GetConn();
                conn.Open();
                cmd = new MySqlCommand(sql, conn);
                int i = cmd.ExecuteNonQuery();
                conn.Close();
                return i;
            }
            catch (Exception ex)
            {
                this.DebugPrint(ex.Message);
                throw;
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
    }
}