using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizAppRepository
{
    public interface IPaths
    {
        public static string UserPath { get; set; } = @"C:\Users\Saba\Desktop\QuizApp\QuizRepository\Users.Json";
        public static string QuizPath { get; set; } = @"C:\Users\Saba\Desktop\QuizApp\QuizRepository\Quizzes.json";
        public static string QuestionPath { get; set; } = @"C:\Users\Saba\Desktop\QuizApp\QuizRepository\Questions.json";
    }
}
