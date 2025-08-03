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
    /// Form for managing existing questions - Edit and Delete functionality
    /// Demonstrates OOP principles: Encapsulation, Inheritance, Polymorphism, Abstraction
    /// </summary>
    public partial class ManageQuestionsForm : Form
    {
        private readonly DatabaseManager _databaseManager;
        private List<Question> _questions;
        private Question? _currentEditingQuestion;

        // UI Controls
        private Panel headerPanel;
        private Panel questionsPanel;
        private Panel editPanel;
        private Panel buttonPanel;
        private Label titleLabel;
        private Label questionsLabel;
        private ListBox questionsListBox;
        private Button editButton;
        private Button deleteButton;
        private Button refreshButton;
        private Button backButton;
        private Label statusLabel;

        // Edit form controls
        private Label editTitleLabel;
        private ComboBox questionTypeComboBox;
        private TextBox questionTextBox;
        private ComboBox continentComboBox;
        private Panel answerPanel;
        private Button updateButton;
        private Button cancelEditButton;
        private Control[] answerControls;

        public ManageQuestionsForm(DatabaseManager databaseManager)
        {
            _databaseManager = databaseManager ?? throw new ArgumentNullException(nameof(databaseManager));
            _questions = new List<Question>();
            _currentEditingQuestion = null;
            answerControls = Array.Empty<Control>();
            
            InitializeComponent();
            SetupUI();
            LoadQuestions();
        }

        private void InitializeComponent()
        {
            this.Text = "Q-Quizzles - Manage Questions";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(240, 255, 240); // Light mint green background

            // Initialize controls
            headerPanel = new Panel();
            questionsPanel = new Panel();
            editPanel = new Panel();
            buttonPanel = new Panel();
            titleLabel = new Label();
            questionsLabel = new Label();
            questionsListBox = new ListBox();
            editButton = new Button();
            deleteButton = new Button();
            refreshButton = new Button();
            backButton = new Button();
            statusLabel = new Label();
            editTitleLabel = new Label();
            questionTypeComboBox = new ComboBox();
            questionTextBox = new TextBox();
            continentComboBox = new ComboBox();
            answerPanel = new Panel();
            updateButton = new Button();
            cancelEditButton = new Button();

            this.Controls.AddRange(new Control[] { headerPanel, questionsPanel, editPanel, buttonPanel });
        }

        private void SetupUI()
        {
            // Header panel
            headerPanel.Size = new Size(1160, 80);
            headerPanel.Location = new Point(20, 20);
            GeographyTheme.ApplyPanelStyle(headerPanel, false, true);

            titleLabel.Text = "🗂️ Manage Geography Questions";
            titleLabel.Font = new Font("Arial", 18, FontStyle.Bold);
            titleLabel.Size = new Size(500, 40);
            titleLabel.Location = new Point(20, 20);
            titleLabel.ForeColor = GeographyTheme.PrimaryOceanBlue;

            statusLabel.Text = "Ready";
            statusLabel.Font = new Font("Arial", 10);
            statusLabel.Size = new Size(600, 25);
            statusLabel.Location = new Point(20, 50);
            statusLabel.ForeColor = GeographyTheme.LightText;

            headerPanel.Controls.AddRange(new Control[] { titleLabel, statusLabel });

            // Questions panel
            questionsPanel.Size = new Size(560, 600);
            questionsPanel.Location = new Point(20, 120);
            GeographyTheme.ApplyPanelStyle(questionsPanel, false, true);

            SetupQuestionsPanel();

            // Edit panel
            editPanel.Size = new Size(560, 600);
            editPanel.Location = new Point(600, 120);
            GeographyTheme.ApplyPanelStyle(editPanel, false, true);
            editPanel.Visible = false;

            SetupEditPanel();

            // Button panel
            buttonPanel.Size = new Size(1160, 60);
            buttonPanel.Location = new Point(20, 740);
            GeographyTheme.ApplyPanelStyle(buttonPanel, false, true);

            SetupButtonPanel();
        }

        private void SetupQuestionsPanel()
        {
            questionsLabel.Text = "📚 Existing Questions";
            questionsLabel.Font = new Font("Arial", 14, FontStyle.Bold);
            questionsLabel.Size = new Size(300, 30);
            questionsLabel.Location = new Point(20, 20);
            questionsLabel.ForeColor = GeographyTheme.PrimaryOceanBlue;

            questionsListBox.Size = new Size(520, 450);
            questionsListBox.Location = new Point(20, 60);
            questionsListBox.Font = new Font("Arial", 10);
            questionsListBox.SelectionMode = SelectionMode.One;
            questionsListBox.SelectedIndexChanged += QuestionsListBox_SelectedIndexChanged;

            editButton.Text = "✏️ Edit Selected";
            editButton.Size = new Size(120, 40);
            editButton.Location = new Point(20, 530);
            editButton.BackColor = Color.FromArgb(255, 193, 7);
            editButton.ForeColor = Color.Black;
            editButton.Font = new Font("Arial", 10, FontStyle.Bold);
            editButton.FlatStyle = FlatStyle.Flat;
            editButton.Enabled = false;
            editButton.Click += EditButton_Click;

            deleteButton.Text = "🗑️ Delete Selected";
            deleteButton.Size = new Size(120, 40);
            deleteButton.Location = new Point(160, 530);
            deleteButton.BackColor = Color.FromArgb(220, 53, 69);
            deleteButton.ForeColor = Color.White;
            deleteButton.Font = new Font("Arial", 10, FontStyle.Bold);
            deleteButton.FlatStyle = FlatStyle.Flat;
            deleteButton.Enabled = false;
            deleteButton.Click += DeleteButton_Click;

            refreshButton.Text = "🔄 Refresh";
            refreshButton.Size = new Size(120, 40);
            refreshButton.Location = new Point(300, 530);
            refreshButton.BackColor = GeographyTheme.ButtonAccent;
            refreshButton.ForeColor = Color.White;
            refreshButton.Font = new Font("Arial", 10, FontStyle.Bold);
            refreshButton.FlatStyle = FlatStyle.Flat;
            refreshButton.Click += RefreshButton_Click;

            questionsPanel.Controls.AddRange(new Control[]
            {
                questionsLabel, questionsListBox, editButton, deleteButton, refreshButton
            });
        }

        private void SetupEditPanel()
        {
            editTitleLabel.Text = "✏️ Edit Question";
            editTitleLabel.Font = new Font("Arial", 14, FontStyle.Bold);
            editTitleLabel.Size = new Size(300, 30);
            editTitleLabel.Location = new Point(20, 20);
            editTitleLabel.ForeColor = GeographyTheme.PrimaryOceanBlue;

            // Question Type
            var typeLabel = new Label
            {
                Text = "Question Type:",
                Font = new Font("Arial", 10, FontStyle.Bold),
                Size = new Size(120, 25),
                Location = new Point(20, 70)
            };

            questionTypeComboBox.Size = new Size(200, 30);
            questionTypeComboBox.Location = new Point(150, 70);
            questionTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            questionTypeComboBox.Items.AddRange(new[] { "True/False", "Open Ended", "Multiple Choice" });
            questionTypeComboBox.SelectedIndexChanged += QuestionTypeComboBox_SelectedIndexChanged;

            // Question Text
            var questionLabel = new Label
            {
                Text = "Question Text:",
                Font = new Font("Arial", 10, FontStyle.Bold),
                Size = new Size(120, 25),
                Location = new Point(20, 120)
            };

            questionTextBox.Size = new Size(500, 60);
            questionTextBox.Location = new Point(20, 150);
            questionTextBox.Multiline = true;
            questionTextBox.Font = new Font("Arial", 10);

            // Continent
            var continentLabel = new Label
            {
                Text = "Continent:",
                Font = new Font("Arial", 10, FontStyle.Bold),
                Size = new Size(120, 25),
                Location = new Point(20, 230)
            };

            continentComboBox.Size = new Size(200, 30);
            continentComboBox.Location = new Point(150, 230);
            continentComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            continentComboBox.Items.AddRange(ContinentHelper.GetAllContinentNames());

            // Answer Panel
            answerPanel.Size = new Size(520, 200);
            answerPanel.Location = new Point(20, 280);
            answerPanel.BorderStyle = BorderStyle.FixedSingle;

            // Buttons
            updateButton.Text = "💾 Update Question";
            updateButton.Size = new Size(140, 40);
            updateButton.Location = new Point(20, 500);
            updateButton.BackColor = Color.FromArgb(40, 167, 69);
            updateButton.ForeColor = Color.White;
            updateButton.Font = new Font("Arial", 10, FontStyle.Bold);
            updateButton.FlatStyle = FlatStyle.Flat;
            updateButton.Click += UpdateButton_Click;

            cancelEditButton.Text = "❌ Cancel Edit";
            cancelEditButton.Size = new Size(140, 40);
            cancelEditButton.Location = new Point(180, 500);
            cancelEditButton.BackColor = Color.FromArgb(108, 117, 125);
            cancelEditButton.ForeColor = Color.White;
            cancelEditButton.Font = new Font("Arial", 10, FontStyle.Bold);
            cancelEditButton.FlatStyle = FlatStyle.Flat;
            cancelEditButton.Click += CancelEditButton_Click;

            editPanel.Controls.AddRange(new Control[]
            {
                editTitleLabel, typeLabel, questionTypeComboBox,
                questionLabel, questionTextBox, continentLabel, continentComboBox,
                answerPanel, updateButton, cancelEditButton
            });
        }

        private void SetupButtonPanel()
        {
            backButton.Text = "🔙 Back to Main Menu";
            backButton.Size = new Size(150, 40);
            backButton.Location = new Point(430, 530); // Next to refresh button
            backButton.BackColor = GeographyTheme.ButtonSecondary;
            backButton.ForeColor = Color.White;
            backButton.Font = new Font("Arial", 10, FontStyle.Bold);
            backButton.FlatStyle = FlatStyle.Flat;
            backButton.Click += BackButton_Click;

            buttonPanel.Controls.Add(backButton);
        }

        /// <summary>
        /// Load all questions from database - demonstrates data encapsulation
        /// </summary>
        private void LoadQuestions()
        {
            try
            {
                _questions = _databaseManager.GetAllQuestions();
                UpdateQuestionsList();
                statusLabel.Text = $"Loaded {_questions.Count} questions";
                statusLabel.ForeColor = GeographyTheme.ButtonPrimary;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading questions: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = "Error loading questions";
                statusLabel.ForeColor = Color.Red;
            }
        }

        /// <summary>
        /// Update questions listbox display - demonstrates UI data binding
        /// </summary>
        private void UpdateQuestionsList()
        {
            questionsListBox.Items.Clear();

            foreach (var question in _questions)
            {
                string displayText = $"[{question.Type}] [{question.Continent}] {question.QuestionText}";
                if (displayText.Length > 80)
                    displayText = displayText.Substring(0, 77) + "...";

                questionsListBox.Items.Add(displayText);
            }

            // Update button states
            bool hasSelection = questionsListBox.SelectedIndex >= 0;
            editButton.Enabled = hasSelection && _currentEditingQuestion == null;
            deleteButton.Enabled = hasSelection && _currentEditingQuestion == null;
        }

        /// <summary>
        /// Handle questions selection change - demonstrates event handling
        /// </summary>
        private void QuestionsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool hasSelection = questionsListBox.SelectedIndex >= 0;
            editButton.Enabled = hasSelection && _currentEditingQuestion == null;
            deleteButton.Enabled = hasSelection && _currentEditingQuestion == null;
        }

        /// <summary>
        /// Edit selected question - demonstrates polymorphic object handling
        /// </summary>
        private void EditButton_Click(object sender, EventArgs e)
        {
            try
            {
                int selectedIndex = questionsListBox.SelectedIndex;
                if (selectedIndex < 0 || selectedIndex >= _questions.Count)
                {
                    MessageBox.Show("Please select a question to edit.", "No Selection",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                _currentEditingQuestion = _questions[selectedIndex];
                LoadQuestionForEditing(_currentEditingQuestion);
                SetEditMode(true);
                statusLabel.Text = "Editing question - make changes and click Update";
                statusLabel.ForeColor = Color.Blue;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading question for editing: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Delete selected question - demonstrates database operations with confirmation
        /// </summary>
        private void DeleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                int selectedIndex = questionsListBox.SelectedIndex;
                if (selectedIndex < 0 || selectedIndex >= _questions.Count)
                {
                    MessageBox.Show("Please select a question to delete.", "No Selection",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var questionToDelete = _questions[selectedIndex];

                // Confirmation dialog
                var result = MessageBox.Show(
                    $"Are you sure you want to delete this question?\n\n{questionToDelete.QuestionText}",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    bool success = _databaseManager.DeleteQuestion(questionToDelete.Id);

                    if (success)
                    {
                        _questions.RemoveAt(selectedIndex);
                        UpdateQuestionsList();
                        statusLabel.Text = "Question deleted successfully";
                        statusLabel.ForeColor = GeographyTheme.ButtonPrimary;
                        MessageBox.Show("Question deleted successfully!", "Success",
                                      MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete question from database.", "Error",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting question: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

     
        private void RefreshButton_Click(object sender, EventArgs e)
        {
            LoadQuestions();
        }

        
        private void LoadQuestionForEditing(Question question)
        {
           
            questionTextBox.Text = question.QuestionText;
            questionTypeComboBox.SelectedIndex = (int)question.Type - 1;

            
            var allContinents = ContinentHelper.GetAllContinents();
            var continentIndex = allContinents.IndexOf(question.Continent);
            continentComboBox.SelectedIndex = continentIndex >= 0 ? continentIndex : 0;

            
            SetupAnswerControls();

           
            switch (question.Type)
            {
                case QuestionType.TrueFalse:
                    LoadTrueFalseQuestion((TrueFalseQuestion)question);
                    break;
                case QuestionType.OpenEnded:
                    LoadOpenEndedQuestion((OpenEndedQuestion)question);
                    break;
                case QuestionType.MultipleChoice:
                    LoadMultipleChoiceQuestion((MultipleChoiceQuestion)question);
                    break;
            }
        }

        
        private void SetupAnswerControls()
        {
            answerPanel.Controls.Clear();

            var selectedType = (QuestionType)(questionTypeComboBox.SelectedIndex + 1);

            switch (selectedType)
            {
                case QuestionType.TrueFalse:
                    SetupTrueFalseControls();
                    break;
                case QuestionType.OpenEnded:
                    SetupOpenEndedControls();
                    break;
                case QuestionType.MultipleChoice:
                    SetupMultipleChoiceControls();
                    break;
            }
        }

        
        private void SetupTrueFalseControls()
        {
            var trueRadio = new RadioButton
            {
                Text = "True",
                Size = new Size(100, 30),
                Location = new Point(20, 20),
                Font = new Font("Arial", 10)
            };

            var falseRadio = new RadioButton
            {
                Text = "False",
                Size = new Size(100, 30),
                Location = new Point(150, 20),
                Font = new Font("Arial", 10)
            };

            answerControls = new Control[] { trueRadio, falseRadio };
            answerPanel.Controls.AddRange(answerControls);
        }

       
        private void SetupOpenEndedControls()
        {
            var controls = new List<Control>();

            // Main answer
            var mainLabel = new Label
            {
                Text = "Correct Answer:",
                Size = new Size(120, 25),
                Location = new Point(20, 20),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            controls.Add(mainLabel);

            var mainTextBox = new TextBox
            {
                Size = new Size(350, 25),
                Location = new Point(150, 20),
                Font = new Font("Arial", 10)
            };
            controls.Add(mainTextBox);

            // Alternative answers
            for (int i = 0; i < 3; i++)
            {
                var altLabel = new Label
                {
                    Text = $"Alternative {i + 1}:",
                    Size = new Size(120, 25),
                    Location = new Point(20, 60 + (i * 35)),
                    Font = new Font("Arial", 10)
                };
                controls.Add(altLabel);

                var altTextBox = new TextBox
                {
                    Size = new Size(350, 25),
                    Location = new Point(150, 60 + (i * 35)),
                    Font = new Font("Arial", 10)
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
                    Size = new Size(30, 25),
                    Location = new Point(20, 20 + (i * 35)),
                    Font = new Font("Arial", 10, FontStyle.Bold)
                };
                controls.Add(radio);

                var textBox = new TextBox
                {
                    Size = new Size(400, 25),
                    Location = new Point(60, 20 + (i * 35)),
                    Font = new Font("Arial", 10)
                };
                controls.Add(textBox);
            }

            answerControls = controls.ToArray();
            answerPanel.Controls.AddRange(answerControls);
        }

        
        private void LoadTrueFalseQuestion(TrueFalseQuestion question)
        {
            var trueRadio = answerControls.OfType<RadioButton>().FirstOrDefault(r => r.Text == "True");
            var falseRadio = answerControls.OfType<RadioButton>().FirstOrDefault(r => r.Text == "False");

            if (question.CorrectAnswerBool)
                trueRadio?.PerformClick();
            else
                falseRadio?.PerformClick();
        }

        
        private void LoadOpenEndedQuestion(OpenEndedQuestion question)
        {
            var textBoxes = answerControls.OfType<TextBox>().ToArray();
            if (textBoxes.Length >= 1)
                textBoxes[0].Text = question.CorrectAnswer;

            // Load alternative answers
            for (int i = 0; i < question.AlternativeAnswers.Length && i < textBoxes.Length - 1; i++)
            {
                textBoxes[i + 1].Text = question.AlternativeAnswers[i];
            }
        }

      
        private void LoadMultipleChoiceQuestion(MultipleChoiceQuestion question)
        {
            var radioButtons = answerControls.OfType<RadioButton>().ToArray();
            var textBoxes = answerControls.OfType<TextBox>().ToArray();

            // Load options
            for (int i = 0; i < question.Options.Length && i < textBoxes.Length; i++)
            {
                textBoxes[i].Text = question.Options[i];
            }

            // Select correct option
            if (question.CorrectOptionIndex < radioButtons.Length)
            {
                radioButtons[question.CorrectOptionIndex].Checked = true;
            }
        }

        
        private void QuestionTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetupAnswerControls();
        }

        
        private void UpdateButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (_currentEditingQuestion == null)
                {
                    MessageBox.Show("No question is currently being edited.", "Error",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Validate form data
                if (!ValidateForm())
                    return;

                // Create updated question using polymorphism
                Question updatedQuestion = CreateQuestionFromForm();
                if (updatedQuestion == null)
                {
                    MessageBox.Show("Failed to create updated question. Please check your inputs.", "Error",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Preserve the original ID and creation date
                updatedQuestion.Id = _currentEditingQuestion.Id;
                updatedQuestion.CreatedDate = _currentEditingQuestion.CreatedDate;

                // Update in database
                bool success = _databaseManager.UpdateQuestion(updatedQuestion);

                if (success)
                {
                    // Update in local list
                    int index = _questions.FindIndex(q => q.Id == _currentEditingQuestion.Id);
                    if (index >= 0)
                    {
                        _questions[index] = updatedQuestion;
                    }

                    UpdateQuestionsList();
                    SetEditMode(false);
                    statusLabel.Text = "Question updated successfully";
                    statusLabel.ForeColor = GeographyTheme.ButtonPrimary;
                    MessageBox.Show("Question updated successfully!", "Success",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to update question in database.", "Error",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating question: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        
        private void CancelEditButton_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to cancel editing? All changes will be lost.",
                                       "Confirm Cancel", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                SetEditMode(false);
                statusLabel.Text = "Edit cancelled";
                statusLabel.ForeColor = GeographyTheme.LightText;
            }
        }

       
        private void SetEditMode(bool isEditing)
        {
            editPanel.Visible = isEditing;
            editButton.Enabled = !isEditing;
            deleteButton.Enabled = !isEditing;
            questionsListBox.Enabled = !isEditing;

            if (!isEditing)
            {
                _currentEditingQuestion = null;
                questionsListBox.ClearSelected();
                questionTextBox.Clear();
                answerPanel.Controls.Clear();
            }
        }

        
        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(questionTextBox.Text))
            {
                MessageBox.Show("Please enter a question text.", "Validation Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                questionTextBox.Focus();
                return false;
            }

            if (questionTextBox.Text.Length < 10)
            {
                MessageBox.Show("Question text must be at least 10 characters long.", "Validation Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                questionTextBox.Focus();
                return false;
            }

            var selectedType = (QuestionType)(questionTypeComboBox.SelectedIndex + 1);

            switch (selectedType)
            {
                case QuestionType.TrueFalse:
                    return ValidateTrueFalseForm();
                case QuestionType.OpenEnded:
                    return ValidateOpenEndedForm();
                case QuestionType.MultipleChoice:
                    return ValidateMultipleChoiceForm();
                default:
                    return false;
            }
        }

        
        private bool ValidateTrueFalseForm()
        {
            var radioButtons = answerControls.OfType<RadioButton>();
            if (!radioButtons.Any(r => r.Checked))
            {
                MessageBox.Show("Please select True or False.", "Validation Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        
        private bool ValidateOpenEndedForm()
        {
            var textBoxes = answerControls.OfType<TextBox>().ToArray();
            if (textBoxes.Length == 0 || string.IsNullOrWhiteSpace(textBoxes[0].Text))
            {
                MessageBox.Show("Please enter at least one correct answer.", "Validation Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxes[0]?.Focus();
                return false;
            }
            return true;
        }

        
        private bool ValidateMultipleChoiceForm()
        {
            var textBoxes = answerControls.OfType<TextBox>().ToArray();
            var radioButtons = answerControls.OfType<RadioButton>();

            // Check all options are filled
            for (int i = 0; i < 4; i++)
            {
                if (i >= textBoxes.Length || string.IsNullOrWhiteSpace(textBoxes[i].Text))
                {
                    MessageBox.Show($"Please fill in option {(char)('A' + i)}.", "Validation Error",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBoxes[i]?.Focus();
                    return false;
                }
            }

            // Check correct option is selected
            if (!radioButtons.Any(r => r.Checked))
            {
                MessageBox.Show("Please select the correct option.", "Validation Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        
        private Question CreateQuestionFromForm()
        {
            string questionText = questionTextBox.Text.Trim();
            var allContinents = ContinentHelper.GetAllContinents();
            Continent selectedContinent = allContinents[continentComboBox.SelectedIndex];
            QuestionType selectedType = (QuestionType)(questionTypeComboBox.SelectedIndex + 1);

            return selectedType switch
            {
                QuestionType.TrueFalse => CreateTrueFalseQuestionFromForm(questionText, selectedContinent),
                QuestionType.OpenEnded => CreateOpenEndedQuestionFromForm(questionText, selectedContinent),
                QuestionType.MultipleChoice => CreateMultipleChoiceQuestionFromForm(questionText, selectedContinent),
                _ => null
            };
        }

        
        private TrueFalseQuestion CreateTrueFalseQuestionFromForm(string questionText, Continent continent)
        {
            var trueRadio = answerControls.OfType<RadioButton>().FirstOrDefault(r => r.Text == "True");
            bool correctAnswer = trueRadio?.Checked ?? false;
            return new TrueFalseQuestion(questionText, correctAnswer, continent);
        }

        
        private OpenEndedQuestion CreateOpenEndedQuestionFromForm(string questionText, Continent continent)
        {
            var textBoxes = answerControls.OfType<TextBox>().ToArray();
            if (textBoxes.Length == 0)
                return null;

            string correctAnswer = textBoxes[0].Text.Trim();
            var alternativeAnswers = textBoxes.Skip(1)
                                              .Select(tb => tb.Text.Trim())
                                              .Where(text => !string.IsNullOrWhiteSpace(text))
                                              .ToArray();

            return new OpenEndedQuestion(questionText, correctAnswer, continent, alternativeAnswers);
        }

        
        private MultipleChoiceQuestion CreateMultipleChoiceQuestionFromForm(string questionText, Continent continent)
        {
            var textBoxes = answerControls.OfType<TextBox>().ToArray();
            var radioButtons = answerControls.OfType<RadioButton>().ToArray();

            if (textBoxes.Length < 4)
                return null;

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
                return null;

            return new MultipleChoiceQuestion(questionText, options, correctOptionIndex, continent);
        }

        
        private void BackButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
