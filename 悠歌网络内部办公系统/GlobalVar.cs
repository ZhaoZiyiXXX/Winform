using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 悠歌网络内部办公系统
{
    public static class GlobalVar
    {
        public static string id;
        public static string username;
        public static string name;
        public static string email;
        public static string tel;
        public static string group;
        public static string lasttime;
        
        public static void ClearAll()
        {
            GlobalVar.id = "";
            GlobalVar.username = "";
            GlobalVar.name = "";
            GlobalVar.email = "";
            GlobalVar.tel = "";
            GlobalVar.group = "";
        }
    }
}
