using System.ComponentModel.DataAnnotations;

namespace MoneyMCS.Models
{
    public class Resource
    {
        public int ResourceId { get; set; }
        public string ResourceName { get; set; }
        
        public string Description { get; set; }

        public string Category { get; set; }

        public string FilePath { get; set; }
    }
}
