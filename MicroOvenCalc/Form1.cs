using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MicroOvenCalc
{
    public partial class Form1 : Form
    {
        // 計算尺円のマージン
        readonly double margin = 20;

        // 円の中心座標
        double cx { get { return margin + ro; } }
        double cy { get { return margin + ro; } }

        // 外円半径
        double ro { get; set; }

        // 内円半径
        double ri { get { return ro / 200.0 * 130.0; } }

        // 長目盛り高さ
        readonly double hh = 40;

        // 中目盛り高さ
        readonly double hm = 20;

        // 短目盛り高さ
        readonly double hl = 10;

        // 内円と外円の角度
        double diff = 0.0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // 初期サイズ
            ro = Math.Min(this.ClientSize.Width - margin * 2 - 70, this.ClientSize.Height - margin * 2) / 2.0;

            Graphics g = e.Graphics;
            Pen whitePen = new Pen(Color.White, 2);
            Pen blackPen = new Pen(Color.Black, 2);
            Pen greenPen = new Pen(Color.Green, 2);
            Pen redPen = new Pen(Color.Red, 2);
            Pen blueVioletPen = new Pen(Color.BlueViolet, 2);
            Font fnt = new Font("ＭＳ ゴシック", 12);
            Font fntLarge = new Font("ＭＳ ゴシック", 14, FontStyle.Bold);

            // 外円 分
            g.DrawEllipse(whitePen, (int)(cx - ro), (int)(cy - ro), (int)(2 * ro), (int)(2 * ro));
            for (int i = 6; i < 60; i++)
            {
                var theta = Math.Log10(i / 6.0) * 2 * Math.PI;
                double len = hl;
                if (i % 6 == 0)
                {
                    // ラベル
                    len = hm;
                    DrawString($"{i / 6}分", fnt, Brushes.Black, theta, ri + hm + 10, g);
                }

                // 目盛り線
                g.DrawLine(blackPen, (int)(cx + Math.Sin(theta) * (ri + len)), (int)(cy - Math.Cos(theta) * (ri + len)),
                    (int)(cx + Math.Sin(theta) * ri), (int)(cy - Math.Cos(theta) * ri));
            }
            DrawString($"10分", fnt, Brushes.Black, 0, ri + hh + 10, g);

            // 外円　Watt
            for (int watt = 400; watt <= 1200; watt += 100)
            {
                if (watt == 1100) continue;
                var theta = Math.Log10(watt / 400.0 * 1.22) * 2 * Math.PI;
                var len = hh;
                switch (watt)
                {
                    case 400:
                    case 500:
                    case 800:
                    case 900:
                    case 1200:
                        len = hm;
                        break;
                }
                DrawString($"{watt}W", fnt, Brushes.BlueViolet, theta, ri + len + 10, g);
                g.DrawLine(blueVioletPen, (int)(cx + Math.Sin(theta) * (ri + len)), (int)(cy - Math.Cos(theta) * (ri + len)),
                    (int)(cx + Math.Sin(theta) * ri), (int)(cy - Math.Cos(theta) * ri));
            }
            DrawArcString($"説明書ワット", fntLarge, Brushes.BlueViolet, Math.Log10(1.3) * 2 * Math.PI, ri + hh + 10, g);
            DrawArcString($"実際の温め時間", fntLarge, Brushes.Black, Math.Log10(6) * 2 * Math.PI, ri + hh + 10, g);

            // 内円
            for (int i = 6; i < 60; i++)
            {
                var theta = Math.Log10(i / 6.0) * 2 * Math.PI + diff;
                double len = hl;
                if (i % 6 == 0)
                {
                    // 分ラベル
                    len = hm;
                    DrawString($"{i / 6}分", fnt, Brushes.BlueViolet, theta, ri - hm - 10, g);
                }

                // 目盛り線
                g.DrawLine(blueVioletPen, (int)(cx + Math.Sin(theta) * (ri - len)), (int)(cy - Math.Cos(theta) * (ri - len)),
                    (int)(cx + Math.Sin(theta) * ri), (int)(cy - Math.Cos(theta) * ri));
            }
            DrawString($"10分", fnt, Brushes.BlueViolet, diff, ri - hh - 10, g);
            g.DrawEllipse(whitePen, (int)(cx - ri), (int)(cy - ri), (int)(2 * ri), (int)(2 * ri));
            for (int watt = 400; watt <= 800; watt += 100)
            {
                var theta = Math.Log10(watt / 400.0 * 1.22) * 2 * Math.PI + diff;
                var len = hh;
                switch (watt)
                {
                    case 400:
                    case 500:
                    case 800:
                        len = hm;
                        break;
                }

                DrawString($"{watt}W", fnt, Brushes.Red, theta, ri - len - 10, g);
                g.DrawLine(redPen, (int)(cx + Math.Sin(theta) * (ri - len)), (int)(cy - Math.Cos(theta) * (ri - len)),
                    (int)(cx + Math.Sin(theta) * ri), (int)(cy - Math.Cos(theta) * ri));
            }
            DrawArcString($"電子レンジワット", fntLarge, Brushes.Red, Math.Log10(1.7) * 2 * Math.PI + diff, ri - hh - 30, g);
            DrawArcString($"説明書時間", fntLarge, Brushes.BlueViolet, Math.Log10(6) * 2 * Math.PI + diff, ri - hh - 10, g);

            // 中心
            g.DrawEllipse(blackPen, (int)(cx - 1), (int)(cy - 1), 2, 2);
        }

        private void DrawArcString(string text, Font font, Brush brash, double angle, double radius, Graphics g)
        {
            var sz = g.MeasureString(text, font);
            var startAngle = angle - sz.Width / 2.0 / radius;
            for (int i = 0; i < text.Length; i++)
            {
                var ch = text.Substring(i, 1);
                var chsz = g.MeasureString(ch, font);
                g.TranslateTransform((int)(cx + Math.Sin(startAngle) * radius), (int)(cy - Math.Cos(startAngle) * radius));
                g.RotateTransform((float)(startAngle / Math.PI * 180.0));
                g.DrawString(ch, font, brash, (float)(-chsz.Width / 2.0), (float)(-chsz.Height / 2.0));
                g.ResetTransform();
                startAngle += chsz.Width / radius;
            }
        }

        private void DrawString(string text, Font font, Brush brash, double angle, double radius, Graphics g)
        {
            var sz = g.MeasureString(text, font);
            g.TranslateTransform((int)(cx + Math.Sin(angle) * radius), (int)(cy - Math.Cos(angle) * radius));
            g.RotateTransform((float)(angle / Math.PI * 180.0));
            g.DrawString(text, font, brash, (float)(-sz.Width / 2.0), (float)(-sz.Height / 2.0));
            g.ResetTransform();
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            diff = trackBar1.Value / 500.0 * Math.PI;
            this.Refresh();
        }

        /// <summary>
        /// リサイズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Resize(object sender, EventArgs e)
        {
            ro = Math.Min(this.ClientSize.Width - margin * 2 - 70, this.ClientSize.Height - margin * 2) / 2.0;
            this.Refresh();
        }
    }
}
