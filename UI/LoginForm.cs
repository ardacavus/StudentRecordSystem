using System;
using System.Windows.Forms;
using StudentSystem.Application; // AuthService için
using StudentSystem.Core;        // User nesnesi için
using StudentSystem.UI;          // MainForm'u bulması için BURASI ŞART

namespace StudentSystem
{
    public partial class LoginForm : Form
    {
        private readonly AuthService _authService;

        public LoginForm()
        {
            InitializeComponent();
            _authService = new AuthService(); // Servisi başlat
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string u = txtUsername.Text;
            string p = txtPassword.Text;

            // Servise sor: Bu kullanıcı kayıtlı mı?
            User user = _authService.Login(u, p);

            if (user != null)
            {
                // 1. Hoşgeldin mesajını göster
                MessageBox.Show($"Hoşgeldin {user.Username} ({user.Role})", "Giriş Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 2. Ana Menü formunu oluştur
                MainForm anaMenu = new MainForm();

                // 3. Login ekranını gizle (arkada açık kalsın ama görünmesin)
                this.Hide();

                // 4. Ana Menüyü aç (Kullanıcı kapatana kadar burada kalır)
                anaMenu.ShowDialog();

                // 5. Ana Menü kapatılınca programı komple kapat
                System.Windows.Forms.Application.Exit();
            }
            else
            {
                MessageBox.Show("Kullanıcı adı veya şifre yanlış!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}