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
using System.Linq.Expressions;
using System.Reflection;

namespace Universe.Helpers.Extensions
{
    /// <summary>
    ///      Расширения для <see cref="Expression"/>
    /// <author>Alex Envision</author>
    /// </summary>
    public static class ExpressionExt
    {
        public static PropertyInfo GetProperty<T>(
            this Expression<Func<T, object>> propertyNameExp,
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)
        {
            if (propertyNameExp == null)
                throw new ArgumentNullException(nameof(propertyNameExp));

            return typeof(T).GetPropertyEx(
                propertyNameExp.PropertyName(),
                bindingFlags);
        }

        public static PropertyInfo GetProperty<T, TValue>(
            this Expression<Func<T, TValue>> propertyNameExp,
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)
        {
            if (propertyNameExp == null)
                throw new ArgumentNullException(nameof(propertyNameExp));

            return typeof(T).GetPropertyEx(
                propertyNameExp.PropertyName(),
                bindingFlags);
        }

        public static string PropertyName<T>(this Expression<Func<T, object>> propertyNameExp)
        {
            if (propertyNameExp == null)
                throw new ArgumentNullException(nameof(propertyNameExp));

            var memberExp = propertyNameExp.Body as MemberExpression;

            if (memberExp == null)
                memberExp = (MemberExpression)((UnaryExpression)propertyNameExp.Body).Operand;

            return memberExp.Member.Name;
        }

        public static string PropertyName<T, TValue>(this Expression<Func<T, TValue>> propertyNameExp)
        {
            if (propertyNameExp == null)
                throw new ArgumentNullException(nameof(propertyNameExp));

            var memberExp = propertyNameExp.Body as MemberExpression;

            if (memberExp == null)
                memberExp = (MemberExpression)((UnaryExpression)propertyNameExp.Body).Operand;

            return memberExp.Member.Name;
        }
    }
}