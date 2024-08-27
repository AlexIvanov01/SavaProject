using System;

namespace CustomerService.Dtos;

public class CustomerPublishedDto
{
    public Guid ExternalId { get; set; }
    public string? Name { get; set; }
    public string? CompanyName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? BankName { get; set; }
    public string? IBAN { get; set; }
    public string? BIC { get; set; }
    public string? VATNumber { get; set; }
    public string? UIC { get; set; }
    public string Event { get; set; } = string.Empty;
}
