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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;

namespace Universe.CQRS.Dal.Base.Extensions.Helpers
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    public static class EntityFrameworkHelpers
    {
        private static readonly ConcurrentDictionary<Type, Type> _collectionTypes = new ConcurrentDictionary<Type, Type>();

        public static bool KeyValuesEqual(object x, object y)
        {
            if (x is DBNull)
                x = null;
            if (y is DBNull)
                y = null;
            if (Equals(x, y))
                return true;

            var numArray1 = x as byte[];
            var numArray2 = y as byte[];
            if (numArray1 == null || numArray2 == null || numArray1.Length != numArray2.Length)
                return false;

            for (var index = 0; index < numArray1.Length; ++index)
            {
                if (numArray1[index] != numArray2[index])
                    return false;
            }

            return true;
        }

        public static string QuoteIdentifier(string identifier)
        {
            return "[" + identifier.Replace("]", "]]") + "]";
        }

        public static bool TreatAsConnectionString(string nameOrConnectionString)
        {
            return nameOrConnectionString.IndexOf('=') >= 0;
        }

        public static bool TryGetConnectionName(string nameOrConnectionString, out string name)
        {
            var length = nameOrConnectionString.IndexOf('=');
            if (length < 0)
            {
                name = nameOrConnectionString;
                return true;
            }

            if (nameOrConnectionString.IndexOf('=', length + 1) >= 0)
            {
                name = null;
                return false;
            }

            if (nameOrConnectionString.Substring(0, length).Trim().Equals("name", StringComparison.OrdinalIgnoreCase))
            {
                name = nameOrConnectionString.Substring(length + 1).Trim();
                return true;
            }

            name = null;
            return false;
        }

        public static bool TryParsePath(Expression expressionBody, out string path)
        {
            path = null;
            var convertedExpression = expressionBody.RemoveConvert();
            var memberExpression = convertedExpression as MemberExpression;
            var methodCallExpression = convertedExpression as MethodCallExpression;
            if (memberExpression != null)
            {
                var name = memberExpression.Member.Name;
                string path1;
                if (!TryParsePath(memberExpression.Expression, out path1))
                    return false;

                path = path1 == null ? name : path1 + "." + name;
            }
            else if (methodCallExpression != null)
            {
                string path1;
                if (methodCallExpression.Method.Name == "Select" && methodCallExpression.Arguments.Count == 2 &&
                    TryParsePath(methodCallExpression.Arguments[0], out path1) && path1 != null)
                {
                    var lambdaExpression = methodCallExpression.Arguments[1] as LambdaExpression;
                    string path2;
                    if (lambdaExpression != null && TryParsePath(lambdaExpression.Body, out path2) && path2 != null)
                    {
                        path = path1 + "." + path2;
                        return true;
                    }
                }

                return false;
            }

            return true;
        }

        public static IEnumerable<DbValidationError> SplitValidationResults(string propertyName, IEnumerable<ValidationResult> validationResults)
        {
            foreach (var validationResult in validationResults)
            {
                if (validationResult != null)
                {
                    var memberNames = validationResult.MemberNames == null || !validationResult.MemberNames.Any()
                        ? new string[1]
                        : validationResult.MemberNames;
                    foreach (var str in memberNames)
                    {
                        yield return new DbValidationError(str ?? propertyName, validationResult.ErrorMessage);
                    }
                }
            }
        }

        public static Type CollectionType(Type elementType)
        {
            return _collectionTypes.GetOrAdd(elementType, t => typeof(ICollection<>).MakeGenericType(t));
        }

        public static string DatabaseName(this Type contextType)
        {
            return contextType.ToString();
        }
    }
}