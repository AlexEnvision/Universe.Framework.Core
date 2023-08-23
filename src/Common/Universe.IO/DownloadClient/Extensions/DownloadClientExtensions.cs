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

using System.Collections.Generic;
using System.IO;
using Universe.Helpers.Extensions;
using Universe.IO.DownloadClient.Interfaces;
using Universe.IO.DownloadClient.Upload;

namespace Universe.IO.DownloadClient.Extensions
{
    /// <summary>
    ///     Расширения для <see cref="IDownloadClient"/>.
    /// <author>Alex Envision</author>
    /// </summary>
    internal static class DownloadClientExtensions
    {
        public static IUploadFolder BuildUploadFolder(this IDownloadClient client, string folderPath)
        {
            var subRootFolder = new UploadFolder
            {
                Name = Path.GetFileName(folderPath)
            };

            return BuildUploadFolder(client, folderPath, subRootFolder);
        }

        public static IUploadFolder BuildUploadFolder(this IDownloadClient client, string folderPath, IUploadFolder currentFolder)
        {
            var folders = Directory.GetDirectories(folderPath);
            var foldersSchemas = new List<UploadFolder>();
            foreach (var folder in folders)
            {
                var name = Path.GetFileName(folder);
                if (name.IsNullOrEmpty())
                    continue;

                var uploadFolder = new UploadFolder
                {
                    Name = name
                };

                uploadFolder = BuildUploadFolder(client, folder, uploadFolder) as UploadFolder;
                foldersSchemas.Add(uploadFolder);
            }
            currentFolder.SubFolders.AddRange(foldersSchemas);

            var files = Directory.GetFiles(folderPath);
            var filesContent = new List<UploadFile>();
            foreach (var file in files)
            {
                var name = Path.GetFileName(file);
                var uploadFile = new UploadFile
                {
                    Name = name
                };

                using (var fs = File.Open(file, FileMode.Open))
                {
                    var buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);

                    uploadFile.Body = buffer;
                }
                filesContent.Add(uploadFile);
            }
            currentFolder.Files.AddRange(filesContent);

            return currentFolder;
        }
    }
}