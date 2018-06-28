using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Converter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            select.SelectedIndex = select.Items.IndexOf("WGS84");
            select2.SelectedIndex = select2.Items.IndexOf("WGS84");
            select3.SelectedIndex = select3.Items.IndexOf("WGS84");
            select4.SelectedIndex = select4.Items.IndexOf("3度带");
            init_DataGridview();
        }
        BLH2XYZ conv = new BLH2XYZ();
        Radius radius = new Radius();
        string res = "";
        //大地转空间
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "文本文件(*.txt)|*.txt";
            open.ShowDialog();
            if (open.FileName == "")
            {
                return;
            }
            conv.ReadFile(open.FileName);
            blh.Text = conv.raw_data;
            res = "";
            foreach(double[] BLH_arr in conv.data_arr)
            {
                double[] XYZ = conv.sub_BLH2XYZ(BLH_arr);
                res += XYZ[0].ToString()+" "+XYZ[1].ToString()+" "+XYZ[2].ToString()+"\r\n";
            }
            xyz.Text = res;
        }
        //空间转大地
        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "文本文件(*.txt)|*.txt";
            open.ShowDialog();
            if (open.FileName == "")
            {
                return;
            }
            conv.ReadFile(open.FileName);
            xyz.Text = conv.raw_data;
            res = "";
            foreach (double[] XYZ_arr in conv.data_arr)
            {
                double[] BLH = conv.sub_XYZ2BLH(XYZ_arr);
                res += BLH[0].ToString() + " " + BLH[1].ToString() + " " + BLH[2].ToString() + "\r\n";
            }
            blh.Text = res;
        }
        //选择椭球
        private void select_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (select.SelectedItem.ToString())
            {
                case "WGS84":
                    conv.A = 6378137;
                    conv.F = 1 / 298.257223563;
                    break;
                case "CGCS2000":
                    conv.A = 6378137;
                    conv.F = 1 / 298.257222101;
                    break;
                case "西安80":
                    conv.A = 6378140;
                    conv.F = 1 / 298.257;
                    break;
            }
            
        }
        //导出结果
        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "文本文件(*.txt)|*.txt";
            save.ShowDialog();
            if(save.FileName == "")
            {
                return;
            }
            FileStream fs = new FileStream(save.FileName, FileMode.Create);
            StreamWriter wt = new StreamWriter(fs);
            wt.Write(res);
            wt.Close();
        }
        //初始化Tab2的DataGridView
        DataTable dt = new DataTable();
        DataTable dt2 = new DataTable();
        private void init_DataGridview()
        {
            //曲率半径datagridview
            dt.Columns.Add("B", typeof(double));
            dt.Columns.Add("M", typeof(double));
            dt.Columns.Add("N", typeof(double));
            dt.Columns.Add("R", typeof(double));
            dt.Columns.Add("RA30", typeof(double));
            dt.Columns.Add("RA60", typeof(double));
            for (int i = 0; i <= 90; i+=10)
            {
                DataRow new_row = dt.NewRow();
                new_row["B"] = i;
                dt.Rows.Add(new_row);
            }
            dataGridView1.DataSource = dt;
            dataGridView1.Columns["B"].HeaderText = "纬度";
            dataGridView1.Columns["M"].HeaderText = "子午圈曲率半径";
            dataGridView1.Columns["N"].HeaderText = "卯酉圈曲率半径";
            dataGridView1.Columns["R"].HeaderText = "平均曲率半径";
            dataGridView1.Columns["RA30"].HeaderText = "大地方位角30°时曲率半径";
            dataGridView1.Columns["RA60"].HeaderText = "大地方位角60°时曲率半径";
            //高斯正反算datagridview
            dt2.Columns.Add("L", typeof(double));
            dt2.Columns.Add("B", typeof(double));
            dt2.Columns.Add("y", typeof(double));
            dt2.Columns.Add("x", typeof(double));
            dataGridView2.DataSource = dt2;
        }
        //计算M N R RA
        private void button4_Click(object sender, EventArgs e)
        {
            foreach(DataRow row in dt.Rows)
            {
                double[] MNR = radius.calculate_MNR((double)row["B"]);
                row["M"] = MNR[0];
                row["N"] = MNR[1];
                row["R"] = MNR[2];
                row["RA30"] = MNR[3];
                row["RA60"] = MNR[4];
            }
        }

        private void select2_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (select2.SelectedItem.ToString())
            {
                case "WGS84":
                    radius.A = 6378137;
                    radius.F = 1 / 298.257223563;
                    break;
                case "CGCS2000":
                    radius.A = 6378137;
                    radius.F = 1 / 298.257222101;
                    break;
                case "西安80":
                    radius.A = 6378140;
                    radius.F = 1 / 298.257;
                    break;
            }
        }
        //导出曲率半径计算结果
        private void button5_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "文本文件(*.txt)|*.txt";
            save.ShowDialog();
            if(save.FileName == "")
            {
                return;
            }
            FileStream fs = new FileStream(save.FileName, FileMode.Create);
            StreamWriter wt = new StreamWriter(fs);
            string header = "";
            foreach(DataGridViewColumn col in dataGridView1.Columns)
            {
                header += col.HeaderText + "\t\t";
            }
            header.Substring(0, header.Length - 2);
            wt.WriteLine(header);
            foreach(DataRow row in dt.Rows)
            {
                wt.Write(row[0].ToString());
                string temp = "";
                for (int i=1;i<dt.Columns.Count;i++)
                {
                    temp += "\t" + string.Format("{0:f8}",row[i]) + "\t";
                }
                temp.Substring(0, temp.Length - 1);
                wt.WriteLine(temp);
            }
            wt.Close();
            fs.Close();
        }
        //选择投影带宽
        Gauss gauss = new Gauss();
        private void select4_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (select4.SelectedItem.ToString())
            {
                case "3度带":
                    gauss.WIDTH = 3;
                    break;
                case "6度带":
                    gauss.WIDTH = 6;
                    break;
            }
        }
        //选择高斯投影椭球
        private void select3_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (select3.SelectedItem.ToString())
            {
                case "WGS84":
                    gauss.A = 6378137;
                    gauss.F = 1 / 298.257223563;
                    break;
                case "CGCS2000":
                    gauss.A = 6378137;
                    gauss.F = 1 / 298.257222101;
                    break;
                case "西安80":
                    gauss.A = 6378140;
                    gauss.F = 1 / 298.257;
                    break;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "文本文件(*.txt)|*.txt";
            open.ShowDialog();
            if(open.FileName == "")
            {
                return;
            }
            FileOperate fo = new FileOperate();
            List<double[]> data = fo.ReadFile(open.FileName);
            foreach(double[] ele in data)
            {
                DataRow new_row = dt2.NewRow();
                new_row["L"] = ele[0];
                new_row["B"] = ele[1];
                dt2.Rows.Add(new_row);
            }
            foreach(DataRow row in dt2.Rows)
            {
                 double[]yx_arr = gauss.gauss_positive((double)row["L"], (double)row["B"]);
                 row["y"] = yx_arr[0];
                 row["x"] = yx_arr[1];
            }
        }
    }
}
