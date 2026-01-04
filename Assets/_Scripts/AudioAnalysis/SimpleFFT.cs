using UnityEngine;

public class SimpleFFT
{
    public void ApplyHanningWindow(float[] data)
    {
        int n = data.Length;
        for (int i = 0; i < n; i++)
            data[i] *= 0.5f * (1 - Mathf.Cos(2 * Mathf.PI * i / (n - 1)));
    }

    public void ApplyMedianFilter(float[] data, int windowSize)
    {
        if (windowSize % 2 == 0) windowSize++;
        int half = windowSize / 2;
        float[] temp = new float[data.Length];
        System.Array.Copy(data, temp, data.Length);

        for (int i = half; i < data.Length - half; i++)
        {
            float[] window = new float[windowSize];
            for (int j = 0; j < windowSize; j++)
                window[j] = temp[i - half + j];
            System.Array.Sort(window);
            data[i] = window[half];
        }
    }

    public float[] ComputeFFT(float[] data)
    {
        int n = data.Length;
        Complex[] buffer = new Complex[n];
        for (int i = 0; i < n; i++)
            buffer[i] = new Complex(data[i], 0);

        FFT(buffer);

        float[] mag = new float[n / 2];
        for (int i = 0; i < mag.Length; i++)
            mag[i] = (float)buffer[i].Magnitude;
        return mag;
    }

    private void FFT(Complex[] buffer)
    {
        int n = buffer.Length;
        
        int j = 0;
        for (int i = 0; i < n - 1; i++)
        {
            if (i < j)
            {
                Complex temp = buffer[i];
                buffer[i] = buffer[j];
                buffer[j] = temp;
            }
            int k = n / 2;
            while (k <= j)
            {
                j -= k;
                k /= 2;
            }
            j += k;
        }

        for (int len = 2; len <= n; len <<= 1)
        {
            float ang = -2 * Mathf.PI / len;
            Complex wlen = Complex.FromPolar(1.0, ang);
            for (int i = 0; i < n; i += len)
            {
                Complex w = new Complex(1.0, 0.0);
                for (int k = 0; k < len / 2; k++)
                {
                    Complex u = buffer[i + k];
                    Complex v = buffer[i + k + len / 2] * w;
                    buffer[i + k] = u + v;
                    buffer[i + k + len / 2] = u - v;
                    w = w * wlen;
                }
            }
        }
    }

    public struct Complex
    {
        public double Real, Imag;
        public Complex(double r, double i) { Real = r; Imag = i; }
        public double Magnitude => System.Math.Sqrt(Real * Real + Imag * Imag);
        public static Complex operator +(Complex a, Complex b) => new Complex(a.Real + b.Real, a.Imag + b.Imag);
        public static Complex operator -(Complex a, Complex b) => new Complex(a.Real - b.Real, a.Imag - b.Imag);
        public static Complex operator *(Complex a, Complex b) => new Complex(a.Real * b.Real - a.Imag * b.Imag, a.Real * b.Imag + a.Imag * b.Real);
        public static Complex FromPolar(double r, double theta) => new Complex(r * System.Math.Cos(theta), r * System.Math.Sin(theta));
    }
}