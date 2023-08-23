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
using Unity;
using Universe.CQRS.Models.Dto;
using Universe.CQRS.Models.Enums;
using Universe.DataAccess;
using Universe.Helpers.Extensions;

namespace Universe.CQRS.Infrastructure
{
    /// <summary>
    ///     Универсальный scope к которому, производится построение команд и запросов
    /// <author>Alex Envision</author>
    /// </summary>
    /// <typeparam name="TUniverseDbContext">Контекст базы данных</typeparam>
    public class UniverseScope<TUniverseDbContext> : IUniverseScope, IDisposable where TUniverseDbContext : class, IUniverseDbContext, new()
    {
        private readonly IAppPrincipalResolver _principalResolver;

        private UserDto _user;

        /// <summary>
        ///     Тип СУБД
        /// </summary>
        public DbSystemManagementTypes DbSystemManagementType { get; set; }

        public IUnityContainer Container { get; }

        public UniverseScope(IWebAppSettings appSettings, IWebAppPrincipalResolver principalResolver, IUnityContainer container)
        {
            Container = container;
            if (appSettings == null)
                throw new ArgumentNullException(nameof(appSettings));

            _principalResolver = principalResolver ?? throw new ArgumentNullException(nameof(principalResolver));

            var connectionString = appSettings.GetUniverseDbConnectionString();
            var universeDbContext = CreateDbContext(connectionString);

            DbSystemManagementType = universeDbContext.DbSystemManagementType.ToEnum(DbSystemManagementTypes.MSSql);
            DbCtx = universeDbContext as DbContext;
            UnitOfWork = new UnitOfWork(DbCtx);

            SessionId = Guid.NewGuid();
        }

        public UniverseScope(IWebAppSettings appSettings, IUnityContainer container)
        {
            Container = container;
            if (appSettings == null)
                throw new ArgumentNullException(nameof(appSettings));

            var connectionString = appSettings.GetUniverseDbConnectionString();
            var universeDbContext = CreateDbContext(connectionString);

            DbSystemManagementType = universeDbContext.DbSystemManagementType.ToEnum(DbSystemManagementTypes.MSSql);
            DbCtx = universeDbContext as DbContext;
            UnitOfWork = new UnitOfWork(DbCtx);

            SessionId = Guid.NewGuid();
        }

        public UniverseScope(IAppSettings appSettings, IAppPrincipalResolver principalResolver, IUnityContainer container)
        {
            Container = container;
            if (appSettings == null)
                throw new ArgumentNullException(nameof(appSettings));

            _principalResolver = principalResolver ?? throw new ArgumentNullException(nameof(principalResolver));

            var connectionString = appSettings.GetUniverseDbConnectionString();
            var universeDbContext = CreateDbContext(connectionString);

            DbSystemManagementType = universeDbContext.DbSystemManagementType.ToEnum(DbSystemManagementTypes.MSSql);
            DbCtx = universeDbContext as DbContext;
            UnitOfWork = new UnitOfWork(DbCtx);

            SessionId = Guid.NewGuid();
        }

        public UniverseScope(IAppSettings appSettings, IUnityContainer container)
        {
            Container = container;
            if (appSettings == null)
                throw new ArgumentNullException(nameof(appSettings));

            var connectionString = appSettings.GetUniverseDbConnectionString();
            var universeDbContext = CreateDbContext(connectionString);

            DbSystemManagementType = universeDbContext.DbSystemManagementType.ToEnum(DbSystemManagementTypes.MSSql);
            DbCtx = universeDbContext as DbContext;
            UnitOfWork = new UnitOfWork(DbCtx);

            SessionId = Guid.NewGuid();
        }

        public UniverseScope()
        {
        }

        /// <summary>
        ///     ИД выполняющейся сессии
        /// </summary>
        public Guid SessionId { get; }

        /// <summary>
        ///     Текущий пользователь
        /// </summary>
        public virtual UserDto CurrentUser
        {
            get { return _user ?? (_user = GetUser(_principalResolver)); }
            private set { _user = value; }
        }

        /// <summary>
        ///     Контекст базы данных
        /// </summary>
        public DbContext DbCtx { get; }

        public UnitOfWork UnitOfWork { get; }

        public void Dispose()
        {
            DbCtx?.Dispose();
        }

        protected virtual UserDto GetUser(IAppPrincipalResolver principalResolver)
        {
            if (principalResolver == null)
                return new UserDto();

            var principal = principalResolver.GetCurrentPrincipal();
            if (principal == null)
                return new UserDto();

            var identity = principal.WebAppIdentity;
            if (identity == null)
                throw new ArgumentException(nameof(identity));

            var userName = identity.Name;
            if (userName.IsNullOrEmpty())
                throw new Exception("userName.IsNullOrEmpty()");

            var user = new UserDto {
                Name = userName
            };

            return user ?? throw new Exception($"Не найден пользователь по логину: {userName}");
        }

        private TUniverseDbContext CreateDbContext(string connectionString)
        {
            // Приходится использовать рефлексию, ибо дженерики с параметрами в конструктуре так просто не создаются
            // Опять же это создание контекста базы, а это сама по себе медленная операция,
            // и поэтому данный подход влияние по производительности сам по себе оказывает минимальное
            var instance = Activator.CreateInstance(typeof(TUniverseDbContext), connectionString);
            var typedInstance = instance as TUniverseDbContext;
            return typedInstance;
        }
    }
}