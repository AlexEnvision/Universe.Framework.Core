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
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Universe.IO.DownloadClient.Interfaces;
using Universe.IO.Security.Principal;

namespace Universe.IO.DownloadClient.Folder
{
    /// <summary>
    ///     Элемент сетевой папки
    /// <author>Alex Envision</author>
    /// </summary>
    public abstract class FolderElement
    {
        protected readonly IDownloadClient DownloadClient;

        protected FolderElement(IDownloadClient downloadClient)
        {
            if (downloadClient == null)
                throw new ArgumentNullException(nameof(downloadClient));

            DownloadClient = downloadClient;
        }

        public abstract Uri Uri { get; }

        protected Stream GetResponse(string method)
        {
            var credentials = DownloadClient.Credentials as NetworkCredential;
            if (credentials != null)
            {
                using (var ctx = new ImpersonationContext())
                {
                    return ctx.RunUnderImpersonationContext(credentials.UserName, credentials.Password,
                        credentials.Domain,
                        () => GetResponseInternal(method));
                }
            }
            else
            {
                using (var ctx = new ImpersonationContext())
                {
                    return ctx.RunUnderImpersonationContext(
                        () => GetResponseInternal(method));
                }
            }

            return GetResponseInternal(method);
        }

        private Stream GetResponseInternal(string method)
        {
            switch (method)
            {
                case WebRequestMethods.Ftp.ListDirectory:
                    var result = Directory.GetDirectories(Uri.LocalPath).ToList();
                    result.AddRange(Directory.GetFiles(Uri.LocalPath));
                    var join = string.Join("\r\n", result);
                    return new MemoryStream(Encoding.UTF8.GetBytes(join));
                default:
                    throw new NotImplementedException();
            }
        }
    }
}