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
using System.Security.Principal;
using Microsoft.Win32.SafeHandles;

namespace Universe.IO.Security.Principal
{
    /// <summary>
    /// Impersonate WindowsIdentity to app pool identity.
    /// </summary>
    /// <seealso cref="System.IDisposable"/>
    public sealed class RunAsAppPoolScope : IDisposable
    {
        //private WindowsImpersonationContext _wix;

        /// <summary>
        /// Initializes a new instance of the <see cref="RunAsAppPoolScope"/> class.
        /// And impersonate WindowsIdentity to app pool identity.
        /// </summary>
        public RunAsAppPoolScope()
        {
            //_wix = WindowsIdentity.Impersonate(IntPtr.Zero);
        }

        public void RunUnderImpersonationContext(Action action)
        {
            var safeUserHandle = new SafeAccessTokenHandle(IntPtr.Zero);
            WindowsIdentity.RunImpersonated(safeUserHandle, action);
        }

        public T RunUnderImpersonationContext<T>(Func<T> func)
        {
            var safeUserHandle = new SafeAccessTokenHandle(IntPtr.Zero);
            return WindowsIdentity.RunImpersonated(safeUserHandle, func);
        }

        /// <summary>
        /// Undo impersonate.
        /// </summary>
        public void Dispose()
        {
            //if (_wix == null)
            //    return;

            //_wix.Undo();
            //_wix.Dispose();
            //_wix = null;
        }
    }
}