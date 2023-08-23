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

namespace Universe.Types.Collection
{
    /// <summary>
    /// Математически обрабатываемая коллекция
    /// <author>Alex Envision</author>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MatList<T> : List<T>
    {
        private List<T> _elements = new List<T>();

        public new T this[int index]
        {
            get => base[index] == null ? _elements[index] : base[index];
            set => base[index] = value;
        }

        protected List<T> X
        {
            get => _elements.Count == 0 ? ToArray().ToList() : _elements;
            set => _elements = value;
        }

        public MatList(List<T> x = null)
        {
            if (x == null)
                return;

            AddRange(x);
            X = x;
        }

        public MatList(T[] x = null)
        {
            if (x == null)
                return;

            AddRange(x);
            X = x.ToList();
        }

        public MatList(MatList<T> x = null)
        {
            if (x == null)
                return;

            AddRange(x);
            X = x;
        }

        public MatList(int capacity)
        {
            if (capacity == 0)
                return;

            var x = new List<T>();
            for (var index = 0; index < capacity; index++)
            {
                var item = default(T);
                x.Add(item);
            }

            AddRange(x);
            X = x;
        }

        public MatList()
        {
        }

        private static void SetSelf(MatList<T> source, List<T> setList)
        {
            source.AddRange(setList);
        }

        // Перегружаем бинарный оператор +
        public static MatList<T> operator +(MatList<T> obj1, MatList<T> obj2)
        {
            var arr = new MatList<T>();
            var lobj1 = obj1.X;
            var lobj2 = obj2.X;

            lobj1.AddRange(lobj2);
            arr.X = lobj1;
            SetSelf(arr, arr.X);
            return arr;
        }

        // Перегружаем бинарный оператор +
        public static MatList<T> operator +(MatList<T> obj1, List<T> obj2)
        {
            var arr = new MatList<T>();
            var lobj1 = obj1.X;
            var lobj2 = obj2;

            lobj1.AddRange(lobj2);
            arr.X = lobj1;
            SetSelf(arr, arr.X);
            return arr;
        }

        // Перегружаем бинарный оператор +
        public static MatList<T> operator +(List<T> obj1, MatList<T> obj2)
        {
            var arr = new MatList<T>();
            var lobj1 = obj1;
            var lobj2 = obj2.X;

            lobj1.AddRange(lobj2);
            arr.X = lobj1;
            SetSelf(arr, arr.X);
            return arr;
        }

        public static MatList<T> operator +(MatList<T> obj1, T obj2)
        {
            var arr = new MatList<T>();
            var lobj1 = obj1.X;
            var lobj2 = obj2;

            lobj1.Add(lobj2);
            arr.X = lobj1;
            SetSelf(arr, arr.X);
            return arr;
        }

        public static MatList<T> operator +(T obj1, MatList<T> obj2)
        {
            var arr = new MatList<T>();
            var lobj1 = obj2.X;
            var lobj2 = obj1;

            lobj1.Add(lobj2);
            arr.X = lobj1;
            SetSelf(arr, arr.X);
            return arr;
        }

        // Перегружаем бинарный оператор -
        public static MatList<T> operator -(MatList<T> obj1, MatList<T> obj2)
        {
            var arr = new MatList<T>();
            var lobj1 = obj1.X;
            var lobj2 = obj2.X;

            foreach (var item in lobj2)
            {
                lobj1.Remove(item);
            }

            arr.X = lobj1;
            SetSelf(arr, arr.X);
            return arr;
        }

        // Перегружаем бинарный оператор -
        public static MatList<T> operator -(T obj1, MatList<T> obj2)
        {
            var arr = new MatList<T>();
            var lobj1 = obj2.X;
            var lobj2 = obj1;

            lobj1.Remove(lobj2);

            arr.X = lobj1;
            SetSelf(arr, arr.X);
            return arr;
        }

        // Перегружаем бинарный оператор -
        public static MatList<T> operator -(MatList<T> obj1, T obj2)
        {
            var arr = new MatList<T>();
            var lobj1 = obj1.X;
            var lobj2 = obj2;

            lobj1.Remove(lobj2);

            arr.X = lobj1;
            SetSelf(arr, arr.X);
            return arr;
        }


        // Перегружаем бинарный оператор -
        public static MatList<T> operator -(List<T> obj1, MatList<T> obj2)
        {
            var arr = new MatList<T>();
            var lobj1 = obj1;
            var lobj2 = obj2.X;

            foreach (var item in lobj2)
            {
                lobj1.Remove(item);
            }

            arr.X = lobj1;
            SetSelf(arr, arr.X);
            return arr;
        }

        // Перегружаем бинарный оператор -
        public static MatList<T> operator -(MatList<T> obj1, List<T> obj2)
        {
            var arr = new MatList<T>();
            var lobj1 = obj1.X;
            var lobj2 = obj2;

            foreach (var item in lobj2)
            {
                lobj1.Remove(item);
            }

            arr.X = lobj1;
            SetSelf(arr, arr.X);
            return arr;
        }
    }
}