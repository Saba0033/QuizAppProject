using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuizAppModel.Interfaces;
using QuizzModel;

namespace QuizAppModel
{
    public class User : IIdentifiable
    {
        public int Id { get; set; }
        public EntityType Type { get; set; } = EntityType.User;
        public string Name { get; set; }
        public string Password { get; set; } 
        public int Record { get; set; } = 0;
        public List<int> QuizIds { get; set; }

      

    }
}
