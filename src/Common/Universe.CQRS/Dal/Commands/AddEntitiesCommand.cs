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
using System.Linq;
using System.Threading.Tasks;
using Universe.CQRS.Dal.Commands.Base;
using Universe.CQRS.Dal.Commands.CommandResults;
using Universe.DataAccess.Models;

namespace Universe.CQRS.Dal.Commands
{
    /// <summary>
    ///     Комманда добавления множества сущностей
    /// <author>Alex Envision</author>
    /// </summary>
    /// <typeparam name="TEntityDb"></typeparam>
    public class AddEntitiesCommand<TEntityDb> : BaseCommand
        where TEntityDb : Entity 
    {
        public virtual async Task<AddEntitiesResult> ExecuteAsync(IList<TEntityDb> entitiesDb)
        {
            if (entitiesDb == null)
                throw new ArgumentNullException(nameof(entitiesDb));

            if (entitiesDb.Count == 0)
                return new AddEntitiesResult {
                    Ids = new List<long>(),
                    IsSuccessful = false
                };

            var setDb = DbCtx.Set<TEntityDb>();
            setDb.AddRange(entitiesDb);

            await DbCtx.SaveChangesAsync();

            var ids = entitiesDb.Select(x => x.Id).ToList();
            return new AddEntitiesResult {
                Ids = ids,
                IsSuccessful = true
            };
        }

        public virtual AddEntitiesResult Execute(IList<TEntityDb> entitiesDb)
        {
            if (entitiesDb == null)
                throw new ArgumentNullException(nameof(entitiesDb));

            if (entitiesDb.Count == 0)
                return new AddEntitiesResult {
                    Ids = new List<long>(),
                    IsSuccessful = false
                };

            var setDb = DbCtx.Set<TEntityDb>();
            setDb.AddRange(entitiesDb);

            DbCtx.SaveChanges();

            var ids = entitiesDb.Select(x => x.Id).ToList();
            return new AddEntitiesResult {
                Ids = ids,
                IsSuccessful = true
            };
        }
    }
}