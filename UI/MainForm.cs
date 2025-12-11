using System;
using System.Drawing;
using System.Windows.Forms;

namespace StudentSystem.UI
{
    public partial class MainForm : Form
    {
        // UI Elemanlarını global tanımlayalım ki her yerden erişelim
        private Panel pnlSidebar;
        private Panel pnlHeader;
        private Panel pnlContent;
        private Label lblTitle;

        public MainForm()
        {
            InitializeComponent();
            SetupCustomUI(); // Kendi tasarımımızı başlatan fonksiyon
        }

        private void SetupCustomUI()
        {
            // 1. FORM AYARLARI
            this.Text = "Öğrenci Kayıt Sistemi - Yönetim Paneli";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(800, 600); // Daha fazla küçülmesin

            // 2. HEADER PANEL (Üst Başlık)
            pnlHeader = new Panel();
            pnlHeader.Height = 80;
            pnlHeader.Dock = DockStyle.Top; // Üste yapış
            pnlHeader.BackColor = Color.FromArgb(0, 150, 136); // Modern Teal Rengi
            this.Controls.Add(pnlHeader); // Form'a ekle

            // Başlık Yazısı
            lblTitle = new Label();
            lblTitle.Text = "Öğrenci Yönetim Sistemi";
            lblTitle.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(20, 25);
            pnlHeader.Controls.Add(lblTitle); // Header'a ekle

            // 3. SIDEBAR PANEL (Sol Menü)
            pnlSidebar = new Panel();
            pnlSidebar.Width = 250;
            pnlSidebar.Dock = DockStyle.Left; // Sola yapış
            pnlSidebar.BackColor = Color.FromArgb(51, 51, 76); // Koyu Gri/Lacivert
            this.Controls.Add(pnlSidebar);
            // DİKKAT: Sidebar'ı Header'dan sonra ekledik ama Dock sırası önemli.
            // Önce Sidebar'ı ekleyip sonra Header'ı eklesek daha iyi oturabilir. 
            // Ama bu haliyle Header üstte boydan boya, Sidebar onun altında kalır.
            // Eğer Sidebar en tepeye kadar çıksın istersen kod sırasını değiştir.

            // 4. CONTENT PANEL (Ortadaki Değişen Alan)
            pnlContent = new Panel();
            pnlContent.Dock = DockStyle.Fill; // Kalan boşluğu doldur
            pnlContent.BackColor = Color.WhiteSmoke;
            this.Controls.Add(pnlContent);
            // Controls.Add sırası çok önemlidir! En son eklenen "Fill" yapan, kalan boşluğa oturur.
            pnlContent.BringToFront(); // Garanti olsun diye öne alıyoruz

            // 5. MENÜ BUTONLARINI OLUŞTUR (Clean Code Yöntemi)
            // Tek tek kod yazmak yerine fonksiyonla üretiyoruz.
            Button btnStudents = CreateMenuButton("Öğrenci İşlemleri", 1);
            Button btnCourses = CreateMenuButton("Ders Yönetimi", 2);
            Button btnDepartments = CreateMenuButton("Bölümler", 3);
            Button btnExit = CreateMenuButton("Çıkış Yap", 4);

            // Çıkış butonuna özel renk verelim (Opsiyonel)
            btnExit.BackColor = Color.FromArgb(192, 0, 0); // Koyu Kırmızı
            btnExit.Click += (s, e) => System.Windows.Forms.Application.Exit(); // Lambda ile tek satırda çıkış kodu

            // Butonları Sidebar'a ekle
            pnlSidebar.Controls.Add(btnExit);
            pnlSidebar.Controls.Add(btnDepartments);
            pnlSidebar.Controls.Add(btnCourses);
            pnlSidebar.Controls.Add(btnStudents);
        }

        // Clean Code Helper: Buton Üretici
        private Button CreateMenuButton(string text, int order)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Dock = DockStyle.Top; // Hepsi üste dizilsin
            btn.Height = 60;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0; // Çerçeve yok
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            btn.Cursor = Cursors.Hand;
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Padding = new Padding(20, 0, 0, 0); // Yazıyı biraz sağa it

            // Hover Efekti (Fare üzerine gelince renk değişsin)
            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(80, 80, 100);
            btn.MouseLeave += (s, e) => btn.BackColor = Color.Transparent;

            // Butonlar Dock.Top olduğu için ters sırayla eklenir. 
            // Bunu çözmek için ya ters ekleyeceğiz ya da BringToFront yapacağız.
            // Şimdilik basit tutalım.
            return btn;
        }
    }
}