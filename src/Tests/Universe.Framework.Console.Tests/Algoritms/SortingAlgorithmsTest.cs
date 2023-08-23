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
using Universe.Algorithm.Sorting;
using Universe.Diagnostic;

namespace Universe.Framework.ConsoleApp.Tests.Algoritms
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    public class SortingAlgorithmsTest
    {
        public void Test()
        {
            float[] testArray =
            {
                78,
                22,
                11,
                55,
                90,
                7,
                1,
                42,
                102
            };

            Console.WriteLine($@"До сортировки: {string.Join("; ", testArray)}");

            float[] sorted;

            using (var watcher = new RunningTimeWatcher())
            {               
                sorted = SortingAlgorithm.Sort(testArray, SortingType.Quck);
                watcher.FreezeTime();
                Console.WriteLine($@"После сортировки Quck: {string.Join("; ", sorted)}{Environment.NewLine}. Время выполнения: {watcher.TakeRunningTime().TotalMilliseconds} мс.");
                watcher.Reset();
                watcher.Continue();

                sorted = SortingAlgorithm.Sort(testArray, SortingType.Bubble);
                watcher.FreezeTime();
                Console.WriteLine($@"После сортировки Bubble: {string.Join("; ", sorted)}{Environment.NewLine}. Время выполнения: {watcher.TakeRunningTime().TotalMilliseconds} мс.");
                watcher.Reset();
                watcher.Continue();

                sorted = SortingAlgorithm.Sort(testArray, SortingType.Comb);
                watcher.FreezeTime();
                Console.WriteLine($@"После сортировки Comb: {string.Join("; ", sorted)}{Environment.NewLine}. Время выполнения: {watcher.TakeRunningTime().TotalMilliseconds} мс.");
                watcher.Reset();
                watcher.Continue();

                sorted = SortingAlgorithm.Sort(testArray, SortingType.Insertion);
                watcher.FreezeTime();
                Console.WriteLine($@"После сортировки Insertion: {string.Join("; ", sorted)}{Environment.NewLine}. Время выполнения: {watcher.TakeRunningTime().TotalMilliseconds} мс.");
                watcher.Reset();
                watcher.Continue();

                sorted = SortingAlgorithm.Sort(testArray, SortingType.Selection);
                watcher.FreezeTime();
                Console.WriteLine($@"После сортировки Selection: {string.Join("; ", sorted)}{Environment.NewLine}. Время выполнения: {watcher.TakeRunningTime().TotalMilliseconds} мс.");
                watcher.Reset();
                watcher.Continue();

                sorted = SortingAlgorithm.Sort(testArray, SortingType.Shaker);
                watcher.FreezeTime();
                Console.WriteLine($@"После сортировки Shaker: {string.Join("; ", sorted)}{Environment.NewLine}. Время выполнения: {watcher.TakeRunningTime().TotalMilliseconds} мс.");
                watcher.Reset();
                watcher.Continue();
            }

            Console.WriteLine(@"Тест завершён!");

            Console.ReadLine();
        }
    }
}
