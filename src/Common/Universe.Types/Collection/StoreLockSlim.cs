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
using System.Threading;
using Universe.Helpers.Extensions;

namespace Universe.Types.Collection
{
    /// <summary>
    /// Stores values of type <typeparamref name="TValue"/> by a key of type <typeparamref name="TKey"/>
    /// and provides thread safe reading and creating values.
    /// <author>Alex Envision</author>
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <seealso cref="DisposableObject"/>
    public class StoreLockSlim<TKey, TValue> : DisposableObject
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        private readonly Dictionary<TKey, TValue> _store = new Dictionary<TKey, TValue>();

        /// <summary>
        /// Gets the or create value of type <typeparamref name="TValue"/> by <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="createFunc">The create function.</param>
        /// <returns>Value.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// key
        /// or
        /// createFunc
        /// or
        /// createFunc
        /// </exception>
        public TValue GetOrCreate(TKey key, Func<TValue> createFunc)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (createFunc == null)
                throw new ArgumentNullException(nameof(createFunc));

            if (createFunc == null)
                throw new ArgumentNullException(nameof(createFunc));

            _lock.EnterUpgradeableReadLock();
            try
            {
                TValue result;
                if (_store.TryGetValue(key, out result))
                    return result;

                _lock.EnterWriteLock();
                try
                {
                    if (_store.TryGetValue(key, out result))
                        return result;

                    result = createFunc();

                    _store.AddOrUpdate(key, result);
                    return result;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
            finally
            {
                _lock.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        ///     <see href="https://msdn.microsoft.com/library/ms244737.aspx">CA1063: следует правильно реализовывать IDisposable</see>
        /// </summary>
        /// <param name="disposing"></param>
        /// <example>
        ///     <code>
        ///  // Пример для реализации в наследниках
        ///  protected override void Dispose(bool disposing)
        ///  {
        ///      // Пример для реализации в наследниках для управляемых ресурсов
        ///      if (disposing)
        ///      {
        ///         // free managed resources
        ///         if (managedResource != null)
        ///         {
        ///             managedResource.Dispose();
        ///             managedResource = null;
        ///         }
        ///      }
        /// 
        ///      // Пример для реализации в наследниках для не управляемых ресурсов
        ///      // free native resources if there are any.
        ///      if (nativeResource != IntPtr.Zero)
        ///      {
        ///          Marshal.FreeHGlobal(nativeResource);
        ///          nativeResource = IntPtr.Zero;
        ///      }
        /// 
        ///      base.Dispose(disposing);
        ///  }
        ///  </code>
        /// </example>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _lock.Dispose();
        }
    }
}