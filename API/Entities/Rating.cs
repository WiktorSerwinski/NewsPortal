namespace API.Entities
{
    public class Rating
    {
        public int RatingId { get; set;}
        public int UserId { get; set;}
        public int ArticleId { get; set;}
        public int Value { get; set;}
        public NewsArticle Article { get; set;}
        public User User {get; set;}

    }
}