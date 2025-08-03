using System;

namespace QQuizzles.Models
{
  
    public class TrueFalseQuestion : Question
    {
    
        public override QuestionType Type => QuestionType.TrueFalse;

       
        public TrueFalseQuestion(string questionText, bool correctAnswer, Continent continent)
            : base(questionText, correctAnswer.ToString(), continent)
        {
        }

      
        public override bool CheckAnswer(string userAnswer)
        {
            if (string.IsNullOrWhiteSpace(userAnswer))
                return false;

           
            string normalizedAnswer = userAnswer.Trim().ToLowerInvariant();
            string correctAnswerNormalized = CorrectAnswer.ToLowerInvariant();

           
            bool userAnswerBool = normalizedAnswer == "true" || 
                                 normalizedAnswer == "yes" || 
                                 normalizedAnswer == "1" ||
                                 normalizedAnswer == "correct" ||
                                 normalizedAnswer == "right";

            bool userAnswerFalse = normalizedAnswer == "false" || 
                                  normalizedAnswer == "no" || 
                                  normalizedAnswer == "0" ||
                                  normalizedAnswer == "incorrect" ||
                                  normalizedAnswer == "wrong";

            bool correctBool = correctAnswerNormalized == "true";

            if (correctBool)
                return userAnswerBool;
            else
                return userAnswerFalse;
        }

       
        public override string GetDisplayText()
        {
            return $"[{Continent}] {QuestionText}\n\nTrue or False?";
        }

        public override string GetFormattedCorrectAnswer()
        {
            return CorrectAnswer == "True" ? "True" : "False";
        }

        
        public bool CorrectAnswerBool
        {
            get { return CorrectAnswer.ToLowerInvariant() == "true"; }
        }
    }
}
