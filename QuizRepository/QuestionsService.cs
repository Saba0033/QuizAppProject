using QuizzModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuizAppModel.Interfaces;

namespace QuizAppRepository
{
    public class QuestionsService : GenericRepository<Question>
    {
        public QuestionsService(string path) : base(path)
        {
        }

        public Question CreateQuestion(int questionNumber, int quizId) 
        {
            Console.WriteLine($"Enter the text for Question {questionNumber+1}:");
            string questionText = Console.ReadLine();

            string[] options = new string[4];
            for (int i = 1; i <= options.Length; i++)
            {
                Console.Write($"Enter answer option {i}: ");
                string option = Console.ReadLine();
                options[i-1] = option;
            }

            Console.Write("Enter the number (1-4) corresponding to the correct answer: ");
            int correctAnswerIndex;
            while (!int.TryParse(Console.ReadLine(), out  correctAnswerIndex))
            {
                Console.WriteLine("Invalid input. Please enter a number between 1 and 4.");
            }

            Question question = new Question
            {
                Id = Data.Count > 0 ? Data.Max(q => q.Id) + 1 : 1,
                Text = questionText,
                Options = options,
                QuizId = quizId,
                CorrectAnswerIndex = correctAnswerIndex - 1,
                Type = EntityType.Question
            };

            AddData(question);
            Console.WriteLine($"Question {questionNumber} added successfully!");
            return question;
        }
        

        public void DisplayQuestion(Question question)
        {
            Console.WriteLine($"Question: {question.Text}");
            for (int i = 0; i < question.Options.Length; i++)
            {
                Console.WriteLine($"{i + 1}: {question.Options[i]}");
            }
        }


        public void ModifyQuiz()
        {

        }



    }
}
