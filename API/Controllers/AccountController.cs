using API.Data;
using API.DTOs;
using API.Entities;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<User> _userManager;
        private readonly TokenService _tokenService;
        private readonly Context _context;
        public AccountController(UserManager<User> userManager, TokenService tokenService, Context context)
        {
            _context = context;
            _tokenService = tokenService;
            _userManager = userManager;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UserName);

            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
                return Unauthorized();

            return new UserDto
            {
                Email = user.Email,
                Token = await _tokenService.GenerateToken(user),
            };
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDto registerDto)
        {
            var user = new User
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
            };
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return ValidationProblem();
            }

            await _userManager.AddToRoleAsync(user, "Member");

            return StatusCode(201);
        }
        [Authorize]
        [HttpGet("currentUser")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            return new UserDto
            {
                Email = user.Email,
                Token = await _tokenService.GenerateToken(user),
            };

        }

        

        [Authorize(Roles = "Admin")]
        [HttpDelete("removeUserByEmail/{email}")]
        public async Task<ActionResult> RemoveUserByEmail(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    return NotFound($"Nie znaleziono użytkownika o adresie e-mail: {email}");
                }

                var result = await _userManager.DeleteAsync(user);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return ValidationProblem();
                }

                return Ok($"Użytkownik o adresie e-mail {email} został pomyślnie usunięty.");
            }
            catch (Exception ex)
            {
                // Obsługa błędów, logowanie itp.
                return StatusCode(500, $"Wystąpił błąd podczas usuwania użytkownika: {ex.Message}");
            }
        }
    }
}