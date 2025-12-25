using StudentSystem.Application;
using StudentSystem.Core;
using StudentSystem.Infrastructure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace StudentSystem.UI
{
    public partial class MainForm : Form
    {
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        private Panel pnlSidebar;
        private Panel pnlHeader;
        private Panel pnlContent;
        private Label lblPageTitle;
        private DashboardRepository _dashboardRepo;

        private StudentService _studentService;
        private CourseService _courseService;
        private EnrollmentService _enrollmentService;
        private InstructorService _instructorService;
        private ClubService _clubService;

        private List<Student> _cachedStudents = new List<Student>();
        private List<Course> _cachedCourses = new List<Course>();
        private List<Enrollment> _cachedEnrollments = new List<Enrollment>();

        public MainForm()
        {
            _studentService = new StudentService();
            _courseService = new CourseService();
            _enrollmentService = new EnrollmentService();
            _dashboardRepo = new DashboardRepository();
            _instructorService = new InstructorService();
            _clubService = new ClubService();

            InitializeUltraModernDashboard();
            LoadDashboardHome();
        }

        private void InitializeUltraModernDashboard()
        {
            this.Text = "Student Record System"; // Changed Name
            this.Size = new Size(1300, 850);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(245, 246, 250);
            this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;

            // --- 1. SIDEBAR ---
            pnlSidebar = new Panel();
            pnlSidebar.Width = 280;
            pnlSidebar.Dock = DockStyle.Left;
            pnlSidebar.BackColor = Color.White;
            this.Controls.Add(pnlSidebar);

            Panel pnlLogo = new Panel();
            pnlLogo.Size = new Size(280, 100);
            pnlLogo.Dock = DockStyle.Top;
            pnlSidebar.Controls.Add(pnlLogo);

            Label lblLogo = new Label();
            lblLogo.Text = "Student\nRecord System"; // Changed Logo Text
            lblLogo.Font = new Font("Segoe UI", 20, FontStyle.Bold); // Slightly smaller font to fit
            lblLogo.ForeColor = Color.FromArgb(10, 23, 100);
            lblLogo.AutoSize = true;
            lblLogo.Location = new Point(30, 20);
            pnlLogo.Controls.Add(lblLogo);

            FlowLayoutPanel pnlMenuContainer = new FlowLayoutPanel();
            pnlMenuContainer.Dock = DockStyle.Fill;
            pnlMenuContainer.FlowDirection = FlowDirection.TopDown;
            pnlMenuContainer.Padding = new Padding(20, 20, 0, 0);
            pnlSidebar.Controls.Add(pnlMenuContainer);
            pnlMenuContainer.BringToFront();

            AddModernMenuButton("Dashboard", pnlMenuContainer, true);
            AddModernMenuButton("Students", pnlMenuContainer);
            AddModernMenuButton("Courses", pnlMenuContainer);
            AddModernMenuButton("Instructors", pnlMenuContainer);
            AddModernMenuButton("Clubs", pnlMenuContainer);
            AddModernMenuButton("Enrollments", pnlMenuContainer);

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
            btnLogout.Click += (s, e) => {
                LoginForm login = new LoginForm();
                login.Show();
                this.Close();
            };
            pnlLogout.Controls.Add(btnLogout);

            // --- 2. HEADER ---
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

            Label lblClose = new Label() { Text = "✕", Font = new Font("Segoe UI", 14), ForeColor = Color.Gray, Location = new Point(this.Width - 50, 20), Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            lblClose.Click += (s, e) => System.Windows.Forms.Application.Exit();
            pnlHeader.Controls.Add(lblClose);

            Label lblMax = new Label() { Text = "☐", Font = new Font("Segoe UI", 16), ForeColor = Color.Gray, Location = new Point(this.Width - 90, 18), Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            lblMax.Click += (s, e) => { if (this.WindowState == FormWindowState.Maximized) this.WindowState = FormWindowState.Normal; else this.WindowState = FormWindowState.Maximized; };
            pnlHeader.Controls.Add(lblMax);

            Label lblMin = new Label() { Text = "―", Font = new Font("Segoe UI", 14), ForeColor = Color.Gray, Location = new Point(this.Width - 130, 20), Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            lblMin.Click += (s, e) => this.WindowState = FormWindowState.Minimized;
            pnlHeader.Controls.Add(lblMin);

            // --- 3. CONTENT ---
            pnlContent = new Panel();
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.Padding = new Padding(30);
            this.Controls.Add(pnlContent);
            pnlContent.BringToFront();
        }

        private void LoadDashboardHome()
        {
            pnlContent.Controls.Clear();
            lblPageTitle.Text = "Dashboard Overview";

            FlowLayoutPanel flowStats = new FlowLayoutPanel();
            flowStats.Dock = DockStyle.Top;
            flowStats.AutoSize = true;
            flowStats.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowStats.BackColor = Color.Transparent;
            flowStats.Padding = new Padding(10);
            flowStats.WrapContents = true;
            pnlContent.Controls.Add(flowStats);

            try
            {
                var stats = _dashboardRepo.GetStats();
                flowStats.Controls.Add(CreateStatCard("Total Students", stats.TotalStudents.ToString(), Color.FromArgb(108, 92, 231)));
                flowStats.Controls.Add(CreateStatCard("Active Courses", stats.TotalCourses.ToString(), Color.FromArgb(253, 121, 168)));
                flowStats.Controls.Add(CreateStatCard("Average Grade", stats.AverageGrade.ToString("F2"), Color.FromArgb(0, 184, 148)));
                var cardBest = CreateStatCard("Top Student", stats.TopStudent, Color.FromArgb(255, 118, 117));
                cardBest.Controls[1].Font = new Font("Segoe UI", 16, FontStyle.Bold);
                cardBest.Controls[1].Location = new Point(20, 80);
                flowStats.Controls.Add(cardBest);
            }
            catch (Exception ex) { MessageBox.Show("Statistics Error: " + ex.Message); }

            Label lblWelcome = new Label();
            lblWelcome.Text = "Welcome to the Student Record System!";
            lblWelcome.Font = new Font("Segoe UI", 14);
            lblWelcome.ForeColor = Color.Gray;
            lblWelcome.AutoSize = true;
            lblWelcome.Dock = DockStyle.Top;
            lblWelcome.Padding = new Padding(30, 20, 0, 0);
            pnlContent.Controls.Add(lblWelcome);
        }

        private void LoadStudentPage()
        {
            pnlContent.Controls.Clear();
            lblPageTitle.Text = "Student Management";

            Panel pnlTop = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.Transparent };
            pnlContent.Controls.Add(pnlTop);
            RoundedPanel btnAdd = CreateAddButton("+ New Student", (s, e) => ShowAddStudentDialog());
            pnlTop.Controls.Add(btnAdd);

            DataGridView grid = CreateModernGrid();

            try
            {
                _cachedStudents = _studentService.GetAllStudents();
                grid.Columns.Add("ID", "ID");
                grid.Columns.Add("Name", "First Name");
                grid.Columns.Add("Last", "Last Name");
                grid.Columns.Add("Mail", "Email");

                DataGridViewButtonColumn btnClubs = new DataGridViewButtonColumn();
                btnClubs.HeaderText = "Clubs";
                btnClubs.Text = "♣️";
                btnClubs.UseColumnTextForButtonValue = true;
                btnClubs.FlatStyle = FlatStyle.Flat;
                btnClubs.DefaultCellStyle.ForeColor = Color.DarkMagenta;
                btnClubs.Width = 60;
                grid.Columns.Add(btnClubs);

                DataGridViewButtonColumn btnDetails = new DataGridViewButtonColumn();
                btnDetails.HeaderText = "Grades"; btnDetails.Text = "📄"; btnDetails.UseColumnTextForButtonValue = true; btnDetails.Width = 60;
                grid.Columns.Add(btnDetails);

                DataGridViewButtonColumn btnDelete = new DataGridViewButtonColumn();
                btnDelete.HeaderText = "Action"; btnDelete.Text = "🗑"; btnDelete.UseColumnTextForButtonValue = true; btnDelete.Width = 60; btnDelete.DefaultCellStyle.ForeColor = Color.Red;
                grid.Columns.Add(btnDelete);

                foreach (var st in _cachedStudents)
                {
                    grid.Rows.Add(st.StudentID, st.FirstName, st.LastName, st.Email);
                }

                grid.CellContentClick += (sender, e) =>
                {
                    if (e.RowIndex >= 0)
                    {
                        int id = (int)grid.Rows[e.RowIndex].Cells[0].Value;
                        string name = grid.Rows[e.RowIndex].Cells[1].Value.ToString() + " " + grid.Rows[e.RowIndex].Cells[2].Value.ToString();

                        if (grid.Columns[e.ColumnIndex] == btnDetails) ShowStudentHistoryDialog(id, name);
                        else if (grid.Columns[e.ColumnIndex] == btnClubs) ShowStudentClubsDialog(id, name);
                        else if (grid.Columns[e.ColumnIndex] == btnDelete)
                        {
                            if (MessageBox.Show("Are you sure you want to delete this student?", "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            { _studentService.RemoveStudent(id); LoadStudentPage(); }
                        }
                    }
                };
            }
            catch (Exception ex) { MessageBox.Show("Data Error: " + ex.Message); }
            AddGridToContent(grid);
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
                grid.Columns.Add("ID", "ID"); grid.Columns.Add("Name", "Course Name"); grid.Columns.Add("Credit", "Credits"); grid.Columns.Add("Inst", "Instructor");
                DataGridViewButtonColumn btnDelete = new DataGridViewButtonColumn();
                btnDelete.HeaderText = "Action"; btnDelete.Text = "🗑"; btnDelete.UseColumnTextForButtonValue = true; btnDelete.Width = 60; btnDelete.DefaultCellStyle.ForeColor = Color.Red;
                grid.Columns.Add(btnDelete);
                foreach (var c in _cachedCourses) grid.Rows.Add(c.CourseID, c.CourseName, c.Credits, c.InstructorName);
                grid.CellContentClick += (sender, e) => { if (grid.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0) { int id = (int)grid.Rows[e.RowIndex].Cells[0].Value; if (MessageBox.Show("Delete this course?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes) { _courseService.RemoveCourse(id); LoadCoursePage(); } } };
            }
            catch (Exception ex) { MessageBox.Show("Data Error: " + ex.Message); }
            AddGridToContent(grid);
        }

        private void LoadInstructorPage()
        {
            pnlContent.Controls.Clear();
            lblPageTitle.Text = "Academic Staff";
            Panel pnlTop = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.Transparent };
            pnlContent.Controls.Add(pnlTop);

            RoundedPanel btnAdd = CreateAddButton("+ New Instructor", (s, e) => ShowAddInstructorDialog());
            pnlTop.Controls.Add(btnAdd);

            DataGridView grid = CreateModernGrid();
            grid.Columns.Add("Title", "Title"); grid.Columns.Add("Name", "First Name"); grid.Columns.Add("Last", "Last Name"); grid.Columns.Add("Phone", "Phone"); grid.Columns.Add("Dept", "Dept ID");
            try
            {
                var list = _instructorService.GetAllInstructors();
                foreach (var i in list) grid.Rows.Add(i.Title, i.FirstName, i.LastName, i.Phone, i.DeptID);
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
            AddGridToContent(grid);
        }

        private void LoadClubPage()
        {
            pnlContent.Controls.Clear();
            lblPageTitle.Text = "Student Clubs";
            Panel pnlTop = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.Transparent };
            pnlContent.Controls.Add(pnlTop);

            RoundedPanel btnAdd = CreateAddButton("+ New Club", (s, e) => ShowAddClubDialog());
            pnlTop.Controls.Add(btnAdd);

            DataGridView grid = CreateModernGrid();
            grid.Columns.Add("ID", "ID");
            grid.Columns.Add("Name", "Club Name");
            grid.Columns.Add("Desc", "Description");
            grid.Columns.Add("Date", "Est. Date");

            DataGridViewButtonColumn btnMembers = new DataGridViewButtonColumn();
            btnMembers.HeaderText = "Members";
            btnMembers.Text = "👥";
            btnMembers.UseColumnTextForButtonValue = true;
            btnMembers.FlatStyle = FlatStyle.Flat;
            btnMembers.Width = 70;
            btnMembers.DefaultCellStyle.ForeColor = Color.Teal;
            grid.Columns.Add(btnMembers);

            try
            {
                var list = _clubService.GetAllClubs();
                foreach (var c in list) grid.Rows.Add(c.ClubID, c.ClubName, c.Description, c.EstablishmentDate.ToShortDateString());

                grid.CellContentClick += (sender, e) =>
                {
                    if (e.RowIndex >= 0 && grid.Columns[e.ColumnIndex] == btnMembers)
                    {
                        int id = (int)grid.Rows[e.RowIndex].Cells[0].Value;
                        string name = grid.Rows[e.RowIndex].Cells[1].Value.ToString();
                        ShowClubMembersDialog(id, name);
                    }
                };
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
            AddGridToContent(grid);
        }

        private void LoadEnrollmentPage()
        {
            pnlContent.Controls.Clear();
            lblPageTitle.Text = "Enrollment & Grades";
            Label lblInfo = new Label { Text = "(Double click row to update grade)", ForeColor = Color.Gray, AutoSize = true, Location = new Point(250, 25) };
            Panel pnlTop = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.Transparent };
            pnlContent.Controls.Add(pnlTop);
            RoundedPanel btnAdd = CreateAddButton("+ Add Grade", (s, e) => ShowAddEnrollmentDialog());
            pnlTop.Controls.Add(btnAdd); pnlTop.Controls.Add(lblInfo);
            DataGridView grid = CreateModernGrid();
            try
            {
                _cachedEnrollments = _enrollmentService.GetAllEnrollments();
                grid.Columns.Add("ID", "ID"); grid.Columns.Add("Student", "Student Name"); grid.Columns.Add("Course", "Course"); grid.Columns.Add("Grade", "Grade"); grid.Columns.Add("Date", "Date");
                foreach (var en in _cachedEnrollments) grid.Rows.Add(en.EnrollmentID, en.StudentName, en.CourseName, en.Grade, en.EnrollmentDate.ToShortDateString());
                grid.CellDoubleClick += (sender, e) => { if (e.RowIndex >= 0) { int enrollId = (int)grid.Rows[e.RowIndex].Cells[0].Value; string studentName = grid.Rows[e.RowIndex].Cells[1].Value.ToString(); string currentGrade = grid.Rows[e.RowIndex].Cells[3].Value.ToString(); ShowUpdateGradeDialog(enrollId, studentName, currentGrade); } };
            }
            catch (Exception ex) { MessageBox.Show("Data Error: " + ex.Message); }
            AddGridToContent(grid);
        }

        // --- POPUP DIALOGS ---

        private void ShowAddClubDialog()
        {
            Form form = CreatePopupForm("Add New Club");
            form.Height = 300;
            form.Controls.Add(new Label() { Text = "Club Name:", Location = new Point(20, 20), AutoSize = true });
            TextBox txtName = new TextBox() { Location = new Point(20, 45), Width = 340, Font = new Font("Segoe UI", 11) };
            form.Controls.Add(txtName);
            form.Controls.Add(new Label() { Text = "Description:", Location = new Point(20, 85), AutoSize = true });
            TextBox txtDesc = new TextBox() { Location = new Point(20, 110), Width = 340, Font = new Font("Segoe UI", 11), Multiline = true, Height = 60 };
            form.Controls.Add(txtDesc);
            Button btnSave = new Button() { Text = "Create Club", Location = new Point(20, 190), Width = 340, Height = 40, BackColor = Color.Teal, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSave.Click += (s, e) => { try { _clubService.CreateClub(txtName.Text, txtDesc.Text); MessageBox.Show("Club Created!"); form.Close(); LoadClubPage(); } catch (Exception ex) { MessageBox.Show(ex.Message); } };
            form.Controls.Add(btnSave); form.ShowDialog();
        }

        private void ShowAddInstructorDialog()
        {
            Form form = CreatePopupForm("Add Instructor");
            form.Height = 450;
            int y = 20;
            form.Controls.Add(new Label() { Text = "First Name:", Location = new Point(20, y), AutoSize = true }); TextBox txtName = new TextBox() { Location = new Point(20, y + 25), Width = 340, Font = new Font("Segoe UI", 11) }; form.Controls.Add(txtName); y += 60;
            form.Controls.Add(new Label() { Text = "Last Name:", Location = new Point(20, y), AutoSize = true }); TextBox txtLast = new TextBox() { Location = new Point(20, y + 25), Width = 340, Font = new Font("Segoe UI", 11) }; form.Controls.Add(txtLast); y += 60;
            form.Controls.Add(new Label() { Text = "Title (Dr., Prof. etc):", Location = new Point(20, y), AutoSize = true }); TextBox txtTitle = new TextBox() { Location = new Point(20, y + 25), Width = 340, Font = new Font("Segoe UI", 11) }; form.Controls.Add(txtTitle); y += 60;
            form.Controls.Add(new Label() { Text = "Phone:", Location = new Point(20, y), AutoSize = true }); TextBox txtPhone = new TextBox() { Location = new Point(20, y + 25), Width = 340, Font = new Font("Segoe UI", 11) }; form.Controls.Add(txtPhone); y += 60;
            form.Controls.Add(new Label() { Text = "Department:", Location = new Point(20, y), AutoSize = true }); ComboBox cmbDept = new ComboBox() { Location = new Point(20, y + 25), Width = 340, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 11) };
            cmbDept.Items.Add("Computer Engineering (1)"); cmbDept.Items.Add("Industrial Engineering (2)"); cmbDept.Items.Add("Architecture (3)"); cmbDept.SelectedIndex = 0; form.Controls.Add(cmbDept); y += 70;
            Button btnSave = new Button() { Text = "Save", Location = new Point(20, y), Width = 340, Height = 40, BackColor = Color.Teal, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSave.Click += (s, e) => { try { int deptId = 1; if (cmbDept.SelectedIndex == 1) deptId = 2; if (cmbDept.SelectedIndex == 2) deptId = 3; _instructorService.AddInstructor(txtName.Text, txtLast.Text, txtTitle.Text, txtPhone.Text, deptId); MessageBox.Show("Instructor Added!"); form.Close(); LoadInstructorPage(); } catch (Exception ex) { MessageBox.Show(ex.Message); } };
            form.Controls.Add(btnSave); form.ShowDialog();
        }

        private void ShowClubMembersDialog(int clubId, string clubName)
        {
            Form form = CreatePopupForm(clubName + " - Members");
            form.Size = new Size(500, 400);
            DataGridView grid = CreateModernGrid();
            grid.Columns.Add("Name", "Student Name"); grid.Columns.Add("Mail", "Email");
            try { var members = _clubService.GetClubMembers(clubId); foreach (var m in members) grid.Rows.Add(m.FirstName + " " + m.LastName, m.Email); } catch (Exception ex) { MessageBox.Show(ex.Message); }
            form.Controls.Add(grid); form.ShowDialog();
        }

        private void ShowStudentClubsDialog(int studentId, string studentName)
        {
            Form form = CreatePopupForm(studentName + " - Clubs");
            form.Size = new Size(500, 400);
            DataGridView grid = CreateModernGrid();
            grid.Columns.Add("Club", "Club Name"); grid.Columns.Add("Desc", "Description");
            try { var clubs = _studentService.GetStudentClubs(studentId); foreach (var c in clubs) grid.Rows.Add(c.ClubName, c.Description); } catch (Exception ex) { MessageBox.Show(ex.Message); }
            form.Controls.Add(grid); form.ShowDialog();
        }

        private void ShowAddStudentDialog()
        {
            Form addForm = CreatePopupForm("Add New Student"); addForm.Size = new Size(400, 550);
            int y = 20;
            addForm.Controls.Add(new Label() { Text = "First Name:", Location = new Point(20, y), AutoSize = true }); TextBox txtName = new TextBox() { Location = new Point(20, y + 25), Width = 340, Font = new Font("Segoe UI", 11) }; addForm.Controls.Add(txtName); y += 60;
            addForm.Controls.Add(new Label() { Text = "Last Name:", Location = new Point(20, y), AutoSize = true }); TextBox txtLast = new TextBox() { Location = new Point(20, y + 25), Width = 340, Font = new Font("Segoe UI", 11) }; addForm.Controls.Add(txtLast); y += 60;
            addForm.Controls.Add(new Label() { Text = "Email:", Location = new Point(20, y), AutoSize = true }); TextBox txtMail = new TextBox() { Location = new Point(20, y + 25), Width = 340, Font = new Font("Segoe UI", 11) }; addForm.Controls.Add(txtMail); y += 60;
            addForm.Controls.Add(new Label() { Text = "Phone:", Location = new Point(20, y), AutoSize = true }); TextBox txtPhone = new TextBox() { Location = new Point(20, y + 25), Width = 340, Font = new Font("Segoe UI", 11) }; addForm.Controls.Add(txtPhone); y += 60;
            addForm.Controls.Add(new Label() { Text = "Gender:", Location = new Point(20, y), AutoSize = true }); ComboBox cmbGender = new ComboBox() { Location = new Point(20, y + 25), Width = 340, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 11) }; cmbGender.Items.AddRange(new object[] { "Male", "Female" }); cmbGender.SelectedIndex = 0; addForm.Controls.Add(cmbGender); y += 60;
            addForm.Controls.Add(new Label() { Text = "Birth Date:", Location = new Point(20, y), AutoSize = true }); DateTimePicker dtPicker = new DateTimePicker() { Location = new Point(20, y + 25), Width = 340, Format = DateTimePickerFormat.Short, Font = new Font("Segoe UI", 11) }; addForm.Controls.Add(dtPicker); y += 70;
            Button btnSave = new Button() { Text = "Save Student", Location = new Point(20, y), Width = 340, Height = 40, BackColor = Color.Teal, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSave.Click += (s, e) => { try { _studentService.RegisterStudent(txtName.Text, txtLast.Text, txtMail.Text, txtPhone.Text, cmbGender.SelectedItem.ToString(), dtPicker.Value, 1); MessageBox.Show("Student Added Successfully!"); addForm.Close(); LoadStudentPage(); } catch (Exception ex) { MessageBox.Show(ex.Message); } };
            addForm.Controls.Add(btnSave); addForm.ShowDialog();
        }

        private void ShowAddCourseDialog() { Form addForm = CreatePopupForm("Add New Course"); addForm.Controls.Add(new Label() { Text = "Course Name:", Location = new Point(20, 20), AutoSize = true }); TextBox txtName = new TextBox() { Location = new Point(20, 45), Width = 340, Font = new Font("Segoe UI", 11) }; addForm.Controls.Add(txtName); addForm.Controls.Add(new Label() { Text = "Credits:", Location = new Point(20, 85), AutoSize = true }); NumericUpDown numCredit = new NumericUpDown() { Location = new Point(20, 110), Width = 340, Font = new Font("Segoe UI", 11), Maximum = 10, Minimum = 1 }; Button btnSave = new Button() { Text = "Save", Location = new Point(20, 170), Width = 340, Height = 40, BackColor = Color.Teal, ForeColor = Color.White, FlatStyle = FlatStyle.Flat }; btnSave.Click += (s, e) => { try { _courseService.CreateCourse(txtName.Text, (int)numCredit.Value, 1); MessageBox.Show("Course Created!"); addForm.Close(); LoadCoursePage(); } catch (Exception ex) { MessageBox.Show(ex.Message); } }; addForm.Controls.Add(txtName); addForm.Controls.Add(numCredit); addForm.Controls.Add(btnSave); addForm.ShowDialog(); }

        private void ShowAddEnrollmentDialog() { Form addForm = CreatePopupForm("Add Grade"); addForm.Controls.Add(new Label() { Text = "Select Student:", Location = new Point(20, 20), AutoSize = true }); ComboBox cmbStudents = new ComboBox() { Location = new Point(20, 45), Width = 340, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 11) }; try { cmbStudents.DataSource = _studentService.GetAllStudents(); cmbStudents.DisplayMember = "FirstName"; cmbStudents.ValueMember = "StudentID"; } catch { } addForm.Controls.Add(cmbStudents); addForm.Controls.Add(new Label() { Text = "Select Course:", Location = new Point(20, 85), AutoSize = true }); ComboBox cmbCourses = new ComboBox() { Location = new Point(20, 110), Width = 340, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 11) }; try { cmbCourses.DataSource = _courseService.GetAllCourses(); cmbCourses.DisplayMember = "CourseName"; cmbCourses.ValueMember = "CourseID"; } catch { } addForm.Controls.Add(cmbCourses); addForm.Controls.Add(new Label() { Text = "Grade (0-100):", Location = new Point(20, 150), AutoSize = true }); NumericUpDown numGrade = new NumericUpDown() { Location = new Point(20, 175), Width = 340, Font = new Font("Segoe UI", 11), Maximum = 100, Minimum = 0 }; addForm.Controls.Add(numGrade); Button btnSave = new Button() { Text = "Save Grade", Location = new Point(20, 230), Width = 340, Height = 40, BackColor = Color.Teal, ForeColor = Color.White, FlatStyle = FlatStyle.Flat }; btnSave.Click += (s, e) => { try { _enrollmentService.AssignGrade((int)cmbStudents.SelectedValue, (int)cmbCourses.SelectedValue, (double)numGrade.Value); MessageBox.Show("Grade Assigned!"); addForm.Close(); LoadEnrollmentPage(); } catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); } }; addForm.Controls.Add(btnSave); addForm.ShowDialog(); }

        private RoundedPanel CreateAddButton(string text, EventHandler onClickAction) { RoundedPanel btnAdd = new RoundedPanel(); btnAdd.Size = new Size(180, 45); btnAdd.Location = new Point(0, 5); btnAdd.GradientTopColor = Color.FromArgb(0, 184, 148); btnAdd.GradientBottomColor = Color.FromArgb(85, 239, 196); btnAdd.BorderRadius = 20; btnAdd.Cursor = Cursors.Hand; btnAdd.Click += onClickAction; Label lblAdd = new Label(); lblAdd.Text = text; lblAdd.ForeColor = Color.White; lblAdd.Font = new Font("Segoe UI", 10, FontStyle.Bold); lblAdd.AutoSize = true; lblAdd.BackColor = Color.Transparent; lblAdd.Location = new Point(30, 12); lblAdd.Click += onClickAction; btnAdd.Controls.Add(lblAdd); return btnAdd; }
        private DataGridView CreateModernGrid() { DataGridView grid = new DataGridView(); grid.Dock = DockStyle.Fill; grid.BackgroundColor = Color.White; grid.BorderStyle = BorderStyle.None; grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal; grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None; grid.EnableHeadersVisualStyles = false; grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(51, 51, 76); grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White; grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Bold); grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(12, 0, 0, 0); grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft; grid.ColumnHeadersHeight = 60; grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing; grid.DefaultCellStyle.BackColor = Color.White; grid.DefaultCellStyle.ForeColor = Color.Black; grid.DefaultCellStyle.Font = new Font("Segoe UI", 10); grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 150, 136); grid.DefaultCellStyle.SelectionForeColor = Color.White; grid.DefaultCellStyle.Padding = new Padding(10, 0, 0, 0); grid.RowTemplate.Height = 45; grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect; grid.ReadOnly = true; grid.AllowUserToAddRows = false; grid.RowHeadersVisible = false; grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; return grid; }
        private void AddGridToContent(DataGridView grid) { RoundedPanel pnlGridContainer = new RoundedPanel(); pnlGridContainer.Dock = DockStyle.Fill; pnlGridContainer.Padding = new Padding(20); pnlGridContainer.GradientTopColor = Color.White; pnlGridContainer.GradientBottomColor = Color.White; pnlGridContainer.BorderRadius = 30; Panel pnlMargin = new Panel(); pnlMargin.Dock = DockStyle.Fill; pnlMargin.Padding = new Padding(15); pnlMargin.BackColor = Color.White; pnlMargin.Controls.Add(grid); pnlGridContainer.Controls.Add(pnlMargin); pnlContent.Controls.Add(pnlGridContainer); }
        private Form CreatePopupForm(string title) { Form form = new Form(); form.Size = new Size(400, 350); form.StartPosition = FormStartPosition.CenterParent; form.Text = title; form.FormBorderStyle = FormBorderStyle.FixedToolWindow; return form; }
        private void AddModernMenuButton(string text, FlowLayoutPanel parent, bool isActive = false) { RoundedPanel btnPanel = new RoundedPanel(); btnPanel.Size = new Size(240, 50); btnPanel.BorderRadius = 20; btnPanel.Margin = new Padding(0, 0, 0, 15); btnPanel.Cursor = Cursors.Hand; btnPanel.GradientTopColor = Color.White; btnPanel.GradientBottomColor = Color.White; Label lbl = new Label(); lbl.Text = text; lbl.Font = new Font("Segoe UI", 11, FontStyle.Regular); lbl.ForeColor = Color.Gray; lbl.AutoSize = true; lbl.Location = new Point(20, 13); EventHandler clickEvent = (s, e) => { if (text == "Dashboard") LoadDashboardHome(); else if (text == "Students") LoadStudentPage(); else if (text == "Courses") LoadCoursePage(); else if (text == "Instructors") LoadInstructorPage(); else if (text == "Clubs") LoadClubPage(); else if (text == "Enrollments") LoadEnrollmentPage(); }; btnPanel.Click += clickEvent; lbl.Click += clickEvent; btnPanel.Controls.Add(lbl); parent.Controls.Add(btnPanel); }
        private RoundedPanel CreateStatCard(string title, string value, Color color) { RoundedPanel card = new RoundedPanel(); card.Size = new Size(300, 180); card.BorderRadius = 30; card.Margin = new Padding(0, 0, 30, 0); card.GradientTopColor = color; card.GradientBottomColor = ControlPaint.Light(color); Label lblTitle = new Label() { Text = title, ForeColor = Color.White, Font = new Font("Segoe UI", 12), Location = new Point(25, 25), AutoSize = true, BackColor = Color.Transparent }; Label lblValue = new Label() { Text = value, ForeColor = Color.White, Font = new Font("Segoe UI", 36, FontStyle.Bold), Location = new Point(20, 60), AutoSize = true, BackColor = Color.Transparent }; card.Controls.Add(lblTitle); card.Controls.Add(lblValue); return card; }
        private void Header_MouseDown(object sender, MouseEventArgs e) { ReleaseCapture(); SendMessage(this.Handle, 0x112, 0xf012, 0); }

        private void ShowUpdateGradeDialog(int enrollId, string name, string currentGrade)
        {
            Form form = CreatePopupForm("Update Grade: " + name);
            form.Height = 250;
            form.Controls.Add(new Label() { Text = "Enter New Grade:", Location = new Point(20, 20), AutoSize = true });
            NumericUpDown num = new NumericUpDown() { Location = new Point(20, 50), Width = 340, Maximum = 100, Minimum = 0, Font = new Font("Segoe UI", 12) };
            if (double.TryParse(currentGrade, out double val)) num.Value = (decimal)val;
            Button btnSave = new Button() { Text = "Update", Location = new Point(20, 100), Width = 340, Height = 40, BackColor = Color.Orange, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSave.Click += (s, e) => { try { _enrollmentService.UpdateGrade(enrollId, (double)num.Value); MessageBox.Show("Grade Updated!"); form.Close(); LoadEnrollmentPage(); } catch (Exception ex) { MessageBox.Show(ex.Message); } };
            form.Controls.Add(num); form.Controls.Add(btnSave); form.ShowDialog();
        }

        private void ShowStudentHistoryDialog(int studentId, string studentName)
        {
            Form historyForm = CreatePopupForm(studentName + " - Grade History");
            historyForm.Size = new Size(500, 400);
            Label lblTitle = new Label();
            lblTitle.Text = studentName + "'s Grades";
            lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(40, 40, 60);
            lblTitle.Location = new Point(20, 15);
            lblTitle.AutoSize = true;
            historyForm.Controls.Add(lblTitle);
            DataGridView grid = CreateModernGrid();
            grid.Location = new Point(20, 50);
            grid.Size = new Size(440, 280);
            grid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grid.Columns.Add("Course", "Course"); grid.Columns.Add("Grade", "Grade"); grid.Columns.Add("Date", "Date");
            try
            {
                var history = _enrollmentService.GetStudentHistory(studentId);
                if (history.Count == 0) { grid.Visible = false; Label lblEmpty = new Label() { Text = "No records found.", Location = new Point(20, 60), AutoSize = true, ForeColor = Color.Gray }; historyForm.Controls.Add(lblEmpty); }
                else
                {
                    foreach (var item in history) { grid.Rows.Add(item.CourseName, item.Grade, item.EnrollmentDate.ToShortDateString()); }
                    double average = history.Average(h => h.Grade);
                    Label lblAvg = new Label();
                    lblAvg.Text = $"Overall Average: {average:F2}";
                    lblAvg.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                    lblAvg.ForeColor = Color.Teal;
                    lblAvg.Location = new Point(20, 340);
                    lblAvg.AutoSize = true;
                    historyForm.Controls.Add(lblAvg);
                }
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
            historyForm.Controls.Add(grid);
            historyForm.ShowDialog();
        }
    }
}