using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ToDoListApp
{   
    public partial class frmToDoList : Form
    {
        ColorDialog colorDialog = new ColorDialog();
        public frmToDoList()
        {
            InitializeComponent();
            chkDarkMode.CheckedChanged += new EventHandler(chkDarkMode_CheckedChanged);
            btnChangeTheme.Click += new EventHandler(btnChangeTheme_Click);
            lstTasks.DrawItem += new DrawItemEventHandler(lstTasks_DrawItem);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtTask.Text))
            {
                lstTasks.Items.Add(txtTask.Text);
                txtTask.Clear();
                UpdateTaskCount();
                CheckTaskDeadlines();
            }
            else
            {
                MessageBox.Show("Please enter a task!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (lstTasks.SelectedIndex != -1)
            {
                txtTask.Text = lstTasks.SelectedItem.ToString();
                lstTasks.Items.RemoveAt(lstTasks.SelectedIndex);
                UpdateTaskCount();
            }
            else
            {
                MessageBox.Show("Please select a task to edit!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstTasks.SelectedIndex != -1)
            {
                lstTasks.Items.RemoveAt(lstTasks.SelectedIndex);
                UpdateTaskCount();
                CheckTaskDeadlines();
            }
            else
            {
                MessageBox.Show("Please select a task to delete!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnMarkComplete_Click(object sender, EventArgs e)
        {
            if (lstTasks.SelectedIndex != -1)
            {
                string selectedTask = lstTasks.SelectedItem.ToString();
                if (!selectedTask.StartsWith("✔ "))
                {
                    lstTasks.Items[lstTasks.SelectedIndex] = "✔ " + selectedTask;
                    CheckTaskDeadlines();
                }
                else
                {
                    MessageBox.Show("This task is already marked as completed!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }          
            else
            {
                MessageBox.Show("Please select a task to mark as completed!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void UpdateTaskCount()
        {
            lblTotalTasks.Text = "Total Tasks: " + lstTasks.Items.Count;
        }

        private void chkDarkMode_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDarkMode.Checked)
            {
                this.BackColor = Color.FromArgb(45, 45, 48); // Koyu Gri
                lblTitle.ForeColor = Color.White;
                lblTotalTasks.ForeColor = Color.White;
                txtTask.BackColor = Color.FromArgb(30, 30, 30);
                txtTask.ForeColor = Color.White;
                lstTasks.BackColor = Color.FromArgb(30, 30, 30);
                lstTasks.ForeColor = Color.White;
                btnAdd.BackColor = Color.Gray;
                btnEdit.BackColor = Color.Gray;
                btnDelete.BackColor = Color.Gray;
                btnMarkComplete.BackColor = Color.Gray;
            }
            else
            {
                this.BackColor = Color.LightSteelBlue;
                lblTitle.ForeColor = Color.Navy;
                lblTotalTasks.ForeColor = Color.DarkBlue;
                txtTask.BackColor = Color.White;
                txtTask.ForeColor = Color.Black;
                lstTasks.BackColor = Color.White;
                lstTasks.ForeColor = Color.Black;
                btnAdd.BackColor = Color.LightGreen;
                btnEdit.BackColor = Color.LightBlue;
                btnDelete.BackColor = Color.LightCoral;
                btnMarkComplete.BackColor = Color.Orange;
            }
        }

        private void btnChangeTheme_Click(object sender, EventArgs e)
        {
              
                if (colorDialog.ShowDialog() == DialogResult.OK) // Kullanıcı "Tamam" dediğinde uygula
                {
                    Color SelectedColor = colorDialog.Color; // Seçilen rengi kaydet
                    
                    this.BackColor = colorDialog.Color;
                    txtTask.BackColor = colorDialog.Color;
                    lstTasks.BackColor= colorDialog.Color;
                    grpPriority.BackColor = colorDialog.Color;

                    btnAdd.BackColor = ControlPaint.Dark(colorDialog.Color);
                    btnEdit.BackColor = ControlPaint.Dark(colorDialog.Color);
                    btnDelete.BackColor = ControlPaint.Dark(colorDialog.Color);
                    btnMarkComplete.BackColor = ControlPaint.Dark(colorDialog.Color);

                }
        }
        

        private void btnAddWithDate_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtTask.Text))
            {
                string taskText = txtTask.Text;
                string dueDate = dtpDueDate.Value.ToShortDateString(); // Seçilen tarihi al
                string priority = GetTaskPriority(); // Öncelik belirleme
                string finalTask = $"{priority}{taskText} - Due: {dueDate}"; // Son hali
               
                lstTasks.Items.Add(finalTask);
                txtTask.Clear();
                UpdateTaskCount();
                CheckTaskDeadlines(); // Tarih kontrolü yap               
            }
            else
            {
                MessageBox.Show("Please enter a task!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void CheckTaskDeadlines()
        {            
            for (int i = 0; i < lstTasks.Items.Count; i++)
            {
                string task = lstTasks.Items[i].ToString();
                if (task.Contains("Due:"))
                {
                    string dateString = task.Substring(task.IndexOf("Due:") + 5);
                    if (DateTime.TryParse(dateString, out DateTime dueDate))
                    {
                        TimeSpan difference = dueDate - DateTime.Today;

                        if (difference.TotalDays < 0) // Tarih geçtiyse kırmızı
                        {
                            lstTasks.Items[i] =task.StartsWith("⛔" ) ? task : $"⛔{task}";
                        }
                        else if (difference.TotalDays <= 2) // 2 gün içinde yaklaşan görevleri sarı yap
                        {
                            lstTasks.Items[i] = task.StartsWith("⚠") ? task : $"⚠ {task}";
                        }
                    }
                }
            }
            lstTasks.Refresh();
        }
        private void lstTasks_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            e.DrawBackground();

            string taskText = lstTasks.Items[e.Index].ToString();
            Color textColor = Color.Black;

            if (taskText.Contains("[High]")) textColor = Color.Red;
            else if (taskText.Contains("[Medium]")) textColor = Color.Blue;
            else if (taskText.Contains("[Low]")) textColor = Color.Green;
            else if (taskText.StartsWith("⛔")) textColor = Color.DarkRed; // Geçmiş görevler
            else if (taskText.StartsWith("⚠")) textColor = Color.Orange;  // Yaklaşan görevler

            using (Brush brush = new SolidBrush(textColor))
            {
                e.Graphics.DrawString(taskText, e.Font, brush, e.Bounds);
            }

            e.DrawFocusRectangle();
        }
        private string GetTaskPriority()
        {
            if (rdoHigh.Checked) return "[High] ";
            if (rdoMedium.Checked) return "[Medium] ";
            if (rdoLow.Checked) return "[Low] ";
            return "";
        }
    }
}







