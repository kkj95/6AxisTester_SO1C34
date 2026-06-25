using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZ4P
{
    public class OISLinCompCoefDW
    {
        public readonly int INPUT_NUM;
        private int _version = 10;
        private int _versionDetail = 240424;

        public double[] g_fRealStroke;                 // LDM data
        public double[] g_fFit_RealStroke;
        public double[] g_fFit_RealStroke_temp;
        public double[] g_fIdealStroke;                 // ideal data
        public double[] g_fRealTargetScale;                 // LDM data to Target Scale
        public int[] g_nTargetPoint;                 // target capture

        public double[] output_debug;

        public int Version { get => _version; }
        public int VersionDetail { get => _versionDetail; }

        public OISLinCompCoefDW(int InputNumber)
        {
            INPUT_NUM = InputNumber;

            g_fRealStroke = new double[INPUT_NUM];                          // LDM data
            g_fFit_RealStroke = new double[INPUT_NUM];
            g_fFit_RealStroke_temp = new double[INPUT_NUM];
            g_fIdealStroke = new double[INPUT_NUM];                         // ideal data
            g_fRealTargetScale = new double[INPUT_NUM];                     // LDM data to Target Scale
            g_nTargetPoint = new int[INPUT_NUM];                               // target capture

            output_debug = new double[INPUT_NUM];
        }

        public void InputValLoad(double[] ldmdata)
        {
            int i;
            int count = 0;
            for (i = 1; i < INPUT_NUM + 1; i++)
            {
                if (ldmdata[i] != 0)
                    count++;
            }
            if (ldmdata[1] == 0)
                count++;

            for (i = 0; i < count; i++)
            {
                g_fRealStroke[i] = ldmdata[i + 1] - ldmdata[1];
            }

            Curve_Fitting_all(count);

            for (i = 0; i < INPUT_NUM - 1; i++)
            {
                g_fRealTargetScale[i] = g_fFit_RealStroke[i] / g_fFit_RealStroke[INPUT_NUM - 1] * 16384;
                if (g_fRealTargetScale[i] > 16383)
                    g_fRealTargetScale[i] = 16383;
            }
            g_fRealTargetScale[INPUT_NUM - 1] = 16383;

        }
        private void Curve_Fitting_all(int data_num)
        {
            int i, j, k, n, N;
            N = data_num;
            n = 3;
            double[] x_axis = new double[INPUT_NUM];
            for (i = 0; i < data_num; i++)
            {
                x_axis[i] = i;
                //output_debug[i] = x_axis[i];
            }
            double[] X = new double[2 * 10 + 1];
            for (i = 0; i < 2 * n + 1; i++)
            {
                X[i] = 0;
                for (j = 0; j < N; j++)
                    X[i] = X[i] + Math.Pow(x_axis[j], i);        //consecutive positions of the array will store N,sigma(xi),sigma(xi^2),sigma(xi^3)....sigma(xi^2n)
            }
            double[,] B = new double[10 + 1, 10 + 2];
            double[] a = new double[10 + 1];

            for (i = 0; i <= n; i++)
                for (j = 0; j <= n; j++)
                    B[i,j] = X[i + j];

            double[] Y =new double[10 + 1];                    //Array to store the values of sigma(yi),sigma(xi*yi),sigma(xi^2*yi)...sigma(xi^n*yi)
            for (i = 0; i < n + 1; i++)
            {
                Y[i] = 0;
                for (j = 0; j < N; j++)
                    Y[i] = Y[i] + Math.Pow(x_axis[j], i) * g_fRealStroke[j];        //consecutive positions will store sigma(yi),sigma(xi*yi),sigma(xi^2*yi)...sigma(xi^n*yi)
            }
            for (i = 0; i <= n; i++)
                B[i,n + 1] = Y[i];
            n = n + 1;

            for (i = 0; i < n; i++)                    //From now Gaussian Elimination starts(can be ignored) to solve the set of linear equations (Pivotisation)
                for (k = i + 1; k < n; k++)
                    if (B[i,i] < B[k,i])
                        for (j = 0; j <= n; j++)
                        {
                            double temp = B[i,j];
                            B[i,j] = B[k,j];
                            B[k,j] = temp;
                        }
            for (i = 0; i < n - 1; i++)            //loop to perform the gauss elimination
                for (k = i + 1; k < n; k++)
                {
                    double t = B[k,i] / B[i,i];
                    for (j = 0; j <= n; j++)
                        B[k,j] = B[k,j] - t * B[i,j];
                }
            for (i = n - 1; i >= 0; i--)                                                //back-substitution
            {                                                                           //x is an array whose values correspond to the values of x,y,z..
                a[i] = B[i,n];                                                          //make the variable to be calculated equal to the rhs of the last equation
                for (j = 0; j < n; j++)
                    if (j != i)                                                         //then subtract all the lhs values except the coefficient of the variable whose value                                   is being calculated
                        a[i] = a[i] - B[i,j] * a[j];
                a[i] = a[i] / B[i,i];                                                   //now finally divide the rhs by the coefficient of the variable to be calculated
            }

            double input_ratio = ((INPUT_NUM - 1) / (double)(data_num - 1));
            for (i = 0; i < INPUT_NUM; i++)
            {
                g_fFit_RealStroke_temp[i] = ((a[3] * (double)(i) / input_ratio * (double)(i) / input_ratio * (double)(i) / input_ratio) + (a[2] * (double)(i) / input_ratio * (double)(i) / input_ratio) + (a[1] * (double)(i) / input_ratio) + a[0]);
                //output_debug[i] = (g_fFit_RealStroke_temp[i]);
            }

            for (i = 0; i < INPUT_NUM; i++)
            {
                g_fFit_RealStroke[i] = g_fFit_RealStroke_temp[i] - g_fFit_RealStroke_temp[0];
                output_debug[i] = (g_fFit_RealStroke[i]);
            }
        }

        public List<int> OutputCoeff()
        {
            int i, j;
            int calpoint;
            double sectionslope;

            int lastCoeffBuffer = -1;

            //int[] resultcoeffBuffer = new int[INPUT_NUM];
            List<int> resultCoeffCollection = new List<int>();

            for (i = 0; i < INPUT_NUM - 1; i++)
            {
                g_nTargetPoint[i] = 512 * i;
            }
            g_nTargetPoint[INPUT_NUM - 1] = 16383;

            for (j = 1; j < (INPUT_NUM - 1) / 2; j++)
            {
                calpoint = j * 2;
                for (i = 0; i < INPUT_NUM - 1; i++)
                {
                    if (g_fRealTargetScale[i] < g_nTargetPoint[calpoint] && g_nTargetPoint[calpoint] <= g_fRealTargetScale[i + 1])
                    {

                        sectionslope = 512.0 / (g_fRealTargetScale[i + 1] - g_fRealTargetScale[i]);
                        lastCoeffBuffer = (int)((sectionslope * (g_nTargetPoint[calpoint] - g_fRealTargetScale[i])) + g_nTargetPoint[i]) >> 6;
                    }
                }
                resultCoeffCollection.Add(lastCoeffBuffer);
            }

            //resultCoeffCollection.AddRange(resultcoeffBuffer);
            return resultCoeffCollection;
        }

    }
}
