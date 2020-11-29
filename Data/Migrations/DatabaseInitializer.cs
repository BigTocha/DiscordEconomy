using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordEconomy.Data.Migrations
{
    public class DatabaseInitializer : IDatabaseInitializer<DataContext>
    {
        public void InitializeDatabase(DataContext context)
        {
            context.Database.Log += Console.WriteLine;
            DbMigrationsConfiguration migrationsConfiguration = new DbMigrationsConfiguration();
            migrationsConfiguration.TargetDatabase = new DbConnectionInfo("DefaultConnection");
            migrationsConfiguration.ContextType = typeof (DataContext);
            DbMigrator migrator = new DbMigrator(migrationsConfiguration);
            IEnumerable<string> pendingMigrations = migrator.GetPendingMigrations();
            foreach (string migration in pendingMigrations)
            {
                migrator.Update(migration);
            }
        }
    }
}
