using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using QQuizzles.Database;
using QQuizzles.Models;
using QQuizzles.Utilities;

namespace QQuizzles.Forms
{
    /// <summary>
    /// Form for viewing quiz results and statistics
    /// </summary>
    public partial class ViewResultsForm : Form
    {
        private readonly DatabaseManager _databaseManager;
        private readonly User _currentUser;
        private List<QuizResult> _userResults;

        // UI Controls
        private Panel headerPanel;
        private Panel statsPanel;
        private Panel resultsPanel;
        private Label titleLabel;
        private Label statsLabel;
        private DataGridView resultsGridView;
        private Button refreshButton;
        private Button closeButton;
        private ComboBox filterComboBox;

        public ViewResultsForm(User currentUser, DatabaseManager databaseManager)
        {
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _databaseManager = databaseManager ?? throw new ArgumentNullException(nameof(databaseManager));
            
            InitializeComponent();
            SetupUI();
            LoadResults();
        }

        private void InitializeComponent()
        {
            // Form settings
            this.Text = "Q-Quizzles - Geography Progress Report";
            this.Size = new Size(1100, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Apply geography theme
            GeographyTheme.ApplyFormStyle(this);

            // Initialize controls
            headerPanel = new Panel();
            statsPanel = new Panel();
            resultsPanel = new Panel();
            titleLabel = new Label();
            statsLabel = new Label();
            resultsGridView = new DataGridView();
            refreshButton = new Button();
            closeButton = new Button();
            filterComboBox = new ComboBox();

            this.Controls.AddRange(new Control[] { headerPanel, statsPanel, resultsPanel });
        }

        private void SetupUI()
        {
            // Header panel
            headerPanel.Size = new Size(980, 80);
            headerPanel.Location = new Point(10, 10);
            headerPanel.BackColor = Color.FromArgb(255, 193, 7);

            titleLabel.Text = "📊 Quiz Results & Statistics";
            titleLabel.Font = new Font("Arial", 18, FontStyle.Bold);
            titleLabel.ForeColor = Color.Black;
            titleLabel.Size = new Size(400, 30);
            titleLabel.Location = new Point(20, 15);

            var userLabel = new Label
            {
                Text = $"User: {_currentUser.Username}",
                Font = new Font("Arial", 12),
                ForeColor = Color.Black,
                Size = new Size(200, 25),
                Location = new Point(20, 45)
            };

            // Filter combo box
            var filterLabel = new Label
            {
                Text = "Filter by:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                Size = new Size(70, 25),
                Location = new Point(700, 20)
            };

            filterComboBox.Size = new Size(150, 30);
            filterComboBox.Location = new Point(780, 20);
            filterComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            filterComboBox.Items.AddRange(new object[] { "All Results", "Last 10", "This Week", "This Month" });
            filterComboBox.SelectedIndex = 0;
            filterComboBox.SelectedIndexChanged += FilterComboBox_SelectedIndexChanged;

            refreshButton.Text = "🔄 Refresh";
            refreshButton.Size = new Size(100, 30);
            refreshButton.Location = new Point(780, 45);
            refreshButton.BackColor = Color.FromArgb(0, 123, 255);
            refreshButton.ForeColor = Color.White;
            refreshButton.Font = new Font("Arial", 10, FontStyle.Bold);
            refreshButton.FlatStyle = FlatStyle.Flat;
            refreshButton.Click += RefreshButton_Click;

            headerPanel.Controls.AddRange(new Control[] { titleLabel, userLabel, filterLabel, filterComboBox, refreshButton });

            // Stats panel
            statsPanel.Size = new Size(980, 120);
            statsPanel.Location = new Point(10, 100);
            statsPanel.BackColor = Color.White;
            statsPanel.BorderStyle = BorderStyle.FixedSingle;

            statsLabel.Font = new Font("Arial", 11);
            statsLabel.Size = new Size(960, 100);
            statsLabel.Location = new Point(10, 10);
            statsLabel.TextAlign = ContentAlignment.TopLeft;

            statsPanel.Controls.Add(statsLabel);

            // Results panel
            resultsPanel.Size = new Size(980, 450);
            resultsPanel.Location = new Point(10, 230);
            resultsPanel.BackColor = Color.White;
            resultsPanel.BorderStyle = BorderStyle.FixedSingle;

            SetupResultsGrid();

            closeButton.Text = "Close";
            closeButton.Size = new Size(120, 40);
            closeButton.Location = new Point(430, 400);
            closeButton.BackColor = Color.FromArgb(220, 53, 69);
            closeButton.ForeColor = Color.White;
            closeButton.Font = new Font("Arial", 12, FontStyle.Bold);
            closeButton.FlatStyle = FlatStyle.Flat;
            closeButton.Click += CloseButton_Click;

            resultsPanel.Controls.AddRange(new Control[] { resultsGridView, closeButton });
        }

        private void SetupResultsGrid()
        {
            resultsGridView.Size = new Size(960, 350);
            resultsGridView.Location = new Point(10, 10);
            resultsGridView.BackgroundColor = Color.White;
            resultsGridView.BorderStyle = BorderStyle.None;
            resultsGridView.AllowUserToAddRows = false;
            resultsGridView.AllowUserToDeleteRows = false;
            resultsGridView.ReadOnly = true;
            resultsGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            resultsGridView.MultiSelect = false;
            resultsGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Add columns
            resultsGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Date",
                HeaderText = "Date",
                Width = 120
            });

            resultsGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Continent",
                HeaderText = "Continent",
                Width = 150
            });

            resultsGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Questions",
                HeaderText = "Questions",
                Width = 80
            });

            resultsGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Correct",
                HeaderText = "Correct",
                Width = 80
            });

            resultsGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Score",
                HeaderText = "Score %",
                Width = 80
            });

            resultsGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Grade",
                HeaderText = "Grade",
                Width = 60
            });

            resultsGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Time",
                HeaderText = "Time",
                Width = 80
            });

            resultsGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Performance",
                HeaderText = "Performance",
                Width = 200
            });

            // Style the grid
            resultsGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 123, 255);
            resultsGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            resultsGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            resultsGridView.ColumnHeadersHeight = 35;

            resultsGridView.DefaultCellStyle.Font = new Font("Arial", 9);
            resultsGridView.RowTemplate.Height = 30;

            // Alternate row colors
            resultsGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 248, 255);
        }

        private void LoadResults()
        {
            try
            {
                _userResults = _databaseManager.GetQuizResultsByUser(_currentUser.Id);
                ApplyFilter();
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading results: {ex.Message}", "Error", 
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyFilter()
        {
            var filteredResults = _userResults;

            switch (filterComboBox.SelectedIndex)
            {
                case 1: // Last 10
                    filteredResults = _userResults.Take(10).ToList();
                    break;
                case 2: // This Week
                    var weekAgo = DateTime.Now.AddDays(-7);
                    filteredResults = _userResults.Where(r => r.CompletedDate >= weekAgo).ToList();
                    break;
                case 3: // This Month
                    var monthAgo = DateTime.Now.AddDays(-30);
                    filteredResults = _userResults.Where(r => r.CompletedDate >= monthAgo).ToList();
                    break;
            }

            PopulateResultsGrid(filteredResults);
        }

        private void PopulateResultsGrid(List<QuizResult> results)
        {
            resultsGridView.Rows.Clear();

            foreach (var result in results)
            {
                var row = new DataGridViewRow();
                row.CreateCells(resultsGridView);

                row.Cells[0].Value = result.CompletedDate.ToString("yyyy-MM-dd HH:mm");
                row.Cells[1].Value = result.ContinentName;
                row.Cells[2].Value = result.TotalQuestions;
                row.Cells[3].Value = result.CorrectAnswers;
                row.Cells[4].Value = $"{result.ScorePercentage:F1}%";
                row.Cells[5].Value = result.GetGrade();
                row.Cells[6].Value = result.TimeTakenFormatted;
                row.Cells[7].Value = result.GetPerformanceMessage();

                // Color code based on performance
                if (result.ScorePercentage >= 80)
                    row.DefaultCellStyle.BackColor = Color.FromArgb(212, 237, 218); // Light green
                else if (result.ScorePercentage >= 60)
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 243, 205); // Light yellow
                else
                    row.DefaultCellStyle.BackColor = Color.FromArgb(248, 215, 218); // Light red

                resultsGridView.Rows.Add(row);
            }
        }

        private void UpdateStatistics()
        {
            if (_userResults == null || _userResults.Count == 0)
            {
                statsLabel.Text = "📈 No quiz results available yet. Take your first quiz to see statistics!";
                return;
            }

            // Calculate statistics
            int totalQuizzes = _userResults.Count;
            double averageScore = _userResults.Average(r => r.ScorePercentage);
            var bestResult = _userResults.OrderByDescending(r => r.ScorePercentage).First();
            var recentResult = _userResults.First(); // Most recent (already ordered by date desc)
            
            // Count by continent
            var continentStats = _userResults.GroupBy(r => r.ContinentName)
                                           .Select(g => new { Continent = g.Key, Count = g.Count(), AvgScore = g.Average(r => r.ScorePercentage) })
                                           .OrderByDescending(s => s.Count)
                                           .ToList();

            string statsText = $@"📈 Your Quiz Statistics:

🎯 Total Quizzes Taken: {totalQuizzes}
📊 Average Score: {averageScore:F1}%
🏆 Best Performance: {bestResult.ScorePercentage:F1}% ({bestResult.GetGrade()}) - {bestResult.ContinentName} on {bestResult.CompletedDate:yyyy-MM-dd}
🕒 Most Recent: {recentResult.ScorePercentage:F1}% - {recentResult.ContinentName} on {recentResult.CompletedDate:yyyy-MM-dd}

🌍 Performance by Continent:";

            foreach (var stat in continentStats.Take(3))
            {
                statsText += $"\n   • {stat.Continent}: {stat.Count} quiz(es), {stat.AvgScore:F1}% average";
            }

            statsLabel.Text = statsText;
        }

        private void FilterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            LoadResults();
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
