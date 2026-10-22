namespace ECommerce.Domain.Models;

public class Supplier {
    private Supplier(){
    }

    private Supplier(string name, string? contactName, string? email, string? phone){
        Id = Guid.NewGuid();
        Name = name;
        ContactName = contactName;
        Email = email;
        Phone = phone;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? ContactName { get; private set; }
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static Supplier Create(string name, string? contactName = null, string? email = null, string? phone = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Supplier name cannot be null or empty.", nameof(name));

        if (name.Length > 150)
            throw new ArgumentException("Supplier name cannot exceed 150 characters.", nameof(name));

        return new Supplier(name, contactName, email, phone);
    }

    public void Update(string name, string? contactName, string? email, string? phone)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Supplier name cannot be null or empty.", nameof(name));

        if (name.Length > 150)
            throw new ArgumentException("Supplier name cannot exceed 150 characters.", nameof(name));

        Name = name;
        ContactName = contactName;
        Email = email;
        Phone = phone;
    }

    public void SetActive(bool isActive)
    {
        IsActive = isActive;
    }
}