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

using System.IO;
using Newtonsoft.Json;

namespace Universe.Windows.Forms.Controls.Settings
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    public static class FormAppSettingsExtensions
    {
        public static T Load<T>(this T programSettings) where T: FormAppSettings, new()
        {
            if (programSettings == null)
                programSettings = new T();

            if (File.Exists(Directory.GetCurrentDirectory() + "\\Settings.json"))
            {
                using (var stream =
                    File.OpenText(Directory.GetCurrentDirectory() + "\\Settings.json"))
                {
                    // в тут же созданную копию класса GeneralSettings под именем _programSettings
                    var content = stream.ReadToEnd();
                    programSettings = JsonConvert.DeserializeObject<T>(content);

                    return programSettings;
                }
            }

            return new T();
        }

        public static T Load<T>(this T programSettings, string settingsName) where T : FormAppSettings, new()
        {
            if (programSettings == null)
                programSettings = new T();

            if (File.Exists(Directory.GetCurrentDirectory() + "\\" + settingsName))
            {
                using (var stream =
                    File.OpenText(Directory.GetCurrentDirectory() + "\\" + settingsName))
                {
                    // в тут же созданную копию класса GeneralSettings под именем _programSettings
                    var content = stream.ReadToEnd();
                    programSettings = JsonConvert.DeserializeObject<T>(content);

                    return programSettings;
                }
            }

            return new T();
        }

        public static void Save<T>(this T programSettings) where T : FormAppSettings, new()
        {
            using (var writer = File.CreateText("Settings.json"))
            {
                //собираем настройки для последующего сохранения
                programSettings = programSettings ?? new T();
                var asStr = JsonConvert.SerializeObject(programSettings, Formatting.Indented);
                writer.WriteLine(asStr);
            }
        }

        public static T Save<T>(this T programSettings, string settingsName) where T : FormAppSettings, new()
        {
            using (var writer = File.CreateText(settingsName))
            {
                //собираем настройки для последующего сохранения
                programSettings = programSettings ?? new T();
                var asStr = JsonConvert.SerializeObject(programSettings, Formatting.Indented);
                writer.WriteLine(asStr);

                return programSettings;
            }
        }
    }
}
