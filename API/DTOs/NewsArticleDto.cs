using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class NewsArticleDto
    {
        [Required]
        public string Title { get; set; }
        
        public IFormFile PictureUrl { get; set; }
        
        [Required]
        public string Content { get; set; }
        
        [Required]
        public string Category { get; set; }
        
    }
}
