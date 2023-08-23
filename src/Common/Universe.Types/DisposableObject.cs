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

namespace Universe.Types
{
    /// <summary>
    /// Implement IDisposable correctly.
    /// <author>Alex Envision</author>
    /// </summary>
    /// <seealso cref="System.IDisposable"/>
    public abstract class DisposableObject : IDisposable
    {
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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
        ///      base.Dispose(disposing); // Если унаследован от абстрактоного DisposableObject 
        ///      // то метод вызывать не нужно, если от наследника, то нужно
        ///  }
        ///  </code>
        /// </example>
        protected abstract void Dispose(bool disposing);

        /// <summary>
        /// Finalizes an instance of the <see cref="DisposableObject"/> class.
        /// </summary>
        ~DisposableObject()
        {
            Dispose(false);
        }
    }
}