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

using System.Linq;

namespace Universe.Helpers.Extensions
{
    /// <summary>
    ///
    /// <author>Alex Envision</author>
    /// </summary>
    public static class MatrixExtensions
    {
        /// <summary>
        ///     Преобразование многомерного массива в матрицу
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jArray">Многомерный массив</param>
        /// <returns></returns>
        public static T[,] ToMatrix<T>(this T[][] jArray)
        {
            int i = jArray.Count();
            int j = jArray.Select(x => x.Count()).Aggregate(0, (current, c) => (current > c) ? current : c);


            var mArray = new T[i, j];

            for (int ii = 0; ii < i; ii++)
            {
                for (int jj = 0; jj < j; jj++)
                {
                    mArray[ii, jj] = jArray[ii][jj];
                }
            }

            return mArray;
        }

        /// <summary>
        ///     Преобразование матрицы в многомерный массив
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mArray">Матрица</param>
        /// <returns></returns>
        public static T[][] ToMultiDimensionArray<T>(this T[,] mArray)
        {
            var cols = mArray.GetLength(0);
            var rows = mArray.GetLength(1);
            var jArray = new T[cols][];
            for (int i = 0; i < cols; i++)
            {
                jArray[i] = new T[rows];
                for (int j = 0; j < rows; j++)
                {
                    jArray[i][j] = mArray[i, j];
                }
            }
            return jArray;
        }
    }
}
