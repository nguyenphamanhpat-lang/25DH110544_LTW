using System;
using System.Security.Cryptography;

namespace MyStoreMVC5.Security
{
    // 16-byte salt + 20-byte PBKDF2 subkey => exactly 48 base64 chars.
    // Fits the NCHAR(50) Password field in the supplied database.
    public static class PasswordHasher
    {
        private const int Iterations = 100000;
        private const int SaltBytes = 16, HashBytes = 20;

        public static string Hash(string password)
        {
            if (string.IsNullOrEmpty(password)) throw new ArgumentException("Password required");
            var salt = new byte[SaltBytes];
            using (var rng = RandomNumberGenerator.Create()) rng.GetBytes(salt);
            byte[] hash;
            using (var pbkdf = new Rfc2898DeriveBytes(password, salt, Iterations)) hash = pbkdf.GetBytes(HashBytes);
            var data = new byte[SaltBytes + HashBytes];
            Buffer.BlockCopy(salt, 0, data, 0, SaltBytes);
            Buffer.BlockCopy(hash, 0, data, SaltBytes, HashBytes);
            return Convert.ToBase64String(data);
        }

        public static bool Verify(string password, string stored)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(stored)) return false;
            byte[] data;
            try { data = Convert.FromBase64String(stored.Trim()); }
            catch (FormatException) { return false; }
            if (data.Length != SaltBytes + HashBytes) return false;
            var salt = new byte[SaltBytes];
            Buffer.BlockCopy(data, 0, salt, 0, SaltBytes);
            byte[] actual;
            using (var pbkdf = new Rfc2898DeriveBytes(password, salt, Iterations)) actual = pbkdf.GetBytes(HashBytes);
            var diff = 0;
            for (var i = 0; i < HashBytes; i++) diff |= (actual[i] ^ data[SaltBytes + i]);
            return diff == 0;
        }
    }
}
