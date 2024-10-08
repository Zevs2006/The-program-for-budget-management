using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace The_program_for_budget_management
{
    public partial class Form1 : Form
    {
        private List<Operation> operations = new List<Operation>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Инициализация DataGridView
            dataGridView1.Columns.Add("Date", "Дата");
            dataGridView1.Columns.Add("Category", "Категория");
            dataGridView1.Columns.Add("Amount", "Сумма");
            dataGridView1.Columns.Add("Type", "Тип");

            // Настройка графика
            chart1.Series.Clear();

            var incomeSeries = new Series("Доходы")
            {
                ChartType = SeriesChartType.Column,
                Color = Color.Green
            };
            var expensesSeries = new Series("Расходы")
            {
                ChartType = SeriesChartType.Column,
                Color = Color.Red
            };

            chart1.Series.Add(incomeSeries);
            chart1.Series.Add(expensesSeries);

            // Настройка осей
            chart1.ChartAreas.Clear();
            var chartArea = new ChartArea("MainArea");
            chart1.ChartAreas.Add(chartArea);

            // Инициализация легенды
            chart1.Legends.Clear();
            var legend = new Legend("Legend")
            {
                Docking = Docking.Top // Положение легенды
            };
            chart1.Legends.Add(legend);
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cmbCategory.SelectedItem?.ToString()) || string.IsNullOrWhiteSpace(txtAmount.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            // Создаем новую операцию
            var operation = new Operation
            {
                Date = dtpDate.Value,
                Category = cmbCategory.SelectedItem.ToString(),
                Amount = decimal.Parse(txtAmount.Text),
                Type = chkIncome.Checked ? "Доход" : "Расход"
            };

            // Добавляем операцию в список
            operations.Add(operation);
            UpdateDataGridView(); // Обновляем таблицу
            UpdateChart(); // Обновляем график
        }


        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridView1.SelectedRows[0];
                operations.RemoveAt(selectedRow.Index);
                UpdateDataGridView();
                UpdateChart();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridView1.SelectedRows[0];

                dtpDate.Value = (DateTime)selectedRow.Cells["Date"].Value;
                cmbCategory.SelectedItem = selectedRow.Cells["Category"].Value.ToString();
                txtAmount.Text = selectedRow.Cells["Amount"].Value.ToString();
                chkIncome.Checked = selectedRow.Cells["Type"].Value.ToString() == "Доход";

                operations.RemoveAt(selectedRow.Index);
                UpdateDataGridView();
                UpdateChart();
            }
        }

        private void btnAddCategory_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtNewCategory.Text))
            {
                cmbCategory.Items.Add(txtNewCategory.Text);
                txtNewCategory.Clear();
            }
        }

        private void UpdateDataGridView()
        {
            dataGridView1.Rows.Clear();

            foreach (var operation in operations)
            {
                dataGridView1.Rows.Add(operation.Date, operation.Category, operation.Amount, operation.Type);
            }
        }

        private void UpdateChart()
        {
            var incomeSeries = chart1.Series["Доходы"];
            var expensesSeries = chart1.Series["Расходы"];

            // Очистка только точек, а не самих серий
            incomeSeries.Points.Clear();
            expensesSeries.Points.Clear();

            // Группировка по категориям и суммирование значений
            var groupedByCategory = operations.GroupBy(o => o.Category);
            foreach (var group in groupedByCategory)
            {
                decimal totalIncome = group.Where(o => o.Type == "Доход").Sum(o => o.Amount);
                decimal totalExpenses = group.Where(o => o.Type == "Расход").Sum(o => o.Amount);

                if (totalIncome > 0)
                {
                    incomeSeries.Points.AddXY(group.Key, totalIncome);
                }
                if (totalExpenses > 0)
                {
                    expensesSeries.Points.AddXY(group.Key, totalExpenses);
                }
            }
        }



        private void ClearInputFields()
        {
            dtpDate.Value = DateTime.Now;
            cmbCategory.SelectedItem = null;
            txtAmount.Clear();
            chkIncome.Checked = false;
        }
    }

    public class Operation
    {
        public DateTime Date { get; set; }
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
    }
}
