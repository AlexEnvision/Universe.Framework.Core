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
using System.ComponentModel;

namespace Universe.IO.Security.Principal.Impersonation
{
    /// <summary>
    /// Exception thrown when impersonation fails.
    /// </summary>
    /// <remarks>
    /// Inherits from <see cref="ApplicationException"/> for backwards compatibility reasons.
    /// </remarks>
    public class ImpersonationException : ApplicationException
    {
        private readonly Win32Exception _win32Exception;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImpersonationException"/> class from a specific <see cref="Win32Exception"/>.
        /// </summary>
        /// <param name="win32Exception">The exception to base this exception on.</param>
        public ImpersonationException(Win32Exception win32Exception)
            : base(win32Exception.Message, win32Exception)
        {
            // Note that the Message is generated inside the Win32Exception class via the Win32 FormatMessage function.

            _win32Exception = win32Exception;
        }

        /// <summary>
        /// Returns the Win32 error code handle for the exception.
        /// </summary>
        public int ErrorCode => _win32Exception.ErrorCode;

        /// <summary>
        /// Returns the Win32 native error code for the exception.
        /// </summary>
        public int NativeErrorCode => _win32Exception.NativeErrorCode;
    }
}
