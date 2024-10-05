using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace PharmaStockManager.Models
{
    public class PharmaContext : DbContext
    {
        public PharmaContext(DbContextOptions<PharmaContext> options) : base(options)
        {
        }

        public DbSet<Drug> Drugs { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
