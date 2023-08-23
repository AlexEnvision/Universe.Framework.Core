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
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Universe.CQRS.Dal.Base.Extensions;
using Universe.CQRS.Dal.Base.MetaInfo;
using Universe.CQRS.Dal.Queries.Base;
using Universe.CQRS.Extensions;
using Universe.CQRS.Models.Base;
using Universe.CQRS.Models.Req;

namespace Universe.CQRS.Dal.Queries
{
    /// <summary>
    ///     Запрос на получение множества сущностей, с включением связанных сущностей посредством проекции.
    /// <author>Alex Envision</author>
    /// </summary>
    /// <typeparam name="TEntityDb"></typeparam>
    /// <typeparam name="TDbEntityRequest"></typeparam>
    public class SelectEntitiesQuery<TEntityDb, TDbEntityRequest> : BaseQuery
        where TEntityDb : class
        where TDbEntityRequest : BaseDbEntityRequest, IDbEntityRequest
    {
        public virtual async Task<RequestedPage<TDbEntityRequest>> ExecuteAsync(
            GetEntitiesReq req,
            Expression<Func<TEntityDb, TDbEntityRequest>> projection,
            bool disablePropsMiSearch = false)
        {
            // Запрос с выбором полей через Select c проекцией унаследуемой от BaseDbEntityRequest
            var query = this.DbCtx.Set<TEntityDb>().Select(projection);

            var container = req.FieldMapContainer as FieldMapContainer<TDbEntityRequest>;

            // Построение метаинформации для фильтрации и сортировки
            var mi = query.CreateDbRequestMetaInfo<TEntityDb, TDbEntityRequest>(container?.FieldMap, disablePropsMiSearch);

            // Результат работы запроса
            var projectionResult = await query
                .ApplyFiltersAtQuery(req.Filters, mi, req.AllowNoTrackingMode)
                .ApplySortingAtQuery(req.Sorting, mi)
                .GetCurrentPageExtensionAsync(req.Paging);

            return projectionResult;
        }

        public virtual RequestedPage<TDbEntityRequest> Execute(
            GetEntitiesReq req,
            Expression<Func<TEntityDb, TDbEntityRequest>> projection,
            bool disablePropsMiSearch = false)
        {
            // Запрос с выбором полей через Select c проекцией унаследуемой от BaseDbEntityRequest
            var query = this.DbCtx.Set<TEntityDb>().Select(projection);

            var container = req.FieldMapContainer as FieldMapContainer<TDbEntityRequest>;

            // Построение метаинформации для фильтрации и сортировки
            var mi = query.CreateDbRequestMetaInfo<TEntityDb, TDbEntityRequest>(container?.FieldMap, disablePropsMiSearch);

            // Результат работы запроса
            var projectionResult = query
                .ApplyFiltersAtQuery(req.Filters, mi, req.AllowNoTrackingMode)
                .ApplySortingAtQuery(req.Sorting, mi)
                .GetCurrentPageExtension(req.Paging);

            return projectionResult;
        }
    }
}
