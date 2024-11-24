using QuizAppModel.Interfaces;

namespace QuizzModel
{
    public class Quiz : IIdentifiable
    {
        public string Title { get; set; }
        public List<int> QuestionIds { get; set; }
        public int Id { get; set; }
        public EntityType Type { get; set; } = EntityType.Quiz;
        public int CreatorId { get; set; }

    }
}
