using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyAPI.Entities;
using MyAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyAPI.Controllers
{
	[ApiController]
	[Route("auth")]
	public class AuthController : ControllerBase
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly string _jwtSecret;

		public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_roleManager = roleManager;
			_jwtSecret = configuration["Jwt:Key"];
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register(RegisterModel model)
		{
			await CreateRole(model.Role);

			var user = new ApplicationUser
			{
				Id = Guid.NewGuid().ToString(),
				UserName = model.FirstName + model.MiddleName + model.LastName,
				Email = model.Email,
				FirstName = model.FirstName,
				MiddleName = model.MiddleName,
				LastName = model.LastName,
				Province = model.Province,
				City = model.City,
				Address = model.Address,
				DocumentId = model.DocumentId,
			};

			var result = await _userManager.CreateAsync(user, model.Password);
			if (result.Succeeded)
			{
				await _userManager.AddToRoleAsync(user, model.Role.ToString());
				return Ok(new { Message = "Usuario registrado exitosamente." });
			}

			return BadRequest(result.Errors);
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginModel model)
		{
			var user = await _userManager.FindByEmailAsync(model.Email);
			if (user == null)
			{
				return Unauthorized(new { Message = "Credenciales inválidas." });
			}

			var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
			if (result.Succeeded)
			{
				var token = GenerateJwtToken(user);
				return Ok(new { Token = token });
			}

			return Unauthorized(new { Message = "Credenciales inválidas." });
		}

		private string GenerateJwtToken(ApplicationUser user)
		{
			var claims = new[]
			{
				new Claim("UserName", user.UserName),
				new Claim("FirstName", user.FirstName),
				new Claim("MiddleName", user.MiddleName),
				new Claim("LastName", user.LastName),
				new Claim("Province", user.Province),
				new Claim("City", user.City),
				new Claim("Address", user.Address),
				new Claim("DocumentId", user.DocumentId),
				new Claim("Role", _userManager.GetRolesAsync(user).Result.FirstOrDefault().ToString())
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken
			(
				issuer: null,
				audience: null,
				claims: claims,
				expires: DateTime.Now.AddDays(30),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		private async Task<IActionResult> CreateRole(string roleName)
		{
			if (string.IsNullOrEmpty(roleName))
				return BadRequest("Role name cannot be empty");

			var roleExists = await _roleManager.RoleExistsAsync(roleName);

			if (roleExists)
				return Conflict("Role already exists");

			var result = await _roleManager.CreateAsync(new IdentityRole(roleName));

			if (result.Succeeded)
				return Ok("Role created successfully");

			return BadRequest(result.Errors);
		}
	}
}