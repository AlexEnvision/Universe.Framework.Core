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
using System.Linq;
using Universe.Helpers.Extensions;

namespace Universe.Framework.ConsoleApp.Tests.BaseTypesTest
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    public class ReverseExtensionsTest
    {
        private string _exampleText;

        public string ExampleText => !string.IsNullOrWhiteSpace(_exampleText) ? _exampleText : ReadFile();

        public const string InputFileName = "subtitlesbase.txt";

        public const string OutputFileName = "subtitlesbase_out.txt";

        public const string StdOutputFileName = "subtitlesbase_out_std.txt";

        private string ReadFile()
        {
            using (var tf = File.OpenText($"{Directory.GetCurrentDirectory()}\\BaseTypesTest\\Text\\{InputFileName}"))
            {
                var content = tf.ReadToEnd();
                tf.Close();
                _exampleText = content;
                return content;
            }
        }

        private void SaveFile(string content, string outputFileName)
        {
            using (var tf = File.CreateText($"{Directory.GetCurrentDirectory()}\\BaseTypesTest\\Text\\{outputFileName}"))
            {
                tf.WriteLine(content);
                tf.Flush();
                tf.Close();
            }
        }

        public void Test()
        {
            var originalContent = ExampleText;

            var reversedContent = originalContent.AlgReverse();

            SaveFile(reversedContent, OutputFileName);

            var reversedStandard = new string(originalContent.Reverse().ToArray());

            SaveFile(reversedStandard, StdOutputFileName);

            Console.WriteLine(@"Порядок символов в тексте изменен на противоположный. Для продолжения нажмите любую клавишу...");
            Console.ReadLine();
        }
    }
}
