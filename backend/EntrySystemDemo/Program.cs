using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSingleton<UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

public class LoginController : ControllerBase
{
    private readonly UserService _userService;

    public LoginController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginModel model)
    {
        var user = _userService.Authenticate(model.Username, model.Password);

        if (user == null)
            return Unauthorized("Invalid username or password");

        // 在實際應用中，這裡應該生成並返回一個JWT令牌
        return Ok($"Welcome {user.Username}!");
    }
}

public class UserService
{
    private List<User> _users = new List<User>
    {
        new User { Id = 1, Username = "admin", Password = "password" }
    };

    public User Authenticate(string username, string password)
    {
        return _users.SingleOrDefault(x => x.Username == username && x.Password == password);
    }
}

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}

public class LoginModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}