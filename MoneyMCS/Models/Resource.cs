using System.ComponentModel.DataAnnotations;

namespace MoneyMCS.Models
{
    public class Resource
    {
        [Display(Name = "Resource Name")]
        public string ResourceName { get; set; }
        
        public string Description { get; set; }

        public string FilePath { get; set; }
    }
}
