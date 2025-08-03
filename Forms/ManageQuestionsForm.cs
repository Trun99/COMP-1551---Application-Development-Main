using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Application_Development_CW.Database;
using Application_Development_CW.Models;
using Application_Development_CW.Utilities;

namespace Application_Development_CW.Forms
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
            this.BackColor = GeographyTheme.BackgroundColor;

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
            titleLabel.ForeColor = GeographyTheme.PrimaryColor;

            statusLabel.Text = "Ready";
            statusLabel.Font = new Font("Arial", 10);
            statusLabel.Size = new Size(600, 25);
            statusLabel.Location = new Point(20, 50);
            statusLabel.ForeColor = GeographyTheme.SecondaryColor;

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
            questionsLabel.ForeColor = GeographyTheme.PrimaryColor;

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
            refreshButton.BackColor = GeographyTheme.AccentColor;
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
            editTitleLabel.ForeColor = GeographyTheme.PrimaryColor;

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
            backButton.Location = new Point(20, 10);
            backButton.BackColor = GeographyTheme.SecondaryColor;
            backButton.ForeColor = Color.White;
            backButton.Font = new Font("Arial", 10, FontStyle.Bold);
            backButton.FlatStyle = FlatStyle.Flat;
            backButton.Click += BackButton_Click;

            buttonPanel.Controls.Add(backButton);
        }
