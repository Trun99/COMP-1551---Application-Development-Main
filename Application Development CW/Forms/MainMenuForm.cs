using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using QQuizzles.Database;
using QQuizzles.Models;
using QQuizzles.Utilities;

namespace QQuizzles.Forms
{
    /// <summary>
    /// Main menu form with 4 options - demonstrates professional UI design
    /// </summary>
    public partial class MainMenuForm : Form
    {
        private readonly DatabaseManager _databaseManager;
        private readonly User _currentUser;

        // UI Controls
        private Panel headerPanel;
        private Panel menuPanel;
        private Label titleLabel;
        private Label welcomeLabel;
        private Button playQuizButton;
        private Button createQuestionButton;

        private Button viewResultsButton;
        private Button exitButton;
        private Label statsLabel;

        public MainMenuForm(User currentUser)
        {
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _databaseManager = new DatabaseManager();
            InitializeComponent();
            SetupUI();
            LoadStats();
        }

        private void InitializeComponent()
        {
            // Form settings
            this.Text = "Q-Quizzles - Geography Explorer";
            this.Size = new Size(700, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Apply geography theme
            GeographyTheme.ApplyFormStyle(this);

            // Initialize controls
            headerPanel = new Panel();
            menuPanel = new Panel();
            titleLabel = new Label();
            welcomeLabel = new Label();
            playQuizButton = new Button();
            createQuestionButton = new Button();

            viewResultsButton = new Button();
            exitButton = new Button();
            statsLabel = new Label();

            this.Controls.AddRange(new Control[] { headerPanel, menuPanel });
        }

        private void SetupUI()
        {
            // Header panel with geography theme
            headerPanel.Size = new Size(680, 140);
            headerPanel.Location = new Point(10, 10);
            GeographyTheme.ApplyPanelStyle(headerPanel, true);

            // Title label with geography theme
            titleLabel.Text = "🌍 Q-Quizzles Geography Explorer";
            titleLabel.Size = new Size(660, 50);
            titleLabel.Location = new Point(10, 25);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            GeographyTheme.ApplyLabelStyle(titleLabel, LabelStyle.Header);

            // Welcome label with geography theme
            welcomeLabel.Text = $"🗺️ Welcome back, {_currentUser.Username}! Ready to explore the world?";
            welcomeLabel.Size = new Size(660, 35);
            welcomeLabel.Location = new Point(10, 75);
            welcomeLabel.TextAlign = ContentAlignment.MiddleCenter;
            GeographyTheme.ApplyLabelStyle(welcomeLabel, LabelStyle.Header);

            headerPanel.Controls.AddRange(new Control[] { titleLabel, welcomeLabel });

            // Menu panel with geography theme
            menuPanel.Size = new Size(680, 420);
            menuPanel.Location = new Point(10, 160);
            GeographyTheme.ApplyPanelStyle(menuPanel);

            // Modern button layout with improved spacing
            int buttonWidth = 280;
            int buttonHeight = 100;
            int spacing = 30;
            int startX = (680 - (2 * buttonWidth + spacing)) / 2;
            int startY = 50;

            // Play Quiz button - Primary action
            playQuizButton.Text = "🎮 Play Quiz";
            playQuizButton.Size = new Size(buttonWidth, buttonHeight);
            playQuizButton.Location = new Point(startX, startY);
            playQuizButton.Click += PlayQuizButton_Click;
            GeographyTheme.ApplyButtonStyle(playQuizButton, ButtonStyle.Primary);

            // Create Question button - Secondary action
            createQuestionButton.Text = "📝 Create Question";
            createQuestionButton.Size = new Size(buttonWidth, buttonHeight);
            createQuestionButton.Location = new Point(startX + buttonWidth + spacing, startY);
            createQuestionButton.Click += CreateQuestionButton_Click;
            GeographyTheme.ApplyButtonStyle(createQuestionButton, ButtonStyle.Secondary);



            // View Results button - Accent action
            viewResultsButton.Text = "📊 View Results";
            viewResultsButton.Size = new Size(buttonWidth, buttonHeight);
            viewResultsButton.Location = new Point(startX, startY + buttonHeight + spacing);
            viewResultsButton.Click += ViewResultsButton_Click;
            GeographyTheme.ApplyButtonStyle(viewResultsButton, ButtonStyle.Accent);

            // Exit button - Danger action
            exitButton.Text = "🚪 Exit Application";
            exitButton.Size = new Size(buttonWidth, buttonHeight);
            exitButton.Location = new Point(startX + buttonWidth + spacing, startY + buttonHeight + spacing);
            exitButton.Click += ExitButton_Click;
            GeographyTheme.ApplyButtonStyle(exitButton, ButtonStyle.Danger);

            // Stats label with geography theme
            statsLabel.Text = "🌍 Loading geography statistics...";
            statsLabel.Size = new Size(620, 100);
            statsLabel.Location = new Point(30, 320);
            statsLabel.TextAlign = ContentAlignment.TopLeft;
            GeographyTheme.ApplyLabelStyle(statsLabel, LabelStyle.Body);

            menuPanel.Controls.AddRange(new Control[]
            {
                playQuizButton, createQuestionButton,
                viewResultsButton, exitButton, statsLabel
            });

            // Add hover effects
            AddHoverEffect(playQuizButton, Color.FromArgb(40, 167, 69), Color.FromArgb(34, 142, 58));
            AddHoverEffect(createQuestionButton, Color.FromArgb(0, 123, 255), Color.FromArgb(0, 105, 217));
            AddHoverEffect(viewResultsButton, Color.FromArgb(255, 193, 7), Color.FromArgb(217, 164, 6));
            AddHoverEffect(exitButton, Color.FromArgb(220, 53, 69), Color.FromArgb(187, 45, 59));
        }

        private void AddHoverEffect(Button button, Color normalColor, Color hoverColor)
        {
            button.MouseEnter += (s, e) => button.BackColor = hoverColor;
            button.MouseLeave += (s, e) => button.BackColor = normalColor;
        }

        private void LoadStats()
        {
            try
            {
                int totalQuestions = _databaseManager.GetQuestionCount();
                var userResults = _databaseManager.GetQuizResultsByUser(_currentUser.Id);
                
                string statsText = $"📈 Statistics:\n\n";
                statsText += $"• Total Questions Available: {totalQuestions}\n";
                statsText += $"• Your Quiz Attempts: {userResults.Count}\n";
                
                if (userResults.Count > 0)
                {
                    // Calculate average score
                    double totalPercentage = 0;
                    foreach (var result in userResults)
                    {
                        totalPercentage += result.ScorePercentage;
                    }
                    double averageScore = totalPercentage / userResults.Count;
                    statsText += $"• Average Score: {averageScore:F1}%\n";
                }
                else
                {
                    statsText += $"• No quiz attempts yet - Start your first quiz!\n";
                }

                statsText += $"\n🌍 Geography awaits your exploration!";
                
                statsLabel.Text = statsText;
            }
            catch (Exception ex)
            {
                statsLabel.Text = $"Error loading statistics: {ex.Message}";
            }
        }

        private void PlayQuizButton_Click(object sender, EventArgs e)
        {
            try
            {
                int totalQuestions = _databaseManager.GetQuestionCount();
                if (totalQuestions == 0)
                {
                    MessageBox.Show("No questions available yet. Please create some questions first!",
                                  "No Questions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using (var playQuizForm = new PlayQuizForm(_currentUser, _databaseManager))
                {
                    // Only show dialog if form wasn't closed during initialization
                    if (!playQuizForm.IsDisposed)
                    {
                        playQuizForm.ShowDialog();
                    }
                }

                // Refresh stats after quiz
                LoadStats();
            }
            catch (ObjectDisposedException)
            {
                // Form was disposed during initialization (user cancelled continent selection)
                // This is normal behavior, no need to show error
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening quiz: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateQuestionButton_Click(object sender, EventArgs e)
        {
            try
            {
                var createQuestionForm = new CreateQuestionForm(_databaseManager);
                createQuestionForm.ShowDialog();

                // Refresh stats after creating questions
                LoadStats();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening question creator: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void ViewResultsButton_Click(object sender, EventArgs e)
        {
            try
            {
                var viewResultsForm = new ViewResultsForm(_currentUser, _databaseManager);
                viewResultsForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening results viewer: {ex.Message}", "Error", 
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to exit Q-Quizzles?", 
                                       "Exit Application", 
                                       MessageBoxButtons.YesNo, 
                                       MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                ExitButton_Click(this, EventArgs.Empty);
            }
            else
            {
                base.OnFormClosing(e);
            }
        }
    }
}
