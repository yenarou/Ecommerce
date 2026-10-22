namespace ECommerce.Domain.Models;

public static class PurchaseOrderStatus
{
    public const string Pending = "Pending";
    public const string Received = "Received";
    public const string Cancelled = "Cancelled";
}

public class PurchaseOrderItem
{
    private PurchaseOrderItem()
    {
    }

    internal PurchaseOrderItem(Guid productId, string productName, int quantity, decimal unitCost)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        UnitCost = unitCost;
    }

    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitCost { get; private set; }
}

public class PurchaseOrder
{
    private PurchaseOrder()
    {
    }

    private PurchaseOrder(Guid supplierId)
    {
        Id = Guid.NewGuid();
        SupplierId = supplierId;
        Status = PurchaseOrderStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid SupplierId { get; private set; }
    public string Status { get; private set; } = PurchaseOrderStatus.Pending;
    public DateTime CreatedAt { get; private set; }
    public DateTime? ReceivedAt { get; private set; }
    public ICollection<PurchaseOrderItem> Items { get; private set; } = new List<PurchaseOrderItem>();

    public decimal Total => Items.Sum(i => i.Quantity * i.UnitCost);

    public static PurchaseOrder Create(Guid supplierId)
    {
        if (supplierId == Guid.Empty)
            throw new ArgumentException("Supplier id cannot be empty.", nameof(supplierId));

        return new PurchaseOrder(supplierId);
    }

    public void AddItem(Guid productId, string productName, int quantity, decimal unitCost)
    {
        if (Status != PurchaseOrderStatus.Pending)
            throw new InvalidOperationException($"Cannot add items to a purchase order with status '{Status}'.");

        if (productId == Guid.Empty)
            throw new ArgumentException("Product id cannot be empty.", nameof(productId));

        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Product name cannot be null or empty.", nameof(productName));

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

        if (unitCost < 0)
            throw new ArgumentException("Unit cost cannot be negative.", nameof(unitCost));

        Items.Add(new PurchaseOrderItem(productId, productName, quantity, unitCost));
    }

    public void MarkAsReceived()
    {
        if (Status != PurchaseOrderStatus.Pending)
            throw new InvalidOperationException($"Cannot receive a purchase order with status '{Status}'.");

        if (!Items.Any())
            throw new InvalidOperationException("Cannot receive a purchase order without items.");

        Status = PurchaseOrderStatus.Received;
        ReceivedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status != PurchaseOrderStatus.Pending)
            throw new InvalidOperationException($"Cannot cancel a purchase order with status '{Status}'.");

        Status = PurchaseOrderStatus.Cancelled;
    }
}