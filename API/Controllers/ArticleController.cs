using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class ArticleController : BaseApiController
    {
        private readonly Context _context;
        private readonly UserManager<User> _userManager;

        public ArticleController(Context context, UserManager<User> userManager)
        {
            _userManager = userManager;
            _context = context;

        }

        [HttpGet]
        public async Task<ActionResult<List<NewsArticle>>> GetNewsArticles()
        {
            var articles = await _context.Articles.ToListAsync();
            return new OkObjectResult(articles);
        }

        [HttpGet("{id}", Name = "GetArticle")]
        public async Task<ActionResult<NewsArticle>> GetNewsArticle(int id)
        {
            var art = await _context.Articles.FindAsync(id);
            art.Views+=1;
            await _context.SaveChangesAsync();
            return art;
        }

        [Authorize]
        [HttpPut("Rate")]
        public async Task<ActionResult<NewsArticle>> RateArticle(int id, int value)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var art = await _context.Articles.FindAsync(id);

            if (art == null)
            {
                return NotFound();
            }
            // Sprawdź, czy użytkownik już ocenił ten produkt
            var existingRating = await _context.Ratings
                .FirstOrDefaultAsync(r => r.ArticleId == id && r.UserId == user.Id);

            if (existingRating != null)
            {
                // Użytkownik już ocenił ten produkt, zaktualizuj istniejącą ocenę
                existingRating.Value = value;

            }
            else
            {
                // Dodaj nową ocenę do tabeli Ratings
                var newRating = new Rating
                {
                    ArticleId = id,
                    UserId = user.Id,
                    Value = value
                };

                _context.Ratings.Add(newRating);
            }
            _context.SaveChanges();
            
            
            var query =  _context.Ratings.Where(r => r.ArticleId == id).Select(r => r.Value);
            
            var rates = await query.ToListAsync();
            
            var updatedAverageRating = rates.Any() ? rates.Average() : 1;

            art.Rate = (float)updatedAverageRating;

            await _context.SaveChangesAsync();

            return Ok(art);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<NewsArticle>> PostArticle(NewsArticleDto articledto)
        {

            string name = User.Identity.Name;
            var article = new NewsArticle
            {
                Author = name,
                Title = articledto.Title,
                Category = articledto.Category,
                Content = articledto.Content,
                PublicationDate = DateTime.Now,
                Comments = null,
                Rate = 0,
                Views = 0
            };

            _context.Articles.Add(article);

            var result = await _context.SaveChangesAsync() > 0;

            if (result) return CreatedAtRoute("GetArticle", new { id = article.Id }, article);

            return BadRequest(new ProblemDetails { Title = "Problem creating News Article" });
        }

        [Authorize]
        [HttpPut("Edit")]
        public async Task<ActionResult<NewsArticle>> EditArticle(NewsArticleDto articledto, int id)
        {
            var art = await _context.Articles.FindAsync(id);
            if(art == null) return BadRequest();
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if(user.UserName != art.Author) return BadRequest("You can edit only your own articles");
            art.articleStatus=ArtComStatus.Edytowany;
            art.PublicationDate = DateTime.Now;
            art.Title = articledto.Title;
            art.Category = articledto.Category;
            art.Content = articledto.Content;
            var result = await _context.SaveChangesAsync() > 0;
            if (result) return Ok(art);
            return BadRequest(new ProblemDetails { Title = "Problem editing Article" });
        }

        [Authorize(Roles ="Admin")]
        [HttpDelete("DeleteArticle")]
        public async Task<ActionResult<NewsArticle>> Delete(int id)
        {
            var art = await _context.Articles.FindAsync(id);
            if(art ==  null) return NotFound(); 
            // if(!string.IsNullOrEmpty(art.CloudId))
            //     await _imageService.DeleteImage(art.CloudId);    
            _context.Articles.Remove(art);
            var result = await _context.SaveChangesAsync() >0;
             if(result) return Ok();
             return BadRequest("Failed to delete Article");
        }





    }
}