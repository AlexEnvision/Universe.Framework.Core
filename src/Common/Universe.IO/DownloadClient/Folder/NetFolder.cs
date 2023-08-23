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
using System.IO;
using System.Net;
using Universe.IO.DownloadClient.Extensions;
using Universe.IO.DownloadClient.Interfaces;
using Universe.IO.DownloadClient.Upload;
using Universe.IO.Security.Principal;

namespace Universe.IO.DownloadClient.Folder
{
    /// <summary>
    ///     Сетевая папка
    /// <author>Alex Envision</author>
    /// </summary>
    public class NetFolder : FolderElement, IDownloadFolder
    {
        private readonly List<IDownloadFile> _files = new List<IDownloadFile>();

        internal NetFolder(IDownloadClient downloadClient, string folder) : base(downloadClient)
        {
            Uri = downloadClient.Host;
            Name = folder;
            if (!string.IsNullOrEmpty(folder))
                Uri = new Uri(Path.Combine(downloadClient.Host.AbsoluteUri, folder));
        }

        public string Name { get; }

        public override Uri Uri { get; }

        public string AddFile(string fileName, byte[] body)
        {
            var credentials = DownloadClient.Credentials as NetworkCredential;
            if (credentials != null)
            {
                using (var ctx = new ImpersonationContext())
                {
                    return ctx.RunUnderImpersonationContext(credentials.UserName, credentials.Password,
                        credentials.Domain,
                        () => AddFileInternal(fileName, body));
                }
            }
            else
            {
                using (var ctx = new ImpersonationContext())
                {
                    return ctx.RunUnderImpersonationContext(
                        () => AddFileInternal(fileName, body));
                }
            }

            return AddFileInternal(fileName, body);
        }

        public IDownloadFolder CreateFolder(string folder)
        {
            var subRootFolder = new UploadFolder
            {
                Name = Path.GetFileName(folder)
            };
            return AddFolder(subRootFolder);
        }

        public IDownloadFolder AddFolder(string folder)
        {
            var subRootFolder = this.DownloadClient.BuildUploadFolder(folder) as UploadFolder;
            return AddFolder(subRootFolder);
        }

        private IDownloadFolder AddFolder(IUploadFolder folder)
        {
            var credentials = DownloadClient.Credentials as NetworkCredential;
            if (credentials != null)
            {
                using (var ctx = new ImpersonationContext())
                {
                    return ctx.RunUnderImpersonationContext(credentials.UserName, credentials.Password,
                        credentials.Domain,
                        () => AddFolderInternal(folder));
                }
            }
            else
            {
                using (var ctx = new ImpersonationContext())
                {
                    return ctx.RunUnderImpersonationContext(
                        () => AddFolderInternal(folder));
                }
            }

            return AddFolderInternal(folder);
        }

        public List<IDownloadFile> GetFiles()
        {
            LoadList(WebRequestMethods.Ftp.ListDirectory);
            return _files;
        }

        public List<IDownloadFolder> GetFolders()
        {
            return LoadList(WebRequestMethods.Ftp.ListDirectory);
        }

        public string RemoveFile(string fileName)
        {
            var credentials = DownloadClient.Credentials as NetworkCredential;
            if (credentials != null)
            {
                using (var ctx = new ImpersonationContext())
                {
                    ctx.RunUnderImpersonationContext(credentials.UserName, credentials.Password, credentials.Domain,
                        () => RemoveFileInternal(fileName));
                }
            }
            else
            {
                using (var ctx = new ImpersonationContext())
                {
                    ctx.RunUnderImpersonationContext(
                        () => RemoveFileInternal(fileName));
                }
            }

            RemoveFileInternal(fileName);
            return "ОК";
        }

        public string RemoveFolder()
        {
            var credentials = DownloadClient.Credentials as NetworkCredential;

            var folderPath = Path.Combine(Uri.LocalPath, this.Name);
            if (credentials != null)
            {
                using (var ctx = new ImpersonationContext())
                {
                    ctx.RunUnderImpersonationContext(credentials.UserName, credentials.Password, credentials.Domain,
                        () => Directory.Delete(folderPath, true));
                }
            }
            else
            {
                //Directory.Delete(folderPath, true);
                using (var ctx = new ImpersonationContext())
                {
                    ctx.RunUnderImpersonationContext(
                        () => Directory.Delete(folderPath, true));
                }
            }

            return "ОК";
        }

        private string AddFileInternal(string fileName, byte[] body)
        {
            var filePath = Path.Combine(Uri.LocalPath, fileName);
            File.WriteAllBytes(filePath, body);
            return string.Empty;
        }

        private void AddFileInternal(string fileName, byte[] body, string currentFolder)
        {
            var relativePath = Path.Combine(currentFolder, fileName);
            var filePath = Path.Combine(Uri.LocalPath, relativePath);
            File.WriteAllBytes(filePath, body);
        }

        private IDownloadFolder AddFolderInternal(IUploadFolder folder, string currentFolder = "")
        {
            var relativePath = Path.Combine(currentFolder, folder.Name);
            var folderPath = Path.Combine(Uri.LocalPath, relativePath);

            if (!folderPath.IsDirectory())
            {
                Directory.CreateDirectory(folderPath);
            }

            foreach (var subFolder in folder.SubFolders)
            {
                AddFolderInternal(subFolder, relativePath);
            }

            foreach (var folderFile in folder.Files)
            {
                AddFileInternal(folderFile.Name, folderFile.Body, relativePath);
            }

            return new NetFolder(DownloadClient, folderPath);
        }

        private List<IDownloadFolder> LoadList(string method)
        {
            _files.Clear();
            var result = new List<IDownloadFolder>();
            using (var reader = new StreamReader(GetResponse(method)))
                while (reader.Peek() != -1)
                {
                    var readLine = reader.ReadLine();
                    var fileName = Path.GetFileName(readLine);

                    if (readLine.IsDirectory())
                        result.Add(new NetFolder(DownloadClient, readLine));
                    else
                        _files.Add(new FolderFile(DownloadClient, Uri, fileName));
                }

            return result;
        }

        private void RemoveFileInternal(string fileName)
        {
            var filePath = Path.Combine(Uri.LocalPath, fileName);
            File.Delete(filePath);
        }
    }
}