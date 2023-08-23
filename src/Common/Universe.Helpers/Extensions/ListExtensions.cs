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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Universe.Helpers.Extensions
{
    /// <summary>
    ///     Расширения для <see cref="IList{T}"/> коллекции.
    ///     Extension for the <see cref="IList{T}"/> collection.
    /// <author>Alex Envision</author>
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Adds to <paramref name="list"/> result function <paramref name="func"/> if result is disposable.
        /// </summary>
        /// <typeparam name="T">Type of the result function <paramref name="func"/>.</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="func">The function create or get object of type <typeparamref name="T"/>.</param>
        /// <returns>Result function <paramref name="func"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// list
        /// or
        /// func
        /// </exception>
        public static T AddIfIDisposable<T>(this IList<IDisposable> list, Func<T> func)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (func == null)
                throw new ArgumentNullException(nameof(func));

            var obj = func();
            var disposableObj = obj as IDisposable;
            if (disposableObj != null)
                list.Add(disposableObj);

            return obj;
        }

        /// <summary>
        /// Disposes the and remove items <paramref name="list"/>.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <exception cref="System.ArgumentNullException">list</exception>
        public static void DisposeAndClear(this IList list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            for (var i = list.Count - 1; i >= 0; i--)
            {
                var item = list[i] as IDisposable;
                item?.Dispose();
                list.RemoveAt(i);
            }
        }

        public static IReadOnlyList<T> ToReadOnly<T>(this IList<T> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            return new ReadOnlyCollection<T>(list);
        }
    }
}