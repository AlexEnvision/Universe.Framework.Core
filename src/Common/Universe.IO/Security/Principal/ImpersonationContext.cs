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
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Microsoft.Win32.SafeHandles;

namespace Universe.IO.Security.Principal
{
    /// <summary>
    /// Switching component security settings for threads.
    /// </summary>
    public sealed class ImpersonationContext : IDisposable
    {
        /// <summary>
        /// A sign, whether made under impersonalization pool account.
        /// </summary>
        private bool _runningAsAppPool;

        /// <summary>
        /// The sign has been impersonalization under a special account.
        /// </summary>
        private bool _runningAsUser;

        /// <summary>
        /// A pointer to the user token.
        /// </summary>
        private IntPtr _userHandle = IntPtr.Zero;

        ///// <summary>
        ///// The context of impersonation.
        ///// </summary>
        //private WindowsImpersonationContext _wix;

        /// <summary>
        /// The constructor for the class.
        /// </summary>
        public ImpersonationContext()
        {
        }

        public void RunUnderImpersonationContext(Action action)
        {
            RunAsAppPool(action);
        }

        public void RunUnderImpersonationContext(string user, string password, string domain, Action action)
        {
            RunAsUser(user, password, domain, action);
        }

        public T RunUnderImpersonationContext<T>(Func<T> func)
        {
            return RunAsAppPool(func);
        }

        public T RunUnderImpersonationContext<T>(string user, string password, string domain, Func<T> func)
        {
            return RunAsUser(user, password, domain, func);
        }

        /// <summary>
        /// Closes open handes returned by LogonUser.
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool CloseHandle(IntPtr handle);

        /// <summary>
        /// Obtains user token.
        /// </summary>
        /// <param name="pszUsername"></param>
        /// <param name="pszDomain"></param>
        /// <param name="pszPassword"></param>
        /// <param name="dwLogonType"></param>
        /// <param name="dwLogonProvider"></param>
        /// <param name="phToken"></param>
        /// <returns></returns>
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LogonUser(
            string pszUsername,
            string pszDomain,
            string pszPassword,
            int dwLogonType,
            int dwLogonProvider,
            ref IntPtr phToken);

        public enum LogonType : int
        {
            Interactive = 2,
            Network = 3,
            Batch = 4,
            Service = 5,
            Unlock = 7,
            NetworkClearText = 8,
            NewCredentials = 9
        }

        public enum LogonProvider : int
        {
            Default = 0,  // LOGON32_PROVIDER_DEFAULT
            WinNT35 = 1,
            WinNT40 = 2,  // Use the NTLM logon provider.
            WinNT50 = 3   // Use the negotiate logon provider.
        }

        /// <summary>
        /// Cleanup resources.
        /// </summary>
        public void Dispose()
        {
            RevertToLoggedInUser();
            //if (_wix != null)
            //{
            //    _wix.Dispose();
            //    _wix = null;
            //}
        }

        /// <summary>
        /// Returns the context of the current thread to the context of the original user.
        /// </summary>
        public void RevertToLoggedInUser()
        {
            if (_runningAsAppPool)
            {
                //_wix.Undo();
                _runningAsAppPool = false;
            }

            if (_runningAsUser)
            {
                //_wix?.Undo();

                _runningAsUser = false;
                if (_userHandle != IntPtr.Zero)
                    CloseHandle(_userHandle);
            }
        }

        /// <summary>
        /// Changes the context impersonate the current thread to the context of the application pool.
        /// </summary>
        /// <param name="action"></param>
        public void RunAsAppPool(Action action)
        {
            if (_runningAsAppPool || _runningAsUser)
                return;

            //_wix = WindowsIdentity.Impersonate(IntPtr.Zero);

            var safeUserHandle = new SafeAccessTokenHandle(_userHandle);
            WindowsIdentity.RunImpersonated(safeUserHandle, action);
            _runningAsAppPool = true;
        }

        /// <summary>
        /// Changes the context impersonate the current thread to the context of the application pool.
        /// </summary>
        /// <param name="func"></param>
        public T RunAsAppPool<T>(Func<T> func)
        {
            if (_runningAsAppPool || _runningAsUser)
                return default(T);

            //_wix = WindowsIdentity.Impersonate(IntPtr.Zero);

            var safeUserHandle = new SafeAccessTokenHandle(_userHandle);

            _runningAsAppPool = true;
            return WindowsIdentity.RunImpersonated(safeUserHandle, func);
        }

        /// <summary>
        /// Changes the context impersonate the current thread to the context of the account with options.
        /// </summary>
        /// <param name="user">
        ///     Login account (no domain).
        /// </param>
        /// <param name="password">
        ///     The password for the account.
        /// </param>
        /// <param name="domain">
        ///     User domain.
        /// </param>
        /// <param name="action"></param>
        public void RunAsUser(string user, string password, string domain, Action action)
        {
            const int logon32ProviderDefault = (int)LogonProvider.Default;
            const int logon32Logon = (int)LogonType.NewCredentials; //(int)LogonType.Interactive

            if (_runningAsAppPool || _runningAsUser)
                return;

            var arr = user.Split('\\');
            if (string.IsNullOrEmpty(domain))
                domain = arr.Length == 2 ? arr[0] : Environment.UserDomainName;

            user = arr.Last();
            var loggedOn = LogonUser(user, domain, password, logon32Logon, logon32ProviderDefault, ref _userHandle);
            if (!loggedOn)
            {
                var lastError = Marshal.GetLastWin32Error();
                throw new ArgumentException(
                    $"Не удалось выполнить имперсонализацию. Некорректно указаны данные {user}, {domain}",
                    new Exception($"Произошла ошибка {lastError} при входе в систему."));
            }

            //_wix = WindowsIdentity.Impersonate(_userHandle);

            var safeUserHandle = new SafeAccessTokenHandle(_userHandle);
            WindowsIdentity.RunImpersonated(safeUserHandle, action);
            _runningAsUser = true;
        }

        /// <summary>
        /// Changes the context impersonate the current thread to the context of the account with options.
        /// </summary>
        /// <param name="user">
        ///     Login account (no domain).
        /// </param>
        /// <param name="password">
        ///     The password for the account.
        /// </param>
        /// <param name="domain">
        ///     User domain.
        /// </param>
        /// <param name="func"></param>
        public T RunAsUser<T>(string user, string password, string domain, Func<T> func)
        {
            const int logon32ProviderDefault = (int)LogonProvider.Default;
            const int logon32Logon = (int)LogonType.NewCredentials;  //(int)LogonType.Interactive

            if (_runningAsAppPool || _runningAsUser)
                return default(T);

            var arr = user.Split('\\');
            if (string.IsNullOrEmpty(domain))
                domain = arr.Length == 2 ? arr[0] : Environment.UserDomainName;

            user = arr.Last();
            var loggedOn = LogonUser(user, domain, password, logon32Logon, logon32ProviderDefault, ref _userHandle);
            if (!loggedOn)
            {
                var lastError = Marshal.GetLastWin32Error();
                throw new ArgumentException(
                    $"Не удалось выполнить имперсонализацию. Некорректно указаны данные {user}, {domain}",
                    new Exception($"Произошла ошибка {lastError} при входе в систему."));
            }

            //_wix = WindowsIdentity.Impersonate(_userHandle);

            var safeUserHandle = new SafeAccessTokenHandle(_userHandle);
            _runningAsUser = true;
            return WindowsIdentity.RunImpersonated(safeUserHandle, func);
        }
    }
}