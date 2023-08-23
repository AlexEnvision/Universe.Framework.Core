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
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Universe.CQRS.Dal.Commands.Base;
using Universe.CQRS.Dal.Commands.CommandResults;
using Universe.DataAccess.Models;

namespace Universe.CQRS.Dal.Commands
{
    /// <summary>
    ///     Команда обновления множества сущностей
    /// <author>Alex Envision</author>
    /// </summary>
    /// <typeparam name="TEntityDb"></typeparam>
    public class UpdateEntitiesCommand<TEntityDb> : BaseCommand
        where TEntityDb : Entity 
    {
        public virtual async Task ExecuteAsync(IList<TEntityDb> entitiesDbs)
        {
            if (entitiesDbs == null)
                throw new ArgumentNullException(nameof(entitiesDbs));

            if (entitiesDbs.Count == 0)
                return;

            var setDb = DbCtx.Set<TEntityDb>();
            setDb.AddOrUpdate(entitiesDbs.ToArray());

            await DbCtx.SaveChangesAsync();
        }

        public virtual UpdateEntitiesResult Execute(IList<TEntityDb> entitiesDbs)
        {
            if (entitiesDbs == null)
                throw new ArgumentNullException(nameof(entitiesDbs));

            if (entitiesDbs.Count == 0)
                return new UpdateEntitiesResult {
                    Ids = new List<long>(),
                    IsSuccessful = false
                };

            var setDb = DbCtx.Set<TEntityDb>();
            var entitiesDbsArray = entitiesDbs.ToArray();
            setDb.AddOrUpdate(entitiesDbsArray);

            DbCtx.SaveChanges();

            var ids = entitiesDbsArray.Select(x => x.Id).ToList();
            return new UpdateEntitiesResult {
                Ids = ids,
                IsSuccessful = true
            };
        }
    }
}