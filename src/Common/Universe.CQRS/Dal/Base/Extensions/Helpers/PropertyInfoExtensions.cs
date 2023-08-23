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

using System.Linq;
using System.Reflection;

namespace Universe.CQRS.Dal.Base.Extensions.Helpers
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    internal static class PropertyInfoExtensions
    {
        public static bool IsSameAs(this PropertyInfo propertyInfo, PropertyInfo otherPropertyInfo)
        {
            if (propertyInfo == otherPropertyInfo)
                return true;
            if (!(propertyInfo.Name == otherPropertyInfo.Name))
                return false;
            if (!(propertyInfo.DeclaringType == otherPropertyInfo.DeclaringType) &&
                !propertyInfo.DeclaringType.IsSubclassOf(otherPropertyInfo.DeclaringType) &&
                !otherPropertyInfo.DeclaringType.IsSubclassOf(propertyInfo.DeclaringType) &&
                !propertyInfo.DeclaringType.GetInterfaces().Contains(otherPropertyInfo.DeclaringType))
                return otherPropertyInfo.DeclaringType.GetInterfaces().Contains(propertyInfo.DeclaringType);

            return true;
        }

        public static MethodInfo Getter(this PropertyInfo property)
        {
            return property.GetMethod;
        }

        public static MethodInfo Setter(this PropertyInfo property)
        {
            return property.SetMethod;
        }

        public static bool IsStatic(this PropertyInfo property)
        {
            var methodInfo = property.Getter();
            if ((object)methodInfo == null)
                methodInfo = property.Setter();
            return methodInfo.IsStatic;
        }

        public static bool IsPublic(this PropertyInfo property)
        {
            var methodInfo1 = property.Getter();
            var methodAttributes1 = methodInfo1 == (MethodInfo)null
                ? MethodAttributes.Private
                : methodInfo1.Attributes & MethodAttributes.MemberAccessMask;
            var methodInfo2 = property.Setter();
            var methodAttributes2 = methodInfo2 == (MethodInfo)null
                ? MethodAttributes.Private
                : methodInfo2.Attributes & MethodAttributes.MemberAccessMask;
            return (methodAttributes1 > methodAttributes2 ? (int)methodAttributes1 : (int)methodAttributes2) == 6;
        }
    }
}