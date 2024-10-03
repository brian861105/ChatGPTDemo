// using EntrySystemDemo.Services;
// using Microsoft.AspNetCore.Mvc;

// namespace EntrySystemDemo;
// public class LoginController : ControllerBase
// {
//     private readonly UserService _userService;

//     public LoginController(UserService userService)
//     {
//         _userService = userService;
//     }

//     [HttpPost("login")]
//     public IActionResult Login([FromBody] LoginModel model)
//     {
//         var user = _userService.Authenticate(model.Username, model.Password);

//         if (user == null)
//             return Unauthorized("Invalid username or password");

//         // 在實際應用中，這裡應該生成並返回一個JWT令牌
//         return Ok($"Welcome {user.Username}!");
//     }
// }

