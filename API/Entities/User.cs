
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    public class User : IdentityUser<int>
    {
       public DateTime CreatedAt { get; set; } = DateTime.Now;
       public string PictureUrl { get; set; }
    
    }
}