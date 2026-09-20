using ECommerce.Domain.Exceptions;

namespace ECommerce.Domain.ValueObjects;

public sealed class OrderStatus : IEquatable<OrderStatus>
{
    public static readonly OrderStatus Pending = new(nameof(Pending));
    public static readonly OrderStatus Confirmed = new(nameof(Confirmed));
    public static readonly OrderStatus Shipped = new(nameof(Shipped));
    public static readonly OrderStatus Delivered = new(nameof(Delivered));
    public static readonly OrderStatus Cancelled = new(nameof(Cancelled));

    public string Value { get; }

    private OrderStatus(string value) => Value = value;

    private static readonly Dictionary<string, OrderStatus> All = new()
    {
        [Pending.Value] = Pending,
        [Confirmed.Value] = Confirmed,
        [Shipped.Value] = Shipped,
        [Delivered.Value] = Delivered,
        [Cancelled.Value] = Cancelled,
    };

    private static readonly Dictionary<string, string[]> AllowedTransitions = new()
    {
        [Pending.Value] = new[] { Confirmed.Value, Cancelled.Value },
        [Confirmed.Value] = new[] { Shipped.Value, Cancelled.Value },
        [Shipped.Value] = new[] { Delivered.Value },
        [Delivered.Value] = Array.Empty<string>(),
        [Cancelled.Value] = Array.Empty<string>(),
    };

    public static OrderStatus FromString(string value)
    {
        if (!All.TryGetValue(value, out var status))
            throw new DomainException($"'{value}' is not a valid order status.");
        return status;
    }

    public OrderStatus TransitionTo(OrderStatus next)
    {
        if (!AllowedTransitions[Value].Contains(next.Value))
            throw new DomainException($"Cannot transition from '{Value}' to '{next.Value}'.");
        return next;
    }

    public bool Equals(OrderStatus? other) => other is not null && Value == other.Value;
    public override bool Equals(object? obj) => Equals(obj as OrderStatus);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value;
}