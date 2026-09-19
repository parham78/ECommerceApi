public static class OrderStatusRules
{
    public static bool CanTransition(
        OrderStatus currentStatus,
        OrderStatus newStatus)
    {
        if (currentStatus == OrderStatus.Pending)
        {
            return newStatus == OrderStatus.Processing;
        }

        if (currentStatus == OrderStatus.Processing)
        {
            return newStatus == OrderStatus.Shipped;
        }

        if (currentStatus == OrderStatus.Shipped)
        {
            return newStatus == OrderStatus.Completed;
        }

        return false;
    }
    public static bool CanCancel(OrderStatus status)
    {
        return status == OrderStatus.Pending;
    }

    public static void EnsureValidTransition(
    OrderStatus currentStatus,
    OrderStatus newStatus)
    {
        if (!CanTransition(currentStatus, newStatus))
        {
            throw new BadRequestException(
                $"Cannot change order status from {currentStatus} to {newStatus}.");
        }
    }
}