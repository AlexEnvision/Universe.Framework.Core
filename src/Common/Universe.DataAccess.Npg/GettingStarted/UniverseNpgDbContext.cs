using Universe.DataAccess.Npg.GettingStarted.Migrations;

namespace Universe.DataAccess.Npg.GettingStarted
{
    public class UniverseNpgDbContext : UniverseNpgDbContext<UniverseNpgDbContext, Configuration>
    {
        public UniverseNpgDbContext()
            : base()
        {
        }

        public UniverseNpgDbContext(string connectionString) : base(connectionString)
        {
        }
    }
}