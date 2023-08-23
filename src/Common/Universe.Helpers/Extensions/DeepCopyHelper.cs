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
using System.Reflection;

namespace Universe.Helpers.Extensions
{
    /// <summary>
    ///     Расширения для клонирования объекта.
    ///     <author>Alex Envision</author>
    /// </summary>
    public static class DeepCopyHelper
    {
        private static readonly MethodInfo memberwiseClone = typeof(object)
            .GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);

        private static void MakeArrayRowDeepCopy(Dictionary<object, object> state,
            Array array, int[] indices, int rank)
        {
            int nextRank = rank + 1;
            int upperBound = array.GetUpperBound(rank);

            while (indices[rank] <= upperBound)
            {
                object value = array.GetValue(indices);
                if (!ReferenceEquals(value, null))
                    array.SetValue(CreateDeepCopyInternal(state, value), indices);

                if (nextRank < array.Rank)
                    MakeArrayRowDeepCopy(state, array, indices, nextRank);

                indices[rank] += 1;
            }

            indices[rank] = array.GetLowerBound(rank);
        }

        private static Array CreateArrayDeepCopy(Dictionary<object, object> state, Array array)
        {
            Array result = (Array) array.Clone();
            int[] indices = new int[result.Rank];
            for (int rank = 0; rank < indices.Length; ++rank)
                indices[rank] = result.GetLowerBound(rank);
            MakeArrayRowDeepCopy(state, result, indices, 0);
            return result;
        }

        private static object CreateDeepCopyInternal(Dictionary<object, object> state,
            object o)
        {
            object existObject;
            if (state.TryGetValue(o, out existObject))
                return existObject;

            if (o is Array)
            {
                object arrayCopy = CreateArrayDeepCopy(state, (Array) o);
                state[o] = arrayCopy;
                return arrayCopy;
            }

            if (o is string)
            {
                object stringCopy = string.Copy((string) o);
                state[o] = stringCopy;
                return stringCopy;
            }

            Type oType = o.GetType();
            if (oType.IsPrimitive)
                return o;
            object copy = memberwiseClone.Invoke(o, null);
            state[o] = copy;
            foreach (FieldInfo f in oType.GetFields(
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                object original = f.GetValue(o);
                if (!ReferenceEquals(original, null))
                    f.SetValue(copy, CreateDeepCopyInternal(state, original));
            }

            return copy;
        }

        public static T CreateDeepCopy<T>(this T o)
        {
            object input = o;
            if (ReferenceEquals(o, null))
                return o;
            return (T) CreateDeepCopyInternal(new Dictionary<object, object>(), input);
        }
    }
}