using Microsoft.EntityFrameworkCore;
using MoneyMCS.Models;

namespace MoneyMCS.Services
{
    public class ClientContext : DbContext
    {
        public ClientContext(DbContextOptions<ClientContext> options)
        : base(options)
        {
        }
        public DbSet<Client> Clients { get; set; }


    }
}
