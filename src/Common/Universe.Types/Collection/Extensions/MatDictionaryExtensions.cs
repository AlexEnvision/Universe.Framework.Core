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

namespace Universe.Types.Collection.Extensions
{
    /// <summary>
    ///     Расширения для <see cref="MatDictionary{TKey,TValue}"/>
    /// <author>Alex Envision</author>
    /// </summary>
    public static class MatDictionaryExtensions
    {
        public static MatDictionary<TKey, TSource> ToMatDictionary<TKey, TSource>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var dictionary = source.ToDictionary(keySelector);
            var matDictionary = new MatDictionary<TKey, TSource>(dictionary);
            return matDictionary;
        }

        public static MatDictionary<TKey, TElement> ToMatDictionary<TSource, TKey, TElement>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector)
        {
            var dictionary = source.ToDictionary(keySelector, elementSelector);
            var matDictionary = new MatDictionary<TKey, TElement>(dictionary);
            return matDictionary;
        }

        public static void AddRange<TKey, TValue>(this MatDictionary<TKey, TValue> source, Dictionary<TKey, TValue> setDict)
        {
            foreach (var kvp in setDict)
            {
                if (source.ContainsKey(kvp.Key))
                    source.Add(kvp.Key, kvp.Value);
                else
                    source[kvp.Key] = kvp.Value;
            }
        }

        internal static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> source, Dictionary<TKey, TValue> setDict)
        {
            foreach (var kvp in setDict)
            {
                if (source.ContainsKey(kvp.Key))
                    source.Add(kvp.Key, kvp.Value);
                else
                    source[kvp.Key] = kvp.Value;
            }
        }
    }
}