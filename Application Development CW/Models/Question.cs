using System;

namespace QQuizzles.Models
{
    
  

    public abstract class Question
    {
        
        private int _id;
        private string _questionText;
        private string _correctAnswer;
        private Continent _continent;
        private DateTime _createdDate;

       
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string QuestionText
        {
            get { return _questionText; }
            set 
            { 
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Question text cannot be empty");
                _questionText = value; 
            }
        }

        public string CorrectAnswer
        {
            get { return _correctAnswer; }
            set 
            { 
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Correct answer cannot be empty");
                _correctAnswer = value; 
            }
        }

        public Continent Continent
        {
            get { return _continent; }
            set { _continent = value; }
        }

        public DateTime CreatedDate
        {
            get { return _createdDate; }
            set { _createdDate = value; }
        }

       
        public abstract QuestionType Type { get; }

       
        protected Question(string questionText, string correctAnswer, Continent continent)
        {
            QuestionText = questionText;
            CorrectAnswer = correctAnswer;
            Continent = continent;
            CreatedDate = DateTime.Now;
        }

       
        public abstract bool CheckAnswer(string userAnswer);

       
        public virtual string GetDisplayText()
        {
            return $"[{Continent}] {QuestionText}";
        }

      
        public virtual string GetFormattedCorrectAnswer()
        {
            return CorrectAnswer;
        }

        public override string ToString()
        {
            return $"{Type}: {QuestionText}";
        }
    }

  
    public enum QuestionType
    {
        TrueFalse = 1,
        OpenEnded = 2,
        MultipleChoice = 3
    }

   
    public enum Continent
    {
        Asia = 1,        
        Europe = 2,     
        America = 3,     
        Africa = 4,      
        Oceania = 5,     
        Antarctica = 6   
    }
}
