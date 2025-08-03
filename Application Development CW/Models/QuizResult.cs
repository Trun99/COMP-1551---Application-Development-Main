using System;

namespace QQuizzles.Models
{
    /// <summary>
    /// Quiz result model class - demonstrates Encapsulation
    /// </summary>
    public class QuizResult
    {
        
        private int _id;
        private int _userId;
        private string _username;
        private int _totalQuestions;
        private int _correctAnswers;
        private TimeSpan _timeTaken;
        private DateTime _completedDate;
        private Continent _selectedContinent;
        private string _continentName;

    
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public int UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        public string Username
        {
            get { return _username; }
            set 
            { 
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Username cannot be empty");
                _username = value; 
            }
        }

        public int TotalQuestions
        {
            get { return _totalQuestions; }
            set 
            { 
                if (value < 0)
                    throw new ArgumentException("Total questions cannot be negative");
                _totalQuestions = value; 
            }
        }

        public int CorrectAnswers
        {
            get { return _correctAnswers; }
            set 
            { 
                if (value < 0)
                    throw new ArgumentException("Correct answers cannot be negative");
                if (value > TotalQuestions)
                    throw new ArgumentException("Correct answers cannot exceed total questions");
                _correctAnswers = value; 
            }
        }

        public TimeSpan TimeTaken
        {
            get { return _timeTaken; }
            set 
            { 
                if (value < TimeSpan.Zero)
                    throw new ArgumentException("Time taken cannot be negative");
                _timeTaken = value; 
            }
        }

        public DateTime CompletedDate
        {
            get { return _completedDate; }
            set { _completedDate = value; }
        }

        public Continent SelectedContinent
        {
            get { return _selectedContinent; }
            set { _selectedContinent = value; }
        }

        public string ContinentName
        {
            get { return _continentName; }
            set { _continentName = value; }
        }

        // Calculated properties
        public double ScorePercentage
        {
            get 
            { 
                if (TotalQuestions == 0) return 0;
                return (double)CorrectAnswers / TotalQuestions * 100; 
            }
        }

        public int WrongAnswers
        {
            get { return TotalQuestions - CorrectAnswers; }
        }

        public string TimeTakenFormatted
        {
            get 
            { 
                if (TimeTaken.TotalHours >= 1)
                    return $"{TimeTaken.Hours:D2}:{TimeTaken.Minutes:D2}:{TimeTaken.Seconds:D2}";
                else
                    return $"{TimeTaken.Minutes:D2}:{TimeTaken.Seconds:D2}";
            }
        }

        public string TimeTakenInMinutes
        {
            get { return $"{TimeTaken.TotalMinutes:F1} minutes"; }
        }

        // Constructors
        public QuizResult()
        {
            CompletedDate = DateTime.Now;
        }

        public QuizResult(int userId, string username, int totalQuestions, int correctAnswers, 
                         TimeSpan timeTaken, Continent selectedContinent, string continentName) : this()
        {
            UserId = userId;
            Username = username;
            TotalQuestions = totalQuestions;
            CorrectAnswers = correctAnswers;
            TimeTaken = timeTaken;
            SelectedContinent = selectedContinent;
            ContinentName = continentName;
        }

        // Method to get grade based on percentage
        public string GetGrade()
        {
            double percentage = ScorePercentage;
            
            if (percentage >= 90) return "A+";
            if (percentage >= 85) return "A";
            if (percentage >= 80) return "A-";
            if (percentage >= 75) return "B+";
            if (percentage >= 70) return "B";
            if (percentage >= 65) return "B-";
            if (percentage >= 60) return "C+";
            if (percentage >= 55) return "C";
            if (percentage >= 50) return "C-";
            if (percentage >= 45) return "D+";
            if (percentage >= 40) return "D";
            return "F";
        }

        // Method to get performance message
        public string GetPerformanceMessage()
        {
            double percentage = ScorePercentage;
            
            if (percentage >= 90) return "Excellent! Outstanding geography knowledge!";
            if (percentage >= 80) return "Great job! Very good understanding of geography!";
            if (percentage >= 70) return "Good work! Solid geography knowledge!";
            if (percentage >= 60) return "Not bad! Keep studying to improve!";
            if (percentage >= 50) return "Fair performance. More practice needed!";
            return "Keep studying! Geography is fascinating!";
        }

        public override string ToString()
        {
            return $"{Username}: {CorrectAnswers}/{TotalQuestions} ({ScorePercentage:F1}%) - {ContinentName} - {TimeTakenFormatted}";
        }
    }
}
