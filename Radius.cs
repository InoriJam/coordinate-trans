using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter
{
    class Radius
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
        //M N R RA计算
        public double[] calculate_MNR(double B)
        {
            double V, e4, b, c, M, N, R, RA30, RA60;
            B = B * Math.PI / 180;
            b = a * (1 - f);
            c = a * a / b;
            e4 = (a * a - b * b) / (b * b);
            V = Math.Sqrt(1 + e4 * Math.Pow(Math.Cos(B), 2));
            M = c / Math.Pow(V, 3);
            N = c / V;
            R = c / Math.Pow(V, 2);
            RA30 = N / (1 + e4 * Math.Pow(Math.Cos(B), 2) * Math.Pow(Math.Cos(30 * Math.PI / 180), 2));
            RA60 = N / (1 + e4 * Math.Pow(Math.Cos(B), 2) * Math.Pow(Math.Cos(60 * Math.PI / 180), 2));
            double[] MNR_arr = { M, N, R ,RA30, RA60};
            return MNR_arr;
        }
    }
}
