using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class CommentController : BaseApiController
    {
        private readonly Context _context;
        private readonly UserManager<User> _userManager;

        public CommentController(Context context, UserManager<User> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet("ArticleComments")]
        public async Task<ActionResult<List<Comment>>> GetCommentsArticle(int articleid)
        {
            var comments = await _context.Comments.Where(c => c.ArticleId == articleid).ToListAsync();
            return new OkObjectResult(comments);
        }
        [HttpGet("UserComments")]
        public async Task<ActionResult<List<Comment>>> GetUserComments(string username)
        {
            // var user = await _userManager.FindByNameAsync(username);

            // if (user == null) return NotFound();

            var comments = await _context.Comments.Where(c => c.Author == username).ToListAsync();

            return new OkObjectResult(comments);
        }

        [HttpGet("{id}", Name = "GetComment")]
        public async Task<ActionResult<Comment>> GetNewsArticle(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            await _context.SaveChangesAsync();
            return comment;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<NewsArticle>> PostArticle(CreateCommentDto commentDto)
        {
            var username = User.Identity.Name;

            var art = await _context.Articles.FindAsync(commentDto.ArticleId);


            if (username == null) return NotFound();

            var comment = new Comment
            {
                Author = username,
                Content = commentDto.Content,
                CreatedAt = DateTime.Now,
                Downvotes = 0,
                Upvotes = 0,
                ArticleId = commentDto.ArticleId
            };

            _context.Comments.Add(comment);
            art.Comments.Add(comment);

            var result = await _context.SaveChangesAsync() > 0;

            if (result) return CreatedAtRoute("GetComment", new { id = comment.Id }, comment);

            return BadRequest(new ProblemDetails { Title = "Problem creating News Comment" });
        }


    }
}