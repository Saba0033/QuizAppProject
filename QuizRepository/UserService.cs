using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using QuizAppModel;
using QuizRepository;
using QuizzModel;

namespace QuizAppRepository
{
    public class UserService : GenericRepository<User>, IListable
    {
        private User _loggedUser;

        public User LoggedUser
        {
            get => _loggedUser;
            set => _loggedUser = value;
        }

        public UserService(string path) : base(path)
        {
        }

        public void Register()
        {
            Console.Write("Enter username: ");
            string name = Console.ReadLine();

            if (GetData(user => user.Name == name) != null)
            {
                Console.WriteLine($"User '{name}' is already registered.");
                return;
            }

            Console.Write("Enter Password: ");
            string password = Console.ReadLine();

            User newUser = new User
            {
                Id = Data.Count > 0 ? Data.Max(user => user.Id) + 1 : 1,
                Name = name,
                Password = password,
                Record = 0,
                QuizIds = new List<int>()
            };

            AddData(newUser);
            Console.WriteLine($"User '{name}' registered successfully!");
        }

        public bool Login()
        {
            Console.Write("Enter username: ");
            string name = Console.ReadLine();

            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            var user = GetData(u => u.Name == name && u.Password == password);

            if (user != null)
            {
                Console.WriteLine($"Welcome back, {user.Name}!");

                _loggedUser = user;
                return true;
            }
            else
            {
                Console.WriteLine("Invalid username or password.");
                return false;
            }
        }

        public void DeleteUser()
        {
            if (_loggedUser == null)
            {
                Console.WriteLine("You need to log in first.");
                return;
            }

            DeepDelete();

            DeleteData(_loggedUser.Id);
            Console.WriteLine($"User '{_loggedUser.Name}' deleted successfully!");
            _loggedUser = null;
        }


        private void DeepDelete()
        {
            
            QuestionsService _questionService = new QuestionsService(IPaths.QuestionPath);
            QuizService _quizService = new QuizService(IPaths.QuizPath, _questionService);
            foreach (int quizId in _loggedUser.QuizIds)
            {
                Quiz quiz = _quizService.GetData(q => q.Id == quizId);
                if (quiz != null)
                {
                    foreach (int questionId in quiz.QuestionIds)
                    {
                        Question question = _questionService.GetData(q => q.Id == questionId);
                        if (question != null)
                        {
                            _questionService.DeleteData(questionId);
                        }
                    }

                    _quizService.DeleteData(quizId);
                }
            }
        }


        public void ChangePassword()
        {
            if (_loggedUser == null)
            {
                Console.WriteLine("You need to log in first.");
                return;
            }

            Console.Write("Enter your current password: ");
            string currentPassword = Console.ReadLine();

            if (_loggedUser.Password != currentPassword)
            {
                Console.WriteLine("Incorrect current password.");
                return;
            }

            Console.Write("Enter your new password: ");
            string newPassword = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                Console.WriteLine("Password cannot be empty.");
                return;
            }

            ModifyData(LoggedUser.Id, user => user.Password = newPassword ); 
            Console.WriteLine("Password changed successfully.");
        }


        public void ListAllData()
        {
            foreach (User user in Data)
            {
                Console.WriteLine($"ID: {user.Id}, Name: {user.Name}, Record: {user.Record}");
            }
        }

        public void ListTop10()
        {
            List<User> top10 = Data.OrderByDescending(U => U.Record).Take(10).ToList();
            foreach (User user in top10)
            {
                Console.WriteLine($"ID: {user.Id}, Name: {user.Name}, Record: {user.Record}");
            }
        }


        public void ListMyQuizzes()
        {
            QuestionsService _questionService = new QuestionsService(IPaths.QuestionPath);

            QuizService _quizService = new QuizService(IPaths.QuizPath, _questionService);
            if (LoggedUser == null)
            {
                Console.WriteLine("You must be logged in to view your quizzes.");
                return;
            }

            var userQuizzes = _quizService.LoadData().Where(quiz => quiz.CreatorId == LoggedUser.Id).ToList();

            if (userQuizzes.Count == 0)
            {
                Console.WriteLine("You have not created any quizzes yet.");
            }
            else
            {
                Console.WriteLine("Your Quizzes:");
                foreach (var quiz in userQuizzes)
                {
                    Console.WriteLine($"Quiz ID: {quiz.Id}, Title: {quiz.Title}, Number of Questions: {quiz.QuestionIds.Count}");
                }
            }
        }

    }
}