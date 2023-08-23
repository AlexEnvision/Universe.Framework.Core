﻿//  ╔═════════════════════════════════════════════════════════════════════════════════╗
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
using System.Linq;
using System.Threading.Tasks;
using Universe.CQRS.Dal.Commands.Base;
using Universe.CQRS.Dal.Commands.CommandResults;
using Universe.DataAccess.Models;

namespace Universe.CQRS.Dal.Commands
{
    /// <summary>
    ///     Команда удаления нескольких сущностей.
    /// <author>Alex Envision</author>
    /// </summary>
    /// <typeparam name="TEntityDb"></typeparam>
    public class DeleteEntitiesCommand<TEntityDb> : BaseCommand
        where TEntityDb : Entity
    {
        public virtual async Task<DeleteEntitiesResult> ExecuteAsync(List<TEntityDb> entitiesDb)
        {
            if (entitiesDb == null)
                throw new ArgumentNullException(nameof(entitiesDb));

            var deletedEntities = DbCtx.Set<TEntityDb>().RemoveRange(entitiesDb);

            await DbCtx.SaveChangesAsync();

            var ids = deletedEntities.Select(x => x.Id).ToList();

            return new DeleteEntitiesResult
            {
                Ids = ids,
                IsSuccessful = true
            };
        }

        public virtual DeleteEntitiesResult Execute(List<TEntityDb> entitiesDb)
        {
            if (entitiesDb == null)
                throw new ArgumentNullException(nameof(entitiesDb));

            var deletedEntities = DbCtx.Set<TEntityDb>().RemoveRange(entitiesDb);

            DbCtx.SaveChanges();

            var ids = deletedEntities.Select(x => x.Id).ToList();

            return new DeleteEntitiesResult
            {
                Ids = ids,
                IsSuccessful = true
            };
        }
    }
}