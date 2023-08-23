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

namespace Universe.NumbesTypes
{

    /// <summary>
    /// Determines the format of the string produced by Quad.ToString(QuadrupleStringFormat).
    /// ScientificApproximate is the default.
    /// </summary>
    public enum QuadupleStringFormat //QuadrupleStringFormat
    {
        /// <summary>
        /// Obtains the quadruple in scientific notation.  Only ~52 bits of significand precision are used to create this string.
        /// </summary>
        ScientificApproximate,

        /// <summary>
        /// Obtains the quadruple in scientific notation with full precision.  This can be very expensive to compute and takes time linear in the value of the exponent.
        /// </summary>
        ScientificExact,

        /// <summary>
        /// Obtains the quadruple in hexadecimal exponential format, consisting of a 64-bit hex integer followed by the binary exponent,
        /// also expressed as a (signed) 64-bit hexadecimal integer.
        /// E.g. ffffffffffffffff*2^-AB3
        /// </summary>
        HexExponential,

        /// <summary>
        /// Obtains the quadruple in decimal exponential format, consisting of a 64-bit decimal integer followed by the 64-bit signed decimal integer exponent.
        /// E.g. 34592233*2^34221
        /// </summary>
        DecimalExponential
    }
}