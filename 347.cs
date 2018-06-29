using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightweightMatrixCSharp;

namespace Converter
{
    class _347
    {
        public double[] four_solve(List<double[]> src,List<double[]> dst)
        {
            string temp1 = "";
            string temp2 = "";
            for (int i = 1; i < dst.Count; i++)
            {
                temp1 += "1 0 -" + src[i][1].ToString() + " " + src[i][0].ToString() + "\r\n0 1 " + src[i][0].ToString() + " " + src[i][1].ToString() + "\r\n";
                temp2 += (dst[i][0] - src[i][0]).ToString() + "\r\n" + (dst[i][1] - src[i][1]).ToString() + "\r\n";
            }
            temp1 = temp1.Substring(0, temp1.Length - 2);
            temp2 = temp2.Substring(0, temp2.Length - 2);
            Matrix B = Matrix.Parse(temp1);
            Matrix L = Matrix.Parse(temp2);
            Matrix X = (Matrix.Transpose(B) * B).Invert() * Matrix.Transpose(B) * L;
            //Matrix res = Matrix.Parse("2343.3509\r\n910.6387") + Matrix.Parse("1 0 -910.6387 2343.3509\r\n0 1 2343.3509 910.6387") *X;
            //MessageBox.Show(res.ToString());
            return X.mat;
        }
        public double[] four_trans(double[] src, double dX,double dY,double theata,double m)
        {
            Matrix X = Matrix.Parse(dX.ToString() + "\r\n" + dY.ToString() + "\r\n" + theata.ToString() + "\r\n" + m.ToString());
            Matrix res = Matrix.Parse(src[0].ToString()+"\r\n"+src[1].ToString()) + Matrix.Parse("1 0 -"+src[1].ToString() + " " + src[0].ToString() +"\r\n0 1 "+src[0].ToString()+" "+src[1].ToString()) * X;
            return res.mat;
        }
    }
}
