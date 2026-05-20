using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

// Enums for Department and Attendance Status
public enum Department { HR, IT, Finance, Marketing, Sales }
public enum AttendanceStatus { Present, Absent, Leave }

// Base Employee class
public abstract class Employee : IComparable<Employee>
{
    public static int EmployeeCount = 0;
    public int Id { get; set; }
    public string Name { get; set; }
    public Department Department { get; set; }
    public double BaseSalary { get; set; }

    public Employee(int id, string name, Department dept, double baseSalary)
    {
        Id = id;
        Name = name;
        Department = dept;
        BaseSalary = baseSalary;
        EmployeeCount++;
    }

    public abstract double CalculateSalary();

    public int CompareTo(Employee other) => CalculateSalary().CompareTo(other.CalculateSalary());
    public static bool operator >(Employee a, Employee b) => a.CalculateSalary() > b.CalculateSalary();
    public static bool operator <(Employee a, Employee b) => a.CalculateSalary() < b.CalculateSalary();
}

public class FullTimeEmployee : Employee
{
    public double Bonus { get; set; }
    public FullTimeEmployee(int id, string name, Department dept, double baseSalary, double bonus)
        : base(id, name, dept, baseSalary) => Bonus = bonus;
    public override double CalculateSalary() => BaseSalary + Bonus;
}

public class PartTimeEmployee : Employee
{
    public int HoursWorked { get; set; }
    public double HourlyRate { get; set; }
    public PartTimeEmployee(int id, string name, Department dept, double hourlyRate, int hoursWorked)
        : base(id, name, dept, hourlyRate * hoursWorked)
    {
        HourlyRate = hourlyRate;
        HoursWorked = hoursWorked;
    }
    public override double CalculateSalary() => HourlyRate * HoursWorked;
}

public class ContractEmployee : Employee
{
    public int ContractDuration { get; set; }
    public ContractEmployee(int id, string name, Department dept, double baseSalary, int contractDuration)
        : base(id, name, dept, baseSalary) => ContractDuration = contractDuration;
    public override double CalculateSalary() => BaseSalary;
}

// Attendance Record
public class AttendanceRecord
{
    public int EmployeeId { get; set; }
    public DateTime Date { get; set; }
    public AttendanceStatus Status { get; set; }
}

// Attendance Utilities
public static class AttendanceUtils
{
    public static void MarkAttendance(ref AttendanceRecord record, AttendanceStatus status)
    {
        record.Status = status;
    }
    public static int CountPresent(List<AttendanceRecord> records, out int total)
    {
        total = records.Count;
        return records.Count(r => r.Status == AttendanceStatus.Present);
    }
}

// Main Form
public class MainForm : Form
{
    private List<Employee> employees = new();
    private List<AttendanceRecord> attendanceRecords = new();
    private Panel mainPanel;
    private System.ComponentModel.IContainer components = null;

    public MainForm()
    {
        InitializeUI();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeUI()
    {
        // Form settings
        Text = "Employee Payroll & Attendance System";
        Size = new Size(600, 500);
        BackColor = Color.White;
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 9F);

        // Main panel
        mainPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White
        };
        Controls.Add(mainPanel);

        ShowMainMenu();
    }

    private void ClearPanel()
    {
        mainPanel.Controls.Clear();
    }

    private void ShowMainMenu()
    {
        ClearPanel();

        Label titleLabel = new Label
        {
            Text = "PAYROLL SYSTEM",
            Font = new Font("Segoe UI", 18F, FontStyle.Bold),
            Location = new Point(150, 30),
            Size = new Size(300, 40),
            TextAlign = ContentAlignment.MiddleCenter
        };
        mainPanel.Controls.Add(titleLabel);

        string[] menuItems = {
            "Add Employee",
            "List Employees",
            "Mark Attendance",
            "View Attendance",
            "Save Data",
            "Load Data",
            "Compare Employees"
        };

        Action[] menuActions = {
            ShowAddEmployee,
            ShowListEmployees,
            ShowMarkAttendance,
            ShowViewAttendance,
            SaveData,
            LoadData,
            ShowCompareEmployees
        };

        int yPos = 100;
        for (int i = 0; i < menuItems.Length; i++)
        {
            Button btn = CreateMenuButton(menuItems[i], menuActions[i]);
            btn.Location = new Point(150, yPos);
            mainPanel.Controls.Add(btn);
            yPos += 45;
        }
    }

    private Button CreateMenuButton(string text, Action onClick)
    {
        Button btn = new Button
        {
            Text = text,
            Size = new Size(300, 40),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.White,
            ForeColor = Color.Black,
            Font = new Font("Segoe UI", 10F),
            Cursor = Cursors.Hand
        };
        btn.FlatAppearance.BorderColor = Color.Black;
        btn.FlatAppearance.BorderSize = 1;
        btn.MouseEnter += (s, e) => { btn.BackColor = Color.Black; btn.ForeColor = Color.White; };
        btn.MouseLeave += (s, e) => { btn.BackColor = Color.White; btn.ForeColor = Color.Black; };
        btn.Click += (s, e) => onClick();
        return btn;
    }

    private void ShowAddEmployee()
    {
        ClearPanel();

        Label titleLabel = new Label
        {
            Text = "ADD EMPLOYEE",
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            Location = new Point(50, 20),
            Size = new Size(200, 30)
        };
        mainPanel.Controls.Add(titleLabel);

        // Employee Type
        Label typeLabel = new Label { Text = "Employee Type:", Location = new Point(50, 70), Size = new Size(120, 20) };
        ComboBox typeCombo = new ComboBox
        {
            Location = new Point(180, 68),
            Size = new Size(200, 25),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        typeCombo.Items.AddRange(new[] { "Full Time", "Part Time", "Contract" });
        typeCombo.SelectedIndex = 0;
        mainPanel.Controls.Add(typeLabel);
        mainPanel.Controls.Add(typeCombo);

        // ID
        Label idLabel = new Label { Text = "Employee ID:", Location = new Point(50, 110), Size = new Size(120, 20) };
        TextBox idBox = new TextBox { Location = new Point(180, 108), Size = new Size(200, 25) };
        mainPanel.Controls.Add(idLabel);
        mainPanel.Controls.Add(idBox);

        // Name
        Label nameLabel = new Label { Text = "Name:", Location = new Point(50, 150), Size = new Size(120, 20) };
        TextBox nameBox = new TextBox { Location = new Point(180, 148), Size = new Size(200, 25) };
        mainPanel.Controls.Add(nameLabel);
        mainPanel.Controls.Add(nameBox);

        // Department
        Label deptLabel = new Label { Text = "Department:", Location = new Point(50, 190), Size = new Size(120, 20) };
        ComboBox deptCombo = new ComboBox
        {
            Location = new Point(180, 188),
            Size = new Size(200, 25),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        deptCombo.Items.AddRange(Enum.GetNames(typeof(Department)));
        deptCombo.SelectedIndex = 0;
        mainPanel.Controls.Add(deptLabel);
        mainPanel.Controls.Add(deptCombo);

        // Dynamic fields panel
        Panel dynamicPanel = new Panel { Location = new Point(50, 230), Size = new Size(400, 120) };
        mainPanel.Controls.Add(dynamicPanel);

        typeCombo.SelectedIndexChanged += (s, e) => UpdateDynamicFields(typeCombo.SelectedIndex, dynamicPanel);
        UpdateDynamicFields(0, dynamicPanel);

        // Buttons
        Button addBtn = new Button
        {
            Text = "Add Employee",
            Location = new Point(180, 370),
            Size = new Size(120, 35),
            BackColor = Color.Black,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        addBtn.Click += (s, e) =>
        {
            try
            {
                int id = int.Parse(idBox.Text);
                string name = nameBox.Text;
                Department dept = (Department)deptCombo.SelectedIndex;

                switch (typeCombo.SelectedIndex)
                {
                    case 0: // Full Time
                        double baseSalary = double.Parse(((TextBox)dynamicPanel.Controls["baseSalary"]).Text);
                        double bonus = double.Parse(((TextBox)dynamicPanel.Controls["bonus"]).Text);
                        employees.Add(new FullTimeEmployee(id, name, dept, baseSalary, bonus));
                        break;
                    case 1: // Part Time
                        double hourlyRate = double.Parse(((TextBox)dynamicPanel.Controls["hourlyRate"]).Text);
                        int hours = int.Parse(((TextBox)dynamicPanel.Controls["hoursWorked"]).Text);
                        employees.Add(new PartTimeEmployee(id, name, dept, hourlyRate, hours));
                        break;
                    case 2: // Contract
                        double contractSalary = double.Parse(((TextBox)dynamicPanel.Controls["contractSalary"]).Text);
                        int duration = int.Parse(((TextBox)dynamicPanel.Controls["contractDuration"]).Text);
                        employees.Add(new ContractEmployee(id, name, dept, contractSalary, duration));
                        break;
                }
                MessageBox.Show("Employee added successfully!", "Success");
                ShowMainMenu();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error");
            }
        };

        Button backBtn = new Button
        {
            Text = "Back",
            Location = new Point(310, 370),
            Size = new Size(70, 35),
            FlatStyle = FlatStyle.Flat
        };
        backBtn.FlatAppearance.BorderColor = Color.Black;
        backBtn.Click += (s, e) => ShowMainMenu();

        mainPanel.Controls.Add(addBtn);
        mainPanel.Controls.Add(backBtn);
    }

    private void UpdateDynamicFields(int type, Panel panel)
    {
        panel.Controls.Clear();

        switch (type)
        {
            case 0: // Full Time
                panel.Controls.Add(new Label { Text = "Base Salary:", Location = new Point(0, 0), Size = new Size(120, 20) });
                panel.Controls.Add(new TextBox { Name = "baseSalary", Location = new Point(130, 0), Size = new Size(200, 25) });
                panel.Controls.Add(new Label { Text = "Bonus:", Location = new Point(0, 40), Size = new Size(120, 20) });
                panel.Controls.Add(new TextBox { Name = "bonus", Location = new Point(130, 40), Size = new Size(200, 25) });
                break;
            case 1: // Part Time
                panel.Controls.Add(new Label { Text = "Hourly Rate:", Location = new Point(0, 0), Size = new Size(120, 20) });
                panel.Controls.Add(new TextBox { Name = "hourlyRate", Location = new Point(130, 0), Size = new Size(200, 25) });
                panel.Controls.Add(new Label { Text = "Hours Worked:", Location = new Point(0, 40), Size = new Size(120, 20) });
                panel.Controls.Add(new TextBox { Name = "hoursWorked", Location = new Point(130, 40), Size = new Size(200, 25) });
                break;
            case 2: // Contract
                panel.Controls.Add(new Label { Text = "Contract Salary:", Location = new Point(0, 0), Size = new Size(120, 20) });
                panel.Controls.Add(new TextBox { Name = "contractSalary", Location = new Point(130, 0), Size = new Size(200, 25) });
                panel.Controls.Add(new Label { Text = "Duration (months):", Location = new Point(0, 40), Size = new Size(120, 20) });
                panel.Controls.Add(new TextBox { Name = "contractDuration", Location = new Point(130, 40), Size = new Size(200, 25) });
                break;
        }
    }

    private void ShowListEmployees()
    {
        ClearPanel();

        Label titleLabel = new Label
        {
            Text = "EMPLOYEE LIST",
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            Location = new Point(50, 20),
            Size = new Size(200, 30)
        };
        mainPanel.Controls.Add(titleLabel);

        ListView listView = new ListView
        {
            Location = new Point(50, 60),
            Size = new Size(500, 300),
            View = View.Details,
            FullRowSelect = true,
            GridLines = true,
            BorderStyle = BorderStyle.FixedSingle
        };

        listView.Columns.Add("ID", 50);
        listView.Columns.Add("Name", 120);
        listView.Columns.Add("Department", 100);
        listView.Columns.Add("Type", 100);
        listView.Columns.Add("Salary", 100);

        foreach (var emp in employees)
        {
            string type = emp switch
            {
                FullTimeEmployee => "FullTime",
                PartTimeEmployee => "PartTime",
                ContractEmployee => "Contract",
                _ => "Unknown"
            };
            var item = new ListViewItem(emp.Id.ToString());
            item.SubItems.Add(emp.Name);
            item.SubItems.Add(emp.Department.ToString());
            item.SubItems.Add(type);
            item.SubItems.Add(emp.CalculateSalary().ToString("C"));
            listView.Items.Add(item);
        }

        Label totalLabel = new Label
        {
            Text = $"Total Employees: {Employee.EmployeeCount}",
            Location = new Point(50, 370),
            Size = new Size(200, 20),
            Font = new Font("Segoe UI", 10F, FontStyle.Bold)
        };

        Button backBtn = new Button
        {
            Text = "Back",
            Location = new Point(480, 370),
            Size = new Size(70, 35),
            FlatStyle = FlatStyle.Flat
        };
        backBtn.FlatAppearance.BorderColor = Color.Black;
        backBtn.Click += (s, e) => ShowMainMenu();

        mainPanel.Controls.Add(listView);
        mainPanel.Controls.Add(totalLabel);
        mainPanel.Controls.Add(backBtn);
    }

    private void ShowMarkAttendance()
    {
        ClearPanel();

        Label titleLabel = new Label
        {
            Text = "MARK ATTENDANCE",
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            Location = new Point(50, 20),
            Size = new Size(250, 30)
        };
        mainPanel.Controls.Add(titleLabel);

        Label idLabel = new Label { Text = "Employee ID:", Location = new Point(50, 80), Size = new Size(120, 20) };
        TextBox idBox = new TextBox { Location = new Point(180, 78), Size = new Size(200, 25) };

        Label statusLabel = new Label { Text = "Status:", Location = new Point(50, 120), Size = new Size(120, 20) };
        ComboBox statusCombo = new ComboBox
        {
            Location = new Point(180, 118),
            Size = new Size(200, 25),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        statusCombo.Items.AddRange(Enum.GetNames(typeof(AttendanceStatus)));
        statusCombo.SelectedIndex = 0;

        Button markBtn = new Button
        {
            Text = "Mark Attendance",
            Location = new Point(180, 170),
            Size = new Size(130, 35),
            BackColor = Color.Black,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        markBtn.Click += (s, e) =>
        {
            try
            {
                int id = int.Parse(idBox.Text);
                var emp = employees.Find(e => e.Id == id);
                if (emp == null)
                {
                    MessageBox.Show("Employee not found!", "Error");
                    return;
                }
                var record = new AttendanceRecord { EmployeeId = id, Date = DateTime.Today, Status = AttendanceStatus.Absent };
                AttendanceUtils.MarkAttendance(ref record, (AttendanceStatus)statusCombo.SelectedIndex);
                attendanceRecords.Add(record);
                MessageBox.Show("Attendance marked successfully!", "Success");
                idBox.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error");
            }
        };

        Button backBtn = new Button
        {
            Text = "Back",
            Location = new Point(320, 170),
            Size = new Size(60, 35),
            FlatStyle = FlatStyle.Flat
        };
        backBtn.FlatAppearance.BorderColor = Color.Black;
        backBtn.Click += (s, e) => ShowMainMenu();

        mainPanel.Controls.Add(idLabel);
        mainPanel.Controls.Add(idBox);
        mainPanel.Controls.Add(statusLabel);
        mainPanel.Controls.Add(statusCombo);
        mainPanel.Controls.Add(markBtn);
        mainPanel.Controls.Add(backBtn);
    }

    private void ShowViewAttendance()
    {
        ClearPanel();

        Label titleLabel = new Label
        {
            Text = "ATTENDANCE RECORDS",
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            Location = new Point(50, 20),
            Size = new Size(250, 30)
        };
        mainPanel.Controls.Add(titleLabel);

        ListView listView = new ListView
        {
            Location = new Point(50, 60),
            Size = new Size(500, 280),
            View = View.Details,
            FullRowSelect = true,
            GridLines = true,
            BorderStyle = BorderStyle.FixedSingle
        };

        listView.Columns.Add("ID", 50);
        listView.Columns.Add("Name", 150);
        listView.Columns.Add("Date", 120);
        listView.Columns.Add("Status", 100);

        foreach (var rec in attendanceRecords)
        {
            var emp = employees.Find(e => e.Id == rec.EmployeeId);
            string name = emp?.Name ?? "Unknown";
            var item = new ListViewItem(rec.EmployeeId.ToString());
            item.SubItems.Add(name);
            item.SubItems.Add(rec.Date.ToString("yyyy-MM-dd"));
            item.SubItems.Add(rec.Status.ToString());
            listView.Items.Add(item);
        }

        int present = AttendanceUtils.CountPresent(attendanceRecords, out int total);
        Label statsLabel = new Label
        {
            Text = $"Present: {present} | Total Records: {total}",
            Location = new Point(50, 350),
            Size = new Size(300, 20),
            Font = new Font("Segoe UI", 10F, FontStyle.Bold)
        };

        Button backBtn = new Button
        {
            Text = "Back",
            Location = new Point(480, 370),
            Size = new Size(70, 35),
            FlatStyle = FlatStyle.Flat
        };
        backBtn.FlatAppearance.BorderColor = Color.Black;
        backBtn.Click += (s, e) => ShowMainMenu();

        mainPanel.Controls.Add(listView);
        mainPanel.Controls.Add(statsLabel);
        mainPanel.Controls.Add(backBtn);
    }

    private void ShowCompareEmployees()
    {
        ClearPanel();

        Label titleLabel = new Label
        {
            Text = "COMPARE EMPLOYEES",
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            Location = new Point(50, 20),
            Size = new Size(250, 30)
        };
        mainPanel.Controls.Add(titleLabel);

        Label id1Label = new Label { Text = "First Employee ID:", Location = new Point(50, 80), Size = new Size(130, 20) };
        TextBox id1Box = new TextBox { Location = new Point(190, 78), Size = new Size(150, 25) };

        Label id2Label = new Label { Text = "Second Employee ID:", Location = new Point(50, 120), Size = new Size(130, 20) };
        TextBox id2Box = new TextBox { Location = new Point(190, 118), Size = new Size(150, 25) };

        Label resultLabel = new Label
        {
            Location = new Point(50, 200),
            Size = new Size(500, 60),
            Font = new Font("Segoe UI", 10F),
            BorderStyle = BorderStyle.FixedSingle,
            TextAlign = ContentAlignment.MiddleCenter
        };

        Button compareBtn = new Button
        {
            Text = "Compare",
            Location = new Point(190, 160),
            Size = new Size(100, 30),
            BackColor = Color.Black,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        compareBtn.Click += (s, e) =>
        {
            try
            {
                int id1 = int.Parse(id1Box.Text);
                int id2 = int.Parse(id2Box.Text);
                var emp1 = employees.Find(e => e.Id == id1);
                var emp2 = employees.Find(e => e.Id == id2);

                if (emp1 == null || emp2 == null)
                {
                    MessageBox.Show("One or both employees not found!", "Error");
                    return;
                }

                if (emp1 > emp2)
                    resultLabel.Text = $"{emp1.Name} has a higher salary than {emp2.Name}\n({emp1.CalculateSalary():C} vs {emp2.CalculateSalary():C})";
                else if (emp1 < emp2)
                    resultLabel.Text = $"{emp2.Name} has a higher salary than {emp1.Name}\n({emp2.CalculateSalary():C} vs {emp1.CalculateSalary():C})";
                else
                    resultLabel.Text = $"Both have equal salary ({emp1.CalculateSalary():C})";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error");
            }
        };

        Button backBtn = new Button
        {
            Text = "Back",
            Location = new Point(480, 370),
            Size = new Size(70, 35),
            FlatStyle = FlatStyle.Flat
        };
        backBtn.FlatAppearance.BorderColor = Color.Black;
        backBtn.Click += (s, e) => ShowMainMenu();

        mainPanel.Controls.Add(id1Label);
        mainPanel.Controls.Add(id1Box);
        mainPanel.Controls.Add(id2Label);
        mainPanel.Controls.Add(id2Box);
        mainPanel.Controls.Add(compareBtn);
        mainPanel.Controls.Add(resultLabel);
        mainPanel.Controls.Add(backBtn);
    }

    private void SaveData()
    {
        try
        {
            using (var writer = new StreamWriter("employees.txt"))
            {
                foreach (var emp in employees)
                {
                    string type = emp.GetType().Name;
                    string line = type switch
                    {
                        nameof(FullTimeEmployee) => $"F|{emp.Id}|{emp.Name}|{emp.Department}|{emp.BaseSalary}|{((FullTimeEmployee)emp).Bonus}",
                        nameof(PartTimeEmployee) => $"P|{emp.Id}|{emp.Name}|{emp.Department}|{((PartTimeEmployee)emp).HourlyRate}|{((PartTimeEmployee)emp).HoursWorked}",
                        nameof(ContractEmployee) => $"C|{emp.Id}|{emp.Name}|{emp.Department}|{emp.BaseSalary}|{((ContractEmployee)emp).ContractDuration}",
                        _ => ""
                    };
                    writer.WriteLine(line);
                }
            }
            using (var writer = new StreamWriter("attendance.txt"))
            {
                foreach (var rec in attendanceRecords)
                {
                    writer.WriteLine($"{rec.EmployeeId}|{rec.Date:yyyy-MM-dd}|{rec.Status}");
                }
            }
            MessageBox.Show("Data saved successfully!", "Success");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Save error: {ex.Message}", "Error");
        }
    }

    private void LoadData()
    {
        try
        {
            employees.Clear();
            attendanceRecords.Clear();
            Employee.EmployeeCount = 0;

            if (File.Exists("employees.txt"))
            {
                foreach (var line in File.ReadAllLines("employees.txt"))
                {
                    var parts = line.Split('|');
                    switch (parts[0])
                    {
                        case "F":
                            employees.Add(new FullTimeEmployee(
                                int.Parse(parts[1]), parts[2], Enum.Parse<Department>(parts[3]),
                                double.Parse(parts[4]), double.Parse(parts[5])));
                            break;
                        case "P":
                            employees.Add(new PartTimeEmployee(
                                int.Parse(parts[1]), parts[2], Enum.Parse<Department>(parts[3]),
                                double.Parse(parts[4]), int.Parse(parts[5])));
                            break;
                        case "C":
                            employees.Add(new ContractEmployee(
                                int.Parse(parts[1]), parts[2], Enum.Parse<Department>(parts[3]),
                                double.Parse(parts[4]), int.Parse(parts[5])));
                            break;
                    }
                }
            }
            if (File.Exists("attendance.txt"))
            {
                foreach (var line in File.ReadAllLines("attendance.txt"))
                {
                    var parts = line.Split('|');
                    attendanceRecords.Add(new AttendanceRecord
                    {
                        EmployeeId = int.Parse(parts[0]),
                        Date = DateTime.Parse(parts[1]),
                        Status = Enum.Parse<AttendanceStatus>(parts[2])
                    });
                }
            }
            MessageBox.Show("Data loaded successfully!", "Success");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Load error: {ex.Message}", "Error");
        }
    }

    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm());
    }
}