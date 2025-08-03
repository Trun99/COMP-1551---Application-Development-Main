using System;
using System.Text.RegularExpressions;

namespace QQuizzles.Utilities
{
    /// <summary>
    /// Helper class for validation operations
    /// </summary>
    public static class ValidationHelper
    {
        /// <summary>
        /// Validate username format
        /// </summary>
        public static bool IsValidUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            username = username.Trim();
            
            // Check length (3-20 characters)
            if (username.Length < 3 || username.Length > 20)
                return false;

            // Check for valid characters (letters, numbers, underscore)
            return Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$");
        }

        /// <summary>
        /// Validate password format
        /// </summary>
        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            // Minimum length check (4 characters)
            return password.Length >= 4;
        }

        /// <summary>
        /// Validate question text
        /// </summary>
        public static bool IsValidQuestionText(string questionText)
        {
            if (string.IsNullOrWhiteSpace(questionText))
                return false;

            questionText = questionText.Trim();
            
            // Check minimum length (10 characters)
            if (questionText.Length < 10)
                return false;

            // Check maximum length (500 characters)
            if (questionText.Length > 500)
                return false;

            return true;
        }

        /// <summary>
        /// Validate answer text
        /// </summary>
        public static bool IsValidAnswerText(string answerText)
        {
            if (string.IsNullOrWhiteSpace(answerText))
                return false;

            answerText = answerText.Trim();
            
            // Check minimum length (1 character)
            if (answerText.Length < 1)
                return false;

            // Check maximum length (200 characters)
            if (answerText.Length > 200)
                return false;

            return true;
        }

        /// <summary>
        /// Validate open-ended answer (1-4 words)
        /// </summary>
        public static bool IsValidOpenEndedAnswer(string answer)
        {
            if (string.IsNullOrWhiteSpace(answer))
                return false;

            string[] words = answer.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return words.Length >= 1 && words.Length <= 4;
        }

        /// <summary>
        /// Validate multiple choice options
        /// </summary>
        public static bool AreValidMultipleChoiceOptions(string[] options)
        {
            if (options == null || options.Length != 4)
                return false;

            foreach (string option in options)
            {
                if (!IsValidAnswerText(option))
                    return false;
            }

            // Check for duplicate options
            for (int i = 0; i < options.Length; i++)
            {
                for (int j = i + 1; j < options.Length; j++)
                {
                    if (string.Equals(options[i], options[j], StringComparison.OrdinalIgnoreCase))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Sanitize input text
        /// </summary>
        public static string SanitizeInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Trim whitespace
            input = input.Trim();
            
            // Remove multiple consecutive spaces
            input = Regex.Replace(input, @"\s+", " ");
            
            // Remove potentially harmful characters
            input = input.Replace("<", "&lt;").Replace(">", "&gt;");
            
            return input;
        }

        /// <summary>
        /// Get username validation error message
        /// </summary>
        public static string GetUsernameValidationError(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return "Username cannot be empty.";

            username = username.Trim();
            
            if (username.Length < 3)
                return "Username must be at least 3 characters long.";
            
            if (username.Length > 20)
                return "Username cannot be longer than 20 characters.";
            
            if (!Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$"))
                return "Username can only contain letters, numbers, and underscores.";

            return string.Empty;
        }

        /// <summary>
        /// Get password validation error message
        /// </summary>
        public static string GetPasswordValidationError(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return "Password cannot be empty.";
            
            if (password.Length < 4)
                return "Password must be at least 4 characters long.";

            return string.Empty;
        }

        /// <summary>
        /// Get question validation error message
        /// </summary>
        public static string GetQuestionValidationError(string questionText)
        {
            if (string.IsNullOrWhiteSpace(questionText))
                return "Question text cannot be empty.";

            questionText = questionText.Trim();
            
            if (questionText.Length < 10)
                return "Question must be at least 10 characters long.";
            
            if (questionText.Length > 500)
                return "Question cannot be longer than 500 characters.";

            return string.Empty;
        }

        /// <summary>
        /// Check if string contains only letters and spaces
        /// </summary>
        public static bool IsAlphabeticWithSpaces(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            return Regex.IsMatch(input.Trim(), @"^[a-zA-Z\s]+$");
        }

        /// <summary>
        /// Check if string is a valid number
        /// </summary>
        public static bool IsValidNumber(string input)
        {
            return double.TryParse(input, out _);
        }

        /// <summary>
        /// Normalize answer for comparison
        /// </summary>
        public static string NormalizeAnswer(string answer)
        {
            if (string.IsNullOrWhiteSpace(answer))
                return string.Empty;

            return answer.Trim()
                        .ToLowerInvariant()
                        .Replace(".", "")
                        .Replace(",", "")
                        .Replace("!", "")
                        .Replace("?", "")
                        .Replace("-", " ")
                        .Replace("_", " ");
        }
    }
}
