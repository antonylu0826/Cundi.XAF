#nullable enable
using Cundi.XAF.ApiKey.BusinessObjects;
using DevExpress.ExpressApp;

namespace Cundi.XAF.ApiKey.Services;

/// <summary>
/// Service for validating API keys against the database.
/// </summary>
public class ApiKeyValidator
{
    /// <summary>
    /// Result of API key validation.
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public Guid? UserOid { get; set; }
        public string? ErrorMessage { get; set; }
        public ApiKeyInfo? ApiKeyInfo { get; set; }

        public static ValidationResult Success(Guid userOid, ApiKeyInfo apiKeyInfo) => new()
        {
            IsValid = true,
            UserOid = userOid,
            ApiKeyInfo = apiKeyInfo
        };

        public static ValidationResult Failure(string errorMessage) => new()
        {
            IsValid = false,
            ErrorMessage = errorMessage
        };
    }

    /// <summary>
    /// Validates an API key and returns the associated user's Oid if valid.
    /// </summary>
    /// <param name="objectSpace">The object space to use for database queries</param>
    /// <param name="plaintextKey">The plaintext API key to validate</param>
    /// <param name="updateLastUsed">Whether to update the LastUsedAt timestamp</param>
    /// <returns>Validation result containing user Oid if valid</returns>
    public ValidationResult Validate(IObjectSpace objectSpace, string plaintextKey, bool updateLastUsed = true)
    {
        // Check format
        if (!ApiKeyGenerator.IsValidFormat(plaintextKey))
        {
            return ValidationResult.Failure("Invalid API key format.");
        }

        // Calculate hash
        string hash = ApiKeyGenerator.ComputeHash(plaintextKey);

        // Find in database
        var apiKeyInfo = objectSpace.FindObject<ApiKeyInfo>(
            DevExpress.Data.Filtering.CriteriaOperator.Parse("ApiKeyHash = ?", hash));

        if (apiKeyInfo == null)
        {
            return ValidationResult.Failure("API key not found.");
        }

        // Check if active
        if (!apiKeyInfo.IsActive)
        {
            return ValidationResult.Failure("API key is inactive.");
        }

        // Check expiration
        if (apiKeyInfo.ExpiresAt.HasValue && apiKeyInfo.ExpiresAt.Value < DateTime.UtcNow)
        {
            return ValidationResult.Failure("API key has expired.");
        }

        // Update last used timestamp
        if (updateLastUsed)
        {
            apiKeyInfo.LastUsedAt = DateTime.UtcNow;
            objectSpace.CommitChanges();
        }

        return ValidationResult.Success(apiKeyInfo.UserOid, apiKeyInfo);
    }

    /// <summary>
    /// Validates an API key without updating the last used timestamp.
    /// Useful for read-only validation scenarios.
    /// </summary>
    public ValidationResult ValidateReadOnly(IObjectSpace objectSpace, string plaintextKey)
    {
        return Validate(objectSpace, plaintextKey, updateLastUsed: false);
    }
}
