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
using System.Windows.Shapes;
using System.Data;

namespace 淘淘管理系统
{
    /// <summary>
    /// 离线查询书主信息.xaml 的交互逻辑
    /// </summary>
    public partial class 离线查询书主信息 : Window
    {
        DataTable dt = new DataTable() ;
        public 离线查询书主信息()
        {
            InitializeComponent();
            dt = ExcelOperation.CsvToDataTable("LocalSellerInfo.csv");
            dataGrid.ItemsSource = dt.DefaultView;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.textBox1.Text))
            {
                MyOperation.MessageShow("请输入查询的书主信息");
                this.textBox1.Focus();
                return;
            }
            DataTable newdt = new DataTable();
            newdt = dt.Clone(); // 克隆dt 的结构，包括所有 dt 架构和约束,并无数据； 
            DataRow[] rows = dt.Select(string.Format("name LIKE '%{0}%' OR sellerid = '{1}'", textBox1.Text, textBox1.Text)); // 从dt 中查询符合条件的记录； 
            foreach (DataRow row in rows)  // 将查询的结果添加到newdt中； 
            {
                newdt.Rows.Add(row.ItemArray);
            }
            dataGrid.ItemsSource = newdt.DefaultView;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.textBox1.Text))
            {
                MyOperation.MessageShow("请输入查询的书主信息");
                this.textBox1.Focus();
                return;
            }
            DataTable newdt = new DataTable();
            newdt = dt.Clone(); // 克隆dt 的结构，包括所有 dt 架构和约束,并无数据； 
            DataRow[] rows = dt.Select(string.Format("phone LIKE '%{0}%'", textBox1.Text)); // 从dt 中查询符合条件的记录； 
            foreach (DataRow row in rows)  // 将查询的结果添加到newdt中； 
            {
                newdt.Rows.Add(row.ItemArray);
            }
            dataGrid.ItemsSource = newdt.DefaultView;
        }
    }
}
