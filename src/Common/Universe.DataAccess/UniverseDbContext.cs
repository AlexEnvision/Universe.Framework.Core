//  ╔═════════════════════════════════════════════════════════════════════════════════╗
//  ║                                                                                 ║
//  ║   Copyright 2021 Universe.Framework.Core                                        ║
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
//  ║   Copyright 2021 Universe.Framework.Core                                        ║
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

namespace Universe.DataAccess
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    public class UniverseDbContext : DbContext, IUniverseDbContext
    {
        public string DbSystemManagementType => "MSSql";

        public string ConnectionString { get; set; }

        public UniverseDbContext()
            : base("name=UniverseDbSystemDb")
        {
        }

        public UniverseDbContext(string connectionString) : base(connectionString)
        {
        }

        public static TUniverseDbContext CreateDbContext<TUniverseDbContext>(string connectionString) where TUniverseDbContext: UniverseDbContext, new()
        {
            // Приходится использовать рефлексию, ибо дженерики с параметрами в конструктуре так просто не создаются
            // Опять же это создание контекста базы, а это сама по себе медленная операция,
            // и поэтому данный подход влияние по производительности сам по себе оказывает минимальное
            var instance = Activator.CreateInstance(typeof(TUniverseDbContext), connectionString);
            var typedInstance = instance as TUniverseDbContext;
            return typedInstance;
        }
    }

    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    public class UniverseDbContext<TUniverseDbContext, TConfiguration> : UniverseDbContext
        where TUniverseDbContext : UniverseDbContext, new()
        where TConfiguration: DbMigrationsConfiguration<TUniverseDbContext>, new()
    {
        public UniverseDbContext()
            : base("name=UniverseDbSystemDb")
        {
            SetConfigurationOptions();
        }

        public UniverseDbContext(string connectionString) : base(connectionString)
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