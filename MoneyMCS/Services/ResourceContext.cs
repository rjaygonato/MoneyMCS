using Microsoft.EntityFrameworkCore;
using MoneyMCS.Areas.Identity.Data;
using MoneyMCS.Models;

namespace MoneyMCS.Services
{
    public class ResourceContext : DbContext
    {
        public ResourceContext(DbContextOptions<ResourceContext> options)
        : base(options)
        {
        }
        public DbSet<Resource> Resources { get; set; }
    }
}
