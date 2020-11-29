using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordEconomy.Data
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Active> Actives { get; set; }

        public DbSet<Note> Notes { get; set; }

        public DbSet<Buy> Buys { get; set; }

        public DataContext() : base("DefaultConnection")
        {
        }

    }
}
