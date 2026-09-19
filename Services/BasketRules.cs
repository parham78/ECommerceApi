public static class BasketRules
{
    public const int MaxItemQuantity = 100;

    public static int CalculateNewQuantity(
        int existingQuantity,
        int quantityToAdd)
    {
        return existingQuantity + quantityToAdd;
    }

    public static bool ExceedsMaximumQuantity(
        int quantity)
    {
        return quantity > MaxItemQuantity;
    }

    public static bool ExceedsStock(
        int quantity,
        int availableStock)
    {
        return quantity > availableStock;
    }
}