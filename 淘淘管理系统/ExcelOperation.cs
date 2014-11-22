using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using MyExcel = Microsoft.Office.Interop.Excel;
using System.Reflection;

namespace 淘淘管理系统
{
    class ExcelOperation
    {
        public static void dataTableToCsv(DataTable table, string file)
        {
            string title = "";
            FileStream fs = new FileStream(file, FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(new BufferedStream(fs), System.Text.Encoding.Default);
            for (int i = 0; i < table.Columns.Count; i++)
            {
                title += table.Columns[i].ColumnName + "\t"; //栏位：自动跳到下一单元格
            }
            title = title.Substring(0, title.Length - 1) + "\n";
            sw.Write(title);
            foreach (DataRow row in table.Rows)
            {
                string line = "";
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    line += row[i].ToString().Trim() + "\t"; //内容：自动跳到下一单元格
                }
                line = line.Substring(0, line.Length - 1) + "\n";
                sw.Write(line);
            }
            sw.Close();
            fs.Close();
            System.Diagnostics.Process.Start(file);  //打开excel文件
        }

        public static void SendToExcel(DataTable Table, String SheetName = "undefine")
        {
            MyExcel.Application app = new MyExcel.Application();
            try
            {
                MyExcel.Workbook obook = app.Workbooks.Add(Missing.Value); // 添加一个工作簿
                MyExcel.Worksheet osheet = (MyExcel.Worksheet)app.ActiveSheet;// 获取当前工作表
                osheet.Name = SheetName; // 修改工作表的名字
                int r,c;
                int rCount = Table.Rows.Count;
                int cCount = Table.Columns.Count;
                osheet.Range["A1:Z1"].Font.Bold = true;//设置列标题加粗
                for(c = 1;c <= cCount ;c++)
                {
                    osheet.Cells[1,c] = Table.Columns[c-1].Caption;//设置列标题
                }
                for(r = 1;r <= rCount;r++)
                {
                    for(c = 1 ;c <= cCount;c++)
                    {
                        osheet.Cells[r+1,c] = Convert.ToString(Table.Rows[r - 1][c - 1].ToString());
                    }
                }
                app.Visible = true;
            }
            catch (Exception ex)
            {
                MyOperation.DebugPrint(ex.Message);
                MyOperation.MessageShow(ex.Message);
            }
            finally
            {
                //留给用户选择是否关闭
                //app.Quit();
                app = null;
            }
        }

        public static void SendFinanceInfo(DataTable Table,string filename = "")
        {
            MyExcel.Application app = new MyExcel.Application();
            try
            {
                MyExcel.Workbook obook = app.Workbooks.Open(filename);//打开标准模板
                MyExcel.Worksheet osheet = (MyExcel.Worksheet)app.ActiveSheet;// 获取当前工作表

                app.Visible = true;
            }
            catch (Exception ex)
            {
                MyOperation.MessageShow(ex.Message);
            }
            finally
            {
                //留给用户选择是否关闭
                //app.Quit();
                app = null;
            }
        }
    }
}
