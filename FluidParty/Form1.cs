using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FluidParty
{
    public partial class Form1 : Form
    {
        static int size=150;
        Fluid fluid; 
        Bitmap bm = new Bitmap(size, size);
        Pen pen;
        Perlin perl = new Perlin();
        ColorHeatMap cmap = new ColorHeatMap();
        int[,] bitm = new int[size, size];
        int counter = 0;
        double timer;
        int sign = 1;
        Point randpos;
            Point lastposition = new Point(0, 0);

        //Function to get a random number 
        private static readonly Random random = new Random();
        
        public static int RandomNumber(int min, int max)
        {
                return random.Next(min, max);
                }



        public Form1()
        {
            fluid = new Fluid(0.00000001f, 0.000000000f, 0.003f, size, size, 1);
            InitializeComponent();
            //timer1.Interval = 1;
            pictureBox1.Image = bm;


            timer1.Enabled = true;
        }
        
        public float RandomNumberPerl(int x,int y)
        {
            float value;
            value = (float)perl.OctavePerlin(x, y, x+y, 5, 0);
            return value;
        }
        private Point Getposition(int px, int py)
        {
            Point unscaled_p = new Point();

            // image and container dimensions
            int w_i = pictureBox1.Image.Width;
            int h_i = pictureBox1.Image.Height;
            int w_c = pictureBox1.Width;
            int h_c = pictureBox1.Height;

            float imageRatio = w_i / (float)h_i; // image W:H ratio
            float containerRatio = w_c / (float)h_c; // container W:H ratio

            if (imageRatio >= containerRatio)
            {
                // horizontal image
                float scaleFactor = w_c / (float)w_i;
                float scaledHeight = h_i * scaleFactor;
                // calculate gap between top of container and top of image
                float filler = Math.Abs(h_c - scaledHeight) / 2;
                unscaled_p.X = (int)(px / scaleFactor);
                unscaled_p.Y = (int)((py - filler) / scaleFactor);
            }
            else
            {
                // vertical image
                float scaleFactor = h_c / (float)h_i;
                float scaledWidth = w_i * scaleFactor;
                float filler = Math.Abs(w_c - scaledWidth) / 2;
                unscaled_p.X = (int)((px - filler) / scaleFactor);
                unscaled_p.Y = (int)(py / scaleFactor);
            }

            return unscaled_p;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer += 0.07f ;
            timer = timer % 6.28;
           
            label1.Text = timer.ToString();
           // if (timer > 3.14f)
            {
                //fluid.addDensity(randpos.X, randpos.X, trackBar2.Value * 5);
                //fluid.addVelocity(randpos.Y, randpos.Y, RandomNumber(-10,10) , RandomNumber(-10, 10));

                //fluid.addVelocity(randpos.Y, randpos.Y, 0 , trackBar3.Value);
                fluid.addDensity(size / 2, size / 2, trackBar2.Value * 5);
                //fluid.addVelocity(size / 2, size / 3, 0, trackBar3.Value);
                ////            fluid.addVelocity(size / 2, size / 2, (float)RandomNumber(-50,50), (float)RandomNumber(-50, 50));
                            fluid.addVelocity(size / 2 , size / 2 , (float)(trackBar3.Value*Math.Cos(timer*sign)), (float)(trackBar3.Value * Math.Sin(timer* sign)));

            }
            fluid.step(trackBar1.Value);
            bitm = fluid.renderToInt();
            float x, y;
            using (Graphics gr = Graphics.FromImage(bm))
            {
                
                gr.Clear(Color.Black);
                for (int j = 0; j < bm.Height; j++)
                {
                    for (int i = 0; i < bm.Width; i++)
                    {
                        x = (float)(i - (float)(size / 2));
                        y = (float)(j - (float)(size / 2));
                        pen = new Pen(cmap.GetColorForValue(Math.Min(255,bitm[i,j]%255 ), 255, 0));
                        if (bitm[i,j]>0) gr.DrawRectangle(pen, i, j, 1, 1);

                        // if (i>5&&i<bm.Width-5&&j>5&&j<bm.Height-5)fluid.addVelocity(i, j, (float)(RandomNumber(-1, 2) / 8), (float)(RandomNumber(-1, 2)/8) );
                        if (i>5&&i<bm.Width-5&&j>5&&j<bm.Height-5)fluid.addVelocity(i, j, (-y/3000)-x/3000, x/3000-y/ 3000);
                        //if(i > 1 && i < bm.Width - 1 && j > 1 && j < bm.Height - 1)fluid.addVelocity(i, j, (float)(Math.Pow(y,3) + 9*y)/10000, (float)(Math.Pow(x, 3)+9*x )/1000);

                    }
                } 
            }

            //pictureBox1.Image = fluid.renderToBitmap();
          
            pictureBox1.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sign *= -1;
        }


        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {

            Point pos = Getposition(e.X, e.Y);

            if (e.Button == MouseButtons.Left)
            {
                fluid.addDensity(pos.X, pos.Y, 250);
                fluid.addVelocity(pos.X, pos.Y, (float)(RandomNumber(-1, 2) * 10), (float)(RandomNumber(-1, 2) * 10));

            }
            else
            { 
            //fluid.addVelocity(pos.X, pos.Y, (float)(RandomNumber(-1, 2) * 20), (float)(RandomNumber(-1, 2) * 20));
            }
            //fluid.addDensity(pos.X, pos.Y, 50);

        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            Point pos = Getposition(e.X, e.Y);
        }
    }
}
