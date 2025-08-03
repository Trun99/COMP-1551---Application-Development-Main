using System;
using System.Linq;

namespace QQuizzles.Models
{
  
    public class MultipleChoiceQuestion : Question
    {
        
        private string[] _options;
        private int _correctOptionIndex;

      
        public override QuestionType Type => QuestionType.MultipleChoice;

        
        public string[] Options
        {
            get { return _options ?? new string[4]; }
            set 
            { 
                if (value == null || value.Length != 4)
                    throw new ArgumentException("Must provide exactly 4 options");
                _options = value; 
            }
        }

        public int CorrectOptionIndex
        {
            get { return _correctOptionIndex; }
            set 
            { 
                if (value < 0 || value > 3)
                    throw new ArgumentException("Correct option index must be between 0 and 3");
                _correctOptionIndex = value; 
            }
        }

        
        public string CorrectOptionText
        {
            get { return Options[CorrectOptionIndex]; }
        }

     
        public MultipleChoiceQuestion(string questionText, string[] options, int correctOptionIndex,
                                    Continent continent)
            : base(questionText, options[correctOptionIndex], continent)
        {
            Options = options;
            CorrectOptionIndex = correctOptionIndex;
            
        }

        
        public override bool CheckAnswer(string userAnswer)
        {
            if (string.IsNullOrWhiteSpace(userAnswer))
                return false;

           
            string normalizedAnswer = userAnswer.Trim().ToUpperInvariant();
            
            if (normalizedAnswer.Length == 1 && "ABCD".Contains(normalizedAnswer))
            {
                int selectedIndex = normalizedAnswer[0] - 'A';
                return selectedIndex == CorrectOptionIndex;
            }

           
            if (int.TryParse(normalizedAnswer, out int optionNumber))
            {
                if (optionNumber >= 1 && optionNumber <= 4)
                {
                    return (optionNumber - 1) == CorrectOptionIndex;
                }
            }

          
            for (int i = 0; i < Options.Length; i++)
            {
                if (string.Equals(Options[i], userAnswer, StringComparison.OrdinalIgnoreCase))
                {
                    return i == CorrectOptionIndex;
                }
            }

            return false;
        }

     
        public override string GetDisplayText()
        {
            string display = $"[{Continent}] {QuestionText}\n\n";
            
            for (int i = 0; i < Options.Length; i++)
            {
                char optionLetter = (char)('A' + i);
                display += $"{optionLetter}. {Options[i]}\n";
            }
            
            display += "\nSelect A, B, C, or D:";
            return display;
        }

        public override string GetFormattedCorrectAnswer()
        {
            char correctLetter = (char)('A' + CorrectOptionIndex);
            return $"{correctLetter}. {CorrectOptionText}";
        }

    
        public string GetOption(int index)
        {
            if (index < 0 || index >= Options.Length)
                throw new ArgumentOutOfRangeException(nameof(index));
            
            return Options[index];
        }

      
        public char GetCorrectOptionLetter()
        {
            return (char)('A' + CorrectOptionIndex);
        }

     
        public bool AreAllOptionsValid()
        {
            return Options != null && 
                   Options.Length == 4 && 
                   Options.All(option => !string.IsNullOrWhiteSpace(option));
        }

       
        public void ShuffleOptions(Random? random = null)
        {
            if (random == null)
                random = new Random();

            string correctOption = Options[CorrectOptionIndex];
            
         
            for (int i = Options.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (Options[i], Options[j]) = (Options[j], Options[i]);
            }

            
            CorrectOptionIndex = Array.IndexOf(Options, correctOption);
            CorrectAnswer = correctOption;
        }
    }
}
