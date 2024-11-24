namespace QuizAppModel.Interfaces
{
    public interface IIdentifiable
    {
        public int Id { get; set; }
        public EntityType Type { get; set; }

      
    }

    public enum EntityType
    {
        User,
        Quiz,
        Question
    }
}