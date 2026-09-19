public class BasketRulesTests
{
    [Theory]
    [InlineData(99, false)]
    [InlineData(100, false)]
    [InlineData(101, true)]
    public void ExceedsMaximumQuantity_ReturnsExpectedResult(
        int quantity,
        bool expected)
    {
        // Act
        var result =
            BasketRules.ExceedsMaximumQuantity(quantity);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(4, 5, false)]
    [InlineData(5, 5, false)]
    [InlineData(6, 5, true)]
    public void ExceedsStock_ReturnsExpectedResult(
        int quantity,
        int availableStock,
        bool expected)
    {
        // Act
        var result =
            BasketRules.ExceedsStock(
                quantity,
                availableStock);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(70, 20, 90)]
    [InlineData(70, 30, 100)]
    [InlineData(70, 31, 101)]
    public void CalculateNewQuantity_ReturnsExpectedQuantity(
        int existingQuantity,
        int quantityToAdd,
        int expected)
    {
        // Act
        var result =
            BasketRules.CalculateNewQuantity(
                existingQuantity,
                quantityToAdd);

        // Assert
        Assert.Equal(expected, result);
    }
}