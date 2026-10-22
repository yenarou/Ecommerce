namespace ECommerce.Domain.Models;

public static class StockMovementReason {
    public const string Initial = "Initial";
    public const string Sale = "Sale";
    public const string Purchase = "Purchase";
    public const string Adjustment = "Adjustment";
    public const string Cancellation = "Cancellation";
}

public class StockMovement {

    private StockMovement(){

    }

    private StockMovement(Guid productId, string productName, int quantity, string reason, Guid? referenceId, string createdBy){
        Id = Guid.NewGuid();
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        Reason = reason;
        ReferenceId = referenceId;
        CreatedBy = createdBy;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public Guid? ReferenceId { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    public static StockMovement Create(Guid productId, string productName, int quantity, string reason, Guid? referenceId = null, string createdBy = "system"){
        if (productId == Guid.Empty)
            throw new ArgumentException("Product id cannot be empty.", nameof(productId));

        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Product name cannot be null or empty.", nameof(productName));

        if (quantity == 0)
            throw new ArgumentException("Quantity cannot be zero.", nameof(quantity));

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Reason cannot be null or empty.", nameof(reason));

        return new StockMovement(productId, productName, quantity, reason, referenceId, createdBy);
    }
}