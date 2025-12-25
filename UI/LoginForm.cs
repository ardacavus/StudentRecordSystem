using StudentSystem.Application;
using StudentSystem.Core;
using StudentSystem.UI;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace StudentSystem
{
    public partial class LoginForm : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        private AuthService _authService;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private RoundedPanel pnlMainCard;

        private bool _isCloseHovered = false;
        private bool _isMaxHovered = false;
        private bool _isMinHovered = false;

        private Rectangle _rectClose;
        private Rectangle _rectMax;
        private Rectangle _rectMin;

        public LoginForm()
        {
            _authService = new AuthService();
            InitializeUltraModernDesign();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                if (e.Clicks == 2) ToggleMaximize();
                else
                {
                    ReleaseCapture();
                    SendMessage(this.Handle, 0x112, 0xf012, 0);
                }
            }
        }

        private void ToggleMaximize()
        {
            if (this.WindowState == FormWindowState.Normal)
                this.WindowState = FormWindowState.Maximized;
            else
                this.WindowState = FormWindowState.Normal;
        }

        private void InitializeUltraModernDesign()
        {
            this.Text = "Login";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.Width, this.Height, 40, 40));

            _rectClose = new Rectangle(this.Width - 50, 15, 35, 35);
            _rectMax = new Rectangle(this.Width - 95, 15, 35, 35);
            _rectMin = new Rectangle(this.Width - 140, 15, 35, 35);

            this.Resize += (s, e) => {
                _rectClose = new Rectangle(this.Width - 50, 15, 35, 35);
                _rectMax = new Rectangle(this.Width - 95, 15, 35, 35);
                _rectMin = new Rectangle(this.Width - 140, 15, 35, 35);
                if (this.WindowState == FormWindowState.Maximized) this.Region = null;
                else this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.Width, this.Height, 40, 40));
                if (pnlMainCard != null) pnlMainCard.Location = new Point((this.Width - pnlMainCard.Width) / 2, (this.Height - pnlMainCard.Height) / 2);
                this.Invalidate();
            };

            this.Paint += (s, e) =>
            {
                if (this.ClientRectangle.Width <= 0 || this.ClientRectangle.Height <= 0) return;
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle, Color.FromArgb(40, 40, 70), Color.FromArgb(20, 20, 30), 45F))
                { e.Graphics.FillRectangle(brush, this.ClientRectangle); }

                Color closeColor = _isCloseHovered ? Color.Red : Color.FromArgb(255, 71, 87);
                using (SolidBrush b = new SolidBrush(closeColor)) e.Graphics.FillEllipse(b, _rectClose);

                Color maxColor = _isMaxHovered ? Color.FromArgb(0, 184, 148) : Color.FromArgb(85, 239, 196);
                using (SolidBrush b = new SolidBrush(maxColor)) e.Graphics.FillEllipse(b, _rectMax);

                Color minColor = _isMinHovered ? Color.Gray : Color.FromArgb(116, 125, 140);
                using (SolidBrush b = new SolidBrush(minColor)) e.Graphics.FillEllipse(b, _rectMin);
            };

            AddWindowButtonLabel("✕", _rectClose, (s, e) => System.Windows.Forms.Application.Exit(), ref _isCloseHovered);
            AddWindowButtonLabel("☐", _rectMax, (s, e) => ToggleMaximize(), ref _isMaxHovered);
            AddWindowButtonLabel("―", _rectMin, (s, e) => this.WindowState = FormWindowState.Minimized, ref _isMinHovered);

            pnlMainCard = new RoundedPanel();
            pnlMainCard.Size = new Size(400, 500);
            pnlMainCard.Location = new Point((this.Width - pnlMainCard.Width) / 2, (this.Height - pnlMainCard.Height) / 2);
            pnlMainCard.GradientTopColor = Color.White;
            pnlMainCard.GradientBottomColor = Color.White;
            pnlMainCard.BorderRadius = 40;
            pnlMainCard.BackColor = Color.Transparent;
            this.Controls.Add(pnlMainCard);

            Label lblTitle = new Label();
            lblTitle.Text = "Welcome Back";
            lblTitle.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(40, 40, 70);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point((pnlMainCard.Width - 210) / 2, 40);
            pnlMainCard.Controls.Add(lblTitle);

            Label lblSub = new Label();
            lblSub.Text = "Student System Login";
            lblSub.Font = new Font("Segoe UI", 10);
            lblSub.ForeColor = Color.Gray;
            lblSub.AutoSize = true;
            lblSub.Location = new Point((pnlMainCard.Width - 140) / 2, 85);
            pnlMainCard.Controls.Add(lblSub);

            // --- USERNAME ---
            RoundedPanel pnlUserBg = new RoundedPanel();
            pnlUserBg.Size = new Size(300, 50);
            pnlUserBg.Location = new Point(50, 150);
            pnlUserBg.GradientTopColor = Color.FromArgb(240, 242, 245);
            pnlUserBg.GradientBottomColor = Color.FromArgb(240, 242, 245);
            pnlUserBg.BorderRadius = 25;
            pnlMainCard.Controls.Add(pnlUserBg);

            txtUsername = new TextBox();
            txtUsername.BorderStyle = BorderStyle.None;
            txtUsername.BackColor = Color.FromArgb(240, 242, 245);
            txtUsername.Font = new Font("Segoe UI", 12);
            txtUsername.ForeColor = Color.DimGray;
            txtUsername.Text = "Username";
            txtUsername.Size = new Size(240, 30);
            txtUsername.Location = new Point(20, 15);
            txtUsername.Enter += (s, e) => { if (txtUsername.Text == "Username") txtUsername.Text = ""; };
            txtUsername.Leave += (s, e) => { if (txtUsername.Text == "") txtUsername.Text = "Username"; };
            pnlUserBg.Controls.Add(txtUsername);

            // --- PASSWORD ---
            RoundedPanel pnlPassBg = new RoundedPanel();
            pnlPassBg.Size = new Size(300, 50);
            pnlPassBg.Location = new Point(50, 220);
            pnlPassBg.GradientTopColor = Color.FromArgb(240, 242, 245);
            pnlPassBg.GradientBottomColor = Color.FromArgb(240, 242, 245);
            pnlPassBg.BorderRadius = 25;
            pnlMainCard.Controls.Add(pnlPassBg);

            txtPassword = new TextBox();
            txtPassword.BorderStyle = BorderStyle.None;
            txtPassword.BackColor = Color.FromArgb(240, 242, 245);
            txtPassword.Font = new Font("Segoe UI", 12);
            txtPassword.ForeColor = Color.DimGray;
            txtPassword.Text = "Password";
            txtPassword.Size = new Size(240, 30);
            txtPassword.Location = new Point(20, 15);
            txtPassword.Enter += (s, e) => { if (txtPassword.Text == "Password") { txtPassword.Text = ""; txtPassword.PasswordChar = '●'; } };
            txtPassword.Leave += (s, e) => { if (txtPassword.Text == "") { txtPassword.Text = "Password"; txtPassword.PasswordChar = '\0'; } };
            pnlPassBg.Controls.Add(txtPassword);

            // --- LOGIN BUTTON ---
            RoundedPanel btnLogin = new RoundedPanel();
            btnLogin.Size = new Size(300, 50);
            btnLogin.Location = new Point(50, 320);
            btnLogin.BorderRadius = 25;
            btnLogin.GradientTopColor = Color.FromArgb(108, 92, 231);
            btnLogin.GradientBottomColor = Color.FromArgb(162, 155, 254);
            btnLogin.Cursor = Cursors.Hand;
            btnLogin.Click += BtnLogin_Click;
            pnlMainCard.Controls.Add(btnLogin);

            Label lblBtn = new Label();
            lblBtn.Text = "LOGIN";
            lblBtn.ForeColor = Color.White;
            lblBtn.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblBtn.AutoSize = true;
            lblBtn.BackColor = Color.Transparent;
            lblBtn.Location = new Point(125, 13);
            lblBtn.Click += BtnLogin_Click;
            btnLogin.Controls.Add(lblBtn);

            this.AcceptButton = null;
        }

        private void AddWindowButtonLabel(string text, Rectangle rect, EventHandler onClick, ref bool hoverFlag)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lbl.ForeColor = Color.White;
            lbl.BackColor = Color.Transparent;
            lbl.AutoSize = true;
            lbl.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lbl.Location = new Point(rect.X + 10, rect.Y + 7);
            if (text == "☐") lbl.Location = new Point(rect.X + 9, rect.Y + 5);
            lbl.Cursor = Cursors.Hand;
            lbl.Click += onClick;
            lbl.MouseEnter += (s, e) => {
                if (text == "✕") _isCloseHovered = true;
                if (text == "☐") _isMaxHovered = true;
                if (text == "―") _isMinHovered = true;
                this.Invalidate(rect);
            };
            lbl.MouseLeave += (s, e) => {
                if (text == "✕") _isCloseHovered = false;
                if (text == "☐") _isMaxHovered = false;
                if (text == "―") _isMinHovered = false;
                this.Invalidate(rect);
            };
            this.Controls.Add(lbl);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string u = txtUsername.Text == "Username" ? "" : txtUsername.Text;
            string p = txtPassword.Text == "Password" ? "" : txtPassword.Text;

            try
            {
                User user = _authService.Login(u, p);
                if (user != null)
                {
                    MainForm anaMenu = new MainForm();
                    this.Hide();
                    anaMenu.ShowDialog();
                    System.Windows.Forms.Application.Exit();
                }
                else
                {
                    MessageBox.Show("Invalid username or password!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection Error: " + ex.Message);
            }
        }
    }
}