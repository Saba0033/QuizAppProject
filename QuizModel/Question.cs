using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuizAppModel.Interfaces;

namespace QuizzModel
{
    public class Question : IIdentifiable
    {
        public string Text { get; set; }
        public string[] Options { get;  set; }
        public int CorrectAnswerIndex { get;  set; }
        public int Id { get; set; }
        public int QuizId { get; set; }
        public EntityType Type { get; set; } = 
            EntityType.Question;

    }
}

