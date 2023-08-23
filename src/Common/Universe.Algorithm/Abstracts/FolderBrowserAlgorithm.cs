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
using System.Linq;
using System.Threading;
using Universe.Algorithm.MultiThreading;

namespace Universe.Algorithm.Abstracts
{
    /// <summary>
    ///     Алгоритмы обзора папок
    /// <author>Alex Envision</author>
    /// </summary>
    public class FolderBrowserAlgorithm
    {
        /// <summary>
        /// Рекурсивный обзор папок
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="folderPath"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static List<T> OpenFolderWithResult<T>(string folderPath, Func<T> func)
        {
            var results = new List<T>();
            var result = func.Invoke();

            if (result != null)
                results.Add(result);

            var childFolders = Directory.GetDirectories(folderPath);
            foreach(var childFolder in childFolders)
            {
                var childResults = OpenFolderWithResult(childFolder, func);

                if (childResults != null && childResults.Count > 0)
                    results.AddRange(childResults);
            }

            return results;
        }

        /// <summary>
        /// Рекурсивный обзор папок
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="folderPath"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static void OpenFolderWithResult(string folderPath, Action<string> func)
        {
            func.Invoke(folderPath);
            var childFolders = Directory.GetDirectories(folderPath);
            foreach (var childFolder in childFolders)
            {
                OpenFolderWithResult(childFolder, func);
            }
        }

        /// <summary>
        /// Рекурсивный обзор папок.
        /// В распараллеленной очереди из указанного количества потоков
        /// </summary>
        /// <param name="folderPath">Путь к папке</param>
        /// <param name="func">Функция</param>
        /// <param name="threadMachine">Машина потоков</param>
        /// <param name="threadsAmount">Число потоков. Задействуется, если не передавалась машина потоков</param>
        /// <returns></returns>
        public static void OpenFolderParallelQueue(string folderPath, Action<string> func, ThreadMachine threadMachine = null, int threadsAmount = 8)
        {
            if (threadMachine == null)
                threadMachine = ThreadMachine.Create(threadsAmount);

            threadMachine.RunInMultiTheadsQueueWithoutWaiting(() => {
                func.Invoke(folderPath);
            });

            var childFolders = Directory.GetDirectories(folderPath);
            foreach (var childFolder in childFolders)
            {
                OpenFolderParallelQueue(childFolder, func, threadMachine);
            }
        }

        /// <summary>
        /// Рекурсивный массовый обзор папок
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="folderPath"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static List<T> OpenFolderMassWithResult<T>(Func<T> func, params string[] folderPath)
        {
            var results = new List<T>();
            var result = func.Invoke();

            if (result != null)
                results.Add(result);

            var childFolders = folderPath.SelectMany(x => Directory.GetDirectories(x)).ToArray();
            var childResults = OpenFolderMassWithResult(func, childFolders);

            if (childResults != null && childResults.Count > 0)
                results.AddRange(childResults);

            return results;
        }

        /// <summary>
        /// Рекурсивный массовый обзор папок
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="folderPath"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static void OpenFolderMassWithResult(Action<string[]> func, params string[] folderPath)
        {
            func.Invoke(folderPath);
            var childFolders = folderPath.SelectMany(x => Directory.GetDirectories(x)).ToArray();
            if (childFolders.Length > 0)
                OpenFolderMassWithResult(func, childFolders);
        }

        /// <summary>
        /// Рекурсивный массовый обзор папок
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="folderPath"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static void OpenFolderMassParallelWithResult(string folderPath, Action<string> func)
        {
            var coresAmount = Environment.ProcessorCount;
            OpenFolderMassParallelWithResultInternal(folderPath, func, coresAmount);
        }

        /// <summary>
        /// Рекурсивный массовый обзор папок
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="folderPath"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        private static void OpenFolderMassParallelWithResultInternal(string folderPath, Action<string> func, int threadsAmount)
        {
            func.Invoke(folderPath);
            var childFolders = Directory.GetDirectories(folderPath);
            var threadMachine = ThreadMachine.Create(threadsAmount);
            for (int index = 0; index < childFolders.Length; index+=threadsAmount)
            {
                string[] items;
                if (index + threadsAmount > childFolders.Length - 1)
                {
                    var distance = (childFolders.Length - 1) - index;
                    items = childFolders.Skip(index).Take(distance).ToArray();
                    threadMachine = ThreadMachine.Create(distance);
                }
                else
                {
                    items = childFolders.Skip(index).Take(threadsAmount).ToArray();
                }

                List<ThreadStart> startActions = new List<ThreadStart>();
                foreach(var item in items)
                {
                    var action = new ThreadStart(() => OpenFolderWithResult(item, func));
                }
                var actionsArray = startActions.ToArray();
                threadMachine.RunInMultiTheadsWithoutWaiting(actionsArray);
            }
        }
    }
}
