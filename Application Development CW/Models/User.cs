using System;

namespace QQuizzles.Models
{
    
    /// User model class 
    
    public class User
    {
        private int _id;
        private string _username;
        private string _password;
        private DateTime _createdDate;
        private DateTime _lastLoginDate;
        
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Username
        {
            get { return _username; }
            set 
            { 
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Username cannot be empty");
                if (value.Length < 3)
                    throw new ArgumentException("Username must be at least 3 characters long");
                _username = value.Trim(); 
            }
        }

        public string Password
        {
            get { return _password; }
            set 
            { 
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Password cannot be empty");
                if (value.Length < 4)
                    throw new ArgumentException("Password must be at least 4 characters long");
                _password = value; 
            }
        }

        public DateTime CreatedDate
        {
            get { return _createdDate; }
            set { _createdDate = value; }
        }

        public DateTime LastLoginDate
        {
            get { return _lastLoginDate; }
            set { _lastLoginDate = value; }
        }

        // Constructors
        public User()
        {
            CreatedDate = DateTime.Now;
            LastLoginDate = DateTime.Now;
        }

        public User(string username, string password) : this()
        {
            Username = username;
            Password = password;
        }

        // Method to validate password
        public bool ValidatePassword(string inputPassword)
        {
            return !string.IsNullOrEmpty(Password) && Password == inputPassword;
        }

        // Method to update last login
        public void UpdateLastLogin()
        {
            LastLoginDate = DateTime.Now;
        }

        // Method to validate username format
        public static bool IsValidUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            username = username.Trim();
            
            // Check length
            if (username.Length < 3 || username.Length > 20)
                return false;

            // Check for valid characters (letters, numbers, underscore)
            foreach (char c in username)
            {
                if (!char.IsLetterOrDigit(c) && c != '_')
                    return false;
            }

            return true;
        }

        // Method to validate password format
        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            // Minimum length check
            if (password.Length < 4)
                return false;

            return true;
        }

        public override string ToString()
        {
            return $"User: {Username} (Created: {CreatedDate:yyyy-MM-dd})";
        }

        public override bool Equals(object? obj)
        {
            if (obj is User other)
            {
                return Username.Equals(other.Username, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Username?.ToLowerInvariant().GetHashCode() ?? 0;
        }
    }
}
