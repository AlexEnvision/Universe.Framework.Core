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

namespace Universe.Helpers.Extensions
{
    /// <summary>
    ///     Расширения для <see cref="double"/>
    /// <author>Alex Envision</author>
    /// </summary>
    public static class DoubleExtensions
    {
        /// <summary>
        /// Преобразует значение к типу Int32
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Значение, преобразованное к целому числу</returns>
        public static int AsInt(this double? value)
        {
            return !value.HasValue ? 0 : Convert.ToInt32(value.Value);
        }

        /// <summary>
        /// Проверяет значение на равенство с заданной точностью
        /// </summary>
        /// <param name="one">Текущее значение</param>
        /// <param name="other">С чем сравниваем</param>
        /// <param name="precision">Точность</param>
        /// <returns>True, если значение равны с заданной точностью</returns>
        public static bool IsEqualsTo(this double? one, double? other, double precision = 0.00001)
        {
            if (one == null && other == null)
                return true;

            if (one == null || other == null)
                return false;

            return Math.Abs(one.Value - other.Value) < precision;
        }
    }
}