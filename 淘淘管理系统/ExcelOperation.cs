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
        public static DataTable  CsvToDataTable(string filepath)
        {
            string pCsvPath = filepath;//文件路径
            DataTable table = new DataTable();
            try
            {
                String line;
                String[] split = null;
                DataRow row = null;
                StreamReader sr = new StreamReader(pCsvPath, System.Text.Encoding.Default);
                //创建与数据源对应的数据列 
                line = sr.ReadLine();
                split = line.Split(',');
                foreach (String colname in split)
                {
                    table.Columns.Add(colname, System.Type.GetType("System.String"));
                }
                //将数据填入数据表 
                int j = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    j = 0;
                    row = table.NewRow();
                    split = line.Split(',');
                    foreach (String colname in split)
                    {
                        row[j] = colname;
                        j++;
                    }
                    table.Rows.Add(row);
                }
                sr.Close();
            }
            catch (Exception ex)
            {
                MyOperation.DebugPrint(ex.Message);
                MyOperation.MessageShow(ex.Message);
            }
            finally
            {
                GC.Collect();
            }
            return table;
        }
        public static void dataTableToCsv(DataTable table, string file,bool open = true)
        {
            string title = "";
            char t = ',';
            char n = '\n';
            FileStream fs = new FileStream(file, FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(new BufferedStream(fs), System.Text.Encoding.Default);
            for (int i = 0; i < table.Columns.Count; i++)
            {
                title += table.Columns[i].ColumnName + t; //栏位：自动跳到下一单元格
            }
            title = title.Substring(0, title.Length - 1) + n;
            sw.Write(title);
            foreach (DataRow row in table.Rows)
            {
                string line = "";
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    line += row[i].ToString().Trim() + t; //内容：自动跳到下一单元格
                }
                line = line.Substring(0, line.Length - 1) + n;
                sw.Write(line);
            }
            sw.Close();
            fs.Close();
            if (open)
            {
                System.Diagnostics.Process.Start(file);  //打开excel文件
            }
        }

        public static void SendToExcel(DataTable Table, String SheetName = "undefine", bool Visible = true,bool close = false )
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
                if (close)
                {
                    obook.Save();
                    obook.Close(false, null, null);
                    app.Quit();
                }
            }
            catch (Exception ex)
            {
                MyOperation.DebugPrint(ex.Message);
                MyOperation.MessageShow(ex.Message);
            }
            finally
            {
                app.Visible = Visible;
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
