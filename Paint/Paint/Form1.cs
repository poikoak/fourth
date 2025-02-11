﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Paint
{
    public partial class Form1 : Form
    {
        Bitmap[] layers = new Bitmap[5];
        float[] trans = new float[5] { 1, 0.1f, 1f, 1, 1 };
        BitmapLayers layers2 = new BitmapLayers(400, 300, 5);
        public Form1()
        {

            InitializeComponent();
            this.Width = 900;
            this.Height = 700;
            bm = new Bitmap(pic.Width, pic.Height);
            g = Graphics.FromImage(bm);
            g.Clear(Color.White);
            pic.Image = bm;
            for (int i = 0; i < 5; i++)
            {
                layers[i] = new Bitmap(400, 300);
            }

        }
        Bitmap bm;
        Graphics g;
        bool paint = false;
        Point px, py;
        Pen p = new Pen(Color.Black, 1);
        Pen erase = new Pen(Color.White, 5);
        int index;
        int x, y, sX, sY, cX, cY;
        ColorDialog cd = new ColorDialog();
        Color new_color;

        private void btn_rect_Click(object sender, EventArgs e)
        {
            index = 4;
        }

        private void btn_line_Click(object sender, EventArgs e)
        {
            index = 5;
        }

        private void pic_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (paint)
            {
                if (index == 3)
                {
                    g.DrawEllipse(p, cX, cY, sX, sY);
                }
                if (index == 4)
                {
                    g.DrawRectangle(p, cX, cY, sX, sY);
                }
                if (index == 5)
                {
                    g.DrawLine(p, cX, cY, x, y);
                }
            }
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            g.Clear(Color.White);
            pic.Image = bm;
            index = 0;

        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = "Image(*.jpg)|*.jpg|(*.*|*.*";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Bitmap btm = bm.Clone(new Rectangle(0, 0, pic.Width, pic.Height), bm.PixelFormat);
                btm.Save(sfd.FileName, ImageFormat.Jpeg);
                MessageBox.Show("Image Saved");
            }
        }

        private void btn_color_Click(object sender, EventArgs e)
        {
            cd.ShowDialog();
            new_color = cd.Color;
            pic_color.BackColor = cd.Color;
            p.Color = cd.Color;
        }

        private void btn_ellipse_Click(object sender, EventArgs e)
        {
            index = 3;
        }

        private void pic_MouseMove(object sender, MouseEventArgs e)
        {
            if (paint)
            {
                if (index == 1)
                {
                    px = e.Location;
                    g.DrawLine(p, px, py);
                    py = px;
                }
                if (index == 2)
                {
                    px = e.Location;
                    g.DrawLine(erase, px, py);
                    py = px;
                }
            }
            pic.Refresh();
            x = e.X;
            y = e.Y;
            sX = e.X - cX;
            sY = e.Y - cY;
        }

        private void pic_MouseClick(object sender, MouseEventArgs e)
        {
            if (index == 7)
            {
                Point point = set_point(pic, e.Location);
                Fill(bm, point.X, point.Y, new_color);
            }
        }
        static Point set_point(PictureBox pb, Point pt)
        {
            float pX = 1f * pb.Image.Width / pb.Width;
            float pY = 1f * pb.Image.Height / pb.Height;
            return new Point((int)(pt.X * pX), (int)(pt.Y * pY));


        }
        private void btn_fill_Click(object sender, EventArgs e)
        {
            index = 7;
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            p.Width = trackBar1.Value;
        }

      

        private void pic_MouseUp(object sender, MouseEventArgs e)
        {
            paint = false;
            sY = y - cY;
            sX = x - cX;
            if (index == 3)
            {
                g.DrawEllipse(p, cX, cY, sX, sY);
            }
            if (index == 4)
            {
                g.DrawRectangle(p, cX, cY, sX, sY);
            }
            if (index == 5)
            {
                g.DrawLine(p, cX, cY, x, y);
            }

        }

        private void btn_pencil_Click(object sender, EventArgs e)
        {
            index = 1;
        }

      

        private void btn_eraser_Click(object sender, EventArgs e)
        {
            index = 2;
        }

       

        private void pic_MouseDown(object sender, MouseEventArgs e)
        {
            paint = true;
            py = e.Location;


            cX = e.X;
            cY = e.Y;

        }

        private void val(Bitmap bm, Stack<Point> sp, int x, int y, Color new_color, Color old_color)
        {
            Color cx = bm.GetPixel(x, y);
            if (cx == old_color)
            {
                sp.Push(new Point(x, y));
                bm.SetPixel(x, y, new_color);
            }
        }

        public void Fill(Bitmap bm, int x, int y, Color new_color)
        {
            Color old_color = bm.GetPixel(x, y);
            Stack<Point> pixel = new Stack<Point>();
            pixel.Push(new Point(x, y));
            bm.SetPixel(x, y, new_color);
            if (old_color == new_color) return;
            while (pixel.Count > 0)
            {
                Point pt = (Point)pixel.Pop();
                if (pt.X > 0 && pt.Y > 0 && pt.X < bm.Width - 1 && pt.Y < bm.Height - 1)
                {
                    val(bm, pixel, pt.X - 1, pt.Y, old_color, new_color);
                    val(bm, pixel, pt.X, pt.Y - 1, old_color, new_color);
                    val(bm, pixel, pt.X + 1, pt.Y, old_color, new_color);
                    val(bm, pixel, pt.X, pt.Y + 1, old_color, new_color);
                }
            }

        }


        // класс, представляющий один слой
        public class Layer
        {
            public Bitmap img;              // картинка со слоем
            private float transition;       // прозрачность слоя

            public float Transition         // свойство прозрачночти
            {
                get { return transition; }
                set { transition = value; }
            }

            public Layer(int w, int h, float tr)    // конструктор задаёт размеры слоя и прозрачность
            {
                transition = tr;
                img = new Bitmap(w, h);
            }
        }

        // класс всех слоёв
        public class BitmapLayers
        {
            int width, height;                  // размеры слоёв

            public int Height                   // высота
            {
                get { return height; }
                set { height = value; }
            }

            public int Width                    // ширина
            {
                get { return width; }
                set { width = value; }
            }

            public List<Layer> layers = new List<Layer>();      // список слоёв

            public BitmapLayers(int w, int h, int count)        // конструктор задаёт размеры и количество слоёв
            {
                width = w;
                height = h;
                for (int i = 0; i < count; i++)
                {
                    layers.Add(new Layer(w, h, 1));             // добавление слоя в список
                }
            }

            public void Add(Layer l)                            // метод добавляет слой в список
            {
                layers.Add(l);
            }

            public void Remove(int n)                           // удаление слоя по номеру
            {
                if (n < layers.Count) layers.RemoveAt(n);
            }

            public void Show(PictureBox pic)                    // показ слоёв в picturebox
            {
                Bitmap res = new Bitmap(width, height);         // создание результирующей картинки
                Graphics resgr = Graphics.FromImage(res);       // graphics для результирующей картинки

                Graphics gr = Graphics.FromImage(layers[0].img); // первый слой является фоном общей картинки

                ImageAttributes attr = new ImageAttributes();    // создание атрибутов изображения
                attr.SetColorKey(Color.FromArgb(255, 255, 255), Color.FromArgb(255, 255, 255)); // белый цвет делаем прозрачным

                // рисование фона на картинке
                resgr.DrawImage(layers[0].img,new Rectangle(0, 0, pic.Width, pic.Height));

                // отображение всех слоёв на результирующей картинке с учётом прозрачности
                for (int k = 1; k < layers.Count; k++)
                {
                    // матрица цветов задаёт прозрачноть для каждого слоя
                    ColorMatrix myColorMatrix = new ColorMatrix();
                    myColorMatrix.Matrix00 = 1.00f;
                    myColorMatrix.Matrix11 = 1.00f;
                    myColorMatrix.Matrix22 = 1.00f;
                    myColorMatrix.Matrix33 = layers[k].Transition;

                    attr.SetColorMatrix(myColorMatrix); // применение матрицы

                    // отображение слоя
                    resgr.DrawImage(layers[k].img, new Rectangle(0, 0, 400, 300), 0, 0, 400, 300, GraphicsUnit.Pixel, attr);
                }

                // выбор результирующей картинки для показа в picturebox
                pic.Image = res;

                gr.Dispose();
                resgr.Dispose();
            }
        }
        private void btn_layer_Click(object sender, EventArgs e)
        {
            Layer l = new Layer(400, 300, 0.8f);
            Image img = l.img;
            Graphics g2 = Graphics.FromImage(img);            
            g2.Clear(Color.White);
            layers2.Add(l);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            layers2.Show(pic);
        }
        
    }
}
