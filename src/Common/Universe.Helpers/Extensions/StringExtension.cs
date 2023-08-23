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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Universe.Helpers.Extensions
{
    /// <summary>
    ///      Расширения для <see cref="string"/>
    /// <author>Alex Envision</author>
    /// </summary>
    public static class StringExtension
    {
        public static string FirstLetterToLower(this string str)
        {
            return !string.IsNullOrEmpty(str) ? str.Substring(0, 1).ToLower() + str.Substring(1, str.Length - 1) : str;
        }

        /// <summary>
        ///     Меняет порядок символов на противоположный.
        ///     Are changing symbols order on reversed.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string AlgReverse(this string str)
        {
            var charArray = new char[str.Length];
            var position = str.Length - 1;
            for (int i = 0; i < str.Length; i++)
            {
                charArray[position] = str[i];
                position--;
            }

            return new string(charArray);
        }

        /// <summary>
        ///     Подготовка к сравнению:
        ///     Удаление пробелов и преобразование в нижний регистр.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string PrepareToCompare(this string str)
        {
            if (str == null)
                return str;

            return str.Trim().ToLowerInvariant();
        }

        /// <summary>
        ///     Объединение (комбинирование) двух частей URL.
        ///     Combines two parts of the URL.
        /// </summary>
        /// <param name="url1">Часть URL 1.</param>
        /// <param name="url2">Часть URL 2.</param>
        /// <returns></returns>
        public static string CombineUrl(string url1, string url2)
        {
            if (url1.EndsWith("/"))
                url1 = url1.TrimEnd('/');

            if (url2.StartsWith("/"))
                url2 = url2.TrimStart('/');

            return url1 + "/" + url2;
        }

        /// <summary>
        ///     Объединение (комбинирование) нескольких частей URL.
        ///     Combines many parts of the URL.
        /// </summary>
        /// <param name="urls">Части URL.</param>
        /// <returns></returns>
        public static string CombineMultiPartUrl(params string[] urls)
        {
            var fUrl = urls.FirstOrDefault();
            if (fUrl == null || fUrl.IsNullOrEmpty())
                return string.Empty;

            if (fUrl.EndsWith("/"))
                fUrl = fUrl.TrimEnd('/');

            var sUrls = new List<string>();
            for (int i = 1; i < urls.Length; i++)
            {
                var sUrl = urls[i];
                if (sUrl.StartsWith("/"))
                    sUrl = sUrl.TrimStart('/');

                sUrls.Add(sUrl);
            }

            return fUrl + "/" + string.Join("/", sUrls);
        }

        /// <summary>
        ///     Eqs the ignore case.
        /// </summary>
        /// <param name="str1">The STR1.</param>
        /// <param name="str2">The STR2.</param>
        /// <returns></returns>
        public static bool EqIgnoreCase(this string str1, string str2)
        {
            return string.Compare(str1, str2, StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        ///     Gets the unique identifier in format document trix document list identifier.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <returns></returns>
        public static string GetGuidInFormatDocTrixDocumentListId(Guid guid)
        {
            return guid.ToString("B").ToUpper();
        }

        /// <summary>
        ///     Получение разметки гипертекстовой ссылки
        ///     Gets the href.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static string GetHref(string url, string name)
        {
            return "<a href =\"" + url.Replace(" ", "%20") + "\">" + name + "</a>";
        }

        /// <summary>
        ///     Determines whether [is null or empty].
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>
        ///   <c>true</c> if [is null or empty] [the specified string]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        ///     Determines whether [is null or WhiteSpace].
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>
        ///   <c>true</c> if [is null or WhiteSpace] [the specified string]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        ///     Replaces the format item in a specified string with the string representation of a corresponding object in a
        ///     specified array.
        /// </summary>
        /// <returns>
        ///     A copy of <paramref name="format" /> in which the format items have been replaced by the string representation of
        ///     the corresponding objects in <paramref name="args" />.
        /// </returns>
        /// <param name="format">
        ///     A composite format string.
        /// </param>
        /// <param name="args">
        ///     An object array that contains zero or more objects to format.
        /// </param>
        /// <exception cref="T:System.FormatException">
        ///     <paramref name="format" /> the index of a format item is greater than or equal to the length of the
        ///     <paramref name="args" /> array.
        /// </exception>
        /// <filterpriority>1</filterpriority>
        public static string F(this string format, params object[] args)
        {
            if (format == null)
                return null;

            if (args == null || args.Length < 1)
                return format;

            return string.Format(format, args);
        }

        /// <summary>
        ///     Поиск кириллических символов
        ///     Search cirillic characters
        /// </summary>
        /// <param name="str">The string</param>
        /// <returns></returns>
        public static bool IsContainsCirillicChars(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            foreach (var ch in str)
            {
                if (ch == ' ')
                    continue;

                if (ch >= 'а' && ch <= 'я' || ch >= 'А' && ch <= 'Я')
                    return true;
            }

            return false;
        }

        /// <summary>
        ///     Поиск латинских символов
        ///     Search lats characters
        /// </summary>
        /// <param name="str">The string</param>
        /// <returns></returns>
        public static bool IsContainLatsChars(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            foreach (var ch in str)
            {
                if (ch == ' ')
                    continue;

                if (ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z')
                    return true;
            }

            return false;
        }

        /// <summary>
        ///     Определяет, содержится ли указанный список вариантов.
        ///     Determines whether the specified variant list is contains.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="variantList">The variant list.</param>
        /// <returns>
        /// <c>true</c> if the specified variant list is contains; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">variantList</exception>
        public static bool IsContains(this string str, params string[] variantList)
        {
            if (str.IsNullOrEmpty())
                return false;

            if (variantList == null)
                throw new ArgumentNullException(nameof(variantList));

            return IsContains(str, variantList.ToList());
        }

        /// <summary>
        ///     Определяет, содержится ли указанный список вариантов.
        ///     Determines whether the specified variant list is contains.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="variantList">The variant list.</param>
        /// <returns>
        /// <c>true</c> if the specified variant list is contains; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsContains(this string str, IEnumerable<string> variantList)
        {
            if (str.IsNullOrEmpty())
                return false;

            if (variantList == null)
                return false;

            foreach (var variant in variantList)
            {
                if (string.CompareOrdinal(str, variant) == 0)
                    return true;
            }

            return false;
        }

        /// <summary>
        ///     Определяет, содержится ли указанный список вариантов и не начинается ли с него вариант
        ///     Determines whether the specified variant list is contains and doesn't start with variant
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="variant">The variant</param>
        /// <returns>
        /// <c>true</c> if the specified variant list is contains; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsContainsDoesntStartWithIgnoreCase(this string str, string variant)
        {
            if (str.IsNullOrEmpty())
                return false;

            var preparedToCompareStr = str.PrepareToCompare();

            if (variant.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(variant));

            return preparedToCompareStr.Contains(variant) && !str.StartsWith(variant, true, CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     Определяет, содержится ли указанный список вариантов и начинается ли с варианта
        ///     Determines whether the specified variant list is contains and start with variant
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="variant">The variant</param>
        /// <returns>
        /// <c>true</c> if the specified variant list is contains; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsContainsStartWithIgnoreCase(this string str, string variant)
        {
            if (str.IsNullOrEmpty())
                return false;

            var preparedToCompareStr = str.PrepareToCompare();

            if (variant.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(variant));

            return preparedToCompareStr.Contains(variant) && preparedToCompareStr.StartsWith(variant, true, CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     Добавление постфикса за именем файла, через символ подчёркивания
        ///     Adding postfix behind filename throught underline symbol
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="postfix"></param>
        /// <returns></returns>
        public static string AddPostfixBehindFileName(this string filePath, string postfix)
        {
            var dirName = Path.GetDirectoryName(filePath);
            var fileExtension = Path.GetExtension(filePath);
            var filenameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
            var nameWithPostfix = postfix.IsNullOrEmpty() ? filePath : $"{filenameWithoutExt}_{postfix}{fileExtension}";
            var renewDataPath = CombineUrl(dirName, nameWithPostfix);
            return renewDataPath;
        }

        /// <summary>
        ///     Частичное сравнение строк и отсечка в соответсвии с пороговым значением
        /// </summary>
        /// <param name="str1">Строка 1</param>
        /// <param name="str2">Строка 2</param>
        /// <param name="partialityThreshold">Пороговое значение</param>
        /// <returns></returns>
        public static bool PartialCompare(string str1, string str2, double partialityThreshold)
        {
            int len = str1.Length;
            if (len != str2.Length)
                throw new NotSupportedException("Этот метод пока не поддерживает сравнение разных по длине строк.");
            if (len == 0)
                throw new ArgumentException("Строка не должна быть пустой.");
            int equalChars = 0;
            for (int i = 0; i < len; i++)
                if (str1[i] == str2[i])
                    equalChars++;
            // Строки не обеспечивают достаточную степень сходства
            if ((double)equalChars / len < partialityThreshold)
                return false;
            // Строки частично равны
            return true;
        }

        /// <summary>
        ///     Частичное сравнение с возвратом показателя по результатам сравнения в процентах
        /// </summary>
        /// <param name="str1">Строка 1</param>
        /// <param name="str2">Строка 2</param>
        /// <returns></returns>
        public static string EqualsPartialityRercentage(string str1, string str2)
        {
            var percentageVal = EqualsPartiality(str1, str2) * 100.0;
            return $"{percentageVal}%";
        }

        /// <summary>
        ///     Вывод результат частичного сравления в процентах
        /// </summary>
        /// <param name="equalsPartiality">Показатель частичного сравнения</param>
        /// <returns></returns>
        public static string EqualsPartialityRercentage(double equalsPartiality)
        {
            var percentageVal = equalsPartiality * 100.0;
            return $"{percentageVal}%";
        }

        /// <summary>
        ///     Частичное сравнение с возвратом показателя по результатам сравнения
        /// </summary>
        /// <param name="str1">Строка 1</param>
        /// <param name="str2">Строка 2</param>
        /// <returns></returns>
        public static double EqualsPartiality(string str1, string str2)
        {
            if (str1.Length < str2.Length)
            {
                str1 = NormalizeLength(str1, str2);
            }

            if (str2.Length < str1.Length)
            {
                str2 = NormalizeLength(str2, str1);
            }

            int len = str1.Length;
            if (len != str2.Length)
                throw new NotSupportedException("Этот метод пока не поддерживает сравнение разных по длине строк.");
            if (len == 0)
                throw new ArgumentException("Строка не должна быть пустой.");
            int equalChars = 0;
            for (int i = 0; i < len; i++)
                if (str1[i] == str2[i])
                    equalChars++;

            var partiality = (double)equalChars / len;
            return partiality;
        }

        /// <summary>
        ///     Нормализация длины автодобавлением пробелов
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        public static string NormalizeLength(string str1, string str2)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(str1);
            var diff = str2.Length - str1.Length;
            for (int i = 0; i < diff; i++)
            {
                sb.Append(" ");
            }
            str1 = sb.ToString();
            return str1;
        }

        /// <summary>
        ///     Частичное сравнение с возвратом показателя по результатам сравнения
        /// </summary>
        /// <param name="input">Строка входящая</param>
        /// <param name="patterns">Паттерны</param>
        /// <returns></returns>
        public static double RegexEqualsPartiality(string input, string[] patterns)
        {
            int len = input.Length;
            if (len == 0)
                throw new ArgumentException("Строка не должна быть пустой.");
            int equalPatterns = 0;
            for (int i = 0; i < patterns.Length; i++)
                if (Regex.IsMatch(input, patterns[i], RegexOptions.CultureInvariant))
                    equalPatterns++;

            var partiality = (double)equalPatterns / patterns.Length;
            return partiality;
        }

        /// <summary>
        ///     Частичное сравнение с возвратом показателя по результатам сравнения в процентах
        /// </summary>
        /// <param name="input">Строка входящая</param>
        /// <param name="patterns">Паттерны</param>
        /// <returns></returns>
        public static string RegexEqualsPartialityRercentage(string input, string[] patterns)
        {
            var percentageVal = RegexEqualsPartiality(input, patterns) * 100.0;
            return $"{percentageVal}%";
        }

        /// <summary>
        ///     Разбивает содержимое файла на строки, либо строку на подстроки через символы переноса строки.
        /// </summary>
        /// <param name="content">Содержимое</param>
        /// <returns></returns>
        public static string[] SplitContentOnRows(this string content)
        {
            var rows = content.Split("\r\n".ToCharArray());

            var rowsEven = new List<string>();
            for (int i = 0; i < rows.Length; i++)
            {
                if (i % 2 == 0)
                    rowsEven.Add(rows[i]);
            }

            rows = rowsEven.ToArray();
            return rows;
        }

        /// <summary>
        ///     Получение отступа строки
        /// </summary>
        /// <param name="s">Строка</param>
        /// <returns></returns>
        public static string GetIndentString(this string s)
        {
            var indentCharsList = new List<char>();
            foreach (var ch in s)
            {
                if (ch == ' ')
                    indentCharsList.Add(ch);
                else
                {
                    break;
                }
            }

            string indent = new string(indentCharsList.ToArray());
            return indent;
        }

        /// <summary>
        ///     sLink transformation for spaces.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static string LinkTransformation(string url, string name)
        {
            return url.Replace(" ", "%20");
        }

        /// <summary>
        ///     Преобразование строкового значения в элемент перечисления
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">Строковое значение</param>
        /// <param name="ignoreCase">Игнорирование регистра</param>
        /// <returns></returns>
        public static T ToEnum<T>(this string value, bool ignoreCase = true)
        {
            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }

        /// <summary>
        ///      Преобразование строкового значения в элемент перечисления
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">Строковое значение</param>
        /// <param name="defaultValue">Элемент перечисления используемый по умолчанию</param>
        /// <param name="ignoreCase">Игнорирование регистра</param>
        /// <returns></returns>
        public static T ToEnum<T>(this string value, T defaultValue, bool ignoreCase = true) where T : struct
        {
            try
            {
                T enumValue;
                if (!Enum.TryParse(value, ignoreCase, out enumValue))
                {
                    return defaultValue;
                }
                return enumValue;
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        /// <summary>
        ///     Выполняет обрезку строки
        /// </summary>
        /// <param name="str">Строка</param>
        /// <param name="length">Ограничение длины</param>
        /// <returns></returns>
        public static string CutString(this string str, int length)
        {
            str ??= string.Empty;
            return str.Length > length ? str.Substring(0, length) : str;
        }
    }
}