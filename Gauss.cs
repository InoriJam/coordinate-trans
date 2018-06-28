using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter
{
    class Gauss
    {
        //默认WGS84椭球参数
        private double a = 6378137;
        private double f = 1 / 298.257223563;
        //默认投影带宽
        private double width = 3;
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
        public double WIDTH
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
            }
        }
        //高斯正算
        public double[] gauss_positive(double L, double B)
        {
            //计算经差
            int no = (int)(L / width);
            double L0 = no * width + width / 2;
            double l = L - L0;
            B = B * Math.PI / 180;
            double b, e2, e4, n2, t, c, V, N;
            b = a * (1 - f);
            e2 = (a * a - b * b) / (a * a);
            e4 = (a * a - b * b) / (b * b);
            n2 = e4 * Math.Pow(Math.Cos(B), 2);
            t = Math.Tan(B);
            c = a * a / b;
            V = Math.Sqrt(1 + e4 * Math.Pow(Math.Cos(B), 2));
            N = c / V;
            double m0, m2, m4, m6, m8;
            m0 = a * (1 - e2);
            m2 = 3 / 2 * e2 * m0;
            m4 = 5 / 4 * e2 * m2;
            m6 = 7 / 6 * e2 * m4;
            m8 = 9 / 8 * e2 * m6;
            double a0, a2, a4, a6, a8;
            a0 = m0 + 1 / 2 * m2 + 3 / 8 * m4 + 5 / 16 * m6 + 35 / 128 * m8;
            a2 = 1 / 2 * m2 + 1 / 2 * m4 + 15 / 32 * m6 + 7 / 16 * m8;
            a4 = 1 / 8 * m4 + 3 / 16 * m6 + 7 / 32 * m8;
            a6 = 1 / 32 * m6 + 1 / 16 * m8;
            a8 = 1 / 128 * m8;
            double X = a0 * B - a2 / 2 * Math.Sin(2 * B) + a4 / 4 * Math.Sin(4 * B) - a6 / 6 * Math.Sin(6 * B) + a8 / 8 * Math.Sin(8 * B);
            double x = X + N / 2 * Math.Sin(B) * Math.Cos(B) * l * l + N / 24 * Math.Sin(B) * Math.Pow(Math.Cos(B), 3) * (5 - t * t + 9 * n2 + 4 * n2 * n2) * Math.Pow(l, 4) + N / 720 * Math.Sin(B) * Math.Pow(Math.Cos(B), 5) * (61 - 58 * t * t + Math.Pow(t, 4)) * Math.Pow(l, 6);
            double y = N * Math.Cos(B) * l + N / 6 * Math.Pow(Math.Cos(B), 3) * (1 - t * t + n2) * Math.Pow(l, 3) + N / 120 * Math.Pow(Math.Cos(B), 5) * (5 - 18 * t * t + Math.Pow(t, 4) + 14 * n2 - 58 * n2 * t * t) * Math.Pow(l, 5);
            double[] yx = { y, x };
            return yx;
        }
        //高斯反算
        public void gauss_negative()
        {

        }
    }
}
