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
using Universe.Framework.ConsoleApp.Tests.Algoritms;
using Universe.Framework.ConsoleApp.Tests.Arithmetic;
using Universe.Framework.ConsoleApp.Tests.BaseTypesTest;
using Universe.Framework.ConsoleApp.Tests.Collections;
using Universe.Framework.ConsoleApp.Tests.CQRS;
using Universe.Framework.ConsoleApp.Tests.IO;
using Universe.Framework.ConsoleApp.Tests.REST;
using Universe.Framework.ConsoleApp.Tests.TextProcessing;

namespace Universe.Framework.ConsoleApp.Tests
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    public class Program
    {     
        public static void Main(string[] args)
        {
            var universeModelDeserializerTest = new UniverseModelDeserializerTest();
            universeModelDeserializerTest.Run();

            var duckDuckGoTest = new DuckDuckGoTest();
            duckDuckGoTest.Run();

            var mathAlgTest = new MathAlgTest();
            mathAlgTest.Run();

            var longArithmeticTest = new LongArithmeticTest();
            longArithmeticTest.Test();

            var matColTest = new MatCollectionsTest();
            matColTest.Test();

            var serializableDictionaryTest = new SerializableDictionaryTest();
            serializableDictionaryTest.Test();

            var linkedMatListTest = new LinkedMatListCollectionTest();
            linkedMatListTest.Test();

            var textTokenizerTest = new TextTokenizerTest();
            textTokenizerTest.Test();

            var batchProcessTest = new BatchProcessesTests();
            batchProcessTest.Run();

            var commandQueryTests = new CommandQueryTests();
            commandQueryTests.GetEntitiesDirectSqlQueryTest();
            commandQueryTests.CreateEntityCommandTest();
            commandQueryTests.CreateEntityCommandTransactionTest();
            commandQueryTests.CreateEntitiesCommandTest();
            commandQueryTests.CreateAndUndoEntityCommandTest();
            commandQueryTests.ReadEntityQueryTest();
            commandQueryTests.ReadEntitiesQueryByDatePeriodTest();
            commandQueryTests.ReadEntitiesQueryTest();
            commandQueryTests.UpdateEntityCommandTest();
            commandQueryTests.UpdateEntitiesCommandTest();
            commandQueryTests.UpdateEntityAndUndoCommandTest();
            commandQueryTests.DeleteEntityQueryTest();
            commandQueryTests.DeleteEntitiesQueryTest();

            var entityReqFullTest = new EntityReqFullTest();
            entityReqFullTest.CreateEntityQueryTest();
            entityReqFullTest.CreateEntityQueryTransactionTest();
            entityReqFullTest.CreateEntitiesQueryTest();
            entityReqFullTest.ReadEntitiesQueryTest();
            entityReqFullTest.UpdateEntitiesCommandTest();
            entityReqFullTest.DeleteEntitiesCommandTest();

            var reverseExtensionsTest = new ReverseExtensionsTest();
            reverseExtensionsTest.Test();

            var sortingAlgorithmsTest = new SortingAlgorithmsTest();
            sortingAlgorithmsTest.Test();

            Console.WriteLine(@"Для продолжения нажмите любую клавишу...");
            Console.ReadLine();
        }
    }
}
