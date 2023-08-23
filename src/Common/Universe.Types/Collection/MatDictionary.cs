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

namespace Universe.Types.Collection
{
    /// <summary>
    ///     Математически обрабатываемый словарь
    /// <author>Alex Envision</author>
    /// </summary>
    public class MatDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        private Dictionary<TKey, TValue> _elements = new Dictionary<TKey, TValue>();

        public new TValue this[TKey key]
        {
            get => base[key] == null ? _elements[key] : base[key];
            set => base[key] = value;
        }

        protected Dictionary<TKey, TValue> X
        {
            get => _elements.Count == 0 ? new Dictionary<TKey, TValue>() : _elements;
            set => _elements = value;
        }

        public MatDictionary(Dictionary<TKey, TValue> x = null)
        {
            if (x == null)
                return;

            this.AddRange(x);
            X = x;
        }

        public MatDictionary(MatDictionary<TKey, TValue> x = null)
        {
            if (x == null)
                return;

            this.AddRange(x);
            X = x;
        }

        public MatDictionary()
        {
        }

        private static void SetSelf(MatDictionary<TKey, TValue> source, Dictionary<TKey, TValue> setDict)
        {
            source.AddRange(setDict);
        }

        // Перегружаем бинарный оператор +
        public static MatDictionary<TKey, TValue> operator +(MatDictionary<TKey, TValue> obj1, MatDictionary<TKey, TValue> obj2)
        {
            var arr = new MatDictionary<TKey, TValue>();
            var lobj1 = obj1.X;
            var lobj2 = obj2.X;

            lobj1.AddRange(lobj2);
            arr.X = lobj1;
            SetSelf(arr, arr.X);
            return arr;
        }

        // Перегружаем бинарный оператор +
        public static MatDictionary<TKey, TValue> operator +(MatDictionary<TKey, TValue> obj1, Dictionary<TKey, TValue> obj2)
        {
            var arr = new MatDictionary<TKey, TValue>();
            var lobj1 = obj1.X;
            var lobj2 = obj2;

            lobj1.AddRange(lobj2);
            arr.X = lobj1;
            SetSelf(arr, arr.X);
            return arr;
        }

        // Перегружаем бинарный оператор +
        public static MatDictionary<TKey, TValue> operator +(Dictionary<TKey, TValue> obj1, MatDictionary<TKey, TValue> obj2)
        {
            var arr = new MatDictionary<TKey, TValue>();
            var lobj1 = obj1;
            var lobj2 = obj2.X;

            lobj1.AddRange(lobj2);
            arr.X = lobj1;
            SetSelf(arr, arr.X);
            return arr;
        }

        public static MatDictionary<TKey, TValue> operator +(MatDictionary<TKey, TValue> obj1, (TKey, TValue) obj2)
        {
            var arr = new MatDictionary<TKey, TValue>();
            var lobj1 = obj1.X;
            var lobj2 = obj2;

            if (!lobj1.ContainsKey(lobj2.Item1))
                lobj1.Add(lobj2.Item1, lobj2.Item2);
            else
                lobj1[lobj2.Item1] = lobj2.Item2;

            arr.X = lobj1;
            SetSelf(arr, arr.X);
            return arr;
        }

        public static MatDictionary<TKey, TValue> operator +((TKey, TValue) obj1, MatDictionary<TKey, TValue> obj2)
        {
            var arr = new MatDictionary<TKey, TValue>();
            var lobj1 = obj2.X;
            var lobj2 = obj1;

            if(!lobj1.ContainsKey(lobj2.Item1))
                lobj1.Add(lobj2.Item1, lobj2.Item2);
            else
                lobj1[lobj2.Item1] = lobj2.Item2;

            arr.X = lobj1;
            SetSelf(arr, arr.X);
            return arr;
        }

        // Перегружаем бинарный оператор -
        public static MatDictionary<TKey, TValue> operator -(MatDictionary<TKey, TValue> obj1, MatDictionary<TKey, TValue> obj2)
        {
            var arr = new MatDictionary<TKey, TValue>();
            var lobj1 = obj1.X;
            var lobj2 = obj2.X;

            foreach (var item in lobj2)
            {
                if (lobj1.ContainsKey(item.Key))
                    lobj1.Remove(item.Key);
            }

            arr.X = lobj1;
            SetSelf(arr, arr.X);
            return arr;
        }

        // Перегружаем бинарный оператор -
        public static MatDictionary<TKey, TValue> operator -((TKey, TValue) obj1, MatDictionary<TKey, TValue> obj2)
        {
            var arr = new MatDictionary<TKey, TValue>();
            var lobj1 = obj2.X;
            var lobj2 = obj1;

            if (lobj1.ContainsKey(lobj2.Item1))
                lobj1.Remove(lobj2.Item1);

            arr.X = lobj1;
            SetSelf(arr, arr.X);
            return arr;
        }

        // Перегружаем бинарный оператор -
        public static MatDictionary<TKey, TValue> operator -(MatDictionary<TKey, TValue> obj1, (TKey, TValue) obj2)
        {
            var arr = new MatDictionary<TKey, TValue>();
            var lobj1 = obj1.X;
            var lobj2 = obj2;

            if (lobj1.ContainsKey(lobj2.Item1))
                lobj1.Remove(lobj2.Item1);

            arr.X = lobj1;
            SetSelf(arr, arr.X);
            return arr;
        }


        // Перегружаем бинарный оператор -
        public static MatDictionary<TKey, TValue> operator -(Dictionary<TKey, TValue> obj1, MatDictionary<TKey, TValue> obj2)
        {
            var arr = new MatDictionary<TKey, TValue>();
            var lobj1 = obj1;
            var lobj2 = obj2.X;

            foreach (var item in lobj2)
            {
                if (lobj1.ContainsKey(item.Key))
                    lobj1.Remove(item.Key);
            }

            arr.X = lobj1;
            SetSelf(arr, arr.X);
            return arr;
        }

        // Перегружаем бинарный оператор -
        public static MatDictionary<TKey, TValue> operator -(MatDictionary<TKey, TValue> obj1, Dictionary<TKey, TValue> obj2)
        {
            var arr = new MatDictionary<TKey, TValue>();
            var lobj1 = obj1.X;
            var lobj2 = obj2;

            foreach (var item in lobj2)
            {
                if (lobj1.ContainsKey(item.Key))
                    lobj1.Remove(item.Key);
            }

            arr.X = lobj1;
            SetSelf(arr, arr.X);
            return arr;
        }
    }
}