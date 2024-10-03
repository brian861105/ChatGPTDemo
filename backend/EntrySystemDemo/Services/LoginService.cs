using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EntrySystemDemo.Services;

public class LoginService : ILoginService
{
    public static char[] SecretKey { get; set; }

    public async Task<bool> AuthenticateAsync(string username, string password)
    {
        throw new NotImplementedException();
    }

    public async Task<string> GenerateJwtTokenAsync(string username)
    {
        throw new NotImplementedException();
    }
}
