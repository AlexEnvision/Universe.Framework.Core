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

using System.Collections;
using System.Collections.Generic;

namespace Universe.Types.Collection.LinkedList
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    public class LinkedMatList<T> : IEnumerable<T>
    {
        private Node<T> _head; // головной/первый элемент
        private Node<T> _tail; // последний/хвостовой элемент
        private int _count;  // количество элементов в списке

        public int Count => _count;

        public bool IsEmpty => _count == 0;

        /// <summary>
        /// Добавление элемента
        /// </summary>
        /// <param name="data"></param>
        public void Add(T data)
        {
            Node<T> node = new Node<T>(data);

            if (_head == null)
            {
                _head = node;
            }
            else
            {
                _tail.Next = node;
            }

            _tail = node;
            _count++;
        }

        /// <summary>
        /// Добавление без последнего элемента
        /// </summary>
        /// <param name="data"></param>
        public void AddWithoutTail(T data)
        {
            Node<T> node = new Node<T>(data);

            if (_head == null)
            {
                _head = node;
            }
            else
            {
                Node<T> current = _head;
                // ищем последний элемент
                while (current.Next != null)
                {
                    current = current.Next;
                }
                //устанавливаем последний элемент
                current.Next = node;
            }

            _count++;
        }

        /// <summary>
        /// Удаление элемента
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Remove(T data)
        {
            Node<T> current = _head;
            Node<T> previous = null;

            while (current != null)
            {
                if (current.Data.Equals(data)) //TODO -> м.б изменить сравнение объектов?
                {
                    // Если узел в середине или в конце
                    if (previous != null)
                    {
                        // убираем узел current, теперь previous ссылается не на current, а на current.Next
                        previous.Next = current.Next;

                        // Если current.Next не установлен, значит узел последний,
                        // изменяем переменную tail
                        if (current.Next == null)
                            _tail = previous;
                    }
                    else
                    {
                        // если удаляется первый элемент
                        // переустанавливаем значение head
                        _head = _head.Next;

                        // если после удаления список пуст, сбрасываем tail
                        if (_head == null)
                            _tail = null;
                    }

                    _count--;
                    return true;
                }

                previous = current;
                current = current.Next;
            }

            return false;
        }

        public static LinkedMatList<T> operator +(LinkedMatList<T> obj1, T obj2)
        {
            obj1.Add(obj2);
            return obj1;
        }

        public static LinkedMatList<T> operator -(LinkedMatList<T> obj1, T obj2)
        {
            obj1.Remove(obj2);
            return obj1;
        }

        public static LinkedMatList<T> operator +(LinkedMatList<T> obj1, LinkedMatList<T> obj2)
        {
            foreach (var item in obj2)
            {
                obj1.Add(item);
            }
            return obj1;
        }

        public static LinkedMatList<T> operator -(LinkedMatList<T> obj1, LinkedMatList<T> obj2)
        {
            foreach (var item in obj2)
            {
                obj1.Remove(item);
            }
            return obj1;
        }

        /// <summary>
        /// Очистка списка
        /// </summary>
        public void Clear()
        {
            _head = null;
            _tail = null;
            _count = 0;
        }

        /// <summary>
        /// Cодержит ли список элемент
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Contains(T data)
        {
            Node<T> current = _head;
            while (current != null)
            {
                if (current.Data.Equals(data)) //TODO -> м.б изменить сравнение объектов?
                    return true;

                current = current.Next;
            }
            return false;
        }

        /// <summary>
        /// Добавление в начало
        /// </summary>
        /// <param name="data"></param>
        public void AppendFirst(T data)
        {
            Node<T> node = new Node<T>(data);
            node.Next = _head;
            _head = node;

            if (_count == 0)
            {
                _tail = _head;
            }

            _count++;
        }

        public IEnumerator<T> GetEnumerator()
        {
            Node<T> current = _head;
            while (current != null)
            {
                yield return current.Data;
                current = current.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this).GetEnumerator();
        }
    }
}