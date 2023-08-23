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

namespace Universe.Framework.ThreadMachine.Tests.Multificator
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    public class Normalizator
    {
        public Normalizator()
        {

        }

        public NResult Execute(Result result)
        {
            var dimention = result.Matrix[1].Length;

            var maxValue = result.Matrix.Where(x => x != null).Max(x => x.Where(y => y != null).Max());
            var minValue = result.Matrix[0][0];

            var matrix = new double[dimention][];
            for (var i = 0; i < dimention; i++)
            {
                matrix[i] = new double[dimention];
                for (var j = 0; j < dimention; j++)
                {
                    var val = result.Matrix[i][j];
                    matrix[i][j] = Normalize(val, minValue, maxValue);
                }
            }

            return new NResult
            {
                Matrix = matrix,
                MaxValue = maxValue,
                MinValue = minValue,
            };
        }

        public NResult Restore(NResult result)
        {
            var dimention = result.Matrix[1].Length;

            var maxValue = result.MaxValue;
            var minValue = result.MinValue;

            var matrix = new double[dimention][];
            for (var i = 0; i < dimention; i++)
            {
                matrix[i] = new double[dimention];
                for (var j = 0; j < dimention; j++)
                {
                    var val = result.Matrix[i][j];
                    matrix[i][j] = Denormalize(val, minValue, maxValue);
                }
            }

            return new NResult
            {
                Matrix = matrix
            };
        }

        private double Normalize(long value, long minValue, long maxValue)
        {
            var subtract = maxValue - minValue;
            var mathRelation = (2 * value - subtract) / (subtract * 1.0);
            return mathRelation;
        }

        private double Denormalize(double nmrValue, long minValue, long maxValue)
        {
            var subtract = maxValue - minValue;
            var origValue = (nmrValue * subtract + subtract) / 2.0;
            return origValue;
        }
    }
}
