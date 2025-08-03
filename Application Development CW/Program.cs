using System;
using System.Windows.Forms;
using QQuizzles.Forms;

namespace QQuizzles
{
    /// <summary>
    /// Main program entry point for Q-Quizzles Geography Quiz Application
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Enable visual styles and set compatible text rendering
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                // Show login form
                using (var loginForm = new LoginForm())
                {
                    if (loginForm.ShowDialog() == DialogResult.OK)
                    {
                        // User authenticated successfully, show main menu
                        var authenticatedUser = loginForm.AuthenticatedUser;

                        if (authenticatedUser != null)
                        {
                            using (var mainMenuForm = new MainMenuForm(authenticatedUser))
                            {
                                Application.Run(mainMenuForm);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}\n\nThe application will now close.",
                              "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
