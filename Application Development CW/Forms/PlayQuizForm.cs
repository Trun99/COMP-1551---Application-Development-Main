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

    public partial class PlayQuizForm : Form
    {
        private readonly DatabaseManager _databaseManager;
        private readonly User _currentUser;
        private List<Question> _questions;
        private int _currentQuestionIndex;
        private int _correctAnswers;
        private DateTime _startTime;
        private Continent _selectedContinent;
        private string _selectedContinentName;

        // UI Controls
        private Panel headerPanel;
        private Panel questionPanel;
        private Panel answerPanel;
        private Panel navigationPanel;
        private Label titleLabel;
        private Label progressLabel;
        private Label questionLabel;
        private TextBox answerTextBox;
        private Button[] optionButtons;
        private Button nextButton;
        private Button previousButton;
        private Button submitQuizButton;
        private Label timerLabel;
        private System.Windows.Forms.Timer uiTimer;

        public PlayQuizForm(User currentUser, DatabaseManager databaseManager)
        {
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _databaseManager = databaseManager ?? throw new ArgumentNullException(nameof(databaseManager));
            
            InitializeComponent();
            SetupUI();
            SelectContinentAndStartQuiz();
        }

        private void InitializeComponent()
        {
            // Form settings
            this.Text = "Q-Quizzles - Geography Quiz Adventure";
            this.Size = new Size(850, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Apply geography theme
            GeographyTheme.ApplyFormStyle(this);

            // Initialize controls
            headerPanel = new Panel();
            questionPanel = new Panel();
            answerPanel = new Panel();
            navigationPanel = new Panel();
            titleLabel = new Label();
            progressLabel = new Label();
            questionLabel = new Label();
            answerTextBox = new TextBox();
            optionButtons = new Button[4];
            nextButton = new Button();
            previousButton = new Button();
            submitQuizButton = new Button();
            timerLabel = new Label();
            uiTimer = new System.Windows.Forms.Timer();

            this.Controls.AddRange(new Control[] { headerPanel, questionPanel, answerPanel, navigationPanel });
        }

        private void SetupUI()
        {
            // Header panel
            headerPanel.Size = new Size(780, 80);
            headerPanel.Location = new Point(10, 10);
            headerPanel.BackColor = Color.FromArgb(40, 167, 69);

            titleLabel.Text = "🎮 Geography Quiz";
            titleLabel.Font = new Font("Arial", 18, FontStyle.Bold);
            titleLabel.ForeColor = Color.White;
            titleLabel.Size = new Size(300, 30);
            titleLabel.Location = new Point(20, 15);

            progressLabel.Text = "Question 1 of 10";
            progressLabel.Font = new Font("Arial", 12);
            progressLabel.ForeColor = Color.White;
            progressLabel.Size = new Size(200, 25);
            progressLabel.Location = new Point(20, 45);

            timerLabel.Text = "Time: 00:00";
            timerLabel.Font = new Font("Arial", 14, FontStyle.Bold);
            timerLabel.ForeColor = Color.White;
            timerLabel.Size = new Size(150, 30);
            timerLabel.Location = new Point(600, 25);
            timerLabel.TextAlign = ContentAlignment.MiddleRight;

            headerPanel.Controls.AddRange(new Control[] { titleLabel, progressLabel, timerLabel });

            // Question panel
            questionPanel.Size = new Size(780, 200);
            questionPanel.Location = new Point(10, 100);
            questionPanel.BackColor = Color.White;
            questionPanel.BorderStyle = BorderStyle.FixedSingle;

            questionLabel.Font = new Font("Arial", 14);
            questionLabel.Size = new Size(760, 180);
            questionLabel.Location = new Point(10, 10);
            questionLabel.TextAlign = ContentAlignment.TopLeft;

            questionPanel.Controls.Add(questionLabel);

            // Answer panel
            answerPanel.Size = new Size(780, 250);
            answerPanel.Location = new Point(10, 310);
            answerPanel.BackColor = Color.White;
            answerPanel.BorderStyle = BorderStyle.FixedSingle;

            // Navigation panel
            navigationPanel.Size = new Size(780, 80);
            navigationPanel.Location = new Point(10, 570);
            navigationPanel.BackColor = Color.White;
            navigationPanel.BorderStyle = BorderStyle.FixedSingle;

            SetupNavigationControls();

            // Setup timer
            uiTimer.Interval = 1000; // 1 second
            uiTimer.Tick += UiTimer_Tick;
        }

        private void SetupNavigationControls()
        {
            previousButton.Text = "⬅️ Previous";
            previousButton.Size = new Size(120, 40);
            previousButton.Location = new Point(50, 20);
            previousButton.BackColor = Color.FromArgb(108, 117, 125);
            previousButton.ForeColor = Color.White;
            previousButton.Font = new Font("Arial", 11, FontStyle.Bold);
            previousButton.FlatStyle = FlatStyle.Flat;
            previousButton.Click += PreviousButton_Click;

            nextButton.Text = "Next ➡️";
            nextButton.Size = new Size(120, 40);
            nextButton.Location = new Point(200, 20);
            nextButton.BackColor = Color.FromArgb(0, 123, 255);
            nextButton.ForeColor = Color.White;
            nextButton.Font = new Font("Arial", 11, FontStyle.Bold);
            nextButton.FlatStyle = FlatStyle.Flat;
            nextButton.Click += NextButton_Click;

            submitQuizButton.Text = "🏁 Submit Quiz";
            submitQuizButton.Size = new Size(150, 40);
            submitQuizButton.Location = new Point(580, 20);
            submitQuizButton.BackColor = Color.FromArgb(40, 167, 69);
            submitQuizButton.ForeColor = Color.White;
            submitQuizButton.Font = new Font("Arial", 11, FontStyle.Bold);
            submitQuizButton.FlatStyle = FlatStyle.Flat;
            submitQuizButton.Click += SubmitQuizButton_Click;

            navigationPanel.Controls.AddRange(new Control[] { previousButton, nextButton, submitQuizButton });
        }

        private void SelectContinentAndStartQuiz()
        {
            // Create continent selection dialog
            var continentForm = new Form
            {
                Text = "Select Continent",
                Size = new Size(500, 400),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var titleLabel = new Label
            {
                Text = "🌍 Choose a Continent for Your Quiz",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Size = new Size(480, 40),
                Location = new Point(10, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var continentListBox = new ListBox
            {
                Size = new Size(460, 200),
                Location = new Point(20, 70),
                Font = new Font("Arial", 12)
            };

            // Add "All Continents" option
            continentListBox.Items.Add("🌍 All Continents");

            // Add individual continents
            foreach (var continent in ContinentHelper.GetAllContinents())
            {
                string displayText = $"{ContinentHelper.GetEmoji(continent)} {ContinentHelper.GetDisplayName(continent)}";
                continentListBox.Items.Add(displayText);
            }

            continentListBox.SelectedIndex = 0;

            var startButton = new Button
            {
                Text = "Start Quiz",
                Size = new Size(150, 40),
                Location = new Point(175, 300),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                Font = new Font("Arial", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };

            startButton.Click += (s, e) =>
            {
                int selectedIndex = continentListBox.SelectedIndex;
                if (selectedIndex == 0)
                {
                    // All continents
                    _selectedContinentName = "All Continents";
                    _questions = _databaseManager.GetAllQuestions();
                }
                else
                {
                    // Specific continent
                    var continent = ContinentHelper.GetAllContinents()[selectedIndex - 1];
                    _selectedContinent = continent;
                    _selectedContinentName = ContinentHelper.GetDisplayName(continent);
                    _questions = _databaseManager.GetQuestionsByContinent(continent);
                }

                continentForm.DialogResult = DialogResult.OK;
                continentForm.Close();
            };

            continentForm.Controls.AddRange(new Control[] { titleLabel, continentListBox, startButton });

            var result = continentForm.ShowDialog();
            continentForm.Dispose();

            if (result == DialogResult.OK)
            {
                StartQuiz();
            }
            else
            {
                // User cancelled - set DialogResult and close properly
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void StartQuiz()
        {
            if (_questions == null || _questions.Count == 0)
            {
                MessageBox.Show($"No questions available for {_selectedContinentName}. Please create some questions first!",
                              "No Questions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.Cancel;
                this.Close();
                return;
            }

            // Shuffle questions for randomness
            var random = new Random();
            _questions = _questions.OrderBy(q => random.Next()).ToList();

            // Initialize quiz state
            _currentQuestionIndex = 0;
            _correctAnswers = 0;
            _startTime = DateTime.Now;

            // Update title to show selected continent
            titleLabel.Text = $"🎮 {_selectedContinentName} Quiz";

            // Start timer
            uiTimer.Start();

            // Display first question
            DisplayCurrentQuestion();
        }

        private void DisplayCurrentQuestion()
        {
            if (_currentQuestionIndex < 0 || _currentQuestionIndex >= _questions.Count)
                return;

            var currentQuestion = _questions[_currentQuestionIndex];

           
            progressLabel.Text = $"Question {_currentQuestionIndex + 1} of {_questions.Count}";

          
            questionLabel.Text = currentQuestion.GetDisplayText();

            
            SetupAnswerControlsForQuestion(currentQuestion);

           
            previousButton.Enabled = _currentQuestionIndex > 0;
            nextButton.Text = _currentQuestionIndex == _questions.Count - 1 ? "Finish ➡️" : "Next ➡️";
        }

        private void SetupAnswerControlsForQuestion(Question question)
        {
            
            answerPanel.Controls.Clear();

            
            switch (question.Type)
            {
                case QuestionType.TrueFalse:
                    SetupTrueFalseAnswerControls();
                    break;
                case QuestionType.OpenEnded:
                    SetupOpenEndedAnswerControls();
                    break;
                case QuestionType.MultipleChoice:
                    SetupMultipleChoiceAnswerControls(question as MultipleChoiceQuestion);
                    break;
            }
        }

        private void SetupTrueFalseAnswerControls()
        {
            var trueButton = new Button
            {
                Text = "✅ True",
                Size = new Size(200, 60),
                Location = new Point(150, 80),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                Font = new Font("Arial", 14, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Tag = "True"
            };

            var falseButton = new Button
            {
                Text = "❌ False",
                Size = new Size(200, 60),
                Location = new Point(400, 80),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                Font = new Font("Arial", 14, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Tag = "False"
            };

            trueButton.Click += TrueFalseButton_Click;
            falseButton.Click += TrueFalseButton_Click;

            answerPanel.Controls.AddRange(new Control[] { trueButton, falseButton });
        }

        private void SetupOpenEndedAnswerControls()
        {
            var instructionLabel = new Label
            {
                Text = "Enter your answer (1-4 words):",
                Font = new Font("Arial", 12, FontStyle.Bold),
                Size = new Size(300, 25),
                Location = new Point(50, 50)
            };

            answerTextBox = new TextBox
            {
                Size = new Size(600, 40),
                Location = new Point(50, 80),
                Font = new Font("Arial", 14),
                BorderStyle = BorderStyle.FixedSingle
            };

            answerTextBox.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    NextButton_Click(s, e);
                }
            };

            answerPanel.Controls.AddRange(new Control[] { instructionLabel, answerTextBox });
            answerTextBox.Focus();
        }

        private void SetupMultipleChoiceAnswerControls(MultipleChoiceQuestion mcQuestion)
        {
            for (int i = 0; i < 4; i++)
            {
                char optionLetter = (char)('A' + i);
                optionButtons[i] = new Button
                {
                    Text = $"{optionLetter}. {mcQuestion.GetOption(i)}",
                    Size = new Size(700, 45),
                    Location = new Point(40, 20 + i * 55),
                    BackColor = Color.FromArgb(0, 123, 255),
                    ForeColor = Color.White,
                    Font = new Font("Arial", 12, FontStyle.Bold),
                    FlatStyle = FlatStyle.Flat,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Tag = optionLetter.ToString()
                };

                optionButtons[i].Click += MultipleChoiceButton_Click;
                answerPanel.Controls.Add(optionButtons[i]);
            }
        }

        private void TrueFalseButton_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            string answer = button.Tag.ToString();

            // Highlight selected button
            foreach (Control control in answerPanel.Controls)
            {
                if (control is Button btn)
                {
                    btn.BackColor = btn == button ? Color.FromArgb(255, 193, 7) :
                                   (btn.Tag.ToString() == "True" ? Color.FromArgb(40, 167, 69) : Color.FromArgb(220, 53, 69));
                }
            }

            // Store answer and move to next question after a short delay
            System.Windows.Forms.Timer delayTimer = new System.Windows.Forms.Timer { Interval = 500 };
            delayTimer.Tick += (s, args) =>
            {
                delayTimer.Stop();
                ProcessAnswer(answer);
                MoveToNextQuestion();
            };
            delayTimer.Start();
        }

        private void MultipleChoiceButton_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            string answer = button.Tag.ToString();

            // Highlight selected button
            foreach (Control control in answerPanel.Controls)
            {
                if (control is Button btn)
                {
                    btn.BackColor = btn == button ? Color.FromArgb(255, 193, 7) : Color.FromArgb(0, 123, 255);
                }
            }

            // Store answer and move to next question after a short delay
            System.Windows.Forms.Timer delayTimer = new System.Windows.Forms.Timer { Interval = 500 };
            delayTimer.Tick += (s, args) =>
            {
                delayTimer.Stop();
                ProcessAnswer(answer);
                MoveToNextQuestion();
            };
            delayTimer.Start();
        }

        private void ProcessAnswer(string userAnswer)
        {
            var currentQuestion = _questions[_currentQuestionIndex];

            // Use polymorphism to check answer
            bool isCorrect = currentQuestion.CheckAnswer(userAnswer);

            if (isCorrect)
            {
                _correctAnswers++;
            }
        }

        private void MoveToNextQuestion()
        {
            if (_currentQuestionIndex < _questions.Count - 1)
            {
                _currentQuestionIndex++;
                DisplayCurrentQuestion();
            }
            else
            {
                FinishQuiz();
            }
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            // For open-ended questions, get the answer from textbox
            if (answerTextBox != null && answerTextBox.Visible)
            {
                string answer = ValidationHelper.SanitizeInput(answerTextBox.Text);
                if (string.IsNullOrWhiteSpace(answer))
                {
                    MessageBox.Show("Please enter an answer before proceeding.", "Answer Required",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    answerTextBox.Focus();
                    return;
                }
                ProcessAnswer(answer);
            }

            MoveToNextQuestion();
        }

        private void PreviousButton_Click(object sender, EventArgs e)
        {
            if (_currentQuestionIndex > 0)
            {
                _currentQuestionIndex--;
                DisplayCurrentQuestion();
            }
        }

        private void SubmitQuizButton_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to submit your quiz? You cannot change your answers after submission.",
                                       "Submit Quiz", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                FinishQuiz();
            }
        }

        private void FinishQuiz()
        {
            uiTimer.Stop();

            TimeSpan timeTaken = DateTime.Now - _startTime;

            // Save quiz result
            var quizResult = new QuizResult(
                _currentUser.Id,
                _currentUser.Username,
                _questions.Count,
                _correctAnswers,
                timeTaken,
                _selectedContinent,
                _selectedContinentName
            );

            try
            {
                _databaseManager.SaveQuizResult(quizResult);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving quiz result: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // Show results
            ShowQuizResults(quizResult);
        }

        private void ShowQuizResults(QuizResult result)
        {
            var resultsForm = new Form
            {
                Text = "Quiz Results",
                Size = new Size(600, 500),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.White
            };

            var titleLabel = new Label
            {
                Text = "🎉 Quiz Completed!",
                Font = new Font("Arial", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 167, 69),
                Size = new Size(580, 40),
                Location = new Point(10, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var resultsText = $@"
📊 Your Results:

Score: {result.CorrectAnswers} out of {result.TotalQuestions} ({result.ScorePercentage:F1}%)
Grade: {result.GetGrade()}
Time Taken: {result.TimeTakenInMinutes}
Continent: {result.ContinentName}

{result.GetPerformanceMessage()}

Date: {result.CompletedDate:yyyy-MM-dd HH:mm:ss}";

            var resultsLabel = new Label
            {
                Text = resultsText,
                Font = new Font("Arial", 12),
                Size = new Size(560, 300),
                Location = new Point(20, 80),
                TextAlign = ContentAlignment.TopLeft
            };

            var closeButton = new Button
            {
                Text = "Close",
                Size = new Size(120, 40),
                Location = new Point(240, 400),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                Font = new Font("Arial", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };

            closeButton.Click += (s, e) =>
            {
                resultsForm.Close();
                this.Close();
            };

            resultsForm.Controls.AddRange(new Control[] { titleLabel, resultsLabel, closeButton });
            resultsForm.ShowDialog();
        }

        private void UiTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsed = DateTime.Now - _startTime;
            timerLabel.Text = $"Time: {elapsed.Minutes:D2}:{elapsed.Seconds:D2}";
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            uiTimer?.Stop();
            base.OnFormClosing(e);
        }
    }
}
