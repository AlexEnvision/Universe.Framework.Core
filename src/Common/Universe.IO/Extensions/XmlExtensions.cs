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
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Universe.IO.Extensions
{
    /// <summary>
    ///      Расширения для XML
    /// <author>Alex Envision</author>
    /// </summary>
    public static class XmlExtensions
    {
        /// <summary>
        ///     Сериализация в XML-строку
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dataContract">Сериализуемый объект</param>
        /// <param name="indented">Применять форматирование с отступами к xml</param>
        /// <returns></returns>
        public static string SerializeObject<TEntity>(TEntity dataContract, bool indented = true)
        {
            var xmlWriterSettings = new XmlWriterSettings
            {
                Indent = indented,
                OmitXmlDeclaration = false,
                Encoding = Encoding.UTF8,
                NamespaceHandling = NamespaceHandling.OmitDuplicates,
            };

            using (var ms = new UniverseMemoryStream())
            {
                ms.AllowClose = false;
                using (var writer = XmlWriter.Create(ms, xmlWriterSettings))
                {

                    XmlSerializer serializer = new XmlSerializer(typeof(TEntity));
                    serializer.Serialize(writer, dataContract);
                    ms.Position = 0;
                    using (var sr = new StreamReader(ms))
                    {
                        var content = sr.ReadToEnd();
                        return content;
                    }
                }
            }
        }

        /// <summary>
        ///     Десериализация строку XML в объект..
        /// </summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <param name="xml">XML-строка</param>
        /// <returns>Десериализованный из XML объект</returns>
        public static T DeserializeObject<T>(string xml) where T : new()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            StringReader stringReader = new StringReader(xml);
            var xmlObject = (T)xmlSerializer.Deserialize(stringReader);
            return xmlObject;
        }

        /// <summary>
        ///     Сериализация в XML-строку с заголовком
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dataContract"></param>
        /// <returns></returns>
        public static string SerializeObjectWithHeader<TEntity>(TEntity dataContract)
        {
            var xmlWriterSettings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true,
                Encoding = Encoding.UTF8,
                NamespaceHandling = NamespaceHandling.OmitDuplicates,
            };

            using (var ms = new UniverseMemoryStream())
            {
                ms.AllowClose = false;
                using (var writer = XmlWriter.Create(ms, xmlWriterSettings))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(TEntity));
                    serializer.Serialize(writer, dataContract);
                    ms.Position = 0;
                    using (var sr = new StreamReader(ms))
                    {
                        var content = sr.ReadToEnd();
                        var mes = $"<?xml version=\"1.0\" encoding=\"utf-8\"?>{Environment.NewLine}{content}";
                        return mes;
                    }
                }
            }
        }

        /// <summary>
        ///     Форматирование XML с отступами
        /// </summary>
        /// <param name="xml">XML для форматирования</param>
        /// <returns></returns>
        public static string FormatXml(this string xml)
        {
            try
            {
                XDocument doc = XDocument.Parse(xml);
                return doc.ToString();
            }
            catch (Exception ex)
            {
                throw new XmlException("Во время форматирования XML произошла ошибка.", ex);
            }
        }

        /// <summary>
        ///     Преобразование XML в Base64-строку
        /// </summary>
        /// <param name="xml">XML для преобразования</param>
        /// <returns></returns>
        public static string XmlToBase64String(this string xml)
        {
            if (string.IsNullOrEmpty(xml))
                return string.Empty;

            var buffer = Encoding.UTF8.GetBytes(xml);
            var base64String = Convert.ToBase64String(buffer);
            return base64String;
        }

        /// <summary>
        ///     Преобразование из Base64-строки в XML
        /// </summary>
        /// <param name="base64String">Base64-строка для преобразования</param>
        /// <returns></returns>
        public static string Base64StringToXml(this string base64String)
        {
            if (string.IsNullOrEmpty(base64String))
                return string.Empty;

            var buffer = Convert.FromBase64String(base64String);
            var xml = Convert.ToBase64String(buffer);
            return xml;
        }

        /// <summary>
        ///     Поиск совпадения начала xml-тега в xml
        /// </summary>
        /// <param name="xml">XML-строка</param>
        /// <param name="tagName">Название xml-тега</param>
        /// <returns></returns>
        public static string BeginXmlTagMatch(this string xml, string tagName)
        {
            return SearchTagMatch(tagName, xml);
        }

        /// <summary>
        ///     Поиск совпадения окончания xml-тега в xml
        /// </summary>
        /// <param name="xml">XML-строка</param>
        /// <param name="tagName">Название xml-тега</param>
        /// <returns></returns>
        public static string EndXmlTagMatch(this string xml, string tagName)
        {
            var asEndTagRecord = $"/{tagName}";
            return SearchTagMatch(asEndTagRecord, xml);
        }

        /// <summary>
        ///     Поиск совпадения тега
        /// </summary>
        /// <param name="xml">XML-строка</param>
        /// <param name="tagName">Название xml-тега</param>
        /// <returns></returns>
        private static string SearchTagMatch(string xml, string tagName)
        {
            if (string.IsNullOrEmpty(tagName))
                return string.Empty;

            var tagNameLettersNum = tagName.Length;
            var lowerCaseText = xml.ToLower();
            var lowerTagName = tagName.ToLower().Trim();
            var matches = 0;

            var startSearchingTagIndex = 0;
            var openTagSearchOver = false;
            for (var i = 0; i < lowerCaseText.Length; i++)
            {
                var textLetter = lowerCaseText[i];
                if (tagNameLettersNum != matches)
                    for (var index = matches; index < lowerTagName.Length;)
                    {
                        var tagLetter = lowerTagName[index];
                        if (textLetter == tagLetter)
                        {
                            matches++;
                            if (index == 0)
                                startSearchingTagIndex = i;
                            break;
                        }

                        matches = 0;
                        break;
                    }
                else
                {
                    if (!openTagSearchOver)
                        for (var index = startSearchingTagIndex; index >= 0; index--)
                        {
                            var revLetter = lowerCaseText[index];
                            if (revLetter != '<')
                                continue;

                            startSearchingTagIndex = index;
                            openTagSearchOver = true;
                            break;
                        }

                    if (textLetter != '>')
                        continue;

                    var endSearchingTagIndex = i;
                    var result = xml.Substring(startSearchingTagIndex, (endSearchingTagIndex + 1) - startSearchingTagIndex);
                    return result;
                }
            }

            return string.Empty;
        }
    }
}