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
using System.Reflection;

namespace Universe.Helpers.Extensions
{
    /// <summary>
    ///      Расширения для <see cref="Type"/>
    /// <author>Alex Envision</author>
    /// </summary>
    public static class TypeExtensions
    {
        public static Type GetBaseType(this Type type, Type baseType)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (baseType == null)
                throw new ArgumentNullException(nameof(baseType));

            if (type == baseType || type.IsGenericType && type.GetGenericTypeDefinition() == baseType)
                return type;

            if (type.BaseType == null)
                return null;

            return type.BaseType.GetBaseType(baseType);
        }

        public static List<Type> GetClassByNameFromAssambly(Assembly assambly, string @namespace, bool includeSubNamespaces, string name)
        {
            if (assambly == null)
                throw new ArgumentNullException(nameof(assambly));

            return assambly.GetTypes().Where(
                _ => _.IsClass
                    && (_.Namespace == @namespace || includeSubNamespaces && _.Namespace != null && _.Namespace.StartsWith($"{@namespace}."))
                    && _.Name == name).ToList();
        }

        public static List<Type> GetClassByNamespaceFromAssambly(Assembly assambly, string @namespace, bool includeSubNamespaces)
        {
            if (assambly == null)
                throw new ArgumentNullException(nameof(assambly));

            return assambly.GetTypes().Where(
                    _ => _.IsClass
                        && (_.Namespace == @namespace || includeSubNamespaces && _.Namespace != null && _.Namespace.StartsWith($"{@namespace}.")))
                .ToList();
        }

        public static List<Type> GetClassesOfHeirsFromAssambly(this Type classType, Assembly assambly)
        {
            if (classType == null)
                throw new ArgumentNullException(nameof(classType));
            if (assambly == null)
                throw new ArgumentNullException(nameof(assambly));

            return assambly.GetTypes().Where(_ => _.IsClass && _ != classType && (_.IsSubclassOf(classType) || _.IsSubclassOfGeneric(classType)))
                .ToList();
        }

        public static PropertyInfo GetPropertyEx(
            this Type type,
            string propName,
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)
        {
            if (propName == null)
                throw new ArgumentNullException(nameof(propName));

            if (!type.IsInterface)
                return type.GetProperty(propName, bindingFlags);

            var queue = new Queue<Type>();
            var considered = new List<Type>();

            queue.Enqueue(type);
            considered.Add(type);

            while (queue.Count > 0)
            {
                var subType = queue.Dequeue();
                var prop = subType.GetProperty(propName, bindingFlags);
                if (prop != null)
                    return prop;

                foreach (var subInterface in subType.GetInterfaces())
                {
                    if (considered.Contains(subInterface)) continue;

                    considered.Add(subInterface);
                    queue.Enqueue(subInterface);
                }
            }

            return null;
        }

        public static List<Type> GetСlassesImplementedInterfaceFromAssambly(this Type interfaceType, Assembly assambly)
        {
            if (interfaceType == null)
                throw new ArgumentNullException(nameof(interfaceType));
            if (assambly == null)
                throw new ArgumentNullException(nameof(assambly));

            return assambly.GetTypes().Where(_ => _.IsClass && _.GetInterfaces().Any(i => i == interfaceType)).ToList();
        }

        public static bool IsSubclassOfGeneric(this Type type, Type genericType)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (genericType == null)
                throw new ArgumentNullException(nameof(genericType));

            if (!genericType.IsGenericType)
                return false;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == genericType)
                return true;

            if (type.BaseType == null)
                return false;

            return type.BaseType.IsSubclassOfGeneric(genericType);
        }
    }
}