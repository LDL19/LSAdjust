using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LSServer
{
    /// <summary>
    /// 计算有关矩阵的运算，是静态类
    /// </summary>
    static class Matrix
    {
        /// <summary>
        /// 矩阵的转置
        /// </summary>
        /// <param name="mat">matirx</param>
        /// <returns>矩阵的转置</returns>
        static public float[,] T(float[,] mat)
        {
            int m = mat.GetUpperBound(0) + 1;
            int n = mat.GetUpperBound(1) + 1;
            float[,] trans = new float[n, m];
            int i, j;
            for (i = 0; i < n; i++)
                for (j = 0; j < m; j++)
                    trans[i, j] = mat[j, i];
            return trans;
        }

        /// <summary>
        /// 矩阵的乘积
        /// </summary>
        /// <param name="lmat">第一个矩阵A</param>
        /// <param name="rmat">第二个矩阵B</param>
        /// <returns>矩阵的乘积：A*B</returns>
        static public float[,] Multi(float[,] lmat, float[,] rmat)
        {
            int m = lmat.GetUpperBound(0) + 1;
            int n = lmat.GetUpperBound(1) + 1;
            int n1 = rmat.GetUpperBound(0) + 1;
            int t = rmat.GetUpperBound(1) + 1;
            float[,] result = new float[m, t];
            if (n != n1)
                throw new System.ArgumentException("两矩阵的大小不匹配，不能相乘");
            int i, j, k;
            float temp;
            for (i = 0; i < m; i++)
                for (j = 0; j < t; j++)
                {
                    temp = 0;
                    for (k = 0; k < n; k++)
                        temp += lmat[i, k] * rmat[k, j];
                    result[i, j] = temp;
                }
            return result;
        }
        static public float[] Multi1(float[,] lmat,float[] rmat)
        {
            int m = lmat.GetUpperBound(0) + 1;
            int n = lmat.GetUpperBound(1) + 1;
            int n1 = rmat.GetUpperBound(0) + 1;
            float[] result = new float[m];
            if (n != n1)
                throw new System.ArgumentException("两矩阵的大小不匹配，不能相乘");
            int i, j;
            float temp;
            for (i = 0; i < m; i++)
            {
                temp = 0;
                for (j = 0; j < n1; j++)
                {
                        temp += lmat[i, j] * rmat[ j];
                }
                result[i] = temp;
            
            }
            return result;
        }

        //staitc public bool MultiplyMatrix1(float[] Res, float[,] LeftM, float[] RightM)
        //{

        //    int i, j;
        //    float Tem;
        //    for (i = 0; i < 2; i++)
        //    {
        //        Tem = 0;
        //        for (j = 0; j < 30; j++)
        //        {
        //                Tem = Tem + LeftM[i, j] * RightM[j];
        //        }
        //            Res[i] = Tem;
        //    }
        //    return true;
        //}

        /// <summary>
        /// 求矩阵的行列式
        /// </summary>
        /// <param name="mat">matrix</param>
        /// <returns>行列式的值</returns>
        static public float Det(float[,] mat)
        {
            int m = mat.GetUpperBound(0) + 1;
            int n = mat.GetUpperBound(1) + 1;
            if (m != n)
                throw new System.ArgumentException("该矩阵，不是方阵不能求逆！");
            if (n == 1)
                return mat[0, 0];
            if (n == 2)
                return mat[1, 1] * mat[0, 0] - mat[1, 0] * mat[0, 1];

            float det = 0;
            float[,] temp = new float[n - 1, n - 1];
            int i, j, k;
            for (i = 0; i < n; i++)
            {
                for (j = 0; j < n - 1; j++)
                    for (k = 0; k < n - 1; k++)
                        temp[j, k] = mat[j + 1, ((k >= i) ? k + 1 : k)];
                float t = Det(temp);
                if (i % 2 == 0)
                {
                    det += mat[0, i] * t;
                }
                else
                {
                    det -= mat[0, i] * t;
                }
            }
            return det;
        }
        /// <summary>
        /// 矩阵求逆
        /// </summary>
        /// <param name="mat">matrix</param>
        /// <returns>矩阵的逆A^-1</returns>
        static public float[,] Inv(float[,] mat)
        {
            int m = mat.GetUpperBound(0) + 1;
            int n = mat.GetUpperBound(1) + 1;
            if (m != n)
                throw new System.ArgumentException("不是方阵，无法求逆！");
            float[,] inv = new float[n, n];
            float[,] temp = new float[n, n + n];
            //构造增广矩阵,你就真的构造增广矩阵？
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                {
                    temp[i, j] = mat[i, j];
                    temp[i, n + j] = ((i == j) ? 1 : 0);
                }
            //换列主元，在主元列找出绝对最大的值作为主元
            float at, bt, am;
            int tt;
            for (int k = 0; k < n; k++)
            {
                at = Math.Abs(temp[k, k]);
                tt = k;
                for (int j = k + 1; j < n; j++)
                {
                    bt = Math.Abs(temp[j, k]);
                    if (at < bt)
                    {
                        at = bt;
                        tt = j;
                    }
                }
                if (tt != k)
                {
                    //for (int j = k; j < n + n; j++)
                    //am = temp[k, j]; temp[k, j] = temp[tt, j]; temp[tt, j] = am;
                }
                am = 1 / temp[k, k];
                for (int j = k; j < n + n; j++)
                    temp[k, j] = temp[k, j] * am;
                for (int i = 0; i < n; i++)
                    if (k != i)
                    {
                        am = temp[i, k];
                        for (int j = 0; j < n + n; j++)
                            temp[i, j] = temp[i, j] - temp[k, j] * am;
                    }
            }

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    inv[i, j] = temp[i, j + n];
            return inv;
        }

        /// <summary>
        /// 用于矩阵的测试
        /// </summary>
        static public void TestMatirx()
        {
            float[,] mat1 = new float[4,4];
            for (int i = 0; i <= mat1.GetUpperBound(0); i++)
                for (int j = 0; j <= mat1.GetUpperBound(1); j++)
                {
                    mat1[i, j] = 1;
                    if (i == j)
                        mat1[i, j] = 3;
                }
            Console.WriteLine(Det(mat1)); //48
            float[] mat2 = new float[4];
            for (int i = 0; i <= mat2.GetUpperBound(0); i++)
                mat2[i] = i;
            mat2 = Multi1(mat1, mat2);
            for (int i = 0; i <=mat1.GetUpperBound(0); i++)
                for (int j = 0; j <=mat1.GetUpperBound(1); j++)
                {
                    mat1[i, j] = 0;
                    if (i == j)
                        mat1[i, j] = 3;
                }
            mat1 = Inv(mat1);

        }
    }
}
