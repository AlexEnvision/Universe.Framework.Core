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

using System.Collections.Generic;
using NUnit.Framework;
using Universe.SqlBulkTools.UnitTests.Model;

namespace Universe.SqlBulkTools.UnitTests
{
    /// <summary>
    ///     Юнит-тесты массовых операций
    /// <author>Alex Envision</author>
    /// </summary>
    [TestFixture]
    class SqlBulkToolsUnitTests
    {

        [Test]
        public void BulkOperationsHelpers_BuildJoinConditionsForUpdateOrInsertWithThreeConditions()
        {
            // Arrange
            List<string> joinOnList = new List<string>() { "MarketPlaceId", "FK_BusinessId", "AddressId" };
            var sut = new BulkOperationsHelpers();

            // Act
            var result = sut.BuildJoinConditionsForUpdateOrInsert(joinOnList.ToArray(), "Source", "Target");

            // Assert
            Assert.AreEqual("ON [Target].[MarketPlaceId] = [Source].[MarketPlaceId] AND [Target].[FK_BusinessId] = [Source].[FK_BusinessId] AND [Target].[AddressId] = [Source].[AddressId] ", result);
        }

        [Test]
        public void BulkOperationsHelpers_BuildJoinConditionsForUpdateOrInsertWithTwoConditions()
        {
            // Arrange
            List<string> joinOnList = new List<string>() { "MarketPlaceId", "FK_BusinessId" };
            var sut = new BulkOperationsHelpers();

            // Act
            var result = sut.BuildJoinConditionsForUpdateOrInsert(joinOnList.ToArray(), "Source", "Target");

            // Assert
            Assert.AreEqual("ON [Target].[MarketPlaceId] = [Source].[MarketPlaceId] AND [Target].[FK_BusinessId] = [Source].[FK_BusinessId] ", result);
        }

        [Test]
        public void BulkOperationsHelpers_BuildJoinConditionsForUpdateOrInsertWitSingleCondition()
        {
            // Arrange
            List<string> joinOnList = new List<string>() { "MarketPlaceId" };
            var sut = new BulkOperationsHelpers();

            // Act
            var result = sut.BuildJoinConditionsForUpdateOrInsert(joinOnList.ToArray(), "Source", "Target");

            // Assert
            Assert.AreEqual("ON [Target].[MarketPlaceId] = [Source].[MarketPlaceId] ", result);
        }

        [Test]
        public void BulkOperationsHelpers_BuildUpdateSet_BuildsCorrectSequenceForMultipleColumns()
        {
            // Arrange
            var updateOrInsertColumns = GetTestParameters();
            var expected =
                "UPDATE SET [Target].[id] = [Source].[id], [Target].[Name] = [Source].[Name], [Target].[Town] = [Source].[Town], [Target].[Email] = [Source].[Email], [Target].[IsCool] = [Source].[IsCool] ";
            var sut = new BulkOperationsHelpers();

            // Act
            var result = sut.BuildUpdateSet(updateOrInsertColumns, "Source", "Target", null);

            // Assert
            Assert.AreEqual(expected, result);

        }

        [Test]
        public void BulkOperationsHelpers_BuildUpdateSet_BuildsCorrectSequenceForSingleColumn()
        {
            // Arrange
            var updateOrInsertColumns = new HashSet<string>();
            updateOrInsertColumns.Add("Id");

            var expected =
                "UPDATE SET [Target].[Id] = [Source].[Id] ";
            var sut = new BulkOperationsHelpers();

            // Act
            var result = sut.BuildUpdateSet(updateOrInsertColumns, "Source", "Target", null);

            // Assert
            Assert.AreEqual(expected, result);

        }

        [Test]
        public void BulkOperationsHelpers_BuildInsertSet_BuildsCorrectSequenceForMultipleColumns()
        {
            // Arrange
            var updateOrInsertColumns = GetTestParameters();
            var expected =
                "INSERT ([Name], [Town], [Email], [IsCool]) values ([Source].[Name], [Source].[Town], [Source].[Email], [Source].[IsCool])";
            var sut = new BulkOperationsHelpers();

            // Act
            var result = sut.BuildInsertSet(updateOrInsertColumns, "Source", "id");

            // Assert
            Assert.AreEqual(expected, result);

        }

        [Test]
        public void BulkOperationsHelpers_BuildInsertSet_BuildsCorrectSequenceForSingleColumn()
        {
            // Arrange
            var updateOrInsertColumns = new HashSet<string>();
            updateOrInsertColumns.Add("Id");
            var expected =
                "INSERT ([Id]) values ([Source].[Id])";
            var sut = new BulkOperationsHelpers();

            // Act
            var result = sut.BuildInsertSet(updateOrInsertColumns, "Source", null);

            // Assert
            Assert.AreEqual(expected, result);

        }

        [Test]
        public void BulkOperationsHelpers_GetAllValueTypeAndStringColumns_ReturnsCorrectSet()
        {
            // Arrange
            BulkOperationsHelpers helper = new BulkOperationsHelpers();
            HashSet<string> expected = new HashSet<string>() {"Title", "CreatedTime", "BoolTest", "IntegerTest", "Price"};

            // Act
            var result = helper.GetAllValueTypeAndStringColumns(typeof (ModelWithMixedTypes));

            // Assert
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void BuilOperationsHelpers_GetIndexManagementCmd_WhenDisableAllIndexesIsTrueReturnsCorrectCmd()
        {
            // Arrange
            string expected =
                @"DECLARE @sql AS VARCHAR(MAX)=''; SELECT @sql = @sql + 'ALTER INDEX ' + sys.indexes.name + ' ON ' + sys.objects.name + ' DISABLE;'FROM sys.indexes JOIN sys.objects ON sys.indexes.object_id = sys.objects.object_id WHERE sys.indexes.type_desc = 'NONCLUSTERED' AND sys.objects.type_desc = 'USER_TABLE' AND sys.objects.name = 'Books'; EXEC(@sql);";
            BulkOperationsHelpers helper = new BulkOperationsHelpers();

            // Act
            string result = helper.GetIndexManagementCmd(IndexOperation.Disable, "Books", null, true);

            // Assert
            Assert.AreEqual(expected, result);

        }

        [Test]
        public void BuilOperationsHelpers_GetIndexManagementCmd_WithOneIndexReturnsCorrectCmd()
        {
            // Arrange
            string expected =
                @"DECLARE @sql AS VARCHAR(MAX)=''; SELECT @sql = @sql + 'ALTER INDEX ' + sys.indexes.name + ' ON ' + sys.objects.name + ' DISABLE;'FROM sys.indexes JOIN sys.objects ON sys.indexes.object_id = sys.objects.object_id WHERE sys.indexes.type_desc = 'NONCLUSTERED' AND sys.objects.type_desc = 'USER_TABLE' AND sys.objects.name = 'Books' AND sys.indexes.name = 'IX_Title'; EXEC(@sql);";
            BulkOperationsHelpers helper = new BulkOperationsHelpers();
            HashSet<string> indexes = new HashSet<string>();
            indexes.Add("IX_Title");

            // Act
            string result = helper.GetIndexManagementCmd(IndexOperation.Disable, "Books", indexes);

            // Assert
            Assert.AreEqual(expected, result);

        }

        [Test]
        public void BuildOperationsHelpers_RebuildSchema_WithExplicitSchemaIsCorrect()
        {
            // Arrange
            string expected = "[db].[CustomSchemaName].[TableName]";
            BulkOperationsHelpers helper = new BulkOperationsHelpers();

            // Act
            string result = helper.GetFullQualifyingTableName("db", "CustomSchemaName", "TableName");

            // Act
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void BuilOperationsHelpers_GetIndexManagementCmd_WithListOfIndexesReturnsCorrectCmd()
        {
            // Arrange
            string expected =
                @"DECLARE @sql AS VARCHAR(MAX)=''; SELECT @sql = @sql + 'ALTER INDEX ' + sys.indexes.name + ' ON ' + sys.objects.name + ' DISABLE;'FROM sys.indexes JOIN sys.objects ON sys.indexes.object_id = sys.objects.object_id WHERE sys.indexes.type_desc = 'NONCLUSTERED' AND sys.objects.type_desc = 'USER_TABLE' AND sys.objects.name = 'Books' AND sys.indexes.name = 'IX_Title' AND sys.indexes.name = 'IX_Price'; EXEC(@sql);";
            BulkOperationsHelpers helper = new BulkOperationsHelpers();
            HashSet<string> indexes = new HashSet<string>();
            indexes.Add("IX_Title");
            indexes.Add("IX_Price");

            // Act
            string result = helper.GetIndexManagementCmd(IndexOperation.Disable, "Books", indexes);

            // Assert
            Assert.AreEqual(expected, result);

        }



        private HashSet<string> GetTestParameters()
        {
            HashSet<string> parameters = new HashSet<string>();

            parameters.Add("id");
            parameters.Add("Name");
            parameters.Add("Town");
            parameters.Add("Email");
            parameters.Add("IsCool");

            return parameters;
        } 
    }
}
