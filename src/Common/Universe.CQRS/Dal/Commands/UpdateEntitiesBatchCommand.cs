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
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using Universe.CQRS.Dal.Commands.Base;
using Universe.CQRS.Extensions;
using Universe.CQRS.Models.Enums;
using Universe.Helpers.Extensions;
using Universe.SqlBulkTools;

namespace Universe.CQRS.Dal.Commands
{
    /// <summary>
    ///     Комманда пакетного обновления множества сущностей
    /// <author>Alex Envision</author>
    /// </summary>
    /// <typeparam name="TEntityDb"></typeparam>
    public class UpdateEntitiesBatchCommand<TEntityDb> : BaseCommand
        where TEntityDb : class
    {
        protected string ConnectionString { get; set; }

        public virtual UpdateEntitiesBatchCommand<TEntityDb> Execute<TChildEntityDb>(
            Expression<Func<TEntityDb, object>> keySelector,
            TEntityDb entityDb,
            params IList<TChildEntityDb>[] entitiesDbs) where TChildEntityDb : class
        {
            BatchProcess(keySelector, new List<TEntityDb> { entityDb });
            return this;
        }

        public UpdateEntitiesBatchCommand<TEntityDb> BatchProcess<TChildEntityDb>(
             Expression<Func<TChildEntityDb, object>> keySelector,
             List<TChildEntityDb> entities)
             where TChildEntityDb : class
        {
            if (DbSystemManagementType == DbSystemManagementTypes.PostgreSQL)
                throw new NotSupportedException("Операция массового обновления элементов в таблицах на данный момент для PostgreSQL не поддерживается!");

            // Сохранение строки подключения
            ConnectionString = DbCtx.Database.Connection.ConnectionString;

            var setDb = DbCtx.Set<TChildEntityDb>();

            var table = setDb.GetTableName();
            var bulk = new BulkOperations();

            //Expression<Func<TChildEntityDb, object>> keySelector =
            //    ExpressionExtensions.CreateExpressionDbeUniversal<TChildEntityDb>("Id");

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                bulk.Setup<TChildEntityDb>(x => x.ForCollection(entities))
                    .WithTable(table)
                    .AddAllColumns()
                    .BulkInsertOrUpdate()
                    .SetIdentityColumn(keySelector)
                    .MatchTargetOn(keySelector);

                bulk.CommitTransaction(connection);
            }

            return this;
        }

        public UpdateEntitiesBatchCommand<TEntityDb> BatchProcess<TParentEntity, TChildEntityDb>(
            Expression<Func<TChildEntityDb, object>> keySelector,
            List<TParentEntity> parentEntitiesAfterUpdate,
            Dictionary<TParentEntity, List<TChildEntityDb>> entitiesDict,
            Func<List<TChildEntityDb>, TParentEntity, List<TChildEntityDb>> parentKeySetterFunc)
            where TChildEntityDb : class
        {
            var setDb = DbCtx.Set<TChildEntityDb>();

            var table = setDb.GetTableName();

            var entitiesKvpList = entitiesDict.ToList();
            var entities = new List<TChildEntityDb>();
            for (var index = 0; index < entitiesDict.Count; index++)
            {
                var parentEntityDb = parentEntitiesAfterUpdate[index];
                var childEntityDbKvp = entitiesKvpList[index].Value;
                entities.AddRange(parentKeySetterFunc.Invoke(childEntityDbKvp, parentEntityDb) ?? new List<TChildEntityDb>());
            }

            // Чтение сохранённой ранее строки подключения т.к в случае повторного обращения к Database.Connection пароль теряется
            string connectionString = ConnectionString.IsNullOrEmpty() ? DbCtx.Database.Connection.ConnectionString : ConnectionString;
            var bulk = new BulkOperations();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                bulk.Setup<TChildEntityDb>(x => x.ForCollection(entities))
                    .WithTable(table)
                    .AddAllColumns()
                    .BulkInsertOrUpdate()
                    .SetIdentityColumn(keySelector)
                    .MatchTargetOn(keySelector);

                bulk.CommitTransaction(connection);
            }

            return this;
        }
    }
}
