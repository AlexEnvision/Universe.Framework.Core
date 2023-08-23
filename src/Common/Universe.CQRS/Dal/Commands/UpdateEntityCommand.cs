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
using System.Data.Entity.Migrations;
using System.Threading.Tasks;
using Universe.CQRS.Dal.Commands.Base;
using Universe.CQRS.Dal.Commands.CommandResults;
using Universe.DataAccess.Models;

namespace Universe.CQRS.Dal.Commands
{
    /// <summary>
    ///     Комманда обновления сущности
    /// <author>Alex Envision</author>
    /// </summary>
    /// <typeparam name="TEntityDb"></typeparam>
    public class UpdateEntityCommand<TEntityDb> : BaseCommand
        where TEntityDb : Entity 
    {
        public virtual async Task<UpdateEntityResult> ExecuteAsync(TEntityDb entityDb)
        {
            if (entityDb == null)
                throw new ArgumentNullException(nameof(entityDb));

            var setDb = DbCtx.Set<TEntityDb>();
            setDb.AddOrUpdate(entityDb);
            await DbCtx.SaveChangesAsync();

            var id = entityDb.Id;
            return new UpdateEntityResult {
                Id = id,
                IsSuccessful = true
            };
        }

        public virtual UpdateEntityResult Execute(TEntityDb entityDb)
        {
            if (entityDb == null)
                throw new ArgumentNullException(nameof(entityDb));

            var setDb = DbCtx.Set<TEntityDb>();
            setDb.AddOrUpdate(entityDb);
            DbCtx.SaveChanges();

            var id = entityDb.Id;
            return new UpdateEntityResult {
                Id = id,
                IsSuccessful = true
            };
        }
    }
}