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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Universe.Diagnostic.Utilities
{
    public static class DiagnosticUtilities
    {
        /// <summary>
        ///     Вычисление оптимистичного размера любого управляемого объекта.
        ///     Получение минимального объема памяти <paramref name = "someObject" />.
        ///     Подсчитываются все поля <paramref />, включая автоматически сгенерированные, private и protected.
        ///     Не учитываются: любые статические поля, любые свойства, функции, методы-члены.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static long SizeInBytes<T>(this T someObject)
        {
            var temp = new Size<T>(someObject);
            var tempSize = temp.GetSizeInBytes();
            return tempSize;
        }

        /// <summary>
        ///     Способ оценки размера в памяти любого управляемого объекта
        /// </summary>
        /// <typeparam name="TT"></typeparam>
        private sealed class Size<TT>
        {
            private static readonly int pointerSize = Environment.Is64BitOperatingSystem
                ? sizeof(long)
                : sizeof(int);

            private readonly TT _obj;
            private readonly HashSet<object> _references;

            public Size(TT obj)
            {
                _obj = obj;
                _references = new HashSet<object> {_obj};
            }

            public long GetSizeInBytes()
            {
                return GetSizeInBytes(_obj);
            }

            private long GetSizeInBytes<T>(T obj)
            {
                if (obj == null) return sizeof(int);
                var type = obj.GetType();

                if (type.IsPrimitive)
                {
                    switch (Type.GetTypeCode(type))
                    {
                        case TypeCode.Boolean:
                        case TypeCode.Byte:
                        case TypeCode.SByte:
                            return sizeof(byte);
                        case TypeCode.Char:
                            return sizeof(char);
                        case TypeCode.Single:
                            return sizeof(float);
                        case TypeCode.Double:
                            return sizeof(double);
                        case TypeCode.Int16:
                        case TypeCode.UInt16:
                            return sizeof(short);
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                            return sizeof(int);
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                            return sizeof(UInt64);
                        default:
                            return sizeof(long);
                    }
                }

                if (obj is decimal)
                {
                    return sizeof(decimal);
                }

                if (obj is string)
                {
                    return sizeof(char) * obj.ToString().Length;
                }

                if (type.IsEnum)
                {
                    return sizeof(int);
                }

                if (type.IsArray)
                {
                    long sizeTemp = pointerSize;
                    var casted = (IEnumerable) obj;
                    foreach (var item in casted)
                    {
                        sizeTemp += GetSizeInBytes(item);
                    }

                    return sizeTemp;
                }

                if (obj is Pointer)
                {
                    return pointerSize;
                }

                long size = 0;
                var t = type;
                while (t != null)
                {
                    size += pointerSize;
                    var fields =
                        t.GetFields(BindingFlags.Instance | 
                                    BindingFlags.Public | 
                                    BindingFlags.NonPublic |
                                    BindingFlags.DeclaredOnly);
                    foreach (var field in fields)
                    {
                        var tempVal = field.GetValue(obj);
                        if (!_references.Contains(tempVal))
                        {
                            _references.Add(tempVal);
                            size += GetSizeInBytes(tempVal);
                        }
                    }

                    t = t.BaseType;
                }

                return size;
            }
        }
    }
}