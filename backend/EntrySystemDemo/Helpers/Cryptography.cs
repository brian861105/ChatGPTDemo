
using BCryptHelper = BCrypt.Net.BCrypt;

namespace EntrySystemDemo;

public static class Cryptography
{

    public static string Encrypto(string passoword)
    {
        return Hash(passoword);
    }

    private static string Hash(string Plaintext)
    {
        string Salt = BCryptHelper.GenerateSalt();
        return BCryptHelper.HashPassword(Plaintext, Salt);
    }

    public static bool VerifyHash(string Text, string HashedText)
    {
        return BCryptHelper.Verify(Text, HashedText);
    }
}
