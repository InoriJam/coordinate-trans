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
            comboBox1.SelectedIndex = comboBox1.Items.IndexOf("四参数");
            groupBox2.Location = new Point(724, 97);
            groupBox3.Location = new Point(724, 97);
            groupBox2.Visible = false;
            groupBox3.Visible = false;
            button9.Visible = false;
            button11.Enabled = false;
            init_DataGridview();
        }
        BLH2XYZ conv = new BLH2XYZ();
        Radius radius = new Radius();
        List<double[]> XYZBLH_res = new List<double[]>();
        //大地转空间
        private void button1_Click(object sender, EventArgs e)
        {
            XYZBLH_res.Clear();
            dt3.Rows.Clear();
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "文本文件(*.txt)|*.txt";
            open.ShowDialog();
            if (open.FileName == "")
            {
                return;
            }
            FileOperate fo = new FileOperate();
            List<double[]> data = fo.ReadFile(open.FileName);
            foreach (double[] arr in data)
            {
                double[] temp = conv.sub_BLH2XYZ(arr);
                DataRow new_row = dt3.NewRow();
                new_row["L"] = arr[0];
                new_row["B"] = arr[1];
                new_row["H"] = arr[2];
                new_row["X"] = temp[0];
                new_row["Y"] = temp[1];
                new_row["Z"] = temp[2];
                XYZBLH_res.Add(temp);
                dt3.Rows.Add(new_row);
            }
        }
        //空间转大地
        private void button2_Click(object sender, EventArgs e)
        {
            XYZBLH_res.Clear();
            dt3.Rows.Clear();
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "文本文件(*.txt)|*.txt";
            open.ShowDialog();
            if (open.FileName == "")
            {
                return;
            }
            FileOperate fo = new FileOperate();
            List<double[]> data = fo.ReadFile(open.FileName);
            foreach(double[] arr in data)
            {
                double[] temp = conv.sub_XYZ2BLH(arr);
                DataRow new_row = dt3.NewRow();
                new_row["X"] = arr[0];
                new_row["Y"] = arr[1];
                new_row["Z"] = arr[2];
                new_row["L"] = temp[0];
                new_row["B"] = temp[1];
                new_row["H"] = temp[2];
                XYZBLH_res.Add(temp);
                dt3.Rows.Add(new_row);
            }
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
        //导出BLHXYZ结果
        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "文本文件(*.txt)|*.txt";
            save.ShowDialog();
            if(save.FileName == "")
            {
                return;
            }
            FileOperate fo = new FileOperate();
            fo.ExportFile(save.FileName, XYZBLH_res);
        }
        //初始化Tab2的DataGridView
        DataTable dt = new DataTable();
        DataTable dt2 = new DataTable();
        DataTable dt3 = new DataTable();
        DataTable dt4 = new DataTable();
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
            //BLH2XYZ的datagridview
            dt3.Columns.Add("X", typeof(double));
            dt3.Columns.Add("Y", typeof(double));
            dt3.Columns.Add("Z", typeof(double));
            dt3.Columns.Add("B", typeof(double));
            dt3.Columns.Add("L", typeof(double));
            dt3.Columns.Add("H", typeof(double));
            dataGridView3.DataSource = dt3;
            //参数转换datagridview
            dataGridView4.DataSource = dt4;
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
            FileOperate fo = new FileOperate();
            fo.datagrid_ExportFile(save.FileName, dataGridView1, dt);
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
        //高斯正反算结果存储
        List<double[]> gauss_positive_res = new List<double[]>();
        List<double[]> gauss_negative_res = new List<double[]>();
        //高斯正算
        private void button7_Click(object sender, EventArgs e)
        {
            gauss_positive_res.Clear();
            gauss_negative_res.Clear();
            dt2.Rows.Clear();
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
                 double[]yx_arr = gauss.gauss_positive((double)row["L"], (double)row["B"], checkBox3);
                 gauss_positive_res.Add(yx_arr);
                 row["y"] = yx_arr[0];
                 row["x"] = yx_arr[1];
            }
        }
        //高斯正反算结果导出
        private void button6_Click(object sender, EventArgs e)
        {
            FileOperate fo = new FileOperate();
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "文本文件(*.txt)|*.txt";
            save.ShowDialog();
            if (save.FileName == "")
            {
                return;
            }
            if(gauss_positive_res.Count == 0 && gauss_negative_res.Count == 0)
            {
                MessageBox.Show("请先进行计算！");
                return;
            }
            else if (gauss_positive_res.Count != 0)
            {
                fo.ExportFile(save.FileName, gauss_positive_res);
            }
            else if (gauss_negative_res.Count != 0)
            {
                fo.ExportFile(save.FileName, gauss_negative_res);
            }
        }
        //高斯反算
        private void button8_Click(object sender, EventArgs e)
        {
            gauss_positive_res.Clear();
            gauss_negative_res.Clear();
            dt2.Rows.Clear();
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "文本文件(*.txt)|*.txt";
            open.ShowDialog();
            if (open.FileName == "")
            {
                return;
            }
            FileOperate fo = new FileOperate();
            List<double[]> data = fo.ReadFile(open.FileName);
            foreach (double[] ele in data)
            {
                DataRow new_row = dt2.NewRow();
                new_row["y"] = ele[0];
                new_row["x"] = ele[1];
                dt2.Rows.Add(new_row);
            }
            foreach (DataRow row in dt2.Rows)
            {
                double[] LB_arr = gauss.gauss_negative((double)row["y"], (double)row["x"]);
                gauss_negative_res.Add(LB_arr);
                row["L"] = LB_arr[0];
                row["B"] = LB_arr[1];
            }
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedItem.ToString())
            {
                case "四参数":
                    groupBox2.Visible = false;
                    groupBox3.Visible = false;
                    groupBox1.Visible = true;
                    dt4.Rows.Clear();
                    dt4.Columns.Clear();
                    dt4.Columns.Add("X0", typeof(double));
                    dt4.Columns.Add("Y0", typeof(double));
                    dt4.Columns.Add("X1", typeof(double));
                    dt4.Columns.Add("Y1", typeof(double));
                    break;
                case "七参数":
                    groupBox1.Visible = false;
                    groupBox3.Visible = false;
                    groupBox2.Visible = true;
                    //清空dt4，重新生成字段
                    dt4.Rows.Clear();
                    dt4.Columns.Clear();
                    dt4.Columns.Add("X0", typeof(double));
                    dt4.Columns.Add("Y0", typeof(double));
                    dt4.Columns.Add("Z0", typeof(double));
                    dt4.Columns.Add("X1", typeof(double));
                    dt4.Columns.Add("Y1", typeof(double));
                    dt4.Columns.Add("Z1", typeof(double));
                    break;
                case "三参数":
                    groupBox1.Visible = false;
                    groupBox2.Visible = false;
                    groupBox3.Visible = true;
                    dt4.Rows.Clear();
                    dt4.Columns.Clear();
                    dt4.Columns.Add("X0", typeof(double));
                    dt4.Columns.Add("Y0", typeof(double));
                    dt4.Columns.Add("Z0", typeof(double));
                    dt4.Columns.Add("X1", typeof(double));
                    dt4.Columns.Add("Y1", typeof(double));
                    dt4.Columns.Add("Z1", typeof(double));
                    break;
            }
        }
        //参数转换
        //存储转换结果
        List<double[]> trans_res = new List<double[]>();
        private void button10_Click(object sender, EventArgs e)
        {
            dt4.Rows.Clear();
            trans_res.Clear();
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "文本文件(*.txt)|*.txt";
            open.ShowDialog();
            if (open.FileName == "")
            {
                return;
            }
            FileOperate fo = new FileOperate();
            List<double[]> data = fo.ReadFile(open.FileName);
            _347 tfs = new _347();
            foreach (double[] arr in data)
            {
                //datagridview显示
                DataRow new_row = dt4.NewRow();
                switch (comboBox1.SelectedItem.ToString()){
                    case "四参数":
                        new_row["X0"] = arr[0];
                        new_row["Y0"] = arr[1];
                        double[] four_res_arr = tfs.four_trans(arr, dX_box.Text, dY_box.Text, theata_box.Text, m_box.Text);
                        new_row["X1"] = four_res_arr[0];
                        new_row["Y1"] = four_res_arr[1];
                        dt4.Rows.Add(new_row);
                        trans_res.Add(four_res_arr);
                        break;
                    case "七参数":
                        new_row["X0"] = arr[0];
                        new_row["Y0"] = arr[1];
                        new_row["Z0"] = arr[2];
                        double[] seven_res_arr = tfs.seven_trans(arr, Tx_box.Text, Ty_box.Text, Tz_box.Text, D_box.Text, Rx_box.Text, Ry_box.Text, Rz_box.Text);
                        new_row["X1"] = seven_res_arr[0];
                        new_row["Y1"] = seven_res_arr[1];
                        new_row["Z1"] = seven_res_arr[2];
                        dt4.Rows.Add(new_row);
                        trans_res.Add(seven_res_arr);
                        break;
                    case "三参数":
                        new_row["X0"] = arr[0];
                        new_row["Y0"] = arr[1];
                        new_row["Z0"] = arr[2];
                        double[] three_res_arr = tfs.three_trans(arr,t_Dx_box.Text, t_Dy_box.Text, t_Dz_box.Text);
                        new_row["X1"] = three_res_arr[0];
                        new_row["Y1"] = three_res_arr[1];
                        new_row["Z1"] = three_res_arr[2];
                        dt4.Rows.Add(new_row);
                        trans_res.Add(three_res_arr);
                        break;
                }
            }
            button11.Enabled = true;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            FileOperate fo = new FileOperate();
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "文本文件(*.txt)|*.txt";
            save.ShowDialog();
            if (save.FileName == "")
            {
                return;
            }
            fo.ExportFile(save.FileName, trans_res);
        }
        //设置是否输入转换参数
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked == false)
            {
                button12.Enabled = true;
                foreach (Control c in groupBox1.Controls.OfType<TextBox>())
                {
                    TextBox tb = c as TextBox;
                    tb.ReadOnly = true;
                }
                foreach (Control c in groupBox2.Controls.OfType<TextBox>())
                {
                    TextBox tb = c as TextBox;
                    tb.ReadOnly = true;
                }
                foreach (Control c in groupBox3.Controls.OfType<TextBox>())
                {
                    TextBox tb = c as TextBox;
                    tb.ReadOnly = true;
                }
            }
            else if (checkBox1.Checked == true)
            {
                button12.Enabled = false;
                foreach (Control c in groupBox1.Controls.OfType<TextBox>())
                {
                    TextBox tb = c as TextBox;
                    tb.ReadOnly = false;
                }
                foreach (Control c in groupBox2.Controls.OfType<TextBox>())
                {
                    TextBox tb = c as TextBox;
                    tb.ReadOnly = false;
                }
                foreach (Control c in groupBox3.Controls.OfType<TextBox>())
                {
                    TextBox tb = c as TextBox;
                    tb.ReadOnly = false;
                }
            }
        }
        //计算转换参数
        private void button12_Click(object sender, EventArgs e)
        {
            dt4.Rows.Clear();
            trans_res.Clear();
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "文本文件(*.txt)|*.txt";
            open.ShowDialog();
            if (open.FileName == "")
            {
                return;
            }
            FileOperate fo = new FileOperate();
            List<double[]> data = fo.ReadFile(open.FileName);
            switch (comboBox1.SelectedItem.ToString())
            {
                case "四参数":
                    //分离源坐标系和目标坐标系的坐标
                    List<double[]> four_src = new List<double[]>();
                    List<double[]> four_dst = new List<double[]>();
                    foreach (double[] arr in data)
                    {
                        //datagridview显示
                        DataRow new_row = dt4.NewRow();
                        new_row["X0"] = arr[0];
                        new_row["Y0"] = arr[1];
                        new_row["X1"] = arr[2];
                        new_row["Y1"] = arr[3];
                        dt4.Rows.Add(new_row);
                        double[] left = { arr[0], arr[1] };
                        double[] right = { arr[2], arr[3] };
                        four_src.Add(left);
                        four_dst.Add(right);
                    }
                    _347 four_tfs = new _347();
                    double[] four_param_arr = four_tfs.four_solve(four_src, four_dst);
                    dX_box.Text = four_param_arr[0].ToString();
                    dY_box.Text = four_param_arr[1].ToString();
                    theata_box.Text = four_param_arr[2].ToString();
                    m_box.Text = four_param_arr[3].ToString();
                    break;
                case "七参数":
                    //分离源坐标系和目标坐标系的坐标
                    List<double[]> seven_src = new List<double[]>();
                    List<double[]> seven_dst = new List<double[]>();
                    foreach (double[] arr in data)
                    {
                        //datagridview显示
                        DataRow new_row = dt4.NewRow();
                        new_row["X0"] = arr[0];
                        new_row["Y0"] = arr[1];
                        new_row["Z0"] = arr[2];
                        new_row["X1"] = arr[3];
                        new_row["Y1"] = arr[4];
                        new_row["Z1"] = arr[5];
                        dt4.Rows.Add(new_row);
                        double[] left = { arr[0], arr[1],arr[2] };
                        double[] right = { arr[3], arr[4],arr[5] };
                        seven_src.Add(left);
                        seven_dst.Add(right);
                    }
                    _347 seven_tfs = new _347();
                    double[] seven_param_arr = seven_tfs.seven_solve(seven_src, seven_dst);
                    Tx_box.Text = seven_param_arr[0].ToString();
                    Ty_box.Text = seven_param_arr[1].ToString();
                    Tz_box.Text = seven_param_arr[2].ToString();
                    Rx_box.Text = seven_param_arr[3].ToString();
                    Ry_box.Text = seven_param_arr[4].ToString();
                    Rz_box.Text = seven_param_arr[5].ToString();
                    D_box.Text = seven_param_arr[6].ToString();
                    break;
                case "三参数":
                    //分离源坐标系和目标坐标系的坐标
                    List<double[]> three_src = new List<double[]>();
                    List<double[]> three_dst = new List<double[]>();
                    foreach (double[] arr in data)
                    {
                        //datagridview显示
                        DataRow new_row = dt4.NewRow();
                        new_row["X0"] = arr[0];
                        new_row["Y0"] = arr[1];
                        new_row["Z0"] = arr[2];
                        new_row["X1"] = arr[3];
                        new_row["Y1"] = arr[4];
                        new_row["Z1"] = arr[5];
                        dt4.Rows.Add(new_row);
                        double[] left = { arr[0], arr[1], arr[2] };
                        double[] right = { arr[3], arr[4], arr[5] };
                        three_src.Add(left);
                        three_dst.Add(right);
                    }
                    _347 three_tfs = new _347();
                    double[] three_param_arr = three_tfs.three_solve(three_src, three_dst);
                    t_Dx_box.Text = three_param_arr[0].ToString();
                    t_Dy_box.Text = three_param_arr[1].ToString();
                    t_Dz_box.Text = three_param_arr[2].ToString();
                    break;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                button9.Visible = true;
                button12.Visible = false;
            }
            else if (checkBox2.Checked == false)
            {
                button9.Visible = false;
                button12.Visible = true;
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            List<double[]> data = new List<double[]>();
            foreach(DataRow row in dt4.Rows)
            {
                List<double> temp = new List<double>();
                foreach(DataColumn col in dt4.Columns)
                {
                    temp.Add((double)row[col]);
                }
                double[] temp_arr = temp.ToArray();
                data.Add(temp_arr);
            }
            switch (comboBox1.SelectedItem.ToString())
            {
                case "四参数":
                    //分离源坐标系和目标坐标系的坐标
                    List<double[]> four_src = new List<double[]>();
                    List<double[]> four_dst = new List<double[]>();
                    foreach (double[] arr in data)
                    {
                        double[] left = { arr[0], arr[1] };
                        double[] right = { arr[2], arr[3] };
                        four_src.Add(left);
                        four_dst.Add(right);
                    }
                    _347 four_tfs = new _347();
                    double[] four_param_arr = four_tfs.four_solve(four_src, four_dst);
                    dX_box.Text = four_param_arr[0].ToString();
                    dY_box.Text = four_param_arr[1].ToString();
                    theata_box.Text = four_param_arr[2].ToString();
                    m_box.Text = four_param_arr[3].ToString();
                    break;
                case "七参数":
                    //分离源坐标系和目标坐标系的坐标
                    List<double[]> seven_src = new List<double[]>();
                    List<double[]> seven_dst = new List<double[]>();
                    foreach (double[] arr in data)
                    {
                        double[] left = { arr[0], arr[1], arr[2] };
                        double[] right = { arr[3], arr[4], arr[5] };
                        seven_src.Add(left);
                        seven_dst.Add(right);
                    }
                    _347 seven_tfs = new _347();
                    double[] seven_param_arr = seven_tfs.seven_solve(seven_src, seven_dst);
                    Tx_box.Text = seven_param_arr[0].ToString();
                    Ty_box.Text = seven_param_arr[1].ToString();
                    Tz_box.Text = seven_param_arr[2].ToString();
                    Rx_box.Text = seven_param_arr[3].ToString();
                    Ry_box.Text = seven_param_arr[4].ToString();
                    Rz_box.Text = seven_param_arr[5].ToString();
                    D_box.Text = seven_param_arr[6].ToString();
                    break;
                case "三参数":
                    //分离源坐标系和目标坐标系的坐标
                    List<double[]> three_src = new List<double[]>();
                    List<double[]> three_dst = new List<double[]>();
                    foreach (double[] arr in data)
                    {
                        double[] left = { arr[0], arr[1], arr[2] };
                        double[] right = { arr[3], arr[4], arr[5] };
                        three_src.Add(left);
                        three_dst.Add(right);
                    }
                    _347 three_tfs = new _347();
                    double[] three_param_arr = three_tfs.three_solve(three_src, three_dst);
                    t_Dx_box.Text = three_param_arr[0].ToString();
                    t_Dy_box.Text = three_param_arr[1].ToString();
                    t_Dz_box.Text = three_param_arr[2].ToString();
                    break;
            }
        }
    }
}
