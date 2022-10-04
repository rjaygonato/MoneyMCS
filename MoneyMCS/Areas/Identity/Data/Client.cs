using System.ComponentModel.DataAnnotations;

namespace MoneyMCS.Areas.Identity.Data
{
    public class Client
    {
        public int ClientId { get; set; }

        [MaxLength(100)]
        public string Company { get; set; }

        [MaxLength(100)]
        public string FirstName { get; set; }

        [MaxLength(100)]
        public string LastName { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        [MaxLength(100)]
        public string Address { get; set; }

        [MaxLength(100)]
        public string City { get; set; }

        [MaxLength(100)]
        public string State { get; set; }

        [MaxLength(6)]
        public string ZipCode { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateAdded { get; set; }
        public string ReferrerId { get; set; }
        public ApplicationUser Referrer { get; set; }
    }
}
