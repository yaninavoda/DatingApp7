using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API;

public class AccountController : BaseApiController
{
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;

    public AccountController(DataContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    [HttpPost("register")] // POST: /api/account/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await UserExists(registerDto))
        {
            return BadRequest("Username already taken");
        }
        using var hmac = new HMACSHA512();
        var newUser = new AppUser
        {
            UserName = registerDto.UserName.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        return new UserDto
        {
            Username = newUser.UserName,
            Token = _tokenService.CreateToken(newUser)
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await _context.Users.SingleOrDefaultAsync(user => 
        user.UserName == loginDto.UserName);

        if (user is null)
        {
            return Unauthorized("User name is incorrect");
        }
        if (IsPasswordCorrect(user, loginDto.Password))
        {
            return new UserDto
        {
            Username = user.UserName,
            Token = _tokenService.CreateToken(user)
        };
        }
        else return Unauthorized("Wrong password");
    }

    private bool IsPasswordCorrect(AppUser user, string password)
    {
        using var hmac = new HMACSHA512(user.PasswordSalt);
        var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        bool result = passwordHash.SequenceEqual(user.PasswordHash);
        return result;
    }

    private async Task<bool> UserExists(RegisterDto registerDto)
    {
        return await _context.Users
        .AnyAsync(user => user.UserName == registerDto.UserName.ToLower());
    }
}
