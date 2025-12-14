using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using StudentSystem.Application;
using StudentSystem.Core;
using System.Collections.Generic;
using System.Linq;

namespace StudentSystem.UI
{
    public partial class MainForm : Form
    {
        // --- Drag & Drop Code ---
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        // --- Global Variables ---
        private Panel pnlSidebar;
        private Panel pnlHeader;
        private Panel pnlContent;
        private Label lblPageTitle;

        private StudentService _studentService;
        private CourseService _courseService;
        private EnrollmentService _enrollmentService;

        // --- Search Cache ---
        private List<Student> _cachedStudents = new List<Student>();
        private List<Course> _cachedCourses = new List<Course>();
        private List<Enrollment> _cachedEnrollments = new List<Enrollment>();

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
            this.Text = "Student Management System";
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

            // Menu Container
            FlowLayoutPanel pnlMenuContainer = new FlowLayoutPanel();
            pnlMenuContainer.Dock = DockStyle.Fill;
            pnlMenuContainer.FlowDirection = FlowDirection.TopDown;
            pnlMenuContainer.Padding = new Padding(20, 20, 0, 0);
            pnlSidebar.Controls.Add(pnlMenuContainer);
            pnlMenuContainer.BringToFront();

            // English Menu Buttons
            AddModernMenuButton("Dashboard", pnlMenuContainer, true);
            AddModernMenuButton("Students", pnlMenuContainer);
            AddModernMenuButton("Courses", pnlMenuContainer);
            AddModernMenuButton("Enrollments", pnlMenuContainer);

            // Logout Button
            Panel pnlLogout = new Panel();
            pnlLogout.Dock = DockStyle.Bottom;
            pnlLogout.Height = 80;
            pnlSidebar.Controls.Add(pnlLogout);

            Button btnLogout = new Button();
            btnLogout.Text = "← Logout";
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
            lblPageTitle.Text = "Overview";
            lblPageTitle.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblPageTitle.ForeColor = Color.FromArgb(40, 40, 60);
            lblPageTitle.Location = new Point(300, 25);
            lblPageTitle.AutoSize = true;
            pnlHeader.Controls.Add(lblPageTitle);

            // Window Controls
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

        // --- PAGE LOADING METHODS ---

        private void LoadDashboardHome()
        {
            pnlContent.Controls.Clear();
            lblPageTitle.Text = "Overview";

            FlowLayoutPanel flowStats = new FlowLayoutPanel();
            flowStats.Dock = DockStyle.Top;
            flowStats.Height = 250;
            flowStats.BackColor = Color.Transparent;
            pnlContent.Controls.Add(flowStats);

            try
            {
                int studentCount = _studentService.GetAllStudents().Count;
                int courseCount = _courseService.GetAllCourses().Count;

                // Grade Average Card Removed as requested

                flowStats.Controls.Add(CreateStatCard("Total Students", studentCount.ToString(), Color.FromArgb(108, 92, 231)));
                flowStats.Controls.Add(CreateStatCard("Active Courses", courseCount.ToString(), Color.FromArgb(253, 121, 168)));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Stats Error: " + ex.Message);
            }
        }

        private void LoadStudentPage()
        {
            pnlContent.Controls.Clear();
            lblPageTitle.Text = "Student Management";

            // Üst Panel ve Ekle Butonu
            Panel pnlTop = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.Transparent };
            pnlContent.Controls.Add(pnlTop);
            RoundedPanel btnAdd = CreateAddButton("+ New Student", (s, e) => ShowAddStudentDialog());
            pnlTop.Controls.Add(btnAdd);

            // Grid Oluştur
            DataGridView grid = CreateModernGrid();

            try
            {
                _cachedStudents = _studentService.GetAllStudents();

                // Sütunlar
                grid.Columns.Add("ID", "ID");
                grid.Columns.Add("Name", "First Name");
                grid.Columns.Add("Last", "Last Name");
                grid.Columns.Add("Mail", "Email");

                // --- MODERN SİLME BUTONU ---
                DataGridViewButtonColumn btnDelete = new DataGridViewButtonColumn();
                btnDelete.HeaderText = "";
                btnDelete.Text = "🗑"; // Çöp Kutusu İkonu (Unicode)
                btnDelete.UseColumnTextForButtonValue = true;
                btnDelete.FlatStyle = FlatStyle.Flat;
                btnDelete.DefaultCellStyle.ForeColor = Color.Red;
                btnDelete.DefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                btnDelete.Width = 50;
                grid.Columns.Add(btnDelete);

                UpdateStudentGrid(grid, _cachedStudents);

                // Tıklama Olayı
                grid.CellContentClick += (sender, e) =>
                {
                    if (grid.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
                    {
                        int id = (int)grid.Rows[e.RowIndex].Cells[0].Value;
                        if (MessageBox.Show("Öğrenciyi silmek istediğine emin misin?", "Sil", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        {
                            try
                            {
                                _studentService.RemoveStudent(id);
                                LoadStudentPage(); // Sayfayı yenile
                            }
                            catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
                        }
                    }
                };
            }
            catch (Exception ex) { MessageBox.Show("Veri Hatası: " + ex.Message); }

            AddGridToContent(grid);
        }

        private void UpdateStudentGrid(DataGridView grid, List<Student> data)
        {
            grid.Rows.Clear();
            foreach (var st in data)
            {
                grid.Rows.Add(st.StudentID, st.FirstName, st.LastName, st.Email);
            }
        }

        private void LoadCoursePage()
        {
            pnlContent.Controls.Clear();
            lblPageTitle.Text = "Course Management";

            Panel pnlTop = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.Transparent };
            pnlContent.Controls.Add(pnlTop);
            RoundedPanel btnAdd = CreateAddButton("+ New Course", (s, e) => ShowAddCourseDialog());
            pnlTop.Controls.Add(btnAdd);

            DataGridView grid = CreateModernGrid();

            try
            {
                _cachedCourses = _courseService.GetAllCourses();

                grid.Columns.Add("ID", "ID");
                grid.Columns.Add("Name", "Course Name");
                grid.Columns.Add("Credit", "Credits");

                // --- MODERN SİLME BUTONU ---
                DataGridViewButtonColumn btnDelete = new DataGridViewButtonColumn();
                btnDelete.HeaderText = "";
                btnDelete.Text = "🗑";
                btnDelete.UseColumnTextForButtonValue = true;
                btnDelete.FlatStyle = FlatStyle.Flat;
                btnDelete.DefaultCellStyle.ForeColor = Color.Red;
                btnDelete.DefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                btnDelete.Width = 50;
                grid.Columns.Add(btnDelete);

                UpdateCourseGrid(grid, _cachedCourses);

                grid.CellContentClick += (sender, e) =>
                {
                    if (grid.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
                    {
                        int id = (int)grid.Rows[e.RowIndex].Cells[0].Value;
                        if (MessageBox.Show("Bu dersi ve derse ait notları silmek istediğine emin misin?", "Ders Sil", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        {
                            try
                            {
                                _courseService.RemoveCourse(id);
                                LoadCoursePage();
                            }
                            catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
                        }
                    }
                };
            }
            catch (Exception ex) { MessageBox.Show("Veri Hatası: " + ex.Message); }

            AddGridToContent(grid);
        }

        private void UpdateCourseGrid(DataGridView grid, List<Course> data)
        {
            grid.Rows.Clear();
            foreach (var c in data)
            {
                grid.Rows.Add(c.CourseID, c.CourseName, c.Credits);
            }
        }

        private void LoadEnrollmentPage()
        {
            pnlContent.Controls.Clear();
            lblPageTitle.Text = "Enrollment & Grades";

            // Bilgilendirme etiketi
            Label lblInfo = new Label { Text = "(Notu güncellemek için satıra çift tıkla)", ForeColor = Color.Gray, AutoSize = true, Location = new Point(250, 25) };

            Panel pnlTop = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.Transparent };
            pnlContent.Controls.Add(pnlTop);
            RoundedPanel btnAdd = CreateAddButton("+ Add Grade", (s, e) => ShowAddEnrollmentDialog());
            pnlTop.Controls.Add(btnAdd);
            pnlTop.Controls.Add(lblInfo); // Bilgiyi ekle

            DataGridView grid = CreateModernGrid();

            try
            {
                _cachedEnrollments = _enrollmentService.GetAllEnrollments();

                grid.Columns.Add("ID", "ID");
                grid.Columns.Add("Student", "Student Name");
                grid.Columns.Add("Course", "Course");
                grid.Columns.Add("Grade", "Grade");
                grid.Columns.Add("Date", "Date");

                UpdateEnrollmentGrid(grid, _cachedEnrollments);

                // --- ÇİFT TIKLAMA İLE GÜNCELLEME ---
                grid.CellDoubleClick += (sender, e) =>
                {
                    if (e.RowIndex >= 0)
                    {
                        int enrollId = (int)grid.Rows[e.RowIndex].Cells[0].Value;
                        string studentName = grid.Rows[e.RowIndex].Cells[1].Value.ToString();
                        string currentGrade = grid.Rows[e.RowIndex].Cells[3].Value.ToString();

                        ShowUpdateGradeDialog(enrollId, studentName, currentGrade);
                    }
                };
            }
            catch (Exception ex) { MessageBox.Show("Data Error: " + ex.Message); }

            AddGridToContent(grid);
        }

        private void UpdateEnrollmentGrid(DataGridView grid, List<Enrollment> data)
        {
            grid.Rows.Clear();
            foreach (var en in data)
            {
                grid.Rows.Add(en.EnrollmentID, en.StudentName, en.CourseName, en.Grade, en.EnrollmentDate.ToShortDateString());
            }
        }

        // --- POPUP DIALOGS (ENGLISH) ---

        private void ShowAddStudentDialog()
        {
            Form addForm = CreatePopupForm("Add New Student");

            addForm.Controls.Add(new Label() { Text = "First Name:", Location = new Point(20, 20), AutoSize = true });
            TextBox txtName = new TextBox() { Location = new Point(20, 45), Width = 340, Font = new Font("Segoe UI", 11) };

            addForm.Controls.Add(new Label() { Text = "Last Name:", Location = new Point(20, 85), AutoSize = true });
            TextBox txtLast = new TextBox() { Location = new Point(20, 110), Width = 340, Font = new Font("Segoe UI", 11) };

            addForm.Controls.Add(new Label() { Text = "Email:", Location = new Point(20, 150), AutoSize = true });
            TextBox txtMail = new TextBox() { Location = new Point(20, 175), Width = 340, Font = new Font("Segoe UI", 11) };

            Button btnSave = new Button() { Text = "Save", Location = new Point(20, 230), Width = 340, Height = 40, BackColor = Color.Teal, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

            btnSave.Click += (s, e) => {
                try
                {
                    _studentService.RegisterStudent(txtName.Text, txtLast.Text, txtMail.Text, 1);
                    MessageBox.Show("Student Added Successfully!");
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
            Form addForm = CreatePopupForm("Add New Course");
            addForm.Controls.Add(new Label() { Text = "Course Name:", Location = new Point(20, 20), AutoSize = true });
            TextBox txtName = new TextBox() { Location = new Point(20, 45), Width = 340, Font = new Font("Segoe UI", 11) };
            addForm.Controls.Add(new Label() { Text = "Credits:", Location = new Point(20, 85), AutoSize = true });
            NumericUpDown numCredit = new NumericUpDown() { Location = new Point(20, 110), Width = 340, Font = new Font("Segoe UI", 11), Maximum = 10, Minimum = 1 };
            Button btnSave = new Button() { Text = "Save", Location = new Point(20, 170), Width = 340, Height = 40, BackColor = Color.Teal, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

            btnSave.Click += (s, e) => {
                try
                {
                    _courseService.CreateCourse(txtName.Text, (int)numCredit.Value, 1);
                    MessageBox.Show("Course Added Successfully!");
                    addForm.Close();
                    LoadCoursePage();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            };
            addForm.Controls.Add(txtName); addForm.Controls.Add(numCredit); addForm.Controls.Add(btnSave);
            addForm.ShowDialog();
        }

        private void ShowAddEnrollmentDialog()
        {
            Form addForm = CreatePopupForm("Add Grade");

            addForm.Controls.Add(new Label() { Text = "Select Student:", Location = new Point(20, 20), AutoSize = true });
            ComboBox cmbStudents = new ComboBox() { Location = new Point(20, 45), Width = 340, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 11) };

            try
            {
                var students = _studentService.GetAllStudents();
                cmbStudents.DataSource = students;
                cmbStudents.DisplayMember = "FirstName";
                cmbStudents.ValueMember = "StudentID";
            }
            catch { }

            addForm.Controls.Add(cmbStudents);

            addForm.Controls.Add(new Label() { Text = "Select Course:", Location = new Point(20, 85), AutoSize = true });
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

            addForm.Controls.Add(new Label() { Text = "Grade (0-100):", Location = new Point(20, 150), AutoSize = true });
            NumericUpDown numGrade = new NumericUpDown() { Location = new Point(20, 175), Width = 340, Font = new Font("Segoe UI", 11), Maximum = 100, Minimum = 0 };
            addForm.Controls.Add(numGrade);

            Button btnSave = new Button() { Text = "Save Grade", Location = new Point(20, 230), Width = 340, Height = 40, BackColor = Color.Teal, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

            btnSave.Click += (s, e) => {
                try
                {
                    int sId = (int)cmbStudents.SelectedValue;
                    int cId = (int)cmbCourses.SelectedValue;
                    double grade = (double)numGrade.Value;

                    _enrollmentService.AssignGrade(sId, cId, grade);
                    MessageBox.Show("Grade Saved Successfully!");
                    addForm.Close();
                    LoadEnrollmentPage();
                }
                catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
            };

            addForm.Controls.Add(btnSave);
            addForm.ShowDialog();
        }

        // --- HELPER METHODS ---

        private RoundedPanel CreateSearchBox(string placeholder, Action<string> onSearch)
        {
            RoundedPanel pnlSearch = new RoundedPanel();
            pnlSearch.Size = new Size(300, 45);
            pnlSearch.GradientTopColor = Color.White;
            pnlSearch.GradientBottomColor = Color.White;
            pnlSearch.BorderRadius = 20;

            TextBox txtSearch = new TextBox();
            txtSearch.BorderStyle = BorderStyle.None;
            txtSearch.Font = new Font("Segoe UI", 11);
            txtSearch.ForeColor = Color.Gray;
            txtSearch.Text = placeholder;
            txtSearch.Width = 260;
            txtSearch.Location = new Point(15, 12);

            txtSearch.Enter += (s, e) => {
                if (txtSearch.Text == placeholder)
                {
                    txtSearch.Text = "";
                    txtSearch.ForeColor = Color.Black;
                }
            };
            txtSearch.Leave += (s, e) => {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    txtSearch.Text = placeholder;
                    txtSearch.ForeColor = Color.Gray;
                    onSearch("");
                }
            };
            txtSearch.TextChanged += (s, e) => {
                if (txtSearch.Text != placeholder) { onSearch(txtSearch.Text); }
            };

            pnlSearch.Controls.Add(txtSearch);
            return pnlSearch;
        }

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
            lblAdd.Location = new Point(30, 12);
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

            // Default to fill, but we override in enrollment page
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
                if (text == "Dashboard") LoadDashboardHome();
                else if (text == "Students") LoadStudentPage();
                else if (text == "Courses") LoadCoursePage();
                else if (text == "Enrollments") LoadEnrollmentPage();
                else MessageBox.Show("Coming Soon!");
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
        private void ShowUpdateGradeDialog(int enrollId, string name, string currentGrade)
        {
            Form form = CreatePopupForm("Not Güncelle: " + name);
            form.Height = 250;

            form.Controls.Add(new Label() { Text = "Yeni Notu Girin:", Location = new Point(20, 20), AutoSize = true });

            NumericUpDown num = new NumericUpDown() { Location = new Point(20, 50), Width = 340, Maximum = 100, Minimum = 0, Font = new Font("Segoe UI", 12) };
            // Mevcut notu kutuya yazalım (Virgül/Nokta dönüşümüne dikkat)
            if (double.TryParse(currentGrade, out double val)) num.Value = (decimal)val;

            Button btnSave = new Button() { Text = "Güncelle", Location = new Point(20, 100), Width = 340, Height = 40, BackColor = Color.Orange, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

            btnSave.Click += (s, e) => {
                try
                {
                    _enrollmentService.UpdateGrade(enrollId, (double)num.Value);
                    MessageBox.Show("Not güncellendi!");
                    form.Close();
                    LoadEnrollmentPage(); // Listeyi yenile
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            };

            form.Controls.Add(num);
            form.Controls.Add(btnSave);
            form.ShowDialog();
        }
    }

}
