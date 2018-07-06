using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Data;

namespace Converter
{
    class FileOperate
    {
        //读取文件
        public List<double[]> ReadFile(string path)
        {
            List<double[]> data_arr = new List<double[]>();
            FileStream fs = new FileStream(path, FileMode.Open);
            StreamReader rd = new StreamReader(fs);
            List<string> data = new List<string>();
            while (true)
            {
                string temp = rd.ReadLine();
                if (temp == null)
                {
                    break;
                }
                data.Add(temp);
            }
            foreach (string line in data)
            {
                data_arr.Add(Array.ConvertAll<string, double>(line.Split('\t'), s => double.Parse(s)));
            }
            rd.Close();
            fs.Close();
            return data_arr;
        }
        //Datagridview导出到文件
        public void datagrid_ExportFile(string path,DataGridView datagrid,DataTable dt)
        {
            FileStream fs = new FileStream(path, FileMode.Create);
            StreamWriter wt = new StreamWriter(fs);
            string header = "";
            foreach (DataGridViewColumn col in datagrid.Columns)
            {
                header += col.HeaderText + "\t";
            }
            header = header.Substring(0, header.Length - 1);
            wt.WriteLine(header);
            foreach (DataRow row in dt.Rows)
            {
                wt.Write(row[0].ToString()+"\t");
                string temp = "";
                for (int i = 1; i < dt.Columns.Count; i++)
                {
                    temp += string.Format("{0:f3}", row[i]) + "\t";
                }
                temp = temp.Substring(0, temp.Length - 1);
                wt.WriteLine(temp);
            }
            wt.Close();
            fs.Close();
        }
        //数组导出到文件
        public void ExportFile(string path,List<double[]>data)
        {
            FileStream fs = new FileStream(path, FileMode.Create);
            StreamWriter wt = new StreamWriter(fs);
            foreach (double[] ele in data)
            {
                string temp = "";
                foreach (double num in ele)
                {
                    temp += string.Format("{0:f8}", num) + "\t";
                }
                temp = temp.Substring(0, temp.Length - 1);
                wt.WriteLine(temp);
            }
            wt.Close();
            fs.Close();
        }
    }
}
