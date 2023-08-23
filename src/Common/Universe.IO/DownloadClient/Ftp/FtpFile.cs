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
using System.Net;
using Universe.IO.DownloadClient.Interfaces;

namespace Universe.IO.DownloadClient.Ftp
{
    /// <summary>
    ///     Класс работы с файлом, который находится в ftp хранилище.
    /// <author>Alex Envision</author>
    /// </summary>
    public class FtpFile : FtpElement, IDownloadFile
    {
        /// <summary>
        ///     Конструктор класса <see cref="FtpFile"/>
        /// </summary>
        /// <param name="downloadClient">
        ///     Интерфейс предоставляющий взаимодействие с хранилищем.
        /// </param>
        /// <param name="folderUri">
        ///     Путь к папке.
        /// </param>
        /// <param name="fileName">
        ///     Наименование файла.
        /// </param>
        internal FtpFile(IDownloadClient downloadClient, Uri folderUri, string fileName) : base(downloadClient)
        {
            if (fileName == null)
                throw new ArgumentNullException(nameof(fileName));

            Name = fileName;
            Uri = new Uri(Path.Combine(folderUri.AbsoluteUri, Path.GetFileName(fileName)));
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public override Uri Uri { get; }

        /// <inheritdoc />
        public byte[] Download()
        {
            using (var stream = GetResponse(WebRequestMethods.Ftp.DownloadFile))
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        /// <inheritdoc />
        public void Remove()
        {
            GetResponse(WebRequestMethods.Ftp.DeleteFile);
        }
    }
}