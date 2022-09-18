using Microsoft.EntityFrameworkCore;
using MoneyMCS.Models;

namespace MoneyMCS.Services
{
    public class ResourceContext : DbContext
    {
        public DbSet<Resource> Resources { get; set; }
    }
}
