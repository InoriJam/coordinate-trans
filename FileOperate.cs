using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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
                data_arr.Add(Array.ConvertAll<string, double>(line.Split(' '), s => double.Parse(s)));
            }
            rd.Close();
            fs.Close();
            return data_arr;
        }
    }
}
