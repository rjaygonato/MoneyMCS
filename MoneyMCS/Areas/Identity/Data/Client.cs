using MoneyMCS.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace MoneyMCS.Areas.Identity.Data
{
    public class Client
    {
        public int ClientId { get; set; }

        [MaxLength(100)]
        public string FirstName { get; set; }

        [MaxLength(100)]
        public string LastName { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateAdded { get; set; }

        public bool IsContacted { get; set; }

        public ClientStatus Status { get; set; }
        public List<string> Services { get; set; }
        public List<ClientNote> Notes { get; set; }
        public Business Business { get; set; }

        public string ReferrerId { get; set; }
        public ApplicationUser Referrer { get; set; }
    }
}


public class ClientNote
{
    public int ClientNoteId { get; set; }
    [MaxLength]
    public string Note { get; set; }
    public int ClientId { get; set; }
    public Client Client { get; set; }

}

public class Business
{
    public int BusinessId { get; set; }
    public string? Name { get; set; }
    public string? EntityType { get; set; }
    public string? Industry { get; set; }
    public string? EIN { get; set; }
    public string? BIN { get; set; }
    public string? License { get; set; }
    [Url]
    public string? Website { get; set; }
    [EmailAddress]
    public string? EmailAddress { get; set; }
    public string? Merchant { get; set; }
    public int AddressId { get; set; }
    public Address Address { get; set; }
    public int BusinesssPhoneId { get; set; }
    public BusinessPhone BusinessPhone { get; set; }
    public int ClientId { get; set; }
    public Client Client { get; set; }
}

public class Address
{
    public int AddressId { get; set; }
    public string? CompanyAddress { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public AddressType? Type { get; set; }
    public int BusinessId { get; set; }
    public Business Business { get; set; }

}

public class BusinessPhone
{
    public int BusinessPhoneId { get; set; }
    [Phone]
    public string? Phone { get; set; }
    public string? Provider { get; set; }
    public string? Fax { get; set; }
    public int BusinessId { get; set; }
    public Business Business { get; set; }
}

public enum AddressType
{
    Home,
    Virtual,
    Business
}

public enum ClientStatus
{
    Responding,
    Unresponsive
}

