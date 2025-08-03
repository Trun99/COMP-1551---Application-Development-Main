using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using QQuizzles.Models;

namespace QQuizzles.Database
{
 
    public class DatabaseManager
    {
        private readonly string _connectionString;
        private readonly string _databasePath;

        public DatabaseManager()
        {
            _databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "QQuizzles.db");
            _connectionString = $"Data Source={_databasePath};Version=3;Timeout=30;Journal Mode=WAL;";

          
            CleanupDatabase();
            InitializeDatabase();
        }

        private void CleanupDatabase()
        {
            try
            {
               
                if (File.Exists(_databasePath + "-wal"))
                {
                    File.Delete(_databasePath + "-wal");
                }
                if (File.Exists(_databasePath + "-shm"))
                {
                    File.Delete(_databasePath + "-shm");
                }

                
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch
            {
                // Ignore cleanup errors
            }
        }

        
        private void InitializeDatabase()
        {
            try
            {
                
                var directory = Path.GetDirectoryName(_databasePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();

                    // Enable WAL mode for better concurrency
                    using (var pragmaCommand = new SQLiteCommand("PRAGMA journal_mode=WAL;", connection))
                    {
                        pragmaCommand.ExecuteNonQuery();
                    }

                    // Create Users table
                    string createUsersTable = @"
                        CREATE TABLE IF NOT EXISTS Users (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Username TEXT UNIQUE NOT NULL,
                            Password TEXT NOT NULL,
                            CreatedDate TEXT NOT NULL,
                            LastLoginDate TEXT NOT NULL
                        )";

                    // Create Questions table
                    string createQuestionsTable = @"
                        CREATE TABLE IF NOT EXISTS Questions (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            QuestionText TEXT NOT NULL,
                            CorrectAnswer TEXT NOT NULL,
                            QuestionType INTEGER NOT NULL,
                            Continent INTEGER NOT NULL,
                            CreatedDate TEXT NOT NULL,
                            Options TEXT,
                            CorrectOptionIndex INTEGER,
                            AlternativeAnswers TEXT
                        )";

                    // Create QuizResults table
                    string createResultsTable = @"
                        CREATE TABLE IF NOT EXISTS QuizResults (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            UserId INTEGER NOT NULL,
                            Username TEXT NOT NULL,
                            TotalQuestions INTEGER NOT NULL,
                            CorrectAnswers INTEGER NOT NULL,
                            TimeTakenSeconds INTEGER NOT NULL,
                            CompletedDate TEXT NOT NULL,
                            SelectedContinent INTEGER NOT NULL,
                            ContinentName TEXT NOT NULL,
                            FOREIGN KEY (UserId) REFERENCES Users (Id)
                        )";

                    ExecuteNonQuery(connection, createUsersTable);
                    ExecuteNonQuery(connection, createQuestionsTable);
                    ExecuteNonQuery(connection, createResultsTable);

                    // Create default admin user if not exists
                    CreateDefaultUser(connection);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to initialize database: {ex.Message}", ex);
            }
        }

        private void CreateDefaultUser(SQLiteConnection connection)
        {
            string checkUser = "SELECT COUNT(*) FROM Users WHERE Username = 'admin'";
            using (var command = new SQLiteCommand(checkUser, connection))
            {
                long count = (long)command.ExecuteScalar();
                if (count == 0)
                {
                    string insertUser = @"
                        INSERT INTO Users (Username, Password, CreatedDate, LastLoginDate)
                        VALUES ('admin', 'admin', @CreatedDate, @LastLoginDate)";
                    
                    using (var insertCommand = new SQLiteCommand(insertUser, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@CreatedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        insertCommand.Parameters.AddWithValue("@LastLoginDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
        }

        private void ExecuteNonQuery(SQLiteConnection connection, string sql)
        {
            using (var command = new SQLiteCommand(sql, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        // User operations
        public User AuthenticateUser(string username, string password)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Users WHERE Username = @Username AND Password = @Password";
                    
                    using (var command = new SQLiteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", password);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var user = new User
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Username = reader["Username"].ToString(),
                                    Password = reader["Password"].ToString(),
                                    CreatedDate = DateTime.Parse(reader["CreatedDate"].ToString()),
                                    LastLoginDate = DateTime.Parse(reader["LastLoginDate"].ToString())
                                };
                                
                                // Update last login
                                UpdateUserLastLogin(user.Id);
                                user.UpdateLastLogin();
                                
                                return user;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Authentication failed: {ex.Message}", ex);
            }
            
            return null;
        }

        private void UpdateUserLastLogin(int userId)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string sql = "UPDATE Users SET LastLoginDate = @LastLoginDate WHERE Id = @Id";
                
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@LastLoginDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@Id", userId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public bool CreateUser(User user)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    string sql = @"
                        INSERT INTO Users (Username, Password, CreatedDate, LastLoginDate)
                        VALUES (@Username, @Password, @CreatedDate, @LastLoginDate)";

                    using (var command = new SQLiteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Username", user.Username);
                        command.Parameters.AddWithValue("@Password", user.Password);
                        command.Parameters.AddWithValue("@CreatedDate", user.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"));
                        command.Parameters.AddWithValue("@LastLoginDate", user.LastLoginDate.ToString("yyyy-MM-dd HH:mm:ss"));

                        int result = command.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch (SQLiteException ex) when (ex.ResultCode == SQLiteErrorCode.Constraint)
            {
                return false; // Username already exists
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create user: {ex.Message}", ex);
            }
        }

        // Question operations
        public bool SaveQuestion(Question question)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    string sql = @"
                        INSERT INTO Questions (QuestionText, CorrectAnswer, QuestionType, Continent, CreatedDate, Options, CorrectOptionIndex, AlternativeAnswers)
                        VALUES (@QuestionText, @CorrectAnswer, @QuestionType, @Continent, @CreatedDate, @Options, @CorrectOptionIndex, @AlternativeAnswers)";

                    using (var command = new SQLiteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@QuestionText", question.QuestionText);
                        command.Parameters.AddWithValue("@CorrectAnswer", question.CorrectAnswer);
                        command.Parameters.AddWithValue("@QuestionType", (int)question.Type);
                        command.Parameters.AddWithValue("@Continent", (int)question.Continent);
                        command.Parameters.AddWithValue("@CreatedDate", question.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"));

                        
                        if (question is MultipleChoiceQuestion mcq)
                        {
                            command.Parameters.AddWithValue("@Options", string.Join("|", mcq.Options));
                            command.Parameters.AddWithValue("@CorrectOptionIndex", mcq.CorrectOptionIndex);
                            command.Parameters.AddWithValue("@AlternativeAnswers", DBNull.Value);
                        }
                        else if (question is OpenEndedQuestion oeq)
                        {
                            command.Parameters.AddWithValue("@Options", DBNull.Value);
                            command.Parameters.AddWithValue("@CorrectOptionIndex", DBNull.Value);
                            command.Parameters.AddWithValue("@AlternativeAnswers", string.Join("|", oeq.AlternativeAnswers));
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@Options", DBNull.Value);
                            command.Parameters.AddWithValue("@CorrectOptionIndex", DBNull.Value);
                            command.Parameters.AddWithValue("@AlternativeAnswers", DBNull.Value);
                        }

                        int result = command.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save question: {ex.Message}", ex);
            }
        }

        public List<Question> GetQuestionsByContinent(Continent continent)
        {
            var questions = new List<Question>();

            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Questions WHERE Continent = @Continent ORDER BY CreatedDate";

                    using (var command = new SQLiteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Continent", (int)continent);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                questions.Add(CreateQuestionFromReader(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get questions: {ex.Message}", ex);
            }

            return questions;
        }

        public List<Question> GetAllQuestions()
        {
            var questions = new List<Question>();

            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Questions ORDER BY Continent, CreatedDate";

                    using (var command = new SQLiteCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                questions.Add(CreateQuestionFromReader(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get all questions: {ex.Message}", ex);
            }

            return questions;
        }

        private Question CreateQuestionFromReader(SQLiteDataReader reader)
        {
            int id = Convert.ToInt32(reader["Id"]);
            string questionText = reader["QuestionText"].ToString();
            string correctAnswer = reader["CorrectAnswer"].ToString();
            var questionType = (QuestionType)Convert.ToInt32(reader["QuestionType"]);
            var continent = (Continent)Convert.ToInt32(reader["Continent"]);
            DateTime createdDate = DateTime.Parse(reader["CreatedDate"].ToString());

            Question question = questionType switch
            {
                QuestionType.TrueFalse => new TrueFalseQuestion(questionText, bool.Parse(correctAnswer), continent),
                QuestionType.OpenEnded => CreateOpenEndedQuestion(reader, questionText, correctAnswer, continent),
                QuestionType.MultipleChoice => CreateMultipleChoiceQuestion(reader, questionText, continent),
                _ => throw new ArgumentException($"Unknown question type: {questionType}")
            };

            question.Id = id;
            question.CreatedDate = createdDate;
            return question;
        }

        private OpenEndedQuestion CreateOpenEndedQuestion(SQLiteDataReader reader, string questionText, string correctAnswer, Continent continent)
        {
            string[] alternativeAnswers = new string[0];
            if (reader["AlternativeAnswers"] != DBNull.Value)
            {
                string altAnswersStr = reader["AlternativeAnswers"].ToString();
                if (!string.IsNullOrEmpty(altAnswersStr))
                {
                    alternativeAnswers = altAnswersStr.Split('|', StringSplitOptions.RemoveEmptyEntries);
                }
            }

            return new OpenEndedQuestion(questionText, correctAnswer, continent, alternativeAnswers);
        }

        private MultipleChoiceQuestion CreateMultipleChoiceQuestion(SQLiteDataReader reader, string questionText, Continent continent)
        {
            string optionsStr = reader["Options"].ToString();
            string[] options = optionsStr.Split('|');
            int correctOptionIndex = Convert.ToInt32(reader["CorrectOptionIndex"]);

            return new MultipleChoiceQuestion(questionText, options, correctOptionIndex, continent);
        }

        // Quiz Result operations
        public bool SaveQuizResult(QuizResult result)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    string sql = @"
                        INSERT INTO QuizResults (UserId, Username, TotalQuestions, CorrectAnswers, TimeTakenSeconds, CompletedDate, SelectedContinent, ContinentName)
                        VALUES (@UserId, @Username, @TotalQuestions, @CorrectAnswers, @TimeTakenSeconds, @CompletedDate, @SelectedContinent, @ContinentName)";

                    using (var command = new SQLiteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", result.UserId);
                        command.Parameters.AddWithValue("@Username", result.Username);
                        command.Parameters.AddWithValue("@TotalQuestions", result.TotalQuestions);
                        command.Parameters.AddWithValue("@CorrectAnswers", result.CorrectAnswers);
                        command.Parameters.AddWithValue("@TimeTakenSeconds", (int)result.TimeTaken.TotalSeconds);
                        command.Parameters.AddWithValue("@CompletedDate", result.CompletedDate.ToString("yyyy-MM-dd HH:mm:ss"));
                        command.Parameters.AddWithValue("@SelectedContinent", (int)result.SelectedContinent);
                        command.Parameters.AddWithValue("@ContinentName", result.ContinentName);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save quiz result: {ex.Message}", ex);
            }
        }

        public List<QuizResult> GetQuizResultsByUser(int userId)
        {
            var results = new List<QuizResult>();

            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM QuizResults WHERE UserId = @UserId ORDER BY CompletedDate DESC";

                    using (var command = new SQLiteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                results.Add(CreateQuizResultFromReader(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get quiz results: {ex.Message}", ex);
            }

            return results;
        }

        public List<QuizResult> GetAllQuizResults()
        {
            var results = new List<QuizResult>();

            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM QuizResults ORDER BY CompletedDate DESC";

                    using (var command = new SQLiteCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                results.Add(CreateQuizResultFromReader(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get all quiz results: {ex.Message}", ex);
            }

            return results;
        }

        private QuizResult CreateQuizResultFromReader(SQLiteDataReader reader)
        {
            return new QuizResult
            {
                Id = Convert.ToInt32(reader["Id"]),
                UserId = Convert.ToInt32(reader["UserId"]),
                Username = reader["Username"].ToString(),
                TotalQuestions = Convert.ToInt32(reader["TotalQuestions"]),
                CorrectAnswers = Convert.ToInt32(reader["CorrectAnswers"]),
                TimeTaken = TimeSpan.FromSeconds(Convert.ToInt32(reader["TimeTakenSeconds"])),
                CompletedDate = DateTime.Parse(reader["CompletedDate"].ToString()),
                SelectedContinent = (Continent)Convert.ToInt32(reader["SelectedContinent"]),
                ContinentName = reader["ContinentName"].ToString()
            };
        }

       
        public int GetQuestionCount()
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "SELECT COUNT(*) FROM Questions";

                    using (var command = new SQLiteCommand(sql, connection))
                    {
                        return Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get question count: {ex.Message}", ex);
            }
        }

        public int GetQuestionCountByContinent(Continent continent)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "SELECT COUNT(*) FROM Questions WHERE Continent = @Continent";

                    using (var command = new SQLiteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Continent", (int)continent);
                        return Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get question count by continent: {ex.Message}", ex);
            }
        }

       
        public bool UpdateQuestion(Question question)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    string sql = @"
                        UPDATE Questions
                        SET QuestionText = @QuestionText,
                            CorrectAnswer = @CorrectAnswer,
                            QuestionType = @QuestionType,
                            Continent = @Continent,
                            Options = @Options,
                            CorrectOptionIndex = @CorrectOptionIndex,
                            AlternativeAnswers = @AlternativeAnswers
                        WHERE Id = @Id";

                    using (var command = new SQLiteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", question.Id);
                        command.Parameters.AddWithValue("@QuestionText", question.QuestionText);
                        command.Parameters.AddWithValue("@CorrectAnswer", question.CorrectAnswer);
                        command.Parameters.AddWithValue("@QuestionType", (int)question.Type);
                        command.Parameters.AddWithValue("@Continent", (int)question.Continent);

                        // Handle polymorphic question types
                        if (question is MultipleChoiceQuestion mcq)
                        {
                            command.Parameters.AddWithValue("@Options", string.Join("|", mcq.Options));
                            command.Parameters.AddWithValue("@CorrectOptionIndex", mcq.CorrectOptionIndex);
                            command.Parameters.AddWithValue("@AlternativeAnswers", DBNull.Value);
                        }
                        else if (question is OpenEndedQuestion oeq)
                        {
                            command.Parameters.AddWithValue("@Options", DBNull.Value);
                            command.Parameters.AddWithValue("@CorrectOptionIndex", DBNull.Value);
                            command.Parameters.AddWithValue("@AlternativeAnswers",
                                oeq.AlternativeAnswers.Length > 0 ? string.Join("|", oeq.AlternativeAnswers) : DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@Options", DBNull.Value);
                            command.Parameters.AddWithValue("@CorrectOptionIndex", DBNull.Value);
                            command.Parameters.AddWithValue("@AlternativeAnswers", DBNull.Value);
                        }

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update question: {ex.Message}", ex);
            }
        }

      
        public bool DeleteQuestion(int questionId)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "DELETE FROM Questions WHERE Id = @Id";

                    using (var command = new SQLiteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", questionId);
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete question: {ex.Message}", ex);
            }
        }

       
        public Question GetQuestionById(int questionId)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Questions WHERE Id = @Id";

                    using (var command = new SQLiteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", questionId);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return CreateQuestionFromReader(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get question by ID: {ex.Message}", ex);
            }

            return null;
        }
    }
}
