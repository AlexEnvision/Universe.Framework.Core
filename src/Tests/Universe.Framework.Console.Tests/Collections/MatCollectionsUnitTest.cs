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

namespace Universe.Framework.ConsoleApp.Tests.Collections
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    public class MatCollectionsUnitTest
    {
        public void Test()
        {
            MatList<string> fstArg = new MatList<string>()
                {"Inferno.2016.1080p.BluRay.Rus.Ukr.Eng.HDCLUB", "The.Da.Vinci.Code.HDTV.720p.x264.Rus-001"};
            MatList<string> sndArg = new MatList<string>()
                {"Angels & Demons [2-in-1 Theatrical & Extended Cut].2009.BDRip.1080p"};

            var sum = fstArg + sndArg;

            var fstDict =
                new List<int>() { 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577 }.ToMatDictionary(x => x.ToString());
            var sndDict =
                new List<int>() { 66578, 66579, 66580, 66581, 66582, 66583, 66584, 66585, 66586, 66587, 66588 }.ToMatDictionary(
                    x => x.ToString());

            var sumDict = fstDict + sndDict;
            sumDict += ("45678", 45678);
        }
    }
}