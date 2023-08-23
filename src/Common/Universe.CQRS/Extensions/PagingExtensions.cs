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

using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Universe.CQRS.Dal.Base.Extensions;
using Universe.CQRS.Models.Page;

namespace Universe.CQRS.Extensions
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    public static class PagingExtensions
    {
        /// <summary>
        /// The foreach paging.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="paging">
        /// The paging.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        public static async Task<RequestedPage<T>> GetCurrentPageExtensionAsync<T>(this IQueryable<T> query, Paging paging)
        {
            if (paging == null)
            {
                var items = await query.ToListAsync();
                return new RequestedPage<T>
                {
                    Items = items,
                    NextPageHavingItems = false
                };
            }

            var allCount = query.Count();
            paging.AllCount = allCount;

            if (paging.AllCount == 0)
            {
                paging.PageIndex = 1;
            }
            else
            {
                if (paging.PageCount < paging.PageIndex)
                    paging.PageIndex = paging.PageCount;

                var pi = paging.PageIndex - 1;
                if (pi < 0)
                    pi = 0;

                var begin = paging.CountOnPage * pi;
                var length = paging.CountOnPage;
                query = query.Skip(() => begin).Take(() => length);

                var lastItemIndex = paging.CountOnPage * pi + paging.CountOnPage;
                var nextPageHaveItem = lastItemIndex < allCount;

                var items = await query.ToListAsync();
                return new RequestedPage<T>
                {
                    Items = items,
                    NextPageHavingItems = nextPageHaveItem
                };
            }

            return new RequestedPage<T>();
        }

        /// <summary>
        /// The foreach paging.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="paging">
        /// The paging.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        public static RequestedPage<T> GetCurrentPageExtension<T>(this IQueryable<T> query, Paging paging)
        {
            if (paging == null)
            {
                var items = query.ToList();
                return new RequestedPage<T>
                {
                    Items = items,
                    NextPageHavingItems = false
                };
            }

            var allCount = query.Count();
            paging.AllCount = allCount;

            if (paging.AllCount == 0)
            {
                paging.PageIndex = 1;
            }
            else
            {
                if (paging.PageCount < paging.PageIndex)
                    paging.PageIndex = paging.PageCount;

                var pi = paging.PageIndex - 1;
                if (pi < 0)
                    pi = 0;

                var begin = paging.CountOnPage * pi;
                var length = paging.CountOnPage;
                query = query.Skip(() => begin).Take(() => length);

                var lastItemIndex = paging.CountOnPage * pi + paging.CountOnPage;
                var nextPageHaveItem = lastItemIndex < allCount;

                var items = query.ToList();
                return new RequestedPage<T>
                {
                    Items = items,
                    NextPageHavingItems = nextPageHaveItem
                };
            }

            return new RequestedPage<T>();
        }
    }
}