using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;


namespace Application.Controllers
{
    public class ProfileController(NoteifyContext context, IConfiguration configuration) : Controller
    {
        private readonly NoteifyContext _context = context;
        private readonly IConfiguration _configuration = configuration;

        public IActionResult Index()
        => View();

        public IActionResult SignUp()
        => View();

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Important data is missing.";
                return View(model);
            }

            if (model.Password != model.PasswordConfirmation)
            {
                ViewBag.Error = "Passwords don't match.";
                return View(model);
            }

            var existingUser = await _context.Customers
                .FirstOrDefaultAsync(user => user.Email == model.Email || user.Username == model.Username);

            if (existingUser != null)
            {
                ViewBag.Error = "The email or username already exists.";
                return View(model);
            }

            Customer customer = new()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Username = model.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password)
            };

            await _context.AddAsync(customer);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Login()
            => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Important data is missing.";
                return View(model);
            }

            var existingUser = await _context.Customers
                .FirstOrDefaultAsync(user => user.Username == model.UsernameOrEmail || user.Email == model.UsernameOrEmail);

            if (existingUser == null)
            {
                ViewBag.Error = "Invalid login attempt.";
                return View(model);
            }

            bool validPass = BCrypt.Net.BCrypt.Verify(model.Password, existingUser.PasswordHash);

            if (!validPass)
            {
                ViewBag.Error = "Invalid login attempt.";
                return View(model);
            }

            var jwt = _configuration.GetSection("Jwt").Get<Jwt>();

            if (jwt == null)
            {
                ViewBag.Error = "Internal error.";
                return View(model);
            }
            // Todo lo que va a encapsular el token
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, existingUser.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim("id", existingUser.Id.ToString()),
                new Claim("first_name", existingUser.FirstName),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));
            var signin = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                jwt.Issuer,
                jwt.Audience,
                claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: signin
            );

            ViewBag.Success = new JwtSecurityTokenHandler().WriteToken(token);
            return RedirectToAction(nameof(Index));
        }
    }
}