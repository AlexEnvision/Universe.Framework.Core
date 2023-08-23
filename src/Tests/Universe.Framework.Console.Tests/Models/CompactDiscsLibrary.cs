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
using Universe.Types.Collection;
using Universe.Types.Collection.Extensions;

namespace Universe.Framework.ConsoleApp.Tests.Models
{
    /// <summary>
    ///     Каталог компакт-дисков.
    ///     Модель используемая для тестирования.
    /// <author>Alex Envision</author>
    /// </summary>
    public class CompactDiscsLibrary
    {
        public SerializableDictionary<string, CompactDiscInfo> CompactDiscs;

        public CompactDiscsLibrary()
        {
            var cdList = new List<CompactDiscInfo>
            {
                new CompactDiscInfo
                {
                    DiscId = "770BC508",
                    Title = "01011001 [Special Ed. CD1]: Y",
                    Performer = "Ayreon",
                    Date = "2008",
                    Genre = "Progressive Metal"
                },
                new CompactDiscInfo
                {
                    DiscId = "490C3207",
                    Title = "01011001 [Special Ed. CD2]: Earth",
                    Performer = "Ayreon",
                    Date = "2008",
                    Genre = "Progressive Metal"
                },
                new CompactDiscInfo
                {
                    DiscId = "8E0D150A",
                    Title = "Space Metal",
                    Performer = "Star One",
                    Date = "2002",
                    Genre = "Progressive Metal"
                },
                new CompactDiscInfo
                {
                    DiscId = "8410920C",
                    Title = "Omega",
                    Performer = "Epica",
                    Date = "2021",
                    Genre = "Symphonic Metal"
                },
                new CompactDiscInfo
                {
                    DiscId = "DD12590E",
                    Title = "Design Your Universe",
                    Performer = "Epica",
                    Date = "2009",
                    Genre = "Symphonic Metal"
                },
                new CompactDiscInfo
                {
                    DiscId = "9D0DD50B",
                    Title = "My Earth Dream",
                    Performer = "Edenbridge",
                    Date = "2008",
                    Genre = "Symphonic Metal"
                },
                new CompactDiscInfo
                {
                    DiscId = "710B8B09",
                    Title = "The Grand Design",
                    Performer = "Edenbridge",
                    Date = "2006",
                    Genre = "Symphonic Metal"
                }
            };

            CompactDiscs = cdList.ToSerializableDictionary(x => $"{x.Performer} - {x.Title}");
        }
    }
}