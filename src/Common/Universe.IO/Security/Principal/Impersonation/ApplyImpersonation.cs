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

namespace Universe.IO.Security.Principal.Impersonation
{
    /// <summary>
    /// Provides ability to run code within the context of a specific user.
    /// </summary>

    public static class ApplyImpersonation
    {
        /// <summary>
        /// Impersonates a specific user account to perform the specified action.
        /// </summary>
        /// <param name="credentials">The credentials of the user account to impersonate.</param>
        /// <param name="logonType">The logon type used when impersonating the user account.</param>
        /// <param name="action">The action to perform.</param>
        public static void RunAsUser(UserCredentials credentials, LogonType logonType, Action action)
        {
            using (var tokenHandle = credentials.Impersonate(logonType))
            {
                RunImpersonated(tokenHandle, _ => action());
            }
        }

        /// <summary>
        /// Impersonates a specific user account to perform the specified action.
        /// </summary>
        /// <param name="credentials">The credentials of the user account to impersonate.</param>
        /// <param name="logonType">The logon type used when impersonating the user account.</param>
        /// <param name="action">The action to perform, which accepts a <see cref="SafeAccessTokenHandle"/> to the user account as its only parameter.</param>
        public static void RunAsUser(UserCredentials credentials, LogonType logonType, Action<SafeAccessTokenHandle> action)
        {
            using (var tokenHandle = credentials.Impersonate(logonType))
            {
                RunImpersonated(tokenHandle, action);
            }
        }

        /// <summary>
        /// Impersonates a specific user account to execute the specified function.
        /// </summary>
        /// <typeparam name="T">The return type of the function.</typeparam>
        /// <param name="credentials">The credentials of the user account to impersonate.</param>
        /// <param name="logonType">The logon type used when impersonating the user account.</param>
        /// <param name="function">The function to execute, which accepts a <see cref="SafeAccessTokenHandle"/> to the user account as its only parameter.</param>
        /// <returns>The result of executing the function.</returns>
        public static T RunAsUser<T>(UserCredentials credentials, LogonType logonType, Func<T> function)
        {
            using (var tokenHandle = credentials.Impersonate(logonType))
            {
                return RunImpersonated(tokenHandle, _ => function());
            }
        }

        /// <summary>
        /// Impersonates a specific user account to execute the specified function.
        /// </summary>
        /// <typeparam name="T">The return type of the function.</typeparam>
        /// <param name="credentials">The credentials of the user account to impersonate.</param>
        /// <param name="logonType">The logon type used when impersonating the user account.</param>
        /// <param name="function">The function to execute.</param>
        /// <returns>The result of executing the function.</returns>
        public static T RunAsUser<T>(UserCredentials credentials, LogonType logonType, Func<SafeAccessTokenHandle, T> function)
        {
            using (var tokenHandle = credentials.Impersonate(logonType))
            {
                return RunImpersonated(tokenHandle, function);
            }
        }

        private static void RunImpersonated(SafeAccessTokenHandle tokenHandle, Action<SafeAccessTokenHandle> action)
        {
            Microsoft.Win32.SafeHandles.SafeAccessTokenHandle accessTokenHandle = new Microsoft.Win32.SafeHandles.SafeAccessTokenHandle(tokenHandle.DangerousGetHandle());
            WindowsIdentity.RunImpersonated(accessTokenHandle, () => action(tokenHandle));
        }

        private static T RunImpersonated<T>(SafeAccessTokenHandle tokenHandle, Func<SafeAccessTokenHandle, T> function)
        {
            Microsoft.Win32.SafeHandles.SafeAccessTokenHandle accessTokenHandle = new Microsoft.Win32.SafeHandles.SafeAccessTokenHandle(tokenHandle.DangerousGetHandle());
            return WindowsIdentity.RunImpersonated(accessTokenHandle, () => function(tokenHandle));
        }
    }
}