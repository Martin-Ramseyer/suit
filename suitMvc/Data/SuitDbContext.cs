using Microsoft.EntityFrameworkCore;
using suitMvc.Models;

namespace suitMvc.Data
{
    public class SuitDbContext : DbContext
    {
        public SuitDbContext(DbContextOptions<SuitDbContext> options) : base(options) { }

        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<Flayer> Flayers { get; set; }
        public DbSet<Invitados> Invitados { get; set; }
    }
}
