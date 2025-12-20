using Cundi.XAF.FullTextSearch.Attributes;

namespace Cundi.XAF.Modules.Tests.FullTextSearch;

/// <summary>
/// Unit tests for GlobalSearchableAttribute.
/// </summary>
public class GlobalSearchableAttributeTests
{
    [Fact]
    public void DefaultConstructor_ShouldSetIsSearchableToTrue()
    {
        // Act
        var attr = new GlobalSearchableAttribute();

        // Assert
        Assert.True(attr.IsSearchable);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Constructor_WithParameter_ShouldSetIsSearchable(bool value)
    {
        // Act
        var attr = new GlobalSearchableAttribute(value);

        // Assert
        Assert.Equal(value, attr.IsSearchable);
    }

    [Fact]
    public void Attribute_ShouldBeApplicableToClass()
    {
        // Arrange
        var attrUsage = typeof(GlobalSearchableAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), false)
            .FirstOrDefault() as AttributeUsageAttribute;

        // Assert
        Assert.NotNull(attrUsage);
        Assert.True(attrUsage.ValidOn.HasFlag(AttributeTargets.Class));
    }
}

/// <summary>
/// Unit tests for GlobalSearchablePropertyAttribute.
/// </summary>
public class GlobalSearchablePropertyAttributeTests
{
    [Fact]
    public void DefaultConstructor_ShouldSetIsSearchableToTrue()
    {
        // Act
        var attr = new GlobalSearchablePropertyAttribute();

        // Assert
        Assert.True(attr.IsSearchable);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Constructor_WithParameter_ShouldSetIsSearchable(bool value)
    {
        // Act
        var attr = new GlobalSearchablePropertyAttribute(value);

        // Assert
        Assert.Equal(value, attr.IsSearchable);
    }

    [Fact]
    public void Attribute_ShouldBeApplicableToProperty()
    {
        // Arrange
        var attrUsage = typeof(GlobalSearchablePropertyAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), false)
            .FirstOrDefault() as AttributeUsageAttribute;

        // Assert
        Assert.NotNull(attrUsage);
        Assert.True(attrUsage.ValidOn.HasFlag(AttributeTargets.Property));
    }
}
