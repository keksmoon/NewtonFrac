using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NewtonFrac {
    public partial class App : Form {
        public App() {
            InitializeComponent();

            
        }

        private void button1_Click(object sender, EventArgs e) {
            Pen myPen = new Pen(Color.Red, 1);
            var bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = bitmap;
            Graphics g = Graphics.FromImage(bitmap);


            Fractal f = new Fractal(pictureBox1.Width, pictureBox1.Height, 0.0005, 50);
            var res = f.Draw();

            for (int i = 0; i < pictureBox1.Height; i++) {
                for (int j = 0; j < pictureBox1.Width; j++) {
                    myPen.Color = Color.FromArgb(0, res[j, i], res[j, i]);
                    g.DrawRectangle(myPen, j, i, 1, 1);
                }
            }

            
        }

        private void button2_Click(object sender, EventArgs e) {
            pictureBox1.Image.Save($"image{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year}" +
                $"-{DateTime.Now.Hour}-{DateTime.Now.Minute}-{DateTime.Now.Millisecond}.png", ImageFormat.Png);
        }
    }

    public class Fractal {
        public int M, N; // Размеры поля для отрисовки фрактала
        public double Rmin; 
        public int Kmax;

        public int[,] Draw() {
            int[,] FracField = new int[M, N];

            Complex z, t;

            int mx = M / 2;
            int my = N / 2;

            for (int y = -my; y < my; y++) {
                for (int x = -mx; x < mx; x++) {
                    z = new Complex(x * 0.005, y * 0.005);

                    double d = 1; // Радиус сходимости

                    int n = 0;
                    while (d > Rmin && n < Kmax) {
                        t = z;

                        int k = 6;

                        // x^k-1
                        //z = z - (z.Pow(k) - 1) / (k * z.Pow(k - 1));

                        // x^k+z^k/2+1
                        z = z - (z.Pow(8) + 15 * z.Pow(4) - 16) / (8 * z.Pow(7) + 15 * 4 * z.Pow(3));

                        d = (t - z).Mod().Abs();
                        n++;
                    }



                    FracField[mx + x, my + y] = (n * 9) % 255;
                }
            }

            return FracField;
        }

        public Fractal(int _M, int _N, double _Rmin, int _Kmax) {
            M = _M;
            N = _N;
            Rmin = _Rmin;
            Kmax = _Kmax;
        }
    }
    public class Complex {
        public double Re, Im;

        public static Complex operator -(Complex first, Complex second) {
            return new Complex(first.Re - second.Re, first.Im - second.Im);
        }

        public static Complex operator +(Complex first, Complex second) {
            return new Complex(first.Re + second.Re, first.Im + second.Im);
        }

        public static Complex operator *(Complex first, Complex second) {
            return new Complex(first.Re * second.Re - first.Im * second.Im, first.Re * second.Im + first.Im * second.Re);
        }

        public static Complex operator /(Complex first, Complex second) {
            return new Complex((second.Re * first.Re + second.Im * first.Im) / (second.Re * second.Re + second.Im * second.Im),
                (second.Re * first.Im - second.Im * first.Re) / (second.Re * second.Re + second.Im * second.Im));
        }

        public static Complex operator *(int first, Complex second) {
            return new Complex(second.Re * first, second.Im * first);
        }

        public static Complex operator +(int first, Complex second) {
            return new Complex(second.Re + first, second.Im);
        }

        public static Complex operator -(Complex first, int second) {
            return new Complex(first.Re - second, first.Im);
        }

        public static Complex operator -(int first, Complex second) {
            return new Complex(first - second.Re, 0 - second.Im);
        }

        public double Abs() {
            return Re * Re + Im * Im;
        }

        public Complex Mod() {
            return new Complex(Math.Abs(this.Re), Math.Abs(this.Im));
        }

        public Complex Pow(int k) {
            var zmul = new Complex(this.Re, this.Im);
            var z = new Complex(this.Re, this.Im);
            for (int i = 0; i < k - 1; i++)
                z *= zmul;
            return z;
        }

        public Complex(double _Re, double _Im) {
            Re = _Re;
            Im = _Im;
        }

        public Complex() {
            Re = 0;
            Im = 0;
        }
    }
}
