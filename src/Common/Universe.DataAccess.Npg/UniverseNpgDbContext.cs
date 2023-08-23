//  ╔═════════════════════════════════════════════════════════════════════════════════╗
//  ║                                                                                 ║
//  ║   Copyright 2021 Universe.Framework                                             ║
//  ║                                                                                 ║
//  ║   Licensed under the Apache License, Version 2.0 (the "License");               ║
//  ║   you may not use this file except in compliance with the License.              ║
//  ║   You may obtain a copy of the License at                                       ║
//  ║                                                                                 ║
//  ║       http://www.apache.org/licenses/LICENSE-2.0                                ║
//  ║                                                                                 ║
//  ║   Unless required by applicable law or agreed to in writing, software           ║
//  ║   distributed under the License is distributed on an "AS IS" BASIS,             ║
//  ║   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.      ║
//  ║   See the License for the specific language governing permissions and           ║
//  ║   limitations under the License.                                                ║
//  ║                                                                                 ║
//  ║                                                                                 ║
//  ║   Copyright 2021 Universe.Framework                                             ║
//  ║                                                                                 ║
//  ║   Лицензировано согласно Лицензии Apache, Версия 2.0 ("Лицензия");              ║
//  ║   вы можете использовать этот файл только в соответствии с Лицензией.           ║
//  ║   Вы можете найти копию Лицензии по адресу                                      ║
//  ║                                                                                 ║
//  ║       http://www.apache.org/licenses/LICENSE-2.0.                               ║
//  ║                                                                                 ║
//  ║   За исключением случаев, когда это регламентировано существующим               ║
//  ║   законодательством или если это не оговорено в письменном соглашении,          ║
//  ║   программное обеспечение распространяемое на условиях данной Лицензии,         ║
//  ║   предоставляется "КАК ЕСТЬ" и любые явные или неявные ГАРАНТИИ ОТВЕРГАЮТСЯ.    ║
//  ║   Информацию об основных правах и ограничениях,                                 ║
//  ║   применяемых к определенному языку согласно Лицензии,                          ║
//  ║   вы можете найти в данной Лицензии.                                            ║
//  ║                                                                                 ║
//  ╚═════════════════════════════════════════════════════════════════════════════════╝

using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using Universe.DataAccess.Mappings.Framework;
using Universe.Helpers.Extensions;

namespace Universe.DataAccess.Npg
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    public class UniverseNpgDbContext : DbContext, IUniverseDbContext
    {
        public string DbSystemManagementType => "PostgreSQL";

        public string ConnectionString { get; set; }

        public UniverseNpgDbContext()
            : base(UniverseNpgDbCache.ConnectionString)  //"name=UniverseNpgDbContext"
        {
        }

        public UniverseNpgDbContext(string connectionString) : base(connectionString)
        {
            if (!connectionString.IsNullOrEmpty() && connectionString.Contains("server", StringComparison.OrdinalIgnoreCase))
            {
                UniverseNpgDbCache.ConnectionString = connectionString;
                ConnectionString = connectionString;
            }

            if (!UniverseNpgDbCache.ConnectionString.IsNullOrEmpty())
                connectionString = UniverseNpgDbCache.ConnectionString;
        }
    }

    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    public class UniverseNpgDbContext<TUniverseDbContext, TConfiguration> : UniverseNpgDbContext
        where TUniverseDbContext : UniverseNpgDbContext, new()
        where TConfiguration : DbMigrationsConfiguration<TUniverseDbContext>, new()
    {
        public UniverseNpgDbContext()
            : base(UniverseNpgDbCache.ConnectionString)   //"name=UniverseNpgDbContext"
        {
            SetConfigurationOptions();
        }

        public UniverseNpgDbContext(string connectionString) : base(connectionString)
        {
            SetConfigurationOptions();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<TUniverseDbContext, TConfiguration>());

            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            var maps = typeof(TUniverseDbContext).Assembly
                .GetTypes()
                .Where(x => !x.IsAbstract && x.GetInterfaces().Contains(typeof(IEntityMap)))
                .Select(Activator.CreateInstance)
                .Cast<IEntityMap>();
            foreach (var map in maps)
            {
                map.Apply(modelBuilder);
            }
        }

        private void SetConfigurationOptions()
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }
    }
}