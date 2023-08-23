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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Universe.CQRS.Models.Condition;

namespace Universe.CQRS.Infrastructure.Json
{
    /// <summary>
    ///     Ковертер dynamic в модели конфигурации.
    ///     Реализация - переводим dynamic в dictionary и конвертируем его в объект по информации о классе из рефлексии
    /// <author>Alex Envision</author>
    /// </summary>
    internal sealed class DynamicModelMapper
    {
        private readonly Func<dynamic, Func<Type, bool>> _concreteTypeResolver = GetConcreteTypeResolver;

        private readonly Func<dynamic, Dictionary<string, object>> _dynamicDictionarySerializer = DynToDict;

        public T Map<T>(dynamic value) where T : class, new()
        {
            return (T)Map(typeof(T), value);
        }

        public object Map(Type type, dynamic value)
        {
            if (type
                .GetInterfaces(
                    ).Count(
                    t => t.IsGenericType
                        && t.GetGenericTypeDefinition() == typeof(IEnumerable<>)) > 0)
            {
                // В случае рассматривания строки как массива, возвращаем строку
                if (type == typeof(string))
                    return value.ToString();

                var destType = type.GetGenericArguments()[0];

                var listType = typeof(List<>).MakeGenericType(destType);
                var list = Activator.CreateInstance(listType);
                var addMethod = listType.GetMethod(
                    "Add",
                    new[] {
                        destType
                    });

                if (addMethod != null)
                    foreach (var item in value)
                    {
                        try
                        {
                            if (destType != typeof(string) && item?._isDeleted == true)
                                item.deleted = DateTimeOffset.UtcNow;
                        }
                        catch (Exception)
                        {
                            // TODO -> У некоторых сущностей не находит в них _isDeleted и падает с ошибкой (не только в string)
                        }

                        if (IsNullJvalue(item))
                            continue;

                        addMethod.Invoke(
                            list,
                            new object[] {
                                Map(destType, item)
                            });
                    }

                return list;
            }

            if (type.IsInterface)
            {
                var typeResolver = _concreteTypeResolver(value);
                var destType = type.Assembly
                    .GetTypes()
                    .Where(x => !x.IsAbstract && x.GetInterfaces().Contains(type))
                    .FirstOrDefault(t => typeResolver(t));

                if (destType == typeof(ValueArgumentConfiguration))
                    value.expression = value.value;

                if (destType == null)
                    destType = type.Assembly
                        .GetTypes()
                        .FirstOrDefault(x => !x.IsAbstract && x.GetInterfaces().Contains(type));

                if (destType == default(Type))
                    throw new ArgumentException($"Cant find destination type for interface {type}");

                return Map(destType, value);
            }

            if (type.IsAbstract)
            {
                var typeResolver = _concreteTypeResolver(value);
                var destType = type.Assembly
                    .GetTypes()
                    .Where(x => !x.IsAbstract && x.IsSubclassOf(type))
                    .FirstOrDefault(t => typeResolver(t));

                if (destType == null && value.type == "between")
                    destType = typeof(BetweenConfiguration);

                if (destType == default(Type))
                    throw new ArgumentException($"Cant find destination type for abstract class {type}");

                return Map(destType, value);
            }

            return DictToObj(type, _dynamicDictionarySerializer(value));
        }

        /// <summary>
        /// Конвертирует dynamic в dictionary
        /// </summary>
        /// <param name="dynObj">Модель для конвертации</param>
        /// <returns>Результат конвертации</returns>
        private static Dictionary<string, object> DynToDict(dynamic dynObj)
        {
            var dictionary = new Dictionary<string, object>();
            var properties = TypeDescriptor.GetProperties(dynObj);
            foreach (PropertyDescriptor propertyDescriptor in properties)
            {
                object obj = propertyDescriptor.GetValue(dynObj);
                dictionary.Add(propertyDescriptor.Name, obj);
            }

            return dictionary;
        }

        /// <summary>
        /// Возвращает функцию для проверки соответствия типа модели, типу из dynamic
        /// </summary>
        /// <param name="value">Модель</param>
        /// <returns>Predicate</returns>
        private static Func<Type, bool> GetConcreteTypeResolver(dynamic value)
        {
            var fieldName = "Type";
            var typeValue = (string)value[fieldName];
            if (string.IsNullOrEmpty(typeValue))
                typeValue = (string)value[fieldName.ToLowerInvariant()];

            return type => {
                var prop = type.GetProperty(fieldName);

                if (type.GetConstructor(Type.EmptyTypes) == null || prop == null)
                    return false;

                return prop.GetValue(Activator.CreateInstance(type)) as string == typeValue;
            };
        }

        /// <summary>
        /// Возвращает результат конвертации набора ключ-значение в экземляр переданного типа
        /// </summary>
        /// <param name="keyValues">Набор значений для конвертации</param>
        /// <param name="type">Тип возвращаемой модели</param>
        /// <returns>Результат конвертации</returns>
        private object DictToObj(Type type, Dictionary<string, object> keyValues)
        {
            string typeName = type.Name;

            if (type.GetConstructor(Type.EmptyTypes) == null)
                throw new ArgumentException($"Cant find default constructor for {type}");

            var instance = Activator.CreateInstance(type);
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.CanWrite).ToList();
            var values = new Dictionary<string, object>(keyValues, StringComparer.OrdinalIgnoreCase);
            foreach (var property in properties)
            {
                if (!values.TryGetValue(property.Name, out var value))
                    continue;

                if (value == null)
                    continue;

                if (value.GetType().IsPrimitive || value is string)
                {
                    property.SetValue(instance, value);
                }
                else if (value is JValue)
                {
                    var propertyType = property.PropertyType;

                    if (propertyType == typeof(DateTimeOffset))
                        try
                        {
                            var asDateTimeOffset = (DateTimeOffset)Convert.ChangeType(value, typeof(DateTimeOffset));
                            property.SetValue(instance, asDateTimeOffset);
                        }
                        catch (Exception)
                        {
                            property.SetValue(instance, DateTimeOffset.MinValue);
                        }
                    else if (propertyType == typeof(int?))
                        try
                        {
                            property.SetValue(instance, Convert.ChangeType(value, property.PropertyType));
                        }
                        catch (Exception)
                        {
                            property.SetValue(instance, 0);
                        }
                    else
                        property.SetValue(instance, Convert.ChangeType(value, property.PropertyType));
                }
                else
                {
                    property.SetValue(instance, Map(property.PropertyType, value));
                }
            }

            return instance;
        }

        private bool IsNullJvalue(object value)
        {
            string strDValue = value.ToString();
            if (strDValue == "{}")
                return true;

            if (value is JValue)
            {
                var jvalue = (JValue)value;
                if (jvalue.Value == null)
                    return true;
            }

            return false;
        }
    }
}