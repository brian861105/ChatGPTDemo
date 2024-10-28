using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Core.Services;

public class RandomStringGenerator
{
    private static readonly Random random = new Random();
    private const string LowercaseLetters = "abcdefghijklmnopqrstuvwxyz";
    private const string UppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Numbers = "0123456789";

    public static string GenerateRandomString(int length)
    {
        if (length < 0)
        {
            throw new ArgumentException("length must be positive");
        }
        string allCharacters = LowercaseLetters + UppercaseLetters + Numbers;
        char[] result = new char[length];

        for (int i = 0; i < length; i++)
        {
            result[i] = allCharacters[random.Next(allCharacters.Length)];
        }

        return new string(result);
    }

    // Overload that allows specifying which character types to include
    public static string GenerateRandomString(int length, bool includeLowercase = true,
        bool includeUppercase = true, bool includeNumbers = true)
    {
        string characters = "";
        if (includeLowercase) characters += LowercaseLetters;
        if (includeUppercase) characters += UppercaseLetters;
        if (includeNumbers) characters += Numbers;

        if (string.IsNullOrEmpty(characters))
            throw new ArgumentException("At least one character type must be included");

        char[] result = new char[length];
        for (int i = 0; i < length; i++)
        {
            result[i] = characters[random.Next(characters.Length)];
        }

        return new string(result);
    }
}