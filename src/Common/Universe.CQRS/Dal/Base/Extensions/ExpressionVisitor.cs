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
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace Universe.CQRS.Dal.Base.Extensions
{
    /// <summary>
    /// The expression visitor.
    /// <author>Alex Envision</author>
    /// </summary>
    internal abstract class ExpressionVisitor
    {
        /// <summary>
        /// The visit.
        /// </summary>
        /// <param name="exp">
        /// The exp.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// </exception>
        internal virtual Expression Visit(Expression exp)
        {
            if (exp == null)
                return null;

            switch (exp.NodeType)
            {
                case ExpressionType.UnaryPlus:
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    return VisitUnary((UnaryExpression)exp);
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.Power:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    return VisitBinary((BinaryExpression)exp);
                case ExpressionType.TypeIs:
                    return VisitTypeIs((TypeBinaryExpression)exp);
                case ExpressionType.Conditional:
                    return VisitConditional((ConditionalExpression)exp);
                case ExpressionType.Constant:
                    return VisitConstant((ConstantExpression)exp);
                case ExpressionType.Parameter:
                    return VisitParameter((ParameterExpression)exp);
                case ExpressionType.MemberAccess:
                    return VisitMemberAccess((MemberExpression)exp);
                case ExpressionType.Call:
                    return VisitMethodCall((MethodCallExpression)exp);
                case ExpressionType.Lambda:
                    return VisitLambda((LambdaExpression)exp);
                case ExpressionType.New:
                    return VisitNew((NewExpression)exp);
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return VisitNewArray((NewArrayExpression)exp);
                case ExpressionType.Invoke:
                    return VisitInvocation((InvocationExpression)exp);
                case ExpressionType.MemberInit:
                    return VisitMemberInit((MemberInitExpression)exp);
                case ExpressionType.ListInit:
                    return VisitListInit((ListInitExpression)exp);
                default:
                    throw new Exception(exp.NodeType.ToString());
            }
        }

        /// <summary>
        /// The visit binary.
        /// </summary>
        /// <param name="b">
        /// The b.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        internal virtual Expression VisitBinary(BinaryExpression b)
        {
            var left = Visit(b.Left);
            var right = Visit(b.Right);
            var conversion = Visit(b.Conversion);
            if (left != b.Left || right != b.Right || conversion != b.Conversion)
            {
                if (b.NodeType == ExpressionType.Coalesce && b.Conversion != null)
                    return Expression.Coalesce(left, right, conversion as LambdaExpression);

                return Expression.MakeBinary(b.NodeType, left, right, b.IsLiftedToNull, b.Method);
            }

            return b;
        }

        /// <summary>
        /// The visit binding.
        /// </summary>
        /// <param name="binding">
        /// The binding.
        /// </param>
        /// <returns>
        /// The <see cref="MemberBinding"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// </exception>
        internal virtual MemberBinding VisitBinding(MemberBinding binding)
        {
            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment:
                    return VisitMemberAssignment((MemberAssignment)binding);
                case MemberBindingType.MemberBinding:
                    return VisitMemberMemberBinding((MemberMemberBinding)binding);
                case MemberBindingType.ListBinding:
                    return VisitMemberListBinding((MemberListBinding)binding);
                default:
                    throw new Exception(binding.BindingType.ToString());
            }
        }

        /// <summary>
        /// The visit binding list.
        /// </summary>
        /// <param name="original">
        /// The original.
        /// </param>
        /// <returns>
        /// The <see cref="System.Collections.IEnumerable"/>.
        /// </returns>
        internal virtual IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
        {
            List<MemberBinding> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                var b = VisitBinding(original[i]);
                if (list != null)
                {
                    list.Add(b);
                }
                else if (b != original[i])
                {
                    list = new List<MemberBinding>(n);
                    for (var j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }

                    list.Add(b);
                }
            }

            if (list != null)
                return list;

            return original;
        }

        /// <summary>
        /// The visit conditional.
        /// </summary>
        /// <param name="c">
        /// The c.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        internal virtual Expression VisitConditional(ConditionalExpression c)
        {
            var test = Visit(c.Test);
            var ifTrue = Visit(c.IfTrue);
            var ifFalse = Visit(c.IfFalse);
            if (test != c.Test || ifTrue != c.IfTrue || ifFalse != c.IfFalse)
                return Expression.Condition(test, ifTrue, ifFalse);

            return c;
        }

        /// <summary>
        /// The visit constant.
        /// </summary>
        /// <param name="c">
        /// The c.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        internal virtual Expression VisitConstant(ConstantExpression c)
        {
            return c;
        }

        /// <summary>
        /// The visit element initializer.
        /// </summary>
        /// <param name="initializer">
        /// The initializer.
        /// </param>
        /// <returns>
        /// The <see cref="ElementInit"/>.
        /// </returns>
        internal virtual ElementInit VisitElementInitializer(ElementInit initializer)
        {
            var arguments = VisitExpressionList(initializer.Arguments);
            if (arguments != initializer.Arguments)
                return Expression.ElementInit(initializer.AddMethod, arguments);

            return initializer;
        }

        /// <summary>
        /// The visit element initializer list.
        /// </summary>
        /// <param name="original">
        /// The original.
        /// </param>
        /// <returns>
        /// The <see cref="System.Collections.IEnumerable"/>.
        /// </returns>
        internal virtual IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original)
        {
            List<ElementInit> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                var init = VisitElementInitializer(original[i]);
                if (list != null)
                {
                    list.Add(init);
                }
                else if (init != original[i])
                {
                    list = new List<ElementInit>(n);
                    for (var j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }

                    list.Add(init);
                }
            }

            if (list != null)
                return list;

            return original;
        }

        /// <summary>
        /// The visit expression list.
        /// </summary>
        /// <param name="original">
        /// The original.
        /// </param>
        /// <returns>
        /// The <see cref="ReadOnlyCollection{T}"/>.
        /// </returns>
        internal virtual ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
        {
            List<Expression> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                var p = Visit(original[i]);
                if (list != null)
                {
                    list.Add(p);
                }
                else if (p != original[i])
                {
                    list = new List<Expression>(n);
                    for (var j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }

                    list.Add(p);
                }
            }

            if (list != null)
                return new ReadOnlyCollection<Expression>(list);

            return original;
        }

        /// <summary>
        /// The visit invocation.
        /// </summary>
        /// <param name="iv">
        /// The iv.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        internal virtual Expression VisitInvocation(InvocationExpression iv)
        {
            IEnumerable<Expression> args = VisitExpressionList(iv.Arguments);
            var expr = Visit(iv.Expression);
            if (!Equals(args, iv.Arguments) || expr != iv.Expression)
                return Expression.Invoke(expr, args);

            return iv;
        }

        /// <summary>
        /// The visit lambda.
        /// </summary>
        /// <param name="lambda">
        /// The lambda.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        internal virtual Expression VisitLambda(LambdaExpression lambda)
        {
            var body = Visit(lambda.Body);
            if (body != lambda.Body)
                return Expression.Lambda(lambda.Type, body, lambda.Parameters);

            return lambda;
        }

        /// <summary>
        /// The visit list init.
        /// </summary>
        /// <param name="init">
        /// The init.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        internal virtual Expression VisitListInit(ListInitExpression init)
        {
            var n = VisitNew(init.NewExpression);
            var initializers = VisitElementInitializerList(init.Initializers);
            if (n != init.NewExpression || !Equals(initializers, init.Initializers))
                return Expression.ListInit(n, initializers);

            return init;
        }

        /// <summary>
        /// The visit member access.
        /// </summary>
        /// <param name="m">
        /// The m.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        internal virtual Expression VisitMemberAccess(MemberExpression m)
        {
            var exp = Visit(m.Expression);
            if (exp != m.Expression)
                return Expression.MakeMemberAccess(exp, m.Member);

            return m;
        }

        /// <summary>
        /// The visit member assignment.
        /// </summary>
        /// <param name="assignment">
        /// The assignment.
        /// </param>
        /// <returns>
        /// The <see cref="MemberAssignment"/>.
        /// </returns>
        internal virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            var e = Visit(assignment.Expression);
            if (e != assignment.Expression)
                return Expression.Bind(assignment.Member, e);

            return assignment;
        }

        /// <summary>
        /// The visit member init.
        /// </summary>
        /// <param name="init">
        /// The init.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        internal virtual Expression VisitMemberInit(MemberInitExpression init)
        {
            var n = VisitNew(init.NewExpression);
            var bindings = VisitBindingList(init.Bindings);
            if (n != init.NewExpression || !Equals(bindings, init.Bindings))
                return Expression.MemberInit(n, bindings);

            return init;
        }

        /// <summary>
        /// The visit member list binding.
        /// </summary>
        /// <param name="binding">
        /// The binding.
        /// </param>
        /// <returns>
        /// The <see cref="MemberListBinding"/>.
        /// </returns>
        internal virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding)
        {
            var initializers = VisitElementInitializerList(binding.Initializers);
            if (!Equals(initializers, binding.Initializers))
                return Expression.ListBind(binding.Member, initializers);

            return binding;
        }

        /// <summary>
        /// The visit member member binding.
        /// </summary>
        /// <param name="binding">
        /// The binding.
        /// </param>
        /// <returns>
        /// The <see cref="MemberMemberBinding"/>.
        /// </returns>
        internal virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            var bindings = VisitBindingList(binding.Bindings);
            if (!Equals(bindings, binding.Bindings))
                return Expression.MemberBind(binding.Member, bindings);

            return binding;
        }

        /// <summary>
        /// The visit method call.
        /// </summary>
        /// <param name="m">
        /// The m.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        internal virtual Expression VisitMethodCall(MethodCallExpression m)
        {
            var obj = Visit(m.Object);
            IEnumerable<Expression> args = VisitExpressionList(m.Arguments);
            if (obj != m.Object || !Equals(args, m.Arguments))
                return Expression.Call(obj, m.Method, args);

            return m;
        }

        /// <summary>
        /// The visit new.
        /// </summary>
        /// <param name="nex">
        /// The nex.
        /// </param>
        /// <returns>
        /// The <see cref="NewExpression"/>.
        /// </returns>
        internal virtual NewExpression VisitNew(NewExpression nex)
        {
            IEnumerable<Expression> args = VisitExpressionList(nex.Arguments);
            if (Equals(args, nex.Arguments))
                return nex;

            return nex.Members != null ? Expression.New(nex.Constructor, args, nex.Members) : Expression.New(nex.Constructor, args);
        }

        /// <summary>
        /// The visit new array.
        /// </summary>
        /// <param name="na">
        /// The na.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        internal virtual Expression VisitNewArray(NewArrayExpression na)
        {
            IEnumerable<Expression> exprs = VisitExpressionList(na.Expressions);
            if (Equals(exprs, na.Expressions))
                return na;

            return na.NodeType == ExpressionType.NewArrayInit
                ? Expression.NewArrayInit(na.Type.GetElementType() ?? throw new InvalidOperationException(), exprs)
                : Expression.NewArrayBounds(na.Type.GetElementType() ?? throw new InvalidOperationException(), exprs);
        }

        /// <summary>
        /// The visit parameter.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        internal virtual Expression VisitParameter(ParameterExpression p)
        {
            return p;
        }

        /// <summary>
        /// The visit type is.
        /// </summary>
        /// <param name="b">
        /// The b.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        internal virtual Expression VisitTypeIs(TypeBinaryExpression b)
        {
            var expr = Visit(b.Expression);
            if (expr != b.Expression)
                return Expression.TypeIs(expr, b.TypeOperand);

            return b;
        }

        /// <summary>
        /// The visit unary.
        /// </summary>
        /// <param name="u">
        /// The u.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        internal virtual Expression VisitUnary(UnaryExpression u)
        {
            var operand = Visit(u.Operand);
            if (operand != u.Operand)
                return Expression.MakeUnary(u.NodeType, operand, u.Type, u.Method);

            return u;
        }
    }
}