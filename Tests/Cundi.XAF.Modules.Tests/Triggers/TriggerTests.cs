using Cundi.XAF.Triggers.BusinessObjects;

namespace Cundi.XAF.Modules.Tests.Triggers;

/// <summary>
/// Unit tests for TriggerEventType enum.
/// </summary>
public class TriggerEventTypeTests
{
    [Fact]
    public void TriggerEventType_ShouldHaveCorrectValues()
    {
        // Assert
        Assert.Equal(0, (int)TriggerEventType.Created);
        Assert.Equal(1, (int)TriggerEventType.Modified);
        Assert.Equal(2, (int)TriggerEventType.Deleted);
    }

    [Theory]
    [InlineData(TriggerEventType.Created, "Created")]
    [InlineData(TriggerEventType.Modified, "Modified")]
    [InlineData(TriggerEventType.Deleted, "Deleted")]
    public void TriggerEventType_ShouldHaveCorrectNames(TriggerEventType eventType, string expectedName)
    {
        // Assert
        Assert.Equal(expectedName, eventType.ToString());
    }

    [Fact]
    public void TriggerEventType_ShouldHaveThreeValues()
    {
        // Arrange
        var values = Enum.GetValues<TriggerEventType>();

        // Assert
        Assert.Equal(3, values.Length);
    }

    [Fact]
    public void TriggerEventType_Created_ShouldBeZero()
    {
        // This ensures the enum starts at 0 for compatibility
        Assert.Equal(0, (int)TriggerEventType.Created);
    }

    [Fact]
    public void AllTriggerEventTypes_ShouldBeDistinct()
    {
        // Arrange
        var values = Enum.GetValues<TriggerEventType>();

        // Assert
        Assert.Equal(values.Length, values.Distinct().Count());
    }
}
