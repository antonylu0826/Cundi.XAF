using Cundi.XAF.ApiKey.Services;

namespace Cundi.XAF.Modules.Tests.ApiKey;

/// <summary>
/// Unit tests for ApiKeyGenerator service.
/// </summary>
public class ApiKeyGeneratorTests
{
    [Fact]
    public void Generate_ShouldReturnKeyWithCorrectPrefix()
    {
        // Act
        var (plaintextKey, _) = ApiKeyGenerator.Generate();

        // Assert
        Assert.StartsWith("cak_", plaintextKey);
    }

    [Fact]
    public void Generate_ShouldReturnDifferentKeysEachTime()
    {
        // Act
        var (key1, _) = ApiKeyGenerator.Generate();
        var (key2, _) = ApiKeyGenerator.Generate();

        // Assert
        Assert.NotEqual(key1, key2);
    }

    [Fact]
    public void Generate_ShouldReturnNonEmptyHash()
    {
        // Act
        var (_, hashValue) = ApiKeyGenerator.Generate();

        // Assert
        Assert.NotNull(hashValue);
        Assert.NotEmpty(hashValue);
        Assert.Equal(64, hashValue.Length); // SHA256 = 64 hex characters
    }

    [Fact]
    public void ComputeHash_ShouldReturnConsistentHash()
    {
        // Arrange
        var testKey = "cak_testkey123";

        // Act
        var hash1 = ApiKeyGenerator.ComputeHash(testKey);
        var hash2 = ApiKeyGenerator.ComputeHash(testKey);

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void ComputeHash_ShouldReturnDifferentHashForDifferentKeys()
    {
        // Arrange
        var key1 = "cak_key1";
        var key2 = "cak_key2";

        // Act
        var hash1 = ApiKeyGenerator.ComputeHash(key1);
        var hash2 = ApiKeyGenerator.ComputeHash(key2);

        // Assert
        Assert.NotEqual(hash1, hash2);
    }

    [Theory]
    [InlineData("cak_validkey123456789012345", true)]   // Valid format
    [InlineData("cak_short", false)]                     // Too short
    [InlineData("invalid_key", false)]                   // Wrong prefix
    [InlineData("", false)]                              // Empty
    [InlineData(null, false)]                            // Null
    public void IsValidFormat_ShouldValidateCorrectly(string? apiKey, bool expectedResult)
    {
        // Act
        var result = ApiKeyGenerator.IsValidFormat(apiKey!);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void Generate_KeyShouldPassFormatValidation()
    {
        // Act
        var (plaintextKey, _) = ApiKeyGenerator.Generate();

        // Assert
        Assert.True(ApiKeyGenerator.IsValidFormat(plaintextKey));
    }

    [Fact]
    public void ComputeHash_ShouldReturnLowercaseHex()
    {
        // Arrange
        var testKey = "cak_testkey";

        // Act
        var hash = ApiKeyGenerator.ComputeHash(testKey);

        // Assert
        Assert.Equal(hash, hash.ToLowerInvariant());
        Assert.Matches("^[a-f0-9]+$", hash);
    }
}
