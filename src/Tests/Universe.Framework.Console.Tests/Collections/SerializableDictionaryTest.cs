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
using Newtonsoft.Json;
using Universe.Framework.ConsoleApp.Tests.Models;
using Universe.IO.Extensions;
using Universe.Types.Collection.Extensions;

namespace Universe.Framework.ConsoleApp.Tests.Collections
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    public class SerializableDictionaryTest
    {
        public void Test()
        {
            Console.WriteLine(@"Тест SerializableDictionary...");

            var catalog = new CompactDiscsLibrary();

            var xmlResult = XmlExtensions.SerializeObject(catalog);
            Console.WriteLine($@"Результат в виде XML: {Environment.NewLine}{xmlResult}");

            var sourceModel = XmlExtensions.DeserializeObject<CompactDiscsLibrary>(xmlResult);
            var jsonResult = JsonConvert.SerializeObject(sourceModel, Formatting.Indented);
            Console.WriteLine($@"Исходная модель в виде JSON: {Environment.NewLine}{jsonResult}");

            var compactDiskDict = new Dictionary<string, CompactDiscInfo>
            {
                {
                    "Eleine - Dancing in Hell", new CompactDiscInfo
                    {
                        Title = "Dancing in Hell",
                        Performer = "Eleine",
                        Date = "2020",
                        Genre = "Symphonic Metal"
                    }
                },
                {
                    "Eleine - Until The End", new CompactDiscInfo
                    {
                        Title = "Until The End",
                        Performer = "Eleine",
                        Date = "2018",
                        Genre = "Symphonic Metal"
                    }
                }
            };
            catalog.CompactDiscs.AddRange(compactDiskDict);
        
            var xmlAddResult = XmlExtensions.SerializeObject(catalog);
            Console.WriteLine($@"Результат в виде XML с добавленными CD в каталог: {Environment.NewLine}{xmlAddResult}");

            Console.WriteLine(@"Для продолжения нажмите любую клавишу...");
            Console.ReadLine();
        }
    }
}