using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string Author { get; set; }
        public NewsArticle Article { get; set; }
        public int ArticleId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public ArtComStatus ArtComStatus { get; set; } = ArtComStatus.Opublikowany;
        public int Upvotes { get; set; } // Liczba łapek w górę
        public int Downvotes { get; set; } // Liczba łapek w dół
    }
}