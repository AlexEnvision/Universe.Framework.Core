using System.Data.Entity;

namespace Universe.DataAccess.Npg.GettingStarted
{
    public class Initializer : IDatabaseInitializer<UniverseNpgDbContext>
    {
        /// <summary>
        /// Executes the strategy to initialize the database for the given context.
        /// </summary>
        /// <param name="context"> The context. </param>
        public void InitializeDatabase(UniverseNpgDbContext context)
        {
        }
    }
}