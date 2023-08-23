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
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Universe.IO.Validators
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    public class Sha256Validator
    {
        public string Hash(string data)
        {
            using (var sha256 = SHA256.Create())
                return GetSHA256Hash(sha256.ComputeHash(Encoding.UTF8.GetBytes(data)));
        }

        public string Hash(Stream data)
        {
            using (var sha256 = SHA256.Create())
                return GetSHA256Hash(sha256.ComputeHash(data));
        }

        public bool Verify(string data, string hash)
        {
            using (var sha256 = SHA256.Create())
                return VerifySha256Hash(sha256.ComputeHash(Encoding.UTF8.GetBytes(data)), hash);
        }

        public bool Verify(Stream data, string hash)
        {
            using (var sha256 = SHA256.Create())
                return VerifySha256Hash(sha256.ComputeHash(data), hash);
        }

        private string GetSHA256Hash(byte[] data)
        {
            var sBuilder = new StringBuilder();
            foreach (var bt in data)
            {
                sBuilder.Append(bt.ToString("x2"));
            }

            return sBuilder.ToString();
        }

        private bool VerifySha256Hash(byte[] data, string hash)
        {
            return 0 == StringComparer.OrdinalIgnoreCase.Compare(GetSHA256Hash(data), hash);
        }
    }
}