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
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using Universe.DataAccess;
using Universe.DataAccess.Mappings.Framework;
using Universe.Framework.DataAccess.Test.Migrations;
using Universe.Framework.DataAccess.Test.Models.Traiset;

namespace Universe.Framework.DataAccess.Test
{
    public class UniverseDbTestContext : UniverseDbContext
    {
        public UniverseDbTestContext()
            : base("UniverseNetCoreDbTestDb")
        {
            SetConfigurationOptions();
        }

        public UniverseDbTestContext(string connectionString) : base(connectionString)
        {
            SetConfigurationOptions();
        }

        public DbSet<TrainsetDb> Trainsets { get; set; }

        public DbSet<TrainsetClassDb> TrainsetClasses { get; set; }

        public DbSet<TrainsetItemDb> TrainsetItems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // В этот момент начинается создание экземпляра UniverseDbTestContext с пустым конструктором. Нужно пофиксить
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<UniverseDbTestContext, Configuration>());

            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            var maps = typeof(UniverseDbTestContext).Assembly
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
