using System.Security.Cryptography;
using System.Text;

namespace Cundi.XAF.ApiKey.Services;

/// <summary>
/// Service for generating secure API keys.
/// </summary>
public static class ApiKeyGenerator
{
    private const string Prefix = "cak"; // Cundi API Key
    private const int KeyLengthBytes = 32; // 256 bits

    /// <summary>
    /// Generates a new high-strength API key.
    /// Returns a tuple containing the plaintext key (to show to user) and its hash (to store in database).
    /// </summary>
    /// <returns>A tuple of (PlaintextKey, HashValue)</returns>
    public static (string PlaintextKey, string HashValue) Generate()
    {
        // Generate random bytes
        byte[] randomBytes = RandomNumberGenerator.GetBytes(KeyLengthBytes);

        // Convert to Base64 and make URL-safe
        string base64Key = Convert.ToBase64String(randomBytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');

        // Create the full key with prefix
        string plaintextKey = $"{Prefix}_{base64Key}";

        // Calculate hash for storage
        string hashValue = ComputeHash(plaintextKey);

        return (plaintextKey, hashValue);
    }

    /// <summary>
    /// Computes the SHA256 hash of an API key.
    /// </summary>
    /// <param name="plaintextKey">The plaintext API key</param>
    /// <returns>The SHA256 hash as a hex string</returns>
    public static string ComputeHash(string plaintextKey)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(plaintextKey);
        byte[] hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    /// <summary>
    /// Validates the format of an API key.
    /// </summary>
    /// <param name="apiKey">The API key to validate</param>
    /// <returns>True if the format is valid</returns>
    public static bool IsValidFormat(string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            return false;

        // Check prefix
        if (!apiKey.StartsWith($"{Prefix}_"))
            return false;

        // Check minimum length (prefix + underscore + base64)
        if (apiKey.Length < Prefix.Length + 1 + 20)
            return false;

        return true;
    }
}
