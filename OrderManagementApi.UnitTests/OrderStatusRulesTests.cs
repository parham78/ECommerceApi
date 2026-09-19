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

    [Theory]
    [InlineData(OrderStatus.Pending, true)]
    [InlineData(OrderStatus.Processing, false)]
    [InlineData(OrderStatus.Shipped, false)]
    [InlineData(OrderStatus.Completed, false)]
    public void CanCancel_ReturnsExpectedResult(
    OrderStatus status,
    bool expected)
    {
        // Act
        var result = OrderStatusRules.CanCancel(status);

        // Assert
        Assert.Equal(expected, result);
    }


    [Fact]
    public void EnsureValidTransition_PendingToCompleted_ThrowsBadRequestException()
    {
        // Act
        var exception = Assert.Throws<BadRequestException>(() =>
            OrderStatusRules.EnsureValidTransition(
                OrderStatus.Pending,
                OrderStatus.Completed));

        // Assert
        Assert.Contains(
            "Cannot change order status",
            exception.Message);
    }
}