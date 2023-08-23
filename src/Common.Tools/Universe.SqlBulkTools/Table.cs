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
using System.Data.SqlClient;
using System.Linq.Expressions;

namespace Universe.SqlBulkTools
{
    /// <summary>
    ///     Конфигурация таблицы
    ///     Configurable options for table. 
    /// <author>Alex Envision</author>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Table<T>
    {
        private HashSet<string> Columns { get; set; }
        private List<string> UpdateOnList { get; set; }
        private List<string> DeleteOnList { get; set; }
        private readonly BulkOperationsHelpers _helper;
        private string _schema;
        private readonly string _tableName;
        private readonly string _sourceAlias;
        private readonly string _targetAlias;
        private readonly BulkOperations _ext;
        private readonly IEnumerable<T> _list;
        private Dictionary<string, string> CustomColumnMappings { get; set; }
        private int _sqlTimeout;
        private int _bulkCopyTimeout;
        private bool _bulkCopyEnableStreaming;
        private int? _bulkCopyNotifyAfter;
        private int? _bulkCopyBatchSize;
        private SqlBulkCopyOptions _sqlBulkCopyOptions;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="tableName"></param>
        /// <param name="sourceAlias"></param>
        /// <param name="targetAlias"></param>
        /// <param name="ext"></param>
        public Table(IEnumerable<T> list, string tableName, string sourceAlias, string targetAlias, BulkOperations ext)
        {
            _bulkCopyBatchSize = null;
            _bulkCopyNotifyAfter = null;
            _bulkCopyEnableStreaming = false;
            _helper = new BulkOperationsHelpers();
            _tableName = tableName;
            _sourceAlias = sourceAlias;
            _targetAlias = targetAlias;
            _sqlTimeout = 600;
            _bulkCopyTimeout = 600;
            _ext = ext;
            _list = list;
            _schema = Constants.DefaultSchemaName;
            Columns = new HashSet<string>();
            UpdateOnList = new List<string>();
            DeleteOnList = new List<string>();
            CustomColumnMappings = new Dictionary<string, string>();
            _sqlBulkCopyOptions = SqlBulkCopyOptions.Default;
        }

        /// <summary>
        /// Add each column that you want to include in the query. Only include the columns that are relevant to the procedure for best performance. 
        /// </summary>
        /// <param name="columnName">Column name as represented in database</param>
        /// <returns></returns>
        public ColumnSelect<T> AddColumn(Expression<Func<T, object>> columnName)
        {
            var propertyName = _helper.GetPropertyName(columnName);
            Columns.Add(propertyName);
            return new ColumnSelect<T>(_list, _tableName, Columns, _schema, _sourceAlias, _targetAlias, 
                _sqlTimeout, _bulkCopyTimeout, _bulkCopyEnableStreaming, _bulkCopyNotifyAfter, _bulkCopyBatchSize, _sqlBulkCopyOptions, _ext);
        }

        /// <summary>
        /// Adds all properties in model that are either value or string type. 
        /// </summary>
        /// <returns></returns>
        public AllColumnSelect<T> AddAllColumns()
        {
            Columns = _helper.GetAllValueTypeAndStringColumns(typeof(T));
            return new AllColumnSelect<T>(_list, _tableName, Columns, _schema, _sourceAlias, _targetAlias,
                _sqlTimeout, _bulkCopyTimeout, _bulkCopyEnableStreaming, _bulkCopyNotifyAfter, _bulkCopyBatchSize, _sqlBulkCopyOptions, _ext);
        }

        /// <summary>
        /// Explicitley set a schema if your table may have a naming conflict within your database. 
        /// If a schema is not added, the system default schema name 'dbo' will used.. 
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        public Table<T> WithSchema(string schema)
        {
            _schema = schema;
            return this;
        }

        /// <summary>
        /// Default is 600 seconds. See docs for more info. 
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public Table<T> WithSqlCommandTimeout(int seconds)
        {
            _sqlTimeout = seconds;
            return this;
        }

        /// <summary>
        /// Default is 600 seconds. See docs for more info. 
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public Table<T> WithBulkCopyCommandTimeout(int seconds)
        {
            _bulkCopyTimeout = seconds;
            return this;
        }

        /// <summary>
        /// Default is false. See docs for more info.
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public Table<T> WithBulkCopyEnableStreaming(bool status)
        {
            _bulkCopyEnableStreaming = status;
            return this;
        }

        /// <summary>
        /// Triggers an event after x rows inserted. See docs for more info. 
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public Table<T> WithBulkCopyNotifyAfter(int rows)
        {
            _bulkCopyNotifyAfter = rows;
            return this;
        }

        /// <summary>
        /// Default is 0. See docs for more info. 
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public Table<T> WithBulkCopyBatchSize(int rows)
        {
            _bulkCopyBatchSize = rows;
            return this;
        }

        /// <summary>
        /// Enum representing options for SqlBulkCopy. Unless explicitely set, the default option will be used. 
        /// See https://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqlbulkcopyoptions(v=vs.110).aspx for a list of available options. 
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public Table<T> WithSqlBulkCopyOptions(SqlBulkCopyOptions options)
        {           
            _sqlBulkCopyOptions = options;
            return this;
        }

    }
}