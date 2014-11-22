﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using FirstFloor.ModernUI.Windows.Controls;
using System.Windows;

namespace 淘淘管理系统
{
    public class MyOperation
    {
        public static void DebugPrint(string logtext,int flag = 0)
        {
            string head = "";
            if (File.Exists("debug.config"))
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

        public static MessageBoxResult MessageShow(string text,string title = "",MessageBoxButton Button = MessageBoxButton.OK )
        {
            return ModernDialog.ShowMessage(text,title,Button);
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
