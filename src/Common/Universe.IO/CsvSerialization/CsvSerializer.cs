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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Universe.Helpers.Extensions;

namespace Universe.IO.CsvSerialization
{
    /// <summary>
    ///     Сериализация и десериализация коллекции объектов в CSV.
	///     Serialize and Deserialize Lists of any object type to CSV.
    /// <author>Alex Envision</author>
	/// </summary>
	public class CsvSerializer<T> where T : class, new()
	{
		#region Fields
		private bool _ignoreEmptyLines = true;

		private bool _ignoreReferenceTypesExceptString = true;

		private string _newlineReplacement = ((char)0x254).ToString();

		private List<PropertyInfo> _properties;

		private string _replacement = ((char)0x255).ToString();

		private string _rowNumberColumnTitle = "RowNumber";

		private char _separator = ';';

		private bool _useEofLiteral = false;

		private bool _useLineNumbers = false;

		private bool _useTextQualifier = false;

		#endregion Fields

		#region Properties
		public bool IgnoreEmptyLines
		{
			get { return _ignoreEmptyLines; }
			set { _ignoreEmptyLines = value; }
		}

		public bool IgnoreReferenceTypesExceptString
		{
			get { return _ignoreReferenceTypesExceptString; }
			set { _ignoreReferenceTypesExceptString = value; }
		}

		public string NewlineReplacement
		{
			get { return _newlineReplacement; }
			set { _newlineReplacement = value; }
		}

		public string Replacement
		{
			get { return _replacement; }
			set { _replacement = value; }
		}

		public string RowNumberColumnTitle
		{
			get { return _rowNumberColumnTitle; }
			set { _rowNumberColumnTitle = value; }
		}

		public char Separator
		{
			get { return _separator; }
			set { _separator = value; }
		}

		public bool UseEofLiteral
		{
			get { return _useEofLiteral; }
			set { _useEofLiteral = value; }
		}

		public bool UseLineNumbers
		{
			get { return _useLineNumbers; }
			set { _useLineNumbers = value; }
		}

		public bool UseTextQualifier
		{
			get { return _useTextQualifier; }
			set { _useTextQualifier = value; }
		}

		#endregion Properties

		/// <summary>
		/// Csv Serializer
		/// Initialize by selected properties from the type to be de/serialized
		/// </summary>
		public CsvSerializer()
		{
		    _properties = GetProperties<T>(true);
		}

	    private List<PropertyInfo> GetProperties<TEntity>(bool isRoot = false) where TEntity : class, new()
        {
	        var type = typeof(TEntity);
            return GetProperties(type, isRoot);
        }

	    private List<PropertyInfo> GetProperties(Type incomingtype, bool isRoot = false)
	    {
	        var type = incomingtype;

	        var properties = type.GetProperties(
	            BindingFlags.Public | BindingFlags.Instance
	            | BindingFlags.GetProperty | BindingFlags.SetProperty);


	        var q = properties.AsQueryable();

	        if (!isRoot && IgnoreReferenceTypesExceptString)
	            q = q.Where(a => a.PropertyType.IsValueType || a.PropertyType.Name == "String");

	        var r = from a in q
	            where a.GetCustomAttribute<CsvIgnoreAttribute>() == null
	            //orderby a.Name
	            select a;

	        return r.ToList();
	    }

        /// <summary>
        ///     Проверка на соответсвие типа всех свойств объекта строковому типу.
        /// </summary>
        /// <returns></returns>
        private bool CheckIsAllStringProperties()
        {
            var isAllStringProperties = true;
            var nameOfStringType = typeof(string).Name;

            foreach (var propertyInfo in _properties)
            {
                if (propertyInfo.PropertyType.Name.PrepareToCompare() != nameOfStringType.PrepareToCompare())
                {
                    isAllStringProperties = false;
                    break;
                }
            }

            return isAllStringProperties;
        }

        /// <summary>
        ///     Десериализация.
        ///     Deserialize.
        /// </summary>
        /// <param name="stream">stream</param>
        /// <returns></returns>
        public IList<T> Deserialize(Stream stream)
		{
            if (!CheckIsAllStringProperties())
                throw new InvalidCsvFormatException("The data model used to deserialize a CSV file row, must only contain string property types.");

            string[] columns;
			string[] rows;

			try
			{
				using (var sr = new StreamReader(stream))
				{
					columns = sr.ReadLine().Split(Separator);
					rows = sr.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
				}
            }
			catch (Exception ex)
			{
				throw new InvalidCsvFormatException("The CSV File is Invalid. See Inner Exception for more information.", ex);
			}

            // Убираем из названий колонок кавычки, апострофы
            for (int index = 0; index < columns.Length; index++)
            {
                columns[index] = columns[index]
                    .Replace("\"", string.Empty)
                    .Replace("'", string.Empty);
            }

            var data = new List<T>();

			for (int row = 0; row < rows.Length; row++)
			{
				var line = rows[row];

				if (IgnoreEmptyLines && string.IsNullOrWhiteSpace(line))
				{
					continue;
				}
				else if (!IgnoreEmptyLines && string.IsNullOrWhiteSpace(line))
				{
					throw new InvalidCsvFormatException(string.Format(@"Error: Empty line at line number: {0}", row));
				}

				var parts = line.Split(Separator);

                if (parts.Length > columns.Length)
                    throw new InvalidCsvFormatException($"The number of columns in the header does not match the number of columns in the row with number: {row + 2} ({parts.Length}/{columns.Length}). Check the file and try again.");

                var firstColumnIndex = UseLineNumbers ? 2 : 1;
				if (parts.Length == firstColumnIndex && parts[firstColumnIndex - 1] != null && parts[firstColumnIndex - 1] == "EOF")
				{
					break;
				}

				var datum = new T();

				var start = UseLineNumbers ? 1 : 0;
				for (int i = start; i < parts.Length; i++)
				{
					var value = parts[i];
					var column = columns[i];

					// continue of deviant RowNumber column condition
					// this allows for the deserializer to implicitly ignore the RowNumber column
					if (column.Equals(RowNumberColumnTitle) && !_properties.Any(a => a.Name.Equals(RowNumberColumnTitle)))
					{
						continue;
					}

					value = value
						.Replace(Replacement, Separator.ToString())
						.Replace(NewlineReplacement, Environment.NewLine).Trim();

                    var p = _properties.FirstOrDefault(a => a.Name.Equals(column, StringComparison.InvariantCultureIgnoreCase));

                    // ignore property csv column, Property not found on targing type
                    if (p == null)
                    {
                        continue;
                    }

                    if (UseTextQualifier)
                    {
                        if (string.IsNullOrEmpty(value))
                        {
                            value = string.Empty;
                        }
                        else
                        {
                            if (value.IndexOf("\"") == 0)
                                value = value.Substring(1);

                            if (value[value.Length - 1].ToString() == "\"")
                                value = value.Substring(0, value.Length - 1);
                        }
                    }

                    var converter = TypeDescriptor.GetConverter(p.PropertyType);
                    var convertedvalue = converter.ConvertFrom(value);

                    p.SetValue(datum, convertedvalue);
                }

				data.Add(datum);
			}

			return data;
		}

        /// <summary>
        ///     Сериализация.
        ///     Serialize.
        /// </summary>
        /// <param name="stream">Поток / Stream</param>
        /// <param name="data">Данные / Data</param>
        public void Serialize(Stream stream, IList<T> data)
		{
            var sb = new StringBuilder();
			var values = new List<string>();

			sb.AppendLine(GetHeader());

			var row = 1;
			foreach (var item in data)
			{
				values.Clear();

                if (UseLineNumbers)
                {
                    values.Add(row++.ToString());
                }

                foreach (var p in _properties)
				{
					var raw = p.GetValue(item);
                    var value = raw == null ? "" :
                        raw.ToString()
                            .Replace(Separator.ToString(), Replacement)
                            .Replace(Environment.NewLine, NewlineReplacement);

                    if (UseTextQualifier)
                    {
                        value = string.Format("\"{0}\"", value);
                    }

                    values.Add(value);
				}
				sb.AppendLine(string.Join(Separator.ToString(), values.ToArray()));
			}

			if (UseEofLiteral)
			{
				values.Clear();

                if (UseLineNumbers)
                {
                    values.Add(row++.ToString());
                }

                values.Add("EOF");

				sb.AppendLine(string.Join(Separator.ToString(), values.ToArray()));
			}

			using (var sw = new StreamWriter(stream))
			{
				sw.Write(sb.ToString().Trim());
			}
		}

		/// <summary>
        ///     Получение заголовка
        ///     Get Header
		/// </summary>
		/// <returns></returns>
		private string GetHeader()
		{
			var header = _properties.Select(a => a.Name);

			if (UseLineNumbers)
			{
				header = new string[] { RowNumberColumnTitle }.Union(header);
			}

			return string.Join(Separator.ToString(), header.ToArray());
		}
	}
}
