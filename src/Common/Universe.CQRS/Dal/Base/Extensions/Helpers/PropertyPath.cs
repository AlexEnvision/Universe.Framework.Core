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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Universe.CQRS.Dal.Base.Extensions.Helpers
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    internal class PropertyPath : IEnumerable<PropertyInfo>, IEnumerable
    {
        private readonly List<PropertyInfo> _components = new List<PropertyInfo>();

        public int Count
        {
            get { return _components.Count; }
        }

        public static PropertyPath Empty { get; } = new PropertyPath();

        public PropertyInfo this[int index]
        {
            get { return _components[index]; }
        }

        public PropertyPath(IEnumerable<PropertyInfo> components)
        {
            _components.AddRange(components);
        }

        public PropertyPath(PropertyInfo component)
        {
            _components.Add(component);
        }

        private PropertyPath()
        {
        }

        public static bool operator ==(PropertyPath left, PropertyPath right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PropertyPath left, PropertyPath right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            var propertyPathName = new StringBuilder();
            _components.Each(
                pi => {
                    propertyPathName.Append(pi.Name);
                    propertyPathName.Append('.');
                });
            return propertyPathName.ToString(0, propertyPathName.Length - 1);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != typeof(PropertyPath))
                return false;

            return Equals((PropertyPath)obj);
        }

        public override int GetHashCode()
        {
            return _components.Aggregate(0, (t, n) => t ^ n.DeclaringType.GetHashCode() * n.Name.GetHashCode() * 397);
        }

        IEnumerator<PropertyInfo> IEnumerable<PropertyInfo>.GetEnumerator()
        {
            return _components.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _components.GetEnumerator();
        }
    }
}