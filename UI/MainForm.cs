using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using StudentSystem.Application;
using StudentSystem.Core;
using System.Collections.Generic; // Listeleri kullanmak için

namespace StudentSystem.UI
{
    public partial class MainForm : Form
    {
        // --- Sürükleme (Drag) Kodları ---
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        // --- Global Değişkenler ---
        private Panel pnlSidebar;
        private Panel pnlHeader;
        private Panel pnlContent;
        private Label lblPageTitle;

        // Servislerimiz
        private StudentService _studentService;
        private CourseService _courseService;
        private EnrollmentService _enrollmentService; // Yeni Servisimiz!

        public MainForm()
        {
            _studentService = new StudentService();
            _courseService = new CourseService();
            _enrollmentService = new EnrollmentService();

            InitializeUltraModernDashboard();
            LoadDashboardHome();
        }

        private void InitializeUltraModernDashboard()
        {
            this.Text = "Student System Dashboard";
            this.Size = new Size(1300, 850);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(245, 246, 250);

            // SIDEBAR
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

            AddModernMenuButton("Ana Sayfa", pnlMenuContainer, true);
            AddModernMenuButton("Öğrenciler", pnlMenuContainer);
            AddModernMenuButton("Dersler", pnlMenuContainer);
            AddModernMenuButton("Not Sistemi", pnlMenuContainer); // İsmi değişti!

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

            // HEADER
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

            Label lblClose = new Label() { Text = "✕", Font = new Font("Segoe UI", 14), ForeColor = Color.Gray, Location = new Point(this.Width - 50, 20), Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            lblClose.Click += (s, e) => System.Windows.Forms.Application.Exit();
            pnlHeader.Controls.Add(lblClose);

            Label lblMin = new Label() { Text = "―", Font = new Font("Segoe UI", 14), ForeColor = Color.Gray, Location = new Point(this.Width - 90, 20), Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            lblMin.Click += (s, e) => this.WindowState = FormWindowState.Minimized;
            pnlHeader.Controls.Add(lblMin);

            // CONTENT
            pnlContent = new Panel();
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.Padding = new Padding(30);
            this.Controls.Add(pnlContent);
            pnlContent.BringToFront();
        }

        // --- SAYFA YÜKLEME METODLARI ---

        private void LoadDashboardHome()
        {
            pnlContent.Controls.Clear();
            lblPageTitle.Text = "Genel Bakış";

            FlowLayoutPanel flowStats = new FlowLayoutPanel();
            flowStats.Dock = DockStyle.Top;
            flowStats.Height = 250;
            flowStats.BackColor = Color.Transparent;
            pnlContent.Controls.Add(flowStats);

            // Sayıları servisten çekebiliriz (Şimdilik statik)
            flowStats.Controls.Add(CreateStatCard("Toplam Öğrenci", "1,245", Color.FromArgb(108, 92, 231)));
            flowStats.Controls.Add(CreateStatCard("Aktif Dersler", "42", Color.FromArgb(253, 121, 168)));
            flowStats.Controls.Add(CreateStatCard("Not Ortalaması", "78.4", Color.FromArgb(0, 184, 148)));
        }

        private void LoadStudentPage()
        {
            pnlContent.Controls.Clear();
            lblPageTitle.Text = "Öğrenci Yönetimi";

            Panel pnlTop = new Panel();
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Height = 60;
            pnlTop.BackColor = Color.Transparent;
            pnlContent.Controls.Add(pnlTop);

            RoundedPanel btnAdd = CreateAddButton("+ Yeni Öğrenci", (s, e) => ShowAddStudentDialog());
            pnlTop.Controls.Add(btnAdd);

            DataGridView grid = CreateModernGrid();
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
                grid.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                foreach (var st in students) { grid.Rows.Add(st.StudentID, st.FirstName, st.LastName, st.Email); }
            }
            catch (Exception ex) { MessageBox.Show("Veri hatası: " + ex.Message); }

            AddGridToContent(grid);
        }

        private void LoadCoursePage()
        {
            pnlContent.Controls.Clear();
            lblPageTitle.Text = "Ders Yönetimi";

            Panel pnlTop = new Panel();
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Height = 60;
            pnlTop.BackColor = Color.Transparent;
            pnlContent.Controls.Add(pnlTop);

            RoundedPanel btnAdd = CreateAddButton("+ Yeni Ders", (s, e) => ShowAddCourseDialog());
            pnlTop.Controls.Add(btnAdd);

            DataGridView grid = CreateModernGrid();
            try
            {
                var courses = _courseService.GetAllCourses();
                grid.Columns.Add("ID", "ID");
                grid.Columns.Add("Name", "Ders Adı");
                grid.Columns.Add("Credit", "Kredi");

                grid.Columns[0].Width = 50;
                grid.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                grid.Columns[2].Width = 100;

                foreach (var c in courses) { grid.Rows.Add(c.CourseID, c.CourseName, c.Credits); }
            }
            catch (Exception ex) { MessageBox.Show("Veri hatası: " + ex.Message); }

            AddGridToContent(grid);
        }

        // --- YENİ SAYFA: NOT SİSTEMİ (ENROLLMENT) ---
        private void LoadEnrollmentPage()
        {
            pnlContent.Controls.Clear();
            lblPageTitle.Text = "Not ve Kayıt Sistemi";

            Panel pnlTop = new Panel();
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Height = 60;
            pnlTop.BackColor = Color.Transparent;
            pnlContent.Controls.Add(pnlTop);

            // Buton: Not Girişi
            RoundedPanel btnAdd = CreateAddButton("+ Not Girişi", (s, e) => ShowAddEnrollmentDialog());
            pnlTop.Controls.Add(btnAdd);

            DataGridView grid = CreateModernGrid();
            try
            {
                var enrollments = _enrollmentService.GetAllEnrollments();
                // SQL Join ile gelen verileri basıyoruz
                grid.Columns.Add("ID", "ID");
                grid.Columns.Add("Student", "Öğrenci Adı");
                grid.Columns.Add("Course", "Ders");
                grid.Columns.Add("Grade", "Not");
                grid.Columns.Add("Date", "Tarih");

                grid.Columns[0].Width = 50;
                grid.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                grid.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                grid.Columns[3].Width = 80;
                grid.Columns[4].Width = 150;

                foreach (var en in enrollments)
                {
                    grid.Rows.Add(en.EnrollmentID, en.StudentName, en.CourseName, en.Grade, en.EnrollmentDate.ToShortDateString());
                }
            }
            catch (Exception ex) { MessageBox.Show("Veri hatası: " + ex.Message); }

            AddGridToContent(grid);
        }

        // --- POPUP: NOT GİRİŞ EKRANI (Açılır Listeli) ---
        private void ShowAddEnrollmentDialog()
        {
            Form addForm = CreatePopupForm("Not Girişi");

            // 1. Öğrenci Seçimi (ComboBox)
            addForm.Controls.Add(new Label() { Text = "Öğrenci Seç:", Location = new Point(20, 20), AutoSize = true });
            ComboBox cmbStudents = new ComboBox() { Location = new Point(20, 45), Width = 340, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 11) };

            // Öğrencileri yükle
            try
            {
                var students = _studentService.GetAllStudents();
                cmbStudents.DataSource = students;
                cmbStudents.DisplayMember = "FirstName"; // Şimdilik sadece adını gösterelim
                // İsterseniz Student.cs'ye FullName ekleyip onu gösterebiliriz
                cmbStudents.ValueMember = "StudentID";
            }
            catch { }

            addForm.Controls.Add(cmbStudents);

            // 2. Ders Seçimi
            addForm.Controls.Add(new Label() { Text = "Ders Seç:", Location = new Point(20, 85), AutoSize = true });
            ComboBox cmbCourses = new ComboBox() { Location = new Point(20, 110), Width = 340, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 11) };

            try
            {
                var courses = _courseService.GetAllCourses();
                cmbCourses.DataSource = courses;
                cmbCourses.DisplayMember = "CourseName";
                cmbCourses.ValueMember = "CourseID";
            }
            catch { }

            addForm.Controls.Add(cmbCourses);

            // 3. Not Girişi
            addForm.Controls.Add(new Label() { Text = "Not (0-100):", Location = new Point(20, 150), AutoSize = true });
            NumericUpDown numGrade = new NumericUpDown() { Location = new Point(20, 175), Width = 340, Font = new Font("Segoe UI", 11), Maximum = 100, Minimum = 0 };
            addForm.Controls.Add(numGrade);

            // Kaydet
            Button btnSave = new Button() { Text = "Notu Kaydet", Location = new Point(20, 230), Width = 340, Height = 40, BackColor = Color.Teal, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

            btnSave.Click += (s, e) => {
                try
                {
                    int sId = (int)cmbStudents.SelectedValue;
                    int cId = (int)cmbCourses.SelectedValue;
                    double grade = (double)numGrade.Value;

                    _enrollmentService.AssignGrade(sId, cId, grade);
                    MessageBox.Show("Not başarıyla girildi!");
                    addForm.Close();
                    LoadEnrollmentPage(); // Listeyi yenile
                }
                catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
            };

            addForm.Controls.Add(btnSave);
            addForm.ShowDialog();
        }

        // --- ESKİ POPUPLAR (Öğrenci & Ders Ekleme) ---
        private void ShowAddStudentDialog()
        {
            Form addForm = CreatePopupForm("Öğrenci Ekle");

            addForm.Controls.Add(new Label() { Text = "Ad:", Location = new Point(20, 20), AutoSize = true });
            TextBox txtName = new TextBox() { Location = new Point(20, 45), Width = 340, Font = new Font("Segoe UI", 11) };

            addForm.Controls.Add(new Label() { Text = "Soyad:", Location = new Point(20, 85), AutoSize = true });
            TextBox txtLast = new TextBox() { Location = new Point(20, 110), Width = 340, Font = new Font("Segoe UI", 11) };

            addForm.Controls.Add(new Label() { Text = "Email:", Location = new Point(20, 150), AutoSize = true });
            TextBox txtMail = new TextBox() { Location = new Point(20, 175), Width = 340, Font = new Font("Segoe UI", 11) };

            Button btnSave = new Button() { Text = "Kaydet", Location = new Point(20, 230), Width = 340, Height = 40, BackColor = Color.Teal, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

            btnSave.Click += (s, e) => {
                try
                {
                    _studentService.RegisterStudent(txtName.Text, txtLast.Text, txtMail.Text, 1);
                    MessageBox.Show("Başarılı!");
                    addForm.Close();
                    LoadStudentPage();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            };

            addForm.Controls.Add(txtName); addForm.Controls.Add(txtLast); addForm.Controls.Add(txtMail); addForm.Controls.Add(btnSave);
            addForm.ShowDialog();
        }

        private void ShowAddCourseDialog()
        {
            Form addForm = CreatePopupForm("Ders Ekle");
            addForm.Controls.Add(new Label() { Text = "Ders Adı:", Location = new Point(20, 20), AutoSize = true });
            TextBox txtName = new TextBox() { Location = new Point(20, 45), Width = 340, Font = new Font("Segoe UI", 11) };
            addForm.Controls.Add(new Label() { Text = "Kredi:", Location = new Point(20, 85), AutoSize = true });
            NumericUpDown numCredit = new NumericUpDown() { Location = new Point(20, 110), Width = 340, Font = new Font("Segoe UI", 11), Maximum = 10, Minimum = 1 };
            Button btnSave = new Button() { Text = "Kaydet", Location = new Point(20, 170), Width = 340, Height = 40, BackColor = Color.Teal, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

            btnSave.Click += (s, e) => {
                try
                {
                    _courseService.CreateCourse(txtName.Text, (int)numCredit.Value, 1);
                    MessageBox.Show("Ders eklendi!");
                    addForm.Close();
                    LoadCoursePage();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            };
            addForm.Controls.Add(txtName); addForm.Controls.Add(numCredit); addForm.Controls.Add(btnSave);
            addForm.ShowDialog();
        }

        // --- YARDIMCI METODLAR ---
        private RoundedPanel CreateAddButton(string text, EventHandler onClickAction)
        {
            RoundedPanel btnAdd = new RoundedPanel();
            btnAdd.Size = new Size(180, 45);
            btnAdd.Location = new Point(0, 5);
            btnAdd.GradientTopColor = Color.FromArgb(0, 184, 148);
            btnAdd.GradientBottomColor = Color.FromArgb(85, 239, 196);
            btnAdd.BorderRadius = 20;
            btnAdd.Cursor = Cursors.Hand;
            btnAdd.Click += onClickAction;

            Label lblAdd = new Label();
            lblAdd.Text = text;
            lblAdd.ForeColor = Color.White;
            lblAdd.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblAdd.AutoSize = true;
            lblAdd.BackColor = Color.Transparent;
            lblAdd.Location = new Point(30, 13);
            lblAdd.Click += onClickAction;
            btnAdd.Controls.Add(lblAdd);
            return btnAdd;
        }

        private DataGridView CreateModernGrid()
        {
            DataGridView grid = new DataGridView();
            grid.Dock = DockStyle.Fill;
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.EnableHeadersVisualStyles = false;

            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(51, 51, 76);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(12, 0, 0, 0);
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            grid.ColumnHeadersHeight = 60;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            grid.DefaultCellStyle.BackColor = Color.White;
            grid.DefaultCellStyle.ForeColor = Color.Black;
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 150, 136);
            grid.DefaultCellStyle.SelectionForeColor = Color.White;
            grid.DefaultCellStyle.Padding = new Padding(10, 0, 0, 0);

            grid.RowTemplate.Height = 45;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.RowHeadersVisible = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            return grid;
        }

        private void AddGridToContent(DataGridView grid)
        {
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

        private Form CreatePopupForm(string title)
        {
            Form form = new Form();
            form.Size = new Size(400, 350);
            form.StartPosition = FormStartPosition.CenterParent;
            form.Text = title;
            form.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            return form;
        }

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
                else if (text == "Dersler") LoadCoursePage();
                else if (text == "Not Sistemi") LoadEnrollmentPage(); // <-- Yeni sayfa bağlantısı
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