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
using System.Collections.ObjectModel;
using System.Linq;

namespace Universe.Helpers.Extensions
{
    /// <summary>
    ///     Расширения для <see cref="IDictionary{TKey,TValue}"/> коллекции.
    ///     Extension for the <see cref="IDictionary{TKey,TValue}"/> collection.
    /// <author>Alex Envision</author>
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Adds the or update.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dic">The dic.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="System.ArgumentNullException">
        /// dic
        /// or
        /// key
        /// </exception>
        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, TValue value)
        {
            if (dic == null)
                throw new ArgumentNullException(nameof(dic));
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (dic.TryGetValue(key, out var outValue))
                dic[key] = value;
            else
                dic.Add(key, value);
        }

        public static void DisposeAndClear<TKey, TValue>(this IDictionary<TKey, TValue> dic)
        {
            if (dic == null)
                throw new ArgumentNullException(nameof(dic));

            foreach (var keyValuePair in dic.ToList())
            {
                var item = keyValuePair.Value as IDisposable;
                item?.Dispose();
                dic.Remove(keyValuePair);
            }
        }

        /// <summary>
        /// Получает значение по указанному ключу и если его нет, то вызывает функцию по созданию/формированию значения для
        /// указанного ключа.
        /// </summary>
        /// <typeparam name="TKey">Тип ключа.</typeparam>
        /// <typeparam name="TValue">Тип значения.</typeparam>
        /// <param name="dic">Словарь.</param>
        /// <param name="key">Ключ.</param>
        /// <param name="createFunc">Функция созданию/формированию значения.</param>
        /// <returns></returns>
        public static TValue GetOrCreate<TKey, TValue>(
            this IDictionary<TKey, TValue> dic,
            TKey key,
            Func<TValue> createFunc)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (createFunc == null)
                throw new ArgumentNullException(nameof(createFunc));

            if (createFunc == null)
                throw new ArgumentNullException(nameof(createFunc));

            if (dic.TryGetValue(key, out var result))
                return result;

            result = createFunc();

            dic.Add(key, result);
            return result;
        }

        /// <summary>
        /// Gets the value by key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dic">The dic.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// dic
        /// or
        /// key
        /// </exception>
        public static TValue GetValueObj<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dic, TKey key)
            where TValue : class
        {
            if (dic == null)
                throw new ArgumentNullException(nameof(dic));
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (dic.TryGetValue(key, out var value))
                return value;

            return null;
        }

        /// <summary>
        /// Gets the value by key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dic">The dic.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">dic</exception>
        public static TValue? GetValueStruc<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dic, TKey key)
            where TValue : struct
        {
            if (dic == null)
                throw new ArgumentNullException(nameof(dic));

            if (dic.TryGetValue(key, out var value))
                return value;

            return null;
        }

        public static IReadOnlyDictionary<TKey, TValue> ToReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dic)
        {
            if (dic == null)
                throw new ArgumentNullException(nameof(dic));

            return new ReadOnlyDictionary<TKey, TValue>(dic);
        }

        public static bool TryContainsValue<TValue>(this Dictionary<string, TValue> dict, string containedSomethingValue, out TValue value)
        {
            foreach (var kvp in dict)
            {
                if (containedSomethingValue.Contains(kvp.Key))
                {
                    value = kvp.Value;
                    return true;
                }
            }

            value = default(TValue);
            return false;
        }
    }
}