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
using Universe.Types.Collection.LinkedList;

namespace Universe.Framework.ConsoleApp.Tests.Collections
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    public class LinkedMatListCollectionTest
    {
        public void Test()
        {
            LinkedMatList<string> linkedList = new LinkedMatList<string>();

            Console.WriteLine(@"Обычный способ: ");

            // добавление элементов
            linkedList.Add("Марина");
            linkedList.Add("Александр");
            linkedList.Add("Алиса");
            linkedList.Add("Алекс");
            linkedList.Add("Джон");

            // выводим элементы
            foreach (var item in linkedList)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine();

            // удаляем элемент
            linkedList.Remove("Марина");
            foreach (var item in linkedList)
            {
                Console.WriteLine(item);
            }
            // проверяем наличие элемента
            bool isPresent = linkedList.Contains("Алекс");
            Console.WriteLine(isPresent == true ? "Алекс присутствует" : "Алекс отсутствует");

            // добавляем элемент в начало            
            linkedList.AppendFirst("Фёдор");

            // Копирум текущее состояние в другой список
            var secondLinkedList = new LinkedMatList<string>();
            foreach (var item in linkedList)
            {
                secondLinkedList.Add(item);
            }

            Console.WriteLine();
            Console.WriteLine(@"Математический способ: ");

            // добавление элементов математическое
            linkedList += "Пётр";
            linkedList += "Розамунд";
            linkedList += "Марина";
            linkedList += "Мария";

            // выводим элементы снова
            foreach (var item in linkedList)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine();

            // удаляем элемент
            linkedList -= "Алиса";
            foreach (var item in linkedList)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine();
            Console.WriteLine(@"Вычитание коллекции из коллекции: ");

            linkedList -= secondLinkedList;
            foreach (var item in linkedList)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine();
            Console.WriteLine(@"Тестирование связанного списка завершено!");
        }
    }
}