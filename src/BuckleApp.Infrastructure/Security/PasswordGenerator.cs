using BuckleApp.Infrastructure.Interfaces;

namespace BuckleApp.Infrastructure.Security;

public class PasswordGenerator : IPasswordGenerator
{
    public string GenerateRandomPassword()
    {
        var options = new 
        {
            RequiredLength = 12,
            RequiredUniqueChars = 4,
            RequireDigit = true,
            RequireLowercase = true,
            RequireNonAlphanumeric = true,
            RequireUppercase = true
        };

        string[] randomChars = new[] {
            "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // Uppercase letters
            "abcdefghijkmnopqrstuvwxyz",    // Lowercase letters
            "0123456789",                   // Digits
            "!@$?_-."                       // Special characters
        };

        Random rand = new Random();
        List<char> chars = new List<char>();

        if (options.RequireUppercase)
            chars.Add(randomChars[0][rand.Next(randomChars[0].Length)]);
    
        if (options.RequireLowercase)
            chars.Add(randomChars[1][rand.Next(randomChars[1].Length)]);
    
        if (options.RequireDigit)
            chars.Add(randomChars[2][rand.Next(randomChars[2].Length)]);
    
        if (options.RequireNonAlphanumeric)
            chars.Add(randomChars[3][rand.Next(randomChars[3].Length)]);

        while (chars.Count < options.RequiredLength)
        {
            string randomCategory = randomChars[rand.Next(randomChars.Length)];
            chars.Add(randomCategory[rand.Next(randomCategory.Length)]);
        }

        return new string(chars.OrderBy(x => rand.Next()).ToArray());
    }

}