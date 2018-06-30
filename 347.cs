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
        //四参数求解
        public double[] four_solve(List<double[]> src,List<double[]> dst)
        {
            string temp1 = "";
            string temp2 = "";
            for (int i = 0; i < dst.Count; i++)
            {
                temp1 += "1 0 " + (-src[i][1]).ToString() + " " + src[i][0].ToString() + "\r\n0 1 " + src[i][0].ToString() + " " + src[i][1].ToString() + "\r\n";
                temp2 += (dst[i][0] - src[i][0]).ToString() + "\r\n" + (dst[i][1] - src[i][1]).ToString() + "\r\n";
            }
            temp1 = temp1.Substring(0, temp1.Length - 2);
            temp2 = temp2.Substring(0, temp2.Length - 2);
            Matrix B = Matrix.Parse(temp1);
            Matrix L = Matrix.Parse(temp2);
            Matrix X = (Matrix.Transpose(B) * B).Invert() * Matrix.Transpose(B) * L;
            return X.mat;
        }
        //四参数转换
        public double[] four_trans(double[] src, string dX,string dY,string theata,string m)
        {
            Matrix X = Matrix.Parse(dX + "\r\n" + dY + "\r\n" + theata + "\r\n" + m);
            Matrix res = Matrix.Parse(src[0].ToString()+"\r\n"+src[1].ToString()) + Matrix.Parse("1 0 "+(-src[1]).ToString() + " " + src[0].ToString() +"\r\n0 1 "+src[0].ToString()+" "+src[1].ToString()) * X;
            return res.mat;
        }
        //七参数求解
        public double[] seven_solve(List<double[]> src,List<double[]> dst)
        {
            string k = "";
            string v = "";
            for (int i = 0; i < dst.Count; i++)
            {
                k += "1 0 0 0 " + (-src[i][2]).ToString() + " " + src[i][1].ToString() + " " + src[i][0].ToString() + "\r\n" + "0 1 0 " + src[i][2].ToString() + " 0 " + (-src[i][0]).ToString() + " " + src[i][1].ToString() + "\r\n" + "0 0 1 " + (-src[i][1]).ToString() + " " + src[i][0].ToString() + " 0 " + src[i][2].ToString() + "\r\n";
                v += (dst[i][0] - src[i][0]).ToString() + "\r\n" + (dst[i][1] - src[i][1]).ToString() + "\r\n" + (dst[i][2] - src[i][2]).ToString() + "\r\n";
            }
            k = k.Substring(0, k.Length - 2);
            v = v.Substring(0, v.Length - 2);
            Matrix K = Matrix.Parse(k);
            Matrix V = Matrix.Parse(v);
            Matrix X = (Matrix.Transpose(K) * K).Invert() * Matrix.Transpose(K) * V;
            return X.mat;
        }
        //七参数转换
        public double[] seven_trans(double[] src, string Tx, string Ty, string Tz, string D, string Rx, string Ry, string Rz)
        {
            Matrix e1 = Matrix.Parse(src[0].ToString() + "\r\n" + src[1].ToString() + "\r\n" + src[2].ToString());
            Matrix e2 = Matrix.Parse(Tx + "\r\n" + Ty + "\r\n" + Tz);
            Matrix e3 = Matrix.Parse(D + " " + Rz + " " + (-Convert.ToDouble(Ry)).ToString() + "\r\n" + (-Convert.ToDouble(Rz)).ToString() + " " + D + " " + Rx + "\r\n" + Ry + " " + (-Convert.ToDouble(Rx)).ToString() + " " + D);
            Matrix e4 = Matrix.Parse(src[0].ToString() + "\r\n" + src[1].ToString() + "\r\n" + src[2].ToString());
            Matrix res = e1 + e2 + e3 * e4;
            return res.mat;
        }
    }
}
