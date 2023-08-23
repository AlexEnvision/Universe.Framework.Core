using System.Data.Entity.Migrations;

namespace Universe.DataAccess.Npg.GettingStarted.Migrations
{
    public sealed class Configuration : DbMigrationsConfiguration<UniverseNpgDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(UniverseNpgDbContext context)
        {
        }
    }
}