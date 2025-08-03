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
    /// Login form for user authentication - demonstrates Encapsulation and UI design
    /// </summary>
    public partial class LoginForm : Form
    {
        private readonly DatabaseManager _databaseManager;
        private User _authenticatedUser;

        // UI Controls
        private Panel mainPanel;
        private Label titleLabel;
        private Label subtitleLabel;
        private TextBox usernameTextBox;
        private TextBox passwordTextBox;
        private Button loginButton;

        private Label usernameLabel;
        private Label passwordLabel;
        private Label statusLabel;

        // Properties for encapsulation
        public User AuthenticatedUser
        {
            get { return _authenticatedUser; }
            private set { _authenticatedUser = value; }
        }

        public LoginForm()
        {
            _databaseManager = new DatabaseManager();
            InitializeComponent();
            SetupUI();
        }

        private void InitializeComponent()
        {
            // Form settings
            this.Text = "Q-Quizzles - Geography Quiz Login";
            this.Size = new Size(500, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Apply geography theme
            GeographyTheme.ApplyFormStyle(this);

            // Initialize controls
            mainPanel = new Panel();
            titleLabel = new Label();
            subtitleLabel = new Label();
            usernameLabel = new Label();
            usernameTextBox = new TextBox();
            passwordLabel = new Label();
            passwordTextBox = new TextBox();
            loginButton = new Button();

            statusLabel = new Label();

            this.Controls.Add(mainPanel);
        }

        private void SetupUI()
        {
            // Main panel with modern card style
            mainPanel.Size = new Size(450, 500);
            mainPanel.Location = new Point(25, 75);
            GeographyTheme.ApplyPanelStyle(mainPanel, false, true);



            // Title with geography theme
            titleLabel.Text = "🌍 Q-Quizzles";
            titleLabel.Size = new Size(400, 50);
            titleLabel.Location = new Point(25, 50);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            GeographyTheme.ApplyLabelStyle(titleLabel, LabelStyle.Title);

            // Subtitle with geography theme
            subtitleLabel.Text = "🗺️ Explore the World Through Geography";
            subtitleLabel.Size = new Size(400, 30);
            subtitleLabel.Location = new Point(25, 100);
            subtitleLabel.TextAlign = ContentAlignment.MiddleCenter;
            GeographyTheme.ApplyLabelStyle(subtitleLabel, LabelStyle.SubHeader);

            // Modern form layout with better spacing
            int formWidth = 350;
            int startX = (450 - formWidth) / 2;

            // Username section
            usernameLabel.Text = "Username";
            usernameLabel.Size = new Size(formWidth, 25);
            usernameLabel.Location = new Point(startX, 160);
            GeographyTheme.ApplyLabelStyle(usernameLabel, LabelStyle.Body);

            usernameTextBox.Size = new Size(formWidth, 40);
            usernameTextBox.Location = new Point(startX, 185);
            GeographyTheme.ApplyTextBoxStyle(usernameTextBox);

            // Password section
            passwordLabel.Text = "Password";
            passwordLabel.Size = new Size(formWidth, 25);
            passwordLabel.Location = new Point(startX, 245);
            GeographyTheme.ApplyLabelStyle(passwordLabel, LabelStyle.Body);

            passwordTextBox.Size = new Size(formWidth, 40);
            passwordTextBox.Location = new Point(startX, 270);
            passwordTextBox.UseSystemPasswordChar = true;
            GeographyTheme.ApplyTextBoxStyle(passwordTextBox);

            // Create completely new login button to fix text rendering
            loginButton = new Button();
            loginButton.Text = "Login";
            loginButton.Size = new Size(formWidth, 50);
            loginButton.Location = new Point(startX, 330);
            loginButton.Font = new Font("Arial", 12, FontStyle.Bold);
            loginButton.FlatStyle = FlatStyle.Flat;
            loginButton.FlatAppearance.BorderSize = 0;
            loginButton.BackColor = Color.FromArgb(33, 150, 243);
            loginButton.ForeColor = Color.White;
            loginButton.Cursor = Cursors.Hand;
            loginButton.Click += LoginButton_Click;

            // Add hover effects
            loginButton.MouseEnter += (s, e) => {
                loginButton.BackColor = Color.FromArgb(25, 118, 210);
            };
            loginButton.MouseLeave += (s, e) => {
                loginButton.BackColor = Color.FromArgb(33, 150, 243);
            };

            // Modern status label
            statusLabel.Text = "Default Account: admin / admin";
            statusLabel.Size = new Size(formWidth, 40);
            statusLabel.Location = new Point(startX, 400);
            statusLabel.TextAlign = ContentAlignment.MiddleCenter;
            GeographyTheme.ApplyLabelStyle(statusLabel, LabelStyle.Small);

            // Add controls to main panel
            mainPanel.Controls.AddRange(new Control[]
            {
                titleLabel, subtitleLabel,
                usernameLabel, usernameTextBox,
                passwordLabel, passwordTextBox,
                loginButton, statusLabel
            });

            // Set default values for testing
            usernameTextBox.Text = "admin";
            passwordTextBox.Text = "admin";

            // Set enter key handling
            usernameTextBox.KeyPress += TextBox_KeyPress;
            passwordTextBox.KeyPress += TextBox_KeyPress;
        }

        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                LoginButton_Click(sender, e);
            }
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            try
            {
                string username = ValidationHelper.SanitizeInput(usernameTextBox.Text);
                string password = passwordTextBox.Text;

                // Validate input
                if (string.IsNullOrWhiteSpace(username))
                {
                    ShowStatus("Please enter a username.", Color.Red);
                    usernameTextBox.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(password))
                {
                    ShowStatus("Please enter a password.", Color.Red);
                    passwordTextBox.Focus();
                    return;
                }

                // Disable login button during authentication
                loginButton.Enabled = false;
                ShowStatus("Authenticating...", Color.Blue);

                // Authenticate user
                AuthenticatedUser = _databaseManager.AuthenticateUser(username, password);

                if (AuthenticatedUser != null)
                {
                    ShowStatus("Login successful!", Color.Green);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    ShowStatus("Invalid username or password.", Color.Red);
                    passwordTextBox.Clear();
                    passwordTextBox.Focus();
                }
            }
            catch (Exception ex)
            {
                ShowStatus($"Login error: {ex.Message}", Color.Red);
            }
            finally
            {
                loginButton.Enabled = true;
            }
        }



        private void ShowStatus(string message, Color color)
        {
            statusLabel.Text = message;
            statusLabel.ForeColor = color;
        }
    }
}
