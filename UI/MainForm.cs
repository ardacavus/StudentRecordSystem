using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using StudentSystem.Application; // Backend Servisi
using StudentSystem.Core;        // Backend Entity

namespace StudentSystem.UI
{
    public partial class MainForm : Form
    {
        // --- Sürükleme (Drag) İçin Gerekli Kodlar ---
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        // --- Global Değişkenler ---
        private Panel pnlSidebar;
        private Panel pnlHeader;
        private Panel pnlContent;
        private Label lblPageTitle;
        private StudentService _studentService; // Backend bağlantısı

        public MainForm()
        {
            _studentService = new StudentService(); // Servisi başlat
            InitializeUltraModernDashboard();
            LoadDashboardHome(); // Açılışta Ana Sayfayı getir
        }

        private void InitializeUltraModernDashboard()
        {
            // 1. FORM AYARLARI
            this.Text = "Student System Dashboard";
            this.Size = new Size(1300, 850);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(245, 246, 250); // Açık gri zemin

            // 2. SIDEBAR (Sol Menü)
            pnlSidebar = new Panel();
            pnlSidebar.Width = 280;
            pnlSidebar.Dock = DockStyle.Left;
            pnlSidebar.BackColor = Color.White;
            this.Controls.Add(pnlSidebar);

            // Logo
            Panel pnlLogo = new Panel();
            pnlLogo.Size = new Size(280, 100);
            pnlLogo.Dock = DockStyle.Top;
            pnlSidebar.Controls.Add(pnlLogo);

            Label lblLogo = new Label();
            lblLogo.Text = "Student\nMaster";
            lblLogo.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            lblLogo.ForeColor = Color.FromArgb(10, 23, 100);
            lblLogo.AutoSize = true;
            lblLogo.Location = new Point(40, 20);
            pnlLogo.Controls.Add(lblLogo);

            // Menü Container
            FlowLayoutPanel pnlMenuContainer = new FlowLayoutPanel();
            pnlMenuContainer.Dock = DockStyle.Fill;
            pnlMenuContainer.FlowDirection = FlowDirection.TopDown;
            pnlMenuContainer.Padding = new Padding(20, 20, 0, 0);
            pnlSidebar.Controls.Add(pnlMenuContainer);
            pnlMenuContainer.BringToFront();

            // Butonları Ekle
            AddModernMenuButton("Ana Sayfa", pnlMenuContainer, true);
            AddModernMenuButton("Öğrenciler", pnlMenuContainer);
            AddModernMenuButton("Dersler", pnlMenuContainer);
            AddModernMenuButton("Bölümler", pnlMenuContainer);

            // Çıkış Butonu
            Panel pnlLogout = new Panel();
            pnlLogout.Dock = DockStyle.Bottom;
            pnlLogout.Height = 80;
            pnlSidebar.Controls.Add(pnlLogout);

            Button btnLogout = new Button();
            btnLogout.Text = "← Oturumu Kapat";
            btnLogout.FlatStyle = FlatStyle.Flat;
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnLogout.ForeColor = Color.IndianRed;
            btnLogout.Dock = DockStyle.Fill;
            btnLogout.Cursor = Cursors.Hand;
            btnLogout.Click += (s, e) => System.Windows.Forms.Application.Exit();
            pnlLogout.Controls.Add(btnLogout);

            // 3. HEADER (Üst Panel)
            pnlHeader = new Panel();
            pnlHeader.Height = 80;
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.BackColor = Color.Transparent;
            pnlHeader.MouseDown += Header_MouseDown;
            this.Controls.Add(pnlHeader);

            lblPageTitle = new Label();
            lblPageTitle.Text = "Genel Bakış";
            lblPageTitle.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblPageTitle.ForeColor = Color.FromArgb(40, 40, 60);
            lblPageTitle.Location = new Point(300, 25);
            lblPageTitle.AutoSize = true;
            pnlHeader.Controls.Add(lblPageTitle);

            // Pencere Kontrolleri
            Label lblClose = new Label() { Text = "✕", Font = new Font("Segoe UI", 14), ForeColor = Color.Gray, Location = new Point(this.Width - 50, 20), Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            lblClose.Click += (s, e) => System.Windows.Forms.Application.Exit();
            pnlHeader.Controls.Add(lblClose);

            Label lblMin = new Label() { Text = "―", Font = new Font("Segoe UI", 14), ForeColor = Color.Gray, Location = new Point(this.Width - 90, 20), Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            lblMin.Click += (s, e) => this.WindowState = FormWindowState.Minimized;
            pnlHeader.Controls.Add(lblMin);

            // 4. CONTENT (İçerik Alanı)
            pnlContent = new Panel();
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.Padding = new Padding(30);
            this.Controls.Add(pnlContent);
            pnlContent.BringToFront();
        }

        // --- SAYFA YÜKLEME MANTIKLARI ---

        private void LoadDashboardHome()
        {
            pnlContent.Controls.Clear();
            lblPageTitle.Text = "Genel Bakış";

            FlowLayoutPanel flowStats = new FlowLayoutPanel();
            flowStats.Dock = DockStyle.Top;
            flowStats.Height = 250;
            flowStats.BackColor = Color.Transparent;
            pnlContent.Controls.Add(flowStats);

            flowStats.Controls.Add(CreateStatCard("Toplam Öğrenci", "1,245", Color.FromArgb(108, 92, 231)));
            flowStats.Controls.Add(CreateStatCard("Aktif Dersler", "42", Color.FromArgb(253, 121, 168)));
            flowStats.Controls.Add(CreateStatCard("Bölümler", "8", Color.FromArgb(0, 184, 148)));
        }

        private void LoadStudentPage()
        {
            pnlContent.Controls.Clear();
            lblPageTitle.Text = "Öğrenci Yönetimi";

            // Üst Bar (Ekle Butonu)
            Panel pnlTop = new Panel();
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Height = 60;
            pnlTop.BackColor = Color.Transparent;
            pnlContent.Controls.Add(pnlTop);

            RoundedPanel btnAdd = new RoundedPanel();
            btnAdd.Size = new Size(180, 45);
            btnAdd.Location = new Point(0, 5);
            btnAdd.GradientTopColor = Color.FromArgb(0, 184, 148);
            btnAdd.GradientBottomColor = Color.FromArgb(85, 239, 196);
            btnAdd.BorderRadius = 20;
            btnAdd.Cursor = Cursors.Hand;
            btnAdd.Click += (s, e) => ShowAddStudentDialog();
            pnlTop.Controls.Add(btnAdd);

            Label lblAdd = new Label() { Text = "+ Yeni Öğrenci", ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true, Location = new Point(40, 12) };
            lblAdd.Click += (s, e) => ShowAddStudentDialog();
            btnAdd.Controls.Add(lblAdd);

            // DataGridView
            DataGridView grid = new DataGridView();
            grid.Dock = DockStyle.Fill;
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.EnableHeadersVisualStyles = false;

            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 246, 250);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.Gray;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(10);
            grid.ColumnHeadersHeight = 40;

            grid.DefaultCellStyle.BackColor = Color.White;
            grid.DefaultCellStyle.ForeColor = Color.DimGray;
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 230, 250);
            grid.DefaultCellStyle.SelectionForeColor = Color.DimGray;
            grid.DefaultCellStyle.Padding = new Padding(10);
            grid.RowTemplate.Height = 40;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.RowHeadersVisible = false;

            try
            {
                var students = _studentService.GetAllStudents();
                grid.Columns.Add("ID", "ID");
                grid.Columns.Add("Name", "Ad");
                grid.Columns.Add("Last", "Soyad");
                grid.Columns.Add("Mail", "Email");

                grid.Columns[0].Width = 50;
                grid.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                grid.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                grid.Columns[3].Width = 200;

                foreach (var st in students)
                {
                    grid.Rows.Add(st.StudentID, st.FirstName, st.LastName, st.Email);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veri çekilemedi: " + ex.Message);
            }

            // Tabloyu Panele Koy
            RoundedPanel pnlGridContainer = new RoundedPanel();
            pnlGridContainer.Dock = DockStyle.Fill;
            pnlGridContainer.Padding = new Padding(20);
            pnlGridContainer.GradientTopColor = Color.White;
            pnlGridContainer.GradientBottomColor = Color.White;
            pnlGridContainer.BorderRadius = 30;

            Panel pnlMargin = new Panel();
            pnlMargin.Dock = DockStyle.Fill;
            pnlMargin.Padding = new Padding(15);
            pnlMargin.BackColor = Color.White;
            pnlMargin.Controls.Add(grid);

            pnlGridContainer.Controls.Add(pnlMargin);
            pnlContent.Controls.Add(pnlGridContainer);
        }

        private void ShowAddStudentDialog()
        {
            Form addForm = new Form();
            addForm.Size = new Size(400, 450);
            addForm.StartPosition = FormStartPosition.CenterParent;
            addForm.Text = "Öğrenci Ekle";
            addForm.FormBorderStyle = FormBorderStyle.FixedToolWindow;

            Label lblTitle = new Label() { Text = "Yeni Kayıt", Location = new Point(20, 20), Font = new Font("Segoe UI", 14, FontStyle.Bold), AutoSize = true };
            addForm.Controls.Add(lblTitle);

            addForm.Controls.Add(new Label() { Text = "Ad:", Location = new Point(20, 70), AutoSize = true });
            TextBox txtName = new TextBox() { Location = new Point(20, 95), Width = 340, Font = new Font("Segoe UI", 11) };

            addForm.Controls.Add(new Label() { Text = "Soyad:", Location = new Point(20, 135), AutoSize = true });
            TextBox txtLast = new TextBox() { Location = new Point(20, 160), Width = 340, Font = new Font("Segoe UI", 11) };

            addForm.Controls.Add(new Label() { Text = "Email:", Location = new Point(20, 200), AutoSize = true });
            TextBox txtMail = new TextBox() { Location = new Point(20, 225), Width = 340, Font = new Font("Segoe UI", 11) };

            Button btnSave = new Button() { Text = "Kaydet", Location = new Point(20, 280), Width = 340, Height = 40, BackColor = Color.Teal, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

            btnSave.Click += (s, e) => {
                try
                {
                    _studentService.RegisterStudent(txtName.Text, txtLast.Text, txtMail.Text, 1);
                    MessageBox.Show("Öğrenci eklendi!");
                    addForm.Close();
                    LoadStudentPage(); // Listeyi yenile
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            };

            addForm.Controls.Add(txtName);
            addForm.Controls.Add(txtLast);
            addForm.Controls.Add(txtMail);
            addForm.Controls.Add(btnSave);
            addForm.ShowDialog();
        }

        // --- YARDIMCI METODLAR ---

        private void AddModernMenuButton(string text, FlowLayoutPanel parent, bool isActive = false)
        {
            RoundedPanel btnPanel = new RoundedPanel();
            btnPanel.Size = new Size(240, 50);
            btnPanel.BorderRadius = 20;
            btnPanel.Margin = new Padding(0, 0, 0, 15);
            btnPanel.Cursor = Cursors.Hand;
            btnPanel.GradientTopColor = Color.White;
            btnPanel.GradientBottomColor = Color.White;

            Label lbl = new Label();
            lbl.Text = text;
            lbl.Font = new Font("Segoe UI", 11, FontStyle.Regular);
            lbl.ForeColor = Color.Gray;
            lbl.AutoSize = true;
            lbl.Location = new Point(20, 13);

            EventHandler clickEvent = (s, e) => {
                if (text == "Ana Sayfa") LoadDashboardHome();
                else if (text == "Öğrenciler") LoadStudentPage();
                else MessageBox.Show("Bu sayfa henüz yapım aşamasında.");
            };

            btnPanel.Click += clickEvent;
            lbl.Click += clickEvent;

            btnPanel.Controls.Add(lbl);
            parent.Controls.Add(btnPanel);
        }

        private RoundedPanel CreateStatCard(string title, string value, Color color)
        {
            RoundedPanel card = new RoundedPanel();
            card.Size = new Size(300, 180);
            card.BorderRadius = 30;
            card.Margin = new Padding(0, 0, 30, 0);
            card.GradientTopColor = color;
            card.GradientBottomColor = ControlPaint.Light(color);

            Label lblTitle = new Label() { Text = title, ForeColor = Color.White, Font = new Font("Segoe UI", 12), Location = new Point(25, 25), AutoSize = true, BackColor = Color.Transparent };
            Label lblValue = new Label() { Text = value, ForeColor = Color.White, Font = new Font("Segoe UI", 36, FontStyle.Bold), Location = new Point(20, 60), AutoSize = true, BackColor = Color.Transparent };

            card.Controls.Add(lblTitle);
            card.Controls.Add(lblValue);
            return card;
        }

        private void Header_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }
    }
}