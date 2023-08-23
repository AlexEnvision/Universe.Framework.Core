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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Universe.CQRS.Dal.Base.Extensions.Helpers
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    internal static class ExpressionExtensions
    {
        private static IEnumerable<PropertyPath> MatchPropertyAccessList(
            this LambdaExpression lambdaExpression,
            Func<Expression, Expression, PropertyPath> propertyMatcher)
        {
            var newExpression = lambdaExpression.Body.RemoveConvert() as NewExpression;
            if (newExpression != null)
            {
                var parameterExpression = lambdaExpression.Parameters.Single();
                var propertyPaths = newExpression.Arguments.Select(a => propertyMatcher(a, parameterExpression)).Where(p => p != (PropertyPath)null);
                if (propertyPaths.Count() == newExpression.Arguments.Count())
                {
                    if (!newExpression.HasDefaultMembersOnly(propertyPaths))
                        return null;

                    return propertyPaths;
                }
            }

            var propertyPath = propertyMatcher(lambdaExpression.Body, lambdaExpression.Parameters.Single());
            if (!(propertyPath != null))
                return null;

            return new PropertyPath[1] {
                propertyPath
            };
        }

        private static bool HasDefaultMembersOnly(this NewExpression newExpression, IEnumerable<PropertyPath> propertyPaths)
        {
            if (newExpression.Members != null)
                return !newExpression.Members.Where(
                    (t, i) => !string.Equals(t.Name, propertyPaths.ElementAt(i).Last().Name, StringComparison.Ordinal)).Any();

            return true;
        }

        private static PropertyPath MatchSimplePropertyAccess(this Expression parameterExpression, Expression propertyAccessExpression)
        {
            var propertyPath = parameterExpression.MatchPropertyAccess(propertyAccessExpression);
            if (!(propertyPath != null) || propertyPath.Count != 1)
                return null;

            return propertyPath;
        }

        private static PropertyPath MatchComplexPropertyAccess(this Expression parameterExpression, Expression propertyAccessExpression)
        {
            return parameterExpression.MatchPropertyAccess(propertyAccessExpression);
        }

        private static PropertyPath MatchPropertyAccess(this Expression parameterExpression, Expression propertyAccessExpression)
        {
            var propertyInfoList = new List<PropertyInfo>();
            MemberExpression memberExpression;
            do
            {
                memberExpression = propertyAccessExpression.RemoveConvert() as MemberExpression;
                if (memberExpression == null)
                    return null;

                var member = memberExpression.Member as PropertyInfo;
                if (member == null)
                    return null;

                propertyInfoList.Insert(0, member);
                propertyAccessExpression = memberExpression.Expression;
            }
            while (memberExpression.Expression != parameterExpression);

            return new PropertyPath(propertyInfoList);
        }

        public static Expression RemoveConvert(this Expression expression)
        {
            while (expression.NodeType == ExpressionType.Convert || expression.NodeType == ExpressionType.ConvertChecked)
            {
                expression = ((UnaryExpression)expression).Operand;
            }

            return expression;
        }

        public static bool IsNullConstant(this Expression expression)
        {
            expression = expression.RemoveConvert();
            if (expression.NodeType != ExpressionType.Constant)
                return false;

            return ((ConstantExpression)expression).Value == null;
        }

        public static bool IsStringAddExpression(this Expression expression)
        {
            var binaryExpression = expression as BinaryExpression;
            if (binaryExpression == null || binaryExpression.Method == null || binaryExpression.NodeType != ExpressionType.Add ||
                !(binaryExpression.Method.DeclaringType == typeof(string)))
                return false;

            return string.Equals(binaryExpression.Method.Name, "Concat", StringComparison.Ordinal);
        }
    }
}