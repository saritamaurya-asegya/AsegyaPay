using AsegyaPay.AuthService.Application.Interfaces;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace AsegyaPay.AuthService.Infrastructure.Services;

/// <summary>
/// Implements password hashing using PBKDF2 with HMAC-SHA512.
/// Follows OWASP guidelines for password storage.
/// </summary>
public sealed class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100_000;
    private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA512;

    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithm, KeySize);
        return $"{Convert.ToHexString(salt)}.{Convert.ToHexString(hash)}.{Iterations}";
    }

    public bool Verify(string password, string hash)
    {
        var parts = hash.Split('.');
        if (parts.Length != 3) return false;

        try
        {
            var salt = Convert.FromHexString(parts[0]);
            var storedHash = Convert.FromHexString(parts[1]);
            var iterations = int.Parse(parts[2]);

            var computedHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithm, KeySize);
            return CryptographicOperations.FixedTimeEquals(storedHash, computedHash);
        }
        catch
        {
            return false;
        }
    }
}
