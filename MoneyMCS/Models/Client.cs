using MoneyMCS.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyMCS.Models
{
    public class Client
    {
        public int ClientId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Company { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        [MaxLength(100)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? State { get; set; }

        [MaxLength(6)]
        public string? ZipCode { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateAdded { get; set; }
        public string? ReferrerId { get; set; }
        public virtual AgentUser Referrer { get; set; }
    }
}
