using System;
using System.Windows.Forms;
using StudentSystem.Application; // Az önce yazdığımız servisi çağırıyoruz
using StudentSystem.Core;        // User nesnesini tanısın diye

namespace StudentSystem
{
    public partial class LoginForm : Form
    {
        // Servisimizi tanımlıyoruz
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

            // Servise sor: Bu adam kayıtlı mı?
            User user = _authService.Login(u, p);

            if (user != null)
            {
                // Giriş Başarılı!
                MessageBox.Show($"Hoşgeldin {user.Username} ({user.Role})", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Normalde burada Ana Menü açılır, şimdilik programı kapatmasın diye böyle bırakıyoruz.
                // this.Hide(); 
            }
            else
            {
                // Giriş Hatalı
                MessageBox.Show("Kullanıcı adı veya şifre yanlış!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}