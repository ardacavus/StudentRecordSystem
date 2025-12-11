using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using StudentSystem.Application;
using StudentSystem.Core;
using StudentSystem.UI;

namespace StudentSystem
{
    public partial class LoginForm : Form
    {
        private AuthService _authService;

        // Inputları global tanımlıyoruz ki butona basınca okuyabilelim
        private TextBox txtUsername;
        private TextBox txtPassword;
        private RoundedPanel pnlMainCard; // Ortadaki beyaz kart

        public LoginForm()
        {
            _authService = new AuthService();
            InitializeUltraModernDesign();
        }

        private void InitializeUltraModernDesign()
        {
            // 1. FORM AYARLARI
            this.Text = "Login";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;

            // Arka Plan Rengi (Web sitelerindeki 'Brand Color' gibi)
            // Çok havalı bir koyu mor-mavi gradyanı yapacağız Paint eventinde.
            this.Paint += (s, e) =>
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    this.ClientRectangle,
                    Color.FromArgb(40, 40, 70), // Koyu Gri-Mavi
                    Color.FromArgb(20, 20, 30), // Neredeyse Siyah
                    45F))
                {
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
                }
            };

            // Çıkış Butonu (Sağ Üst)
            Label lblExit = new Label();
            lblExit.Text = "X";
            lblExit.ForeColor = Color.White;
            lblExit.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblExit.Location = new Point(this.Width - 40, 10);
            lblExit.Cursor = Cursors.Hand;
            lblExit.Click += (s, e) => System.Windows.Forms.Application.Exit();
            this.Controls.Add(lblExit);

            // 2. ORTA KART (LOGIN KUTUSU)
            // Yuvarlak köşeli beyaz panel
            pnlMainCard = new RoundedPanel();
            pnlMainCard.Size = new Size(400, 500);
            // Formun tam ortasına koyma matematiği:
            pnlMainCard.Location = new Point((this.Width - pnlMainCard.Width) / 2, (this.Height - pnlMainCard.Height) / 2);
            pnlMainCard.GradientTopColor = Color.White; // Düz beyaz
            pnlMainCard.GradientBottomColor = Color.White;
            pnlMainCard.BorderRadius = 40; // Köşeler iyice yuvarlansın
            pnlMainCard.BackColor = Color.Transparent; // Arkası görünsün diye (Formun rengini almasın)
            this.Controls.Add(pnlMainCard);

            // -- KART İÇERİĞİ --

            // Logo / Başlık
            Label lblTitle = new Label();
            lblTitle.Text = "Welcome Back";
            lblTitle.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(40, 40, 70);
            lblTitle.AutoSize = true;
            // Kartın içinde ortalama
            lblTitle.Location = new Point((pnlMainCard.Width - 210) / 2, 40);
            pnlMainCard.Controls.Add(lblTitle);

            Label lblSub = new Label();
            lblSub.Text = "Öğrenci Sistemine Giriş";
            lblSub.Font = new Font("Segoe UI", 10);
            lblSub.ForeColor = Color.Gray;
            lblSub.AutoSize = true;
            lblSub.Location = new Point((pnlMainCard.Width - 140) / 2, 85);
            pnlMainCard.Controls.Add(lblSub);


            // 3. KULLANICI ADI (HAYALET INPUT TEKNİĞİ)
            // Önce gri yuvarlak bir zemin (Panel) oluşturuyoruz
            RoundedPanel pnlUserBg = new RoundedPanel();
            pnlUserBg.Size = new Size(300, 50);
            pnlUserBg.Location = new Point(50, 150);
            pnlUserBg.GradientTopColor = Color.FromArgb(240, 242, 245); // Çok açık gri
            pnlUserBg.GradientBottomColor = Color.FromArgb(240, 242, 245);
            pnlUserBg.BorderRadius = 25;
            pnlMainCard.Controls.Add(pnlUserBg);

            // Sonra içine kenarlıksız TextBox koyuyoruz
            txtUsername = new TextBox();
            txtUsername.BorderStyle = BorderStyle.None; // Kenarlığı yok et!
            txtUsername.BackColor = Color.FromArgb(240, 242, 245); // Panel ile aynı renk
            txtUsername.Font = new Font("Segoe UI", 12);
            txtUsername.ForeColor = Color.DimGray;
            txtUsername.Text = "Kullanıcı Adı"; // Placeholder gibi
            txtUsername.Size = new Size(240, 30);
            txtUsername.Location = new Point(20, 15); // Panelin içinde ortala

            // Tıklayınca yazıyı sil (Placeholder mantığı)
            txtUsername.Enter += (s, e) => { if (txtUsername.Text == "Kullanıcı Adı") txtUsername.Text = ""; };
            txtUsername.Leave += (s, e) => { if (txtUsername.Text == "") txtUsername.Text = "Kullanıcı Adı"; };

            pnlUserBg.Controls.Add(txtUsername);


            // 4. ŞİFRE (HAYALET INPUT)
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
            txtPassword.Text = "Şifre";
            txtPassword.Size = new Size(240, 30);
            txtPassword.Location = new Point(20, 15);

            txtPassword.Enter += (s, e) => {
                if (txtPassword.Text == "Şifre")
                {
                    txtPassword.Text = "";
                    txtPassword.PasswordChar = '●'; // Yazmaya başlayınca gizle
                }
            };
            txtPassword.Leave += (s, e) => {
                if (txtPassword.Text == "")
                {
                    txtPassword.Text = "Şifre";
                    txtPassword.PasswordChar = '\0'; // Boşsa yazıyı göster
                }
            };

            pnlPassBg.Controls.Add(txtPassword);


            // 5. LOGIN BUTONU (MODERN GRADYANLI BUTON)
            // Buton yerine RoundedPanel kullanıp tıklama özelliği vereceğiz.
            // Bu sayede buton da tam yuvarlak ve gradyanlı olabilir.
            RoundedPanel btnLogin = new RoundedPanel();
            btnLogin.Size = new Size(300, 50);
            btnLogin.Location = new Point(50, 320);
            btnLogin.BorderRadius = 25;
            // Instagram/Spotify tarzı renk geçişi
            btnLogin.GradientTopColor = Color.FromArgb(108, 92, 231); // Mor
            btnLogin.GradientBottomColor = Color.FromArgb(162, 155, 254); // Açık Mor
            btnLogin.Cursor = Cursors.Hand;
            btnLogin.Click += BtnLogin_Click; // Panele tıklama özelliği
            pnlMainCard.Controls.Add(btnLogin);

            Label lblBtn = new Label();
            lblBtn.Text = "GİRİŞ YAP";
            lblBtn.ForeColor = Color.White;
            lblBtn.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblBtn.AutoSize = true;
            lblBtn.BackColor = Color.Transparent;
            // Yazıyı ortala ve tıklamayı panele aktar
            lblBtn.Location = new Point(105, 13);
            lblBtn.Click += BtnLogin_Click;
            btnLogin.Controls.Add(lblBtn);

            this.AcceptButton = null; // Panel kullandığımız için Enter tuşunu manuel bağlamak gerekir, şimdilik iptal.
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            // Placeholder kontrolü
            string u = txtUsername.Text == "Kullanıcı Adı" ? "" : txtUsername.Text;
            string p = txtPassword.Text == "Şifre" ? "" : txtPassword.Text;

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
                    MessageBox.Show("Hatalı kullanıcı adı veya şifre!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }
    }
}