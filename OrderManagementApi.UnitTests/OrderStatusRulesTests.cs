public class OrderStatusRulesTests
{
    [Theory]


    [InlineData(OrderStatus.Pending, OrderStatus.Processing, true)]
    [InlineData(OrderStatus.Processing, OrderStatus.Shipped, true)]
    [InlineData(OrderStatus.Shipped, OrderStatus.Completed, true)]
    [InlineData(OrderStatus.Pending, OrderStatus.Completed, false)]
    [InlineData(OrderStatus.Completed, OrderStatus.Processing, false)]
    [InlineData(OrderStatus.Pending, OrderStatus.Shipped, false)]
    public void CanTransition_ReturnsExpectedResult(
     OrderStatus currentStatus,
     OrderStatus newStatus,
     bool expected)
    {
        // Act
        var result = OrderStatusRules.CanTransition(
            currentStatus,
            newStatus);

        // Assert
        Assert.Equal(expected, result);
    }
}