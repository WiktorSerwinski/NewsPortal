namespace API.Entities
{
    public class NewsArticle
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public string PictureUrl { get; set; }
        public string Content { get; set; }
        public DateTime PublicationDate { get; set; } = DateTime.Now;
        public string Author { get; set; }

        public string Category { get; set; }

        public List<Comment> Comments { get; set; }

        public int Views { get; set; }

        public float Rate { get; set; } = 0;

        public ArtComStatus articleStatus { get; set; } = ArtComStatus.Opublikowany;

    }
}