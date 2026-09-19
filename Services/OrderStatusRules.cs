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
}