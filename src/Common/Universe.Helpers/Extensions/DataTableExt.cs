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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Universe.Helpers.Extensions
{
    /// <summary>
    ///     Расширения для <see cref="DataTable"/>
    /// <author>Alex Envision</author>
    /// </summary>
    public static class DataTableExt
    {
        /// <summary>
        /// Excel hashtable
        /// </summary>
        /// <param name="count"></param>
        /// <returns>Return hashtable</returns>
        public static Hashtable ExcelTableHeaderHashCode(int count)
        {
            var head = new Dictionary<string, string> {
                {
                    "Date", DateTime.Today.Date.ToShortDateString()
                }, {
                    "Count", count.ToString()
                }
            };
            return new Hashtable(head);
        }

        /// <summary>
        /// Header data of the Excel file.
        /// </summary>
        /// <param name="count"></param>
        /// <returns>Return hashtable</returns>
        public static Hashtable ExcelTableLabelCode(int count, DataTable labelsTable)
        {
            if (count < 1)
                throw new Exception("Количество строк в таблице не может быть меньше одной!");

            var head = new Dictionary<string, string>();
            foreach (var column in labelsTable.Columns)
            {
                head.Add(column.ToString(), labelsTable.Rows[0][column.ToString()].ToString());
            }

            return new Hashtable(head);
        }

        public static DataTable ConvertEntity<T>(List<T> dt) where T : new()
        {
            //List<T> data = new List<T>();
            var temp = typeof(T);
            var t = dt.First();
            var data = new DataTable(t.GetType().Name);

            foreach (var pro in temp.GetProperties())
            {
                data.Columns.Add(pro.Name);
            }

            foreach (var item in dt)
            {
                var row = data.NewRow();
                var datarowFilled = GetRow(item, row);
                data.Rows.Add(datarowFilled);
            }

            return data;
        }

        private static DataRow GetRow<T>(T dr, DataRow row) where T : new()
        {
            var temp = typeof(T);
            //T obj = Activator.CreateInstance<T>();

            foreach (var pro in temp.GetProperties())
            {
                foreach (DataColumn column in row.Table.Columns)
                {
                    //if (pro.Name == column.ColumnName)
                    //pro.SetValue(obj, dr[column.ColumnName], null);
                    if (column.ColumnName == pro.Name)
                        row[column.ColumnName] = pro.GetValue(dr);
                    else
                        continue;
                }
            }

            return row;
        }
    }
}