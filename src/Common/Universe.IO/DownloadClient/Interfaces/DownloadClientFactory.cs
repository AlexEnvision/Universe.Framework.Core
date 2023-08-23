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
using System.Net;
using Universe.IO.DownloadClient.Folder;
using Universe.IO.DownloadClient.Ftp;

namespace Universe.IO.DownloadClient.Interfaces
{
    /// <summary>
    ///     Фабрика клиентов, работающих с файлами и папками.
    /// <author>Alex Envision</author>
    /// </summary>
    public class DownloadClientFactory
    {
        /// <summary>
        ///     Создаёт клиент к указанной в настройках папке
        /// </summary>
        /// <param name="settings">Настройки папки: URL и учётные данные</param>
        /// <returns></returns>
        public static IDownloadClient Create(FolderSettings settings)
        {
            IDownloadClient client;
            ICredentials credentials = null;

            if (!settings.AllowRunUnderCurrentUser)
            {
                if (!string.IsNullOrEmpty(settings.UserName) && !string.IsNullOrEmpty(settings.Password))
                    credentials = new NetworkCredential(settings.UserName, settings.Password, settings.Domain);
            }

            if (settings.FolderUrl.Scheme == Uri.UriSchemeFile)
            {
                client = new FolderClient(settings.FolderUrl);
                client.Credentials = credentials;
            }
            else if (settings.FolderUrl.Scheme == Uri.UriSchemeFtp)
            {
                client = new FtpClient(settings.FolderUrl);
                client.Credentials = credentials;
            }
            else
            {
                throw new NotSupportedException("Uri scheme not supported");
            }

            return client;
        }
    }
}