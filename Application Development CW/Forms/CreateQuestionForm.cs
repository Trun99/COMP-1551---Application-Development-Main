using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QQuizzles.Database;
using QQuizzles.Models;
using QQuizzles.Utilities;

namespace QQuizzles.Forms
{
    /// <summary>
    /// Form for creating new geography questions
    /// Demonstrates OOP principles: Encapsulation, Inheritance, Polymorphism, Abstraction
    /// </summary>
    public partial class CreateQuestionForm : Form
    {
        private readonly DatabaseManager _databaseManager;
        private readonly List<Question> _pendingQuestions;

        // UI Controls
        private Panel headerPanel;
        private Panel questionPanel;
        private Panel continentPanel;
        private Panel actionsPanel;
        private Label titleLabel;
        private ComboBox questionTypeComboBox;
        private TextBox questionTextBox;
        private Panel answerPanel;
        private ComboBox continentComboBox;
        private ListBox pendingQuestionsListBox;
        private Button saveAllButton;
        private Button clearAllButton;
        private Button addQuestionButton;
        private Button backButton;
        private Button manageQuestionsButton; // New button for managing questions

        private Label continentDisplayLabel;

        // Dynamic answer controls
        private Control[] answerControls;

        public CreateQuestionForm(DatabaseManager databaseManager)
        {
            _databaseManager = databaseManager ?? throw new ArgumentNullException(nameof(databaseManager));
            _pendingQuestions = new List<Question>();
            answerControls = Array.Empty<Control>();
            InitializeComponent();
            SetupUI();
        }

        private void InitializeComponent()
        {
            this.Text = "Q-Quizzles - Create Geography Questions";
            this.Size = new Size(1000, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = GeographyTheme.LightBackground;

            // Initialize controls
            headerPanel = new Panel();
            questionPanel = new Panel();
            continentPanel = new Panel();
            actionsPanel = new Panel();
            titleLabel = new Label();
            questionTypeComboBox = new ComboBox();
            questionTextBox = new TextBox();
            answerPanel = new Panel();
            continentComboBox = new ComboBox();
            pendingQuestionsListBox = new ListBox();
            saveAllButton = new Button();
            clearAllButton = new Button();
            addQuestionButton = new Button();
            backButton = new Button();
            manageQuestionsButton = new Button();

            continentDisplayLabel = new Label();

            this.Controls.AddRange(new Control[] { headerPanel, questionPanel, continentPanel, actionsPanel });
        }

        private void SetupUI()
        {
            // Header panel with modern card style
            headerPanel.Size = new Size(960, 80);
            headerPanel.Location = new Point(20, 20);
            GeographyTheme.ApplyPanelStyle(headerPanel, false, true);

            titleLabel.Text = "🌍 Create Geography Questions";
            titleLabel.Font = new Font("Arial", 18, FontStyle.Bold);
            titleLabel.Size = new Size(400, 40);
            titleLabel.Location = new Point(20, 20);
            titleLabel.ForeColor = GeographyTheme.PrimaryOceanBlue;

            headerPanel.Controls.AddRange(new Control[] { titleLabel });

            // Question panel with modern card style
            questionPanel.Size = new Size(640, 520);
            questionPanel.Location = new Point(20, 110);
            GeographyTheme.ApplyPanelStyle(questionPanel, false, true);

            SetupQuestionControls();

            // Continent panel with modern card style
            continentPanel.Size = new Size(320, 520);
            continentPanel.Location = new Point(680, 110);
            GeographyTheme.ApplyPanelStyle(continentPanel, false, true);

            SetupContinentControls();

            // Actions panel with modern card style
            actionsPanel.Size = new Size(960, 200);
            actionsPanel.Location = new Point(20, 650);
            GeographyTheme.ApplyPanelStyle(actionsPanel, false, true);

            SetupActionsControls();
        }

        private void SetupQuestionControls()
        {
            var questionTypeLabel = new Label
            {
                Text = "Question Type:",
                Font = new Font("Arial", 12, FontStyle.Bold),
                Size = new Size(150, 30),
                Location = new Point(20, 20),
                ForeColor = GeographyTheme.PrimaryOceanBlue
            };

            questionTypeComboBox.Size = new Size(200, 30);
            questionTypeComboBox.Location = new Point(180, 20);
            questionTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            questionTypeComboBox.Items.AddRange(new[] { "True/False", "Open Ended", "Multiple Choice" });
            questionTypeComboBox.SelectedIndex = 0;
            questionTypeComboBox.SelectedIndexChanged += QuestionTypeComboBox_SelectedIndexChanged;

            var questionTextLabel = new Label
            {
                Text = "Question Text:",
                Font = new Font("Arial", 12, FontStyle.Bold),
                Size = new Size(150, 30),
                Location = new Point(20, 70),
                ForeColor = GeographyTheme.PrimaryOceanBlue
            };

            questionTextBox.Size = new Size(580, 80);
            questionTextBox.Location = new Point(20, 100);
            questionTextBox.Multiline = true;
            questionTextBox.Font = new Font("Arial", 11);
            questionTextBox.ScrollBars = ScrollBars.Vertical;

            var answerLabel = new Label
            {
                Text = "Answer Options:",
                Font = new Font("Arial", 12, FontStyle.Bold),
                Size = new Size(150, 30),
                Location = new Point(20, 200),
                ForeColor = GeographyTheme.PrimaryOceanBlue
            };

            answerPanel.Size = new Size(580, 250);
            answerPanel.Location = new Point(20, 230);
            answerPanel.BorderStyle = BorderStyle.FixedSingle;

            addQuestionButton.Text = "➕ Add Question";
            addQuestionButton.Size = new Size(150, 40);
            addQuestionButton.Location = new Point(450, 480);
            addQuestionButton.BackColor = GeographyTheme.ButtonAccent;
            addQuestionButton.ForeColor = Color.White;
            addQuestionButton.Font = new Font("Arial", 11, FontStyle.Bold);
            addQuestionButton.FlatStyle = FlatStyle.Flat;
            addQuestionButton.Click += AddQuestionButton_Click;

            questionPanel.Controls.AddRange(new Control[]
            {
                questionTypeLabel, questionTypeComboBox,
                questionTextLabel, questionTextBox,
                answerLabel, answerPanel,
                addQuestionButton
            });

            // Setup initial answer controls
            SetupAnswerControls();
        }

        private void SetupContinentControls()
        {
            var continentLabel = new Label
            {
                Text = "🌍 Select Continent:",
                Font = new Font("Arial", 12, FontStyle.Bold),
                Size = new Size(200, 30),
                Location = new Point(20, 20),
                ForeColor = GeographyTheme.PrimaryOceanBlue
            };

            continentComboBox.Size = new Size(280, 30);
            continentComboBox.Location = new Point(20, 60);
            continentComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            continentComboBox.Items.AddRange(ContinentHelper.GetAllContinentNames());
            continentComboBox.SelectedIndex = 0;
            continentComboBox.SelectedIndexChanged += ContinentComboBox_SelectedIndexChanged;

            continentDisplayLabel.Size = new Size(280, 200);
            continentDisplayLabel.Location = new Point(20, 100);
            continentDisplayLabel.BorderStyle = BorderStyle.FixedSingle;
            continentDisplayLabel.TextAlign = ContentAlignment.MiddleCenter;
            continentDisplayLabel.Font = new Font("Arial", 24, FontStyle.Bold);
            continentDisplayLabel.ForeColor = GeographyTheme.PrimaryOceanBlue;
            continentDisplayLabel.BackColor = Color.FromArgb(245, 250, 255);

            continentPanel.Controls.AddRange(new Control[]
            {
                continentLabel, continentComboBox, continentDisplayLabel
            });

            // Load initial continent text
            LoadContinentDisplay();
        }

        private void SetupActionsControls()
        {
            var pendingLabel = new Label
            {
                Text = "📝 Pending Questions:",
                Font = new Font("Arial", 12, FontStyle.Bold),
                Size = new Size(200, 30),
                Location = new Point(20, 20),
                ForeColor = GeographyTheme.PrimaryOceanBlue
            };

            pendingQuestionsListBox.Size = new Size(600, 120);
            pendingQuestionsListBox.Location = new Point(20, 50);
            pendingQuestionsListBox.Font = new Font("Arial", 10);

            saveAllButton.Text = "💾 Save All Questions";
            saveAllButton.Size = new Size(150, 40);
            saveAllButton.Location = new Point(640, 50);
            saveAllButton.BackColor = GeographyTheme.ButtonPrimary;
            saveAllButton.ForeColor = Color.White;
            saveAllButton.Font = new Font("Arial", 11, FontStyle.Bold);
            saveAllButton.FlatStyle = FlatStyle.Flat;
            saveAllButton.Click += SaveAllButton_Click;

            clearAllButton.Text = "🗑️ Clear All";
            clearAllButton.Size = new Size(150, 40);
            clearAllButton.Location = new Point(640, 100);
            clearAllButton.BackColor = Color.FromArgb(220, 53, 69);
            clearAllButton.ForeColor = Color.White;
            clearAllButton.Font = new Font("Arial", 11, FontStyle.Bold);
            clearAllButton.FlatStyle = FlatStyle.Flat;
            clearAllButton.Click += ClearAllButton_Click;

            manageQuestionsButton.Text = "🗂️ Manage Questions";
            manageQuestionsButton.Size = new Size(150, 40);
            manageQuestionsButton.Location = new Point(810, 50);
            manageQuestionsButton.BackColor = Color.FromArgb(255, 193, 7);
            manageQuestionsButton.ForeColor = Color.Black;
            manageQuestionsButton.Font = new Font("Arial", 11, FontStyle.Bold);
            manageQuestionsButton.FlatStyle = FlatStyle.Flat;
            manageQuestionsButton.Click += ManageQuestionsButton_Click;

            backButton.Text = "🔙 Back to Menu";
            backButton.Size = new Size(150, 40);
            backButton.Location = new Point(810, 100);
            backButton.BackColor = GeographyTheme.ButtonSecondary;
            backButton.ForeColor = Color.White;
            backButton.Font = new Font("Arial", 11, FontStyle.Bold);
            backButton.FlatStyle = FlatStyle.Flat;
            backButton.Click += BackButton_Click;

            var instructionLabel = new Label
            {
                Text = "💡 Create questions and add them to the pending list, then save all at once.",
                Font = new Font("Arial", 9, FontStyle.Italic),
                Size = new Size(600, 20),
                Location = new Point(20, 175),
                ForeColor = GeographyTheme.LightText
            };

            actionsPanel.Controls.AddRange(new Control[]
            {
                pendingLabel, pendingQuestionsListBox,
                saveAllButton, clearAllButton, manageQuestionsButton, backButton, instructionLabel
            });
        }

        /// <summary>
        /// Setup answer controls based on question type - demonstrates polymorphism
        /// </summary>
        private void SetupAnswerControls()
        {
            answerPanel.Controls.Clear();

            var selectedType = questionTypeComboBox.SelectedIndex;

            switch (selectedType)
            {
                case 0: // True/False
                    SetupTrueFalseControls();
                    break;
                case 1: // Open Ended
                    SetupOpenEndedControls();
                    break;
                case 2: // Multiple Choice
                    SetupMultipleChoiceControls();
                    break;
            }
        }

        /// <summary>
        /// Setup True/False answer controls
        /// </summary>
        private void SetupTrueFalseControls()
        {
            var answerLabel = new Label
            {
                Text = "Correct Answer:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                Size = new Size(120, 25),
                Location = new Point(10, 20)
            };

            var trueRadio = new RadioButton
            {
                Text = "True",
                Font = new Font("Arial", 11),
                Size = new Size(80, 25),
                Location = new Point(140, 20),
                Checked = true
            };

            var falseRadio = new RadioButton
            {
                Text = "False",
                Font = new Font("Arial", 11),
                Size = new Size(80, 25),
                Location = new Point(240, 20)
            };

            answerControls = new Control[] { answerLabel, trueRadio, falseRadio };
            answerPanel.Controls.AddRange(answerControls);
        }

        /// <summary>
        /// Setup Open-Ended answer controls
        /// </summary>
        private void SetupOpenEndedControls()
        {
            var controls = new List<Control>();

            // Main answer
            var mainLabel = new Label
            {
                Text = "Correct Answer:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                Size = new Size(120, 25),
                Location = new Point(10, 20)
            };
            controls.Add(mainLabel);

            var mainTextBox = new TextBox
            {
                Size = new Size(400, 25),
                Location = new Point(140, 20),
                Font = new Font("Arial", 11)
            };
            controls.Add(mainTextBox);

            // Alternative answers
            for (int i = 0; i < 3; i++)
            {
                var altLabel = new Label
                {
                    Text = $"Alternative {i + 1}:",
                    Font = new Font("Arial", 10),
                    Size = new Size(120, 25),
                    Location = new Point(10, 60 + (i * 35))
                };
                controls.Add(altLabel);

                var altTextBox = new TextBox
                {
                    Size = new Size(400, 25),
                    Location = new Point(140, 60 + (i * 35)),
                    Font = new Font("Arial", 11)
                };
                controls.Add(altTextBox);
            }

            answerControls = controls.ToArray();
            answerPanel.Controls.AddRange(answerControls);
        }

      
        private void SetupMultipleChoiceControls()
        {
            var controls = new List<Control>();

            for (int i = 0; i < 4; i++)
            {
                char optionLetter = (char)('A' + i);

                var radio = new RadioButton
                {
                    Text = optionLetter.ToString(),
                    Font = new Font("Arial", 11, FontStyle.Bold),
                    Size = new Size(30, 25),
                    Location = new Point(10, 20 + (i * 40)),
                    Checked = i == 0
                };
                controls.Add(radio);

                var textBox = new TextBox
                {
                    Size = new Size(500, 25),
                    Location = new Point(50, 20 + (i * 40)),
                    Font = new Font("Arial", 11)
                };
                controls.Add(textBox);
            }

            answerControls = controls.ToArray();
            answerPanel.Controls.AddRange(answerControls);
        }

       
        private void QuestionTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetupAnswerControls();
        }

        
        private void ContinentComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadContinentDisplay();
        }

        
        private void LoadContinentDisplay()
        {
            try
            {
                var allContinents = ContinentHelper.GetAllContinents();
                if (continentComboBox.SelectedIndex >= 0 && continentComboBox.SelectedIndex < allContinents.Count)
                {
                    var selectedContinent = allContinents[continentComboBox.SelectedIndex];
                    continentDisplayLabel.Text = ContinentHelper.GetDisplayName(selectedContinent);
                }
                else
                {
                    continentDisplayLabel.Text = "Unknown";
                }
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Error loading continent display: {ex.Message}");
                continentDisplayLabel.Text = "Unknown";
            }
        }

        
        private void AddQuestionButton_Click(object sender, EventArgs e)
        {
            try
            {
                var question = CreateQuestionFromInputs();
                if (question != null)
                {
                    _pendingQuestions.Add(question);
                    UpdatePendingQuestionsList();
                    ClearQuestionForm();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding question: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

       
        private Question CreateQuestionFromInputs()
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(questionTextBox.Text))
            {
                MessageBox.Show("Please enter a question text.", "Validation Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                questionTextBox.Focus();
                return null;
            }

            if (questionTextBox.Text.Length < 10)
            {
                MessageBox.Show("Question text must be at least 10 characters long.", "Validation Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                questionTextBox.Focus();
                return null;
            }

            string questionText = questionTextBox.Text.Trim();
            var allContinents = ContinentHelper.GetAllContinents();
            Continent selectedContinent = allContinents[continentComboBox.SelectedIndex];
            int selectedType = questionTypeComboBox.SelectedIndex;

            switch (selectedType)
            {
                case 0: // True/False
                    return CreateTrueFalseQuestion(questionText, selectedContinent);
                case 1: // Open Ended
                    return CreateOpenEndedQuestion(questionText, selectedContinent);
                case 2: // Multiple Choice
                    return CreateMultipleChoiceQuestion(questionText, selectedContinent);
                default:
                    return null;
            }
        }

      
        private TrueFalseQuestion CreateTrueFalseQuestion(string questionText, Continent continent)
        {
            var trueRadio = answerControls.OfType<RadioButton>().FirstOrDefault(r => r.Text == "True");
            bool correctAnswer = trueRadio?.Checked ?? false;
            return new TrueFalseQuestion(questionText, correctAnswer, continent);
        }

        
        private OpenEndedQuestion CreateOpenEndedQuestion(string questionText, Continent continent)
        {
            var textBoxes = answerControls.OfType<TextBox>().ToArray();
            if (textBoxes.Length == 0)
            {
                MessageBox.Show("Please enter at least one correct answer.", "Validation Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            string correctAnswer = textBoxes[0].Text.Trim();
            if (string.IsNullOrWhiteSpace(correctAnswer))
            {
                MessageBox.Show("Please enter a correct answer.", "Validation Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxes[0].Focus();
                return null;
            }

            var alternativeAnswers = textBoxes.Skip(1)
                                              .Select(tb => tb.Text.Trim())
                                              .Where(text => !string.IsNullOrWhiteSpace(text))
                                              .ToArray();

            return new OpenEndedQuestion(questionText, correctAnswer, continent, alternativeAnswers);
        }

       
        private MultipleChoiceQuestion CreateMultipleChoiceQuestion(string questionText, Continent continent)
        {
            var textBoxes = answerControls.OfType<TextBox>().ToArray();
            var radioButtons = answerControls.OfType<RadioButton>().ToArray();

            if (textBoxes.Length < 4)
            {
                MessageBox.Show("Please provide all 4 options.", "Validation Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            
            for (int i = 0; i < 4; i++)
            {
                if (string.IsNullOrWhiteSpace(textBoxes[i].Text))
                {
                    MessageBox.Show($"Please fill in option {(char)('A' + i)}.", "Validation Error",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBoxes[i].Focus();
                    return null;
                }
            }

            string[] options = textBoxes.Take(4).Select(tb => tb.Text.Trim()).ToArray();
            int correctOptionIndex = -1;

            for (int i = 0; i < radioButtons.Length && i < 4; i++)
            {
                if (radioButtons[i].Checked)
                {
                    correctOptionIndex = i;
                    break;
                }
            }

            if (correctOptionIndex == -1)
            {
                MessageBox.Show("Please select the correct option.", "Validation Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            return new MultipleChoiceQuestion(questionText, options, correctOptionIndex, continent);
        }

        
        private void UpdatePendingQuestionsList()
        {
            pendingQuestionsListBox.Items.Clear();

            foreach (var question in _pendingQuestions)
            {
                string displayText = $"[{question.Type}] [{question.Continent}] {question.QuestionText}";
                if (displayText.Length > 80)
                    displayText = displayText.Substring(0, 77) + "...";

                pendingQuestionsListBox.Items.Add(displayText);
            }

            // Update button states
            saveAllButton.Enabled = _pendingQuestions.Count > 0;
            clearAllButton.Enabled = _pendingQuestions.Count > 0;
        }

        
        private void ClearQuestionForm()
        {
            questionTextBox.Clear();
            questionTypeComboBox.SelectedIndex = 0;
            continentComboBox.SelectedIndex = 0;
            SetupAnswerControls();
            questionTextBox.Focus();
        }



      
        private void SaveAllButton_Click(object sender, EventArgs e)
        {
            if (_pendingQuestions.Count == 0)
            {
                MessageBox.Show("No questions to save.", "Information",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                int savedCount = 0;
                var failedQuestions = new List<string>();

                foreach (var question in _pendingQuestions)
                {
                    bool success = _databaseManager.SaveQuestion(question);
                    if (success)
                    {
                        savedCount++;
                    }
                    else
                    {
                        failedQuestions.Add($"[{question.Type}] {question.QuestionText}");
                    }
                }

                if (failedQuestions.Count > 0)
                {
                    string failedList = string.Join("\n", failedQuestions);
                    MessageBox.Show($"Failed to save {failedQuestions.Count} questions:\n\n{failedList}",
                                  "Partial Save", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                _pendingQuestions.Clear();
                UpdatePendingQuestionsList();



                // Show success message
                MessageBox.Show($"Successfully saved {savedCount} questions to the database!",
                              "Questions Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving questions: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

     
        private void ClearAllButton_Click(object sender, EventArgs e)
        {
            if (_pendingQuestions.Count == 0)
                return;

            var result = MessageBox.Show($"Are you sure you want to clear all {_pendingQuestions.Count} pending questions?",
                                       "Confirm Clear", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _pendingQuestions.Clear();
                UpdatePendingQuestionsList();

            }
        }

        /// <summary>
        /// Open Manage Questions form
        /// </summary>
        private void ManageQuestionsButton_Click(object sender, EventArgs e)
        {
            try
            {
                var manageForm = new ManageQuestionsForm(_databaseManager);
                manageForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening manage questions form: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Back to main menu
        /// </summary>
        private void BackButton_Click(object sender, EventArgs e)
        {
            // Check if there are unsaved questions
            if (_pendingQuestions.Count > 0)
            {
                var result = MessageBox.Show($"You have {_pendingQuestions.Count} unsaved questions. Do you want to save them before leaving?",
                                           "Unsaved Questions", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    SaveAllButton_Click(sender, e);
                }
                else if (result == DialogResult.Cancel)
                {
                    return; // Don't close the form
                }
            }

            this.Close();
        }

        /// <summary>
        /// Handle form closing
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // No need to dispose anything for text-based continent display
            base.OnFormClosing(e);
        }
    }
}
