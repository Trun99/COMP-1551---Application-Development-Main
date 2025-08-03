using System;
using System.Linq;

namespace QQuizzles.Models
{
    
    public class OpenEndedQuestion : Question
    {
       
        private string[] _alternativeAnswers;

        
        public override QuestionType Type => QuestionType.OpenEnded;

      
        public string[] AlternativeAnswers
        {
            get { return _alternativeAnswers ?? Array.Empty<string>(); }
            set { _alternativeAnswers = value; }
        }

        // Constructor
        public OpenEndedQuestion(string questionText, string correctAnswer, Continent continent,
                               string[]? alternativeAnswers = null)
            : base(questionText, correctAnswer, continent)
        {
            AlternativeAnswers = alternativeAnswers ?? Array.Empty<string>();
        }

        
        public override bool CheckAnswer(string userAnswer)
        {
            if (string.IsNullOrWhiteSpace(userAnswer))
                return false;

            string normalizedUserAnswer = NormalizeAnswer(userAnswer);
            
            
            if (normalizedUserAnswer == NormalizeAnswer(CorrectAnswer))
                return true;

         
            foreach (string altAnswer in AlternativeAnswers)
            {
                if (!string.IsNullOrWhiteSpace(altAnswer) && 
                    normalizedUserAnswer == NormalizeAnswer(altAnswer))
                    return true;
            }

            return false;
        }

        
        private string NormalizeAnswer(string answer)
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

        
        public override string GetDisplayText()
        {
            return $"[{Continent}] {QuestionText}\n\nEnter your answer (1-4 words):";
        }

        public override string GetFormattedCorrectAnswer()
        {
            if (AlternativeAnswers.Length > 0)
            {
                var allAnswers = new[] { CorrectAnswer }.Concat(AlternativeAnswers);
                return string.Join(" / ", allAnswers.Where(a => !string.IsNullOrWhiteSpace(a)));
            }
            return CorrectAnswer;
        }

        
        public void AddAlternativeAnswer(string alternativeAnswer)
        {
            if (string.IsNullOrWhiteSpace(alternativeAnswer))
                return;

            var currentAnswers = AlternativeAnswers.ToList();
            if (!currentAnswers.Contains(alternativeAnswer, StringComparer.OrdinalIgnoreCase))
            {
                currentAnswers.Add(alternativeAnswer);
                AlternativeAnswers = currentAnswers.ToArray();
            }
        }

        
        public bool IsValidAnswerLength(string answer)
        {
            if (string.IsNullOrWhiteSpace(answer))
                return false;

            int wordCount = answer.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
            return wordCount >= 1 && wordCount <= 4;
        }
    }
}
