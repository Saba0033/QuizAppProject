using System.Runtime.InteropServices.JavaScript;
using QuizAppModel;
using QuizAppRepository;
using QuizzModel;
using static System.Formats.Asn1.AsnWriter;

namespace QuizRepository
{
    public class QuizService : GenericRepository<Quiz>, IListable
    {
        private readonly QuestionsService _questionsService;
        private readonly double _time = 0.5;
        public QuizService(string path, QuestionsService questionsService) : base(path)
        {
            _questionsService = questionsService;
        }

        public void CreateQuiz(User user)
        {
            if (user == null)
            {
                Console.WriteLine("You must be logged in to create a quiz.");
                return;
            }

            Console.Write("Enter the quiz title: ");
            string title = Console.ReadLine();

            if (Data.Any(q => q.Title.Equals(title, StringComparison.OrdinalIgnoreCase) && q.CreatorId == user.Id))
            {
                Console.WriteLine("You already have a quiz with this title. Please choose a different title.");
                return;
            }

            var newQuiz = new Quiz
            {
                Id = Data.Count > 0 ? Data.Max(q => q.Id) + 1 : 1,
                Title = title,
                QuestionIds = new List<int>(),
                CreatorId = user.Id
            };

            Console.WriteLine("This quiz will have 5 questions, each with 4 answer options.");
            for (int i = 1; i <= 5; i++)
            {
                Question question = _questionsService.CreateQuestion(i-1, newQuiz.Id);
                newQuiz.QuestionIds.Add(question.Id); 
            }

            AddData(newQuiz);
            user.QuizIds.Add(newQuiz.Id);
            UserService tmp = new UserService(IPaths.UserPath);
            tmp.LoggedUser = user;
            tmp.ModifyData(user.Id, u=>u.QuizIds.Add(newQuiz.Id));
            Console.WriteLine($"Quiz '{title}' created successfully with {newQuiz.QuestionIds.Count} questions!");
        }

        public void TakeQuiz(User user)
        {
            ListAllData();
            Console.Write("Enter the quiz ID to take: ");
            if (!int.TryParse(Console.ReadLine(), out int quizId))
            {
                Console.WriteLine("Invalid input. Quiz ID must be a number.");
                return;
            }
            Quiz quiz = GetData(q => q.Id == quizId);

            if (quiz == null)
            {
                Console.WriteLine("Quiz not found.");
                return;
            }

            if (quiz.CreatorId == user.Id)
            {
                Console.WriteLine("You can't participate in your own Quiz");
                return;
            }

            int score = 0;
            DateTime startTime = DateTime.Now;

            foreach (int questionId in quiz.QuestionIds)
            {
                if (checkForTime(startTime)) break;
                Question question = _questionsService.GetData(q => q.Id == questionId);
                _questionsService.DisplayQuestion(question);
                Console.Write("Enter your answer (1-4): ");
                if (!int.TryParse(Console.ReadLine(), out int answerChoice) || answerChoice < 1 || answerChoice > 4)
                {
                    Console.WriteLine("Invalid input. Please enter a number between 1 and 4.");
                    continue; 
                }
                if (answerChoice - 1 == question.CorrectAnswerIndex)
                {
                    score += 20;
                } else score -= 20;
            }

            if (score > user.Record)
            {
                UserService tmp = new UserService(IPaths.UserPath);
                tmp.LoggedUser = user;
                tmp.ModifyData(user.Id, user => user.Record = score);
                user.Record = score;
            }
                

            Console.WriteLine($"You scored {score} out of {quiz.QuestionIds.Count}.");
        }



        public void DeleteQuiz(User loggedUser)
        {

            Console.Write("Enter the quiz ID to take: ");
            if (!int.TryParse(Console.ReadLine(), out int quizId))
            {
                Console.WriteLine("Invalid input. Quiz ID must be a number.");
                return;
            }
            if (!loggedUser.QuizIds.Contains(quizId))
            {
                Console.WriteLine("You can only delete your own quizzes");
                return;
            }

            Quiz quiz = GetData(q => q.Id == quizId);
            if (quiz == null || quiz.CreatorId != loggedUser.Id)
            {
                Console.WriteLine("Quiz not found or you don't have permission to delete it.");
                return;
            }

            foreach (int questionId in quiz.QuestionIds)
            {
                Question question = _questionsService.GetData(q => q.Id == questionId);
                if (question != null)
                {
                    ModifyData(quiz.Id, q => q.QuestionIds.Remove(questionId));
                    QuestionsService tp = new QuestionsService(IPaths.QuestionPath);
                    tp.DeleteData(questionId);
                }
            }

            DeleteData(quizId);
            UserService tmp = new UserService(IPaths.UserPath);
            tmp.LoggedUser = loggedUser;
            tmp.ModifyData(loggedUser.Id, u => u.QuizIds.Remove(quizId));
            Console.WriteLine($"Quiz '{quiz.Title}' deleted successfully!");
        }


        private bool checkForTime(DateTime startTime)
        {
            if ((DateTime.Now - startTime).TotalMinutes > _time)
            {
                Console.WriteLine("Time is up!");
                return true;
            }

            return false;
        }

        public void ListAllData()
        {
            foreach (Quiz quiz in Data)
            {
                Console.WriteLine($"Quize Name: {quiz.Title}     Quiz Id: {quiz.Id}");
            }
        }


        public void ModifyQuiz(User loggedUser)
        {
            if (loggedUser == null)
            {
                Console.WriteLine("You must be logged in to modify a quiz.");
                return;
            }

            Console.Write("Enter the ID of the quiz you want to modify: ");
            if (!int.TryParse(Console.ReadLine(), out int quizId))
            {
                Console.WriteLine("Invalid ID. Please enter a valid numeric value.");
                return;
            }

            // Check if the quiz exists and belongs to the logged-in user
            Quiz quizToModify = Data.FirstOrDefault(quiz => quiz.Id == quizId && quiz.CreatorId == loggedUser.Id);

            if (quizToModify == null)
            {
                Console.WriteLine("Quiz not found, or you do not have permission to modify this quiz.");
                return;
            }

            Console.WriteLine($"You are modifying the quiz: {quizToModify.Title}");
            Console.WriteLine("What would you like to modify?");
            Console.WriteLine("1. Title");
            Console.WriteLine("2. Questions");
            Console.WriteLine("3. Cancel");
            Console.Write("Select an option: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Enter new title: ");
                    string newTitle = Console.ReadLine();
                    ModifyData(quizToModify.Id, q=>q.Title = newTitle);
                    break;

                case "2":
                    
                    break;

                case "3":
                    Console.WriteLine("Modification canceled.");
                    return;

                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }



    }
}