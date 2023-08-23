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

namespace Universe.Helpers.Utils
{
    /// <summary>
    ///     Общие математические и числовые служебные функции
    /// <author>Alex Envision</author>
    /// </summary>
    public static class MathUtils
    {
        /// <summary>
        ///     В итоге получается немного быстрее, чем {Math.Round(float)}. Это просто округляет его
        ///     аргумент до ближайшего целого числа, где x.5 округляется до x + 1. Семантика этого ярлыка
        ///     немного отличается от {Math.Round(float)} тем, что на половину округляет отрицательное значение
        ///     значения. -2,5 оборота до -3, а не -2. Для этих целей здесь это не имеет значения.
        /// </summary>
        /// <param name="d">real value to round</param>
        /// <returns>nearest <c>int</c></returns>
        public static int Round(float d)
        {
            if (float.IsNaN(d))
                return 0;
            if (float.IsPositiveInfinity(d))
                return int.MaxValue;
            return (int) (d + (d < 0.0f ? -0.5f : 0.5f));
        }

        /// <summary>
        /// </summary>
        /// <param name="aX"></param>
        /// <param name="aY"></param>
        /// <param name="bX"></param>
        /// <param name="bY"></param>
        /// <returns>Евклидово расстояние между точками A и B</returns>
        public static float Distance(float aX, float aY, float bX, float bY)
        {
            double xDiff = aX - bX;
            double yDiff = aY - bY;
            return (float) Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
        }

        /// <summary>
        /// </summary>
        /// <param name="aX"></param>
        /// <param name="aY"></param>
        /// <param name="bX"></param>
        /// <param name="bY"></param>
        /// <returns>Евклидово расстояние между точками A и B</returns>
        public static float Distance(int aX, int aY, int bX, int bY)
        {
            double xDiff = aX - bX;
            double yDiff = aY - bY;
            return (float) Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
        }

        /// <summary>
        /// </summary>
        /// <param name="array">Значения для суммирования</param>
        /// <returns>Сумма значений в массиве</returns>
        public static int Sum(int[] array)
        {
            var count = 0;
            foreach (var a in array)
                count += a;
            return count;
        }
    }
}