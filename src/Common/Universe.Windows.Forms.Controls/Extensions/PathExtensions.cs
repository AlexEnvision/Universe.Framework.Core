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
using System.Linq;

namespace Universe.Windows.Forms.Controls.Extensions
{
    /// <summary>
    ///     Расширения для работы с путями к файлам и папкам.
    /// <author>Alex Envision</author>
    /// </summary>
    public static class PathExtensions
    {
        /// <summary>
        ///     Получает родительскую папку элемента и путь к ней
        /// </summary>
        /// <param name="path">Путь к файлу, либо к папке</param>
        /// <returns></returns>
        public static string GetParentFolder(this string path)
        {
            var dirSeparators = new[] { '\\', '/' };

            if (!string.IsNullOrEmpty(path))
            {
                var chunks = path.Split(dirSeparators);
                var lastChunk = chunks.LastOrDefault();
                if (!string.IsNullOrEmpty(lastChunk))
                    return path.Replace(lastChunk, string.Empty);
            }

            return null;
        }

        /// <summary>
        ///     Получает название файла из пути к нему
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns></returns>
        public static string GetFileName(this string path)
        {
            var dirSeparators = new[] { '\\', '/' };

            if (!string.IsNullOrEmpty(path))
            {
                var chunks = path.Split(dirSeparators);
                var lastChunk = chunks.LastOrDefault();
                if (!string.IsNullOrEmpty(lastChunk))
                    return lastChunk;
            }

            return null;
        }

        /// <summary>
        ///     Получает название папки из пути к ней
        /// </summary>
        /// <param name="path">Путь к папке</param>
        /// <returns></returns>
        public static string GetFolderName(this string path)
        {
            var dirSeparators = new[] { '\\', '/' };

            if (!string.IsNullOrEmpty(path))
            {
                var chunks = path.Split(dirSeparators);
                var lastChunk = chunks.LastOrDefault();
                if (!string.IsNullOrEmpty(lastChunk))
                    return lastChunk;
            }

            return null;
        }

        /// <summary>
        ///     Объединение нескольких частей директории.
        /// </summary>
        /// <param name="parts">Части URL.</param>
        /// <returns></returns>
        public static string Combine(params string[] parts)
        {
            var fUrl = parts.FirstOrDefault();
            if (fUrl == null || string.IsNullOrEmpty(fUrl))
                return string.Empty;

            if (fUrl.EndsWith("\\"))
                fUrl = fUrl.TrimEnd('\\');

            var sParts = new List<string>();
            for (int i = 1; i < parts.Length; i++)
            {
                var sUrl = parts[i];
                if (sUrl.StartsWith("\\"))
                    sUrl = sUrl.TrimStart('\\');

                sParts.Add(sUrl);
            }

            return fUrl + "\\" + string.Join("\\", sParts);
        }
    }
}