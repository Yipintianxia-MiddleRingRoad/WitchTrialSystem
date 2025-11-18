using System.Security.Cryptography;
using System.Text;

namespace WitchTrialSystem.BLL
{
    public static class Security
    {
        public static string CreateSalt(int bytes = 16)
        {
            var buf = new byte[bytes];
            RandomNumberGenerator.Fill(buf);
            return Convert.ToHexString(buf);
        }

        public static string Sha256Hex(string text)
        {
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(text));
            return Convert.ToHexString(hash);
        }

        public static (string Salt, string Hash) HashPassword(string password)
        {
            var salt = CreateSalt();
            var hash = Sha256Hex(password + salt);
            return (salt, hash);
        }

        public static bool Verify(string password, string salt, string hash)
            => Sha256Hex(password + salt).Equals(hash, StringComparison.OrdinalIgnoreCase);
    }
}
