using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using QuizAppRepository;
using QuizAppModel;
using QuizRepository;

namespace QuizProgram
{
    public class Program : IPaths
    {
        static UserService userService;
        static QuizService quizService;
        static QuestionsService questionService;
        private static readonly int sleepTime = 1500;

        static void Main(string[] args)
        {
            questionService = new QuestionsService(IPaths.QuestionPath);
            quizService = new QuizService(IPaths.QuizPath, questionService);
            userService = new UserService(IPaths.UserPath);
            
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Welcome to the Quiz Program!");
                Console.WriteLine("1. Register");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. List top 10 users");
                Console.WriteLine("4. List all users");
                Console.WriteLine("5. Exit");
                Console.Write("Select an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        userService.Register();
                        break;

                    case "2":
                        if(!userService.Login()) break;
                        Thread.Sleep(sleepTime);
                        ShowMainMenu();
                        break;

                    case "3":
                        userService.ListTop10();
                        Console.ReadLine();
                        break;

                    case "4":
                        userService.ListAllData();
                        Console.ReadLine();
                        break;

                    case "5":
                        return;

                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
                Thread.Sleep(sleepTime);


            }
        }

        private static void ShowMainMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Logged in as: {userService.LoggedUser.Name}");
                Console.WriteLine($"Your Personal Record: {userService.LoggedUser.Record}");
                Console.WriteLine("Main Menu:");
                Console.WriteLine("1. Take a Quiz");
                Console.WriteLine("2. Create a Quiz");
                Console.WriteLine("3. Modify a Quiz");
                Console.WriteLine("4. Delete a Quiz");
                Console.WriteLine("5. List my quizzes");
                Console.WriteLine("6. Change Password");
                Console.WriteLine("7. Delete Account");
                Console.WriteLine("8. Logout");
                Console.Write("Select an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        quizService.TakeQuiz(userService.LoggedUser);
                        break;

                    case "2":
                        quizService.CreateQuiz(userService.LoggedUser);
                        break;

                    case "3":
                        quizService.ModifyQuiz(userService.LoggedUser);
                        break;

                    case "4":
                        userService.ListMyQuizzes();
                        quizService.DeleteQuiz(userService.LoggedUser);
                        break;

                    case "5":
                        userService.ListMyQuizzes();
                        Console.ReadLine();
                        break;

                    case "6":
                        userService.ChangePassword();
                        break;

                    case "7":
                        userService.DeleteUser();
                        return; 

                    case "8":
                        Console.WriteLine("You have logged out.");
                        return;

                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
                Thread.Sleep(sleepTime);
            }
        }
    }
}