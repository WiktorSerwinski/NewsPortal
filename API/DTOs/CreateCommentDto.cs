using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class CreateCommentDto
    {
        public string Content { get; set; }

        public int ArticleId { get; set; }
    }

}