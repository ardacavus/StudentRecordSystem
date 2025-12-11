using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace StudentSystem.UI
{
    public class RoundedPanel : Panel
    {
        // Yarıçap ayarı (Ne kadar yuvarlak?)
        public int BorderRadius { get; set; } = 30;
        public float GradientAngle { get; set; } = 90F;
        public Color GradientTopColor { get; set; } = Color.White;
        public Color GradientBottomColor { get; set; } = Color.White;

        public RoundedPanel()
        {
            this.BackColor = Color.White;
            this.ForeColor = Color.Black;
            this.Size = new Size(350, 200);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Arka planı gradyanlı boya
            LinearGradientBrush brush = new LinearGradientBrush(
                this.ClientRectangle, this.GradientTopColor, this.GradientBottomColor, this.GradientAngle);

            GraphicsPath graphicsPath = new GraphicsPath();
            graphicsPath.AddArc(0, 0, BorderRadius, BorderRadius, 180, 90);
            graphicsPath.AddArc(this.Width - BorderRadius, 0, BorderRadius, BorderRadius, 270, 90);
            graphicsPath.AddArc(this.Width - BorderRadius, this.Height - BorderRadius, BorderRadius, BorderRadius, 0, 90);
            graphicsPath.AddArc(0, this.Height - BorderRadius, BorderRadius, BorderRadius, 90, 90);
            graphicsPath.CloseAllFigures();

            // Paneli çiz
            e.Graphics.FillPath(brush, graphicsPath);

            // Hafif bir border (isteğe bağlı)
            // Pen pen = new Pen(Color.LightGray, 1.0F);
            // e.Graphics.DrawPath(pen, graphicsPath);
        }
    }
}