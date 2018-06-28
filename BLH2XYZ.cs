using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace Converter
{
    class BLH2XYZ
    {
        //默认WGS84椭球参数
        private double a = 6378137;
        private double f = 1 / 298.257223563;
        public double A
        {
            get
            {
                return a;
            }
            set
            {
                a = value;
            }
        }
        public double F
        {
            get
            {
                return f;
            }
            set
            {
                f = value;
            }
        }
        //读取外部txt文件
        public List<double[]> data_arr = new List<double[]>();
        public string raw_data = "";
        public void ReadFile(string path)
        {
            raw_data = "";
            data_arr.Clear();
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
                raw_data += temp + "\r\n";
            }
            foreach (string line in data) {
                data_arr.Add( Array.ConvertAll<string, double>(line.Split(' '), s => double.Parse(s)));
            }
            rd.Close();
            fs.Close();
        }
        //大地坐标转空间直角坐标
        public double[] sub_BLH2XYZ(double[] BLH_arr)
        {
            //double []BLH_arr = ReadFile(path);
            double lon = BLH_arr[0];
            double lat = BLH_arr[1];
            double H1 = BLH_arr[2];
            double b = a * (1 - f);
            double e = Math.Sqrt(a * a - b * b) / a;
            double N = a / Math.Sqrt(1 - e * e * Math.Sin(lat * Math.PI / 180) * Math.Sin(lat * Math.PI / 180));
            double X = (N + H1) * Math.Cos(lat * Math.PI / 180) * Math.Cos(lon * Math.PI / 180);
            double Y = (N + H1) * Math.Cos(lat * Math.PI / 180) * Math.Sin(lon * Math.PI / 180);
            double Z = (N * (1 - (e * e)) + H1) * Math.Sin(lat * Math.PI / 180);
            double[] XYZ = new double[]{ X, Y, Z };
            return XYZ;
        }
        //空间直角转大地坐标
        public double[] sub_XYZ2BLH(double[] XYZ_arr)
        {
            //double[] XYZ_arr = ReadFile(path);
            double X, Y, Z, b, e2, e4, L, t0, N, B;
            X = XYZ_arr[0];
            Y = XYZ_arr[1];
            Z = XYZ_arr[2];
            b = a * (1 - f);
            e2 = (a * a - b * b) / (a * a);
            e4 = (a * a - b * b) / (b * b);
            t0 = Z / Math.Sqrt(X * X + Y * Y);
            double t = t0;
            while (true)
            {
                double last_t = t;
                B = Math.Atan(t);
                N = a / Math.Sqrt(1 - e2 * Math.Pow(Math.Sin(B),2));
                t = (Z + N * e2 * Math.Sin(B)) / Math.Sqrt(X * X + Y * Y);
                if(Math.Abs (t-last_t) <= 5e-10)
                {
                    break;
                }
            }
            B = Math.Atan(t);
            double H = Z / Math.Sin(B) - N*(1 - e2);
            L = Math.Atan(Y / X) * 180 / Math.PI;
            if (L < 0)
            {
                L = L + 180;
            }
            B = B * 180 / Math.PI;
            double[] BLH = { L, B, H };
            return BLH;
        }
    }
}
