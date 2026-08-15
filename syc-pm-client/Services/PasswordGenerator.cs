using System.Security.Cryptography;
using System.Text;

namespace syc_pm_client.Services
{
    public static class PasswordGenerator
    {
        public static string GeneratePassword(int length = 16)
        {
            const string lowercase = "abcdefghijklmnopqrstuvwxyz";
            const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "0123456789";
            const string symbols = "!@#$%^&*()_+-=[]{}|;:,.<>?";
            const string allChars = lowercase + uppercase + digits + symbols;

            var res = new StringBuilder();

            res.Append(lowercase[RandomNumberGenerator.GetInt32(lowercase.Length)]);
            res.Append(uppercase[RandomNumberGenerator.GetInt32(uppercase.Length)]);
            res.Append(digits[RandomNumberGenerator.GetInt32(digits.Length)]);
            res.Append(symbols[RandomNumberGenerator.GetInt32(symbols.Length)]);

            for (int i = 4; i < length; i++)
            {
                res.Append(allChars[RandomNumberGenerator.GetInt32(allChars.Length)]);
            }

            char[] passwordBuffer = res.ToString().ToCharArray();
            for (int i = passwordBuffer.Length - 1; i > 0; i--)
            {
                int j = RandomNumberGenerator.GetInt32(i + 1);
                char temp = passwordBuffer[i];
                passwordBuffer[i] = passwordBuffer[j];
                passwordBuffer[j] = temp;
            }

            return new string(passwordBuffer);
        }
    }
}