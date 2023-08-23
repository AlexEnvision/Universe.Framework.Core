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
using Universe.IO.DownloadClient.Interfaces;

namespace Universe.IO.DownloadClient.Ftp
{
    /// <summary>
    ///     Класс работы с папкой, которая находится в ftp хранилище.
    /// <author>Alex Envision</author>
    /// </summary>
    public class FtpFolder : FtpElement, IDownloadFolder
    {
        private readonly List<IDownloadFile> _files = new List<IDownloadFile>();

        internal FtpFolder(IDownloadClient downloadClient, string folder) : base(downloadClient)
        {
            Uri = DownloadClient.Host;
            Name = folder;
            if (!string.IsNullOrEmpty(folder))
                Uri = new Uri(Path.Combine(DownloadClient.Host.AbsoluteUri, folder));
        }

        public override Uri Uri { get; }

        public string Name { get; }

        public List<IDownloadFile> GetFiles()
        {
            LoadList(WebRequestMethods.Ftp.ListDirectory);
            return _files;
        }

        public List<IDownloadFolder> GetFolders()
        {
            return LoadList(WebRequestMethods.Ftp.ListDirectory);
        }

        public string AddFile(string fileName, byte[] body)
        {
            var ftpWebRequest = (FtpWebRequest)WebRequest.Create(Path.Combine(Uri.AbsoluteUri, fileName));
            if (DownloadClient.Credentials != null)
                ftpWebRequest.Credentials = DownloadClient.Credentials;

            ftpWebRequest.Method = WebRequestMethods.Ftp.UploadFile;
            ftpWebRequest.ContentLength = body.Length;
            ftpWebRequest.UsePassive = true;
            ftpWebRequest.UseBinary = true;
            ftpWebRequest.KeepAlive = true;

            using (var requestStream = ftpWebRequest.GetRequestStream())
            {
                requestStream.Write(body, 0, body.Length);
                requestStream.Close();

                using (var response = (FtpWebResponse)ftpWebRequest.GetResponse())
                    return response.StatusDescription;
            }
        }

        public IDownloadFolder CreateFolder(string folder)
        {
            throw new NotImplementedException();
        }

        public IDownloadFolder AddFolder(string folder)
        {
            throw new NotImplementedException();
        }

        public string RemoveFile(string fileName)
        {
            var ftpWebRequest = (FtpWebRequest)WebRequest.Create(Path.Combine(Uri.AbsoluteUri, fileName));
            if (DownloadClient.Credentials != null)
                ftpWebRequest.Credentials = DownloadClient.Credentials;

            ftpWebRequest.Method = WebRequestMethods.Ftp.DeleteFile;
            ftpWebRequest.UsePassive = true;
            ftpWebRequest.UseBinary = true;
            ftpWebRequest.KeepAlive = true;

            using (var response = (FtpWebResponse) ftpWebRequest.GetResponse())
            {
                return response.StatusDescription;
            }
        }

        public string RemoveFolder()
        {
            var ftpWebRequest = (FtpWebRequest)WebRequest.Create(Path.Combine(Uri.AbsoluteUri, this.Name));
            if (DownloadClient.Credentials != null)
                ftpWebRequest.Credentials = DownloadClient.Credentials;

            ftpWebRequest.Method = WebRequestMethods.Ftp.RemoveDirectory;
            ftpWebRequest.UsePassive = true;
            ftpWebRequest.UseBinary = true;
            ftpWebRequest.KeepAlive = true;

            using (var response = (FtpWebResponse)ftpWebRequest.GetResponse())
            {
                return response.StatusDescription;
            }
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

                    if (string.IsNullOrEmpty(Path.GetExtension(fileName)))
                        result.Add(new FtpFolder(DownloadClient, readLine));
                    else
                        _files.Add(new FtpFile(DownloadClient, Uri, fileName));
                }

            return result;
        }
    }
}