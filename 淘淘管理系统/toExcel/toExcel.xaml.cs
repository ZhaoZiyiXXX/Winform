using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;

namespace 淘淘管理系统.toExcel
{
    /// <summary>
    /// toExcel.xaml 的交互逻辑
    /// </summary>
    public partial class toExcel : UserControl
    {
        DBOperation dbo = new DBOperation();
        public toExcel()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tb1.Text))
            {
                MyOperation.MessageShow("请输入书主号");
                tb1.Focus();
                return;
            }
            string sql = string.Format("SELECT s.sellinfoid AS `图书ID`,b.`name` AS `书名`,b.press AS `出版社`,s.price AS `售价`," +
                "s.issold AS `是否已售` FROM tt_sellinfo AS s , tt_bookinfo AS b WHERE s.bookid = b.id AND " +
                "s.sellerid = '{0}'", tb1.Text);
            DataTable dt = dbo.Selectinfo(sql);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["是否已售"] = ("0" == dt.Rows[i]["是否已售"].ToString()) ? "否" : "是";
            }
            MyOperation.DebugPrint("导出了书主信息：" + tb1.Text);
            ExcelOperation.SendToExcel(dt, string.Format("书主【{0}】交易详情表", tb1.Text));
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tb1.Text))
            {
                MyOperation.MessageShow("请输入书主号");
                tb1.Focus();
                return;
            }
            string sql = string.Format("SELECT ts.`name` AS `姓名`,tt.count AS `数额`,tt.date AS `提款时间` "+
                "FROM tt_tikuan AS tt ,tt_sellerinfo AS ts WHERE tt.sellerid = ts.sellerid AND ts.sellerid = '{0}'", tb1.Text);
            DataTable dt = dbo.Selectinfo(sql);
            MyOperation.DebugPrint("导出了取现信息："+tb1.Text );
            ExcelOperation.SendToExcel(dt, string.Format("书主【{0}】取现详情表", tb1.Text));
        }

        //导出全部员工信息
        private void button3_Click(object sender, RoutedEventArgs e)
        {
            string sql = "SELECT ts.staffid AS `员工号`,ts.`name` AS `姓名`,ts.email AS `邮箱`,ts.phone AS `手机`," +
                "ts.role AS `角色`,ts.mark AS `备注` FROM tt_staffinfo AS ts";
            DataTable dt = dbo.Selectinfo(sql);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["角色"] = ("admin" == dt.Rows[i]["角色"].ToString()) ? "管理员" : "员工";
            }
            MyOperation.DebugPrint("导出了全部员工信息");
            ExcelOperation.SendToExcel(dt, "员工信息表");
        }

        //书主信息表
        private void button4_Click(object sender, RoutedEventArgs e)
        {
            
            string sql = "SELECT ts.sellerid AS `书主号`,ts.`name` AS `姓名`,ts.phone AS `手机`," +
                "ts.grade AS `年级`,ts.mark AS `备注` FROM tt_sellerinfo AS ts";
            DataTable dt = dbo.Selectinfo(sql);
            MyOperation.DebugPrint("导出了全部书主信息");
            ExcelOperation.SendToExcel(dt, "书主信息表");
        }

        //按时间导出取现信息
        private void button5_Click(object sender, RoutedEventArgs e)
        {
            if (null == datePicker1.SelectedDate || null == datePicker2.SelectedDate)
            {
                MyOperation.MessageShow("请选择开始和结束日期");
                return;
            }

            string sql =string.Format( "SELECT ts.`name` AS `姓名`,tt.count AS `数额`,tt.date AS `提款时间` FROM tt_tikuan AS tt ,"+
                "tt_sellerinfo AS ts WHERE tt.sellerid = ts.sellerid AND tt.date > '{0}' AND tt.date < '{1}'",datePicker1.SelectedDate,datePicker2.SelectedDate );
            DataTable dt = dbo.Selectinfo(sql);
            MyOperation.DebugPrint("导出了取现信息");
            ExcelOperation.SendToExcel(dt, "取现详情表");
        }

        //出售信息详情表
        private void button6_Click(object sender, RoutedEventArgs e)
        {
            if (null == datePicker3.SelectedDate || null == datePicker4.SelectedDate)
            {
                MyOperation.MessageShow("请选择开始和结束日期");
                return;
            }

            string sql = string.Format("SELECT tb.`name` AS `书名`,tb.press AS `出版社`,ts.sellinfoid AS `图书ID`,ts.price " +
                "AS `售价`,tsr.sellerid AS `书主ID`,tsr.`name` AS `书主`,ts.soldtime AS `出售时间` FROM tt_sellinfo AS ts ," +
                "tt_bookinfo AS tb ,tt_sellerinfo AS tsr WHERE ts.sellerid = tsr.sellerid AND ts.bookid = tb.id AND ts.soldtime" +
                " > '{0}' AND ts.soldtime < '{1}'", datePicker3.SelectedDate, datePicker4.SelectedDate);
            DataTable dt = dbo.Selectinfo(sql);
            MyOperation.DebugPrint("导出了出售信息");
            ExcelOperation.SendToExcel(dt, "出售详情表");
        }

        private void button7_Click(object sender, RoutedEventArgs e)
        {
            if (null == datePicker5.SelectedDate || null == datePicker6.SelectedDate)
            {
                MyOperation.MessageShow("请选择开始和结束日期");
                return;
            }

            string sql = string.Format("SELECT tt_fanance.money AS `余额`, tt_fanance.date AS `结算时间` FROM tt_fanance "+
            "WHERE date > '{0}' AND date < '{1}'", datePicker5.SelectedDate, datePicker6.SelectedDate);
            DataTable dt = dbo.Selectinfo(sql);
            MyOperation.DebugPrint("导出了现金流向信息");
            ExcelOperation.SendToExcel(dt, "现金详情表");
        }

        private void button8_Click(object sender, RoutedEventArgs e)
        {
            if (null == datePicker7.SelectedDate || null == datePicker8.SelectedDate)
            {
                MyOperation.MessageShow("请选择开始和结束日期");
                return;
            }

            string sql = string.Format("SELECT ts.`name` AS `姓名`,tt.time AS `时长`,tt.group AS `班次`,tt.date AS `签到时间`,"+
                "tt.mark AS `备注` FROM tt_time AS tt ,tt_staffinfo AS ts WHERE ts.staffid = tt.staffid AND tt.date > '{0}' AND "+
                "tt.date < '{1}'", datePicker7.SelectedDate, datePicker8.SelectedDate);
            DataTable dt = dbo.Selectinfo(sql);
            MyOperation.DebugPrint("导出了工时信息");
            ExcelOperation.SendToExcel(dt, "工时详情表");
        }
        private void datePicker1_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            datePicker2.BlackoutDates.Clear();
            CalendarDateRange cdr = new CalendarDateRange(new DateTime(),datePicker1.SelectedDate??(new DateTime()));
            datePicker2.BlackoutDates.Add(cdr);
        }

        private void datePicker3_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            datePicker4.BlackoutDates.Clear();
            CalendarDateRange cdr = new CalendarDateRange(new DateTime(), datePicker3.SelectedDate ?? (new DateTime()));
            datePicker4.BlackoutDates.Add(cdr);
        }

        private void datePicker5_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            datePicker6.BlackoutDates.Clear();
            CalendarDateRange cdr = new CalendarDateRange(new DateTime(), datePicker5.SelectedDate ?? (new DateTime()));
            datePicker6.BlackoutDates.Add(cdr);
        }

        private void datePicker7_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            datePicker8.BlackoutDates.Clear();
            CalendarDateRange cdr = new CalendarDateRange(new DateTime(), datePicker7.SelectedDate ?? (new DateTime()));
            datePicker8.BlackoutDates.Add(cdr);
        }




    }
}
