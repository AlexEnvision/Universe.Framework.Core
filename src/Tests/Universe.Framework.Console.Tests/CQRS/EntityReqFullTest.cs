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
using Newtonsoft.Json;
using Universe.CQRS.Dal.Commands;
using Universe.CQRS.Dal.Queries;
using Universe.CQRS.Extensions;
using Universe.CQRS.Infrastructure;
using Universe.CQRS.Models.Sort;
using Universe.Diagnostic;
using Universe.Framework.ConsoleApp.Tests.CQRS.Base;
using Universe.Framework.ConsoleApp.Tests.CQRS.Models;
using Universe.Framework.ConsoleApp.Tests.Infrastructure;
using Universe.Framework.DataAccess.Test;
using Universe.Framework.DataAccess.Test.Models.Traiset;

namespace Universe.Framework.ConsoleApp.Tests.CQRS
{
    /// <summary>
    ///     Тест построителя запростов <see cref="EntityReqHelper"/>.
    /// <author>Alex Envision</author>
    /// </summary>
    public class EntityReqFullTest : BaseCommandQueryTests
    {
        public EntityReqFullTest()
        {
            PrepareToStart();
        }

        private void PrepareToStart()
        {
            Console.WriteLine(@"Готовится запуск EntityReqFullTest...");
        }

        private TrainsetClassDto GetTrainsetClassDto(string searchTemplate, UniverseScope<UniverseDbTestContext> scope)
        {
            var trainSetClass =
                scope.GetQuery<SelectEntitiesQuery<TrainsetClassDb, TrainsetClassDto>>().Execute(
                    EntityReqHelper.GetInTextsReq(
                        MetaInfoHelper.FieldMap(
                            MetaInfoHelper.MapRule<TrainsetClassDb>(nameof(TrainsetClassDto.Id), x => x.Id),
                            MetaInfoHelper.MapRule<TrainsetClassDb>(nameof(TrainsetClassDto.ClassName), x => x.ClassName)
                        ),
                        searchfieldName: nameof(TrainsetClassDto.ClassName),
                        patternValues: new List<string> { searchTemplate }
                    ),
                    projection: item =>
                        new TrainsetClassDto
                        {
                            Id = item.Id,
                            SessionId = item.SessionID,
                            ClassName = item.ClassName,
                            TrainSet = item.TrainsetId.HasValue
                                ? new TrainsetDto
                                {
                                    Id = item.Trainset.Id,
                                    Name = item.Trainset.Name
                                }
                                : null
                        }
                ).Items.FirstOrDefault();
            return trainSetClass;
        }

        private TrainsetClassDto CreateTrainSetClass(
            UniverseScope<UniverseDbTestContext> scope,
            Guid sessionId,
            string searchTemplate)
        {
            var searchTemplateTrainSet = "001";

            var trainSet = GetTrainSetDto(scope, searchTemplate);

            if (trainSet == null)
            {
                Console.WriteLine(
                    $@"По заданному шаблону поиска {searchTemplateTrainSet} не найдено ни одного подходящей выборки!");
                Console.WriteLine(
                    @"Будет создан новый класс...");
                trainSet = CreateTrainSet(scope, sessionId, searchTemplateTrainSet);
            }

            var itemDb = new TrainsetClassDb
            {
                SessionID = sessionId,
                ClassName = $"ClassName{searchTemplate}",
                TrainsetId = trainSet.Id,
                Created = DateTimeOffset.UtcNow
            };

            scope.GetCommand<AddEntityCommand<TrainsetClassDb>>().Execute(itemDb);
            var trainSetClass = GetTrainsetClassDto(searchTemplate, scope);

            return trainSetClass;
        }

        private TrainsetDto GetTrainSetDto(UniverseScope<UniverseDbTestContext> scope, string searchTemplate)
        {
            var trainSet =
                scope.GetQuery<SelectEntitiesQuery<TrainsetDb, TrainsetDto>>().Execute(
                    EntityReqHelper.GetInTextsReq(
                        MetaInfoHelper.FieldMap(
                            MetaInfoHelper.MapRule<TrainsetDb>(nameof(TrainsetDto.Id), x => x.Id),
                            MetaInfoHelper.MapRule<TrainsetDb>(nameof(TrainsetDto.Name), x => x.Name)
                        ),
                        searchfieldName: nameof(TrainsetDto.Name),
                        patternValues: new List<string> { searchTemplate }
                    ),
                    projection: item =>
                        new TrainsetDto
                        {
                            Id = item.Id,
                            Name = item.Name
                        }
                ).Items.FirstOrDefault();
            return trainSet;
        }

        private TrainsetDto CreateTrainSet(UniverseScope<UniverseDbTestContext> scope, Guid sessionId, string searchTemplate)
        {
            var itemDb = new TrainsetDb
            {
                SessionID = sessionId,
                Name = $"TrainSet{searchTemplate}",
                Created = DateTimeOffset.UtcNow
            };

            scope.GetCommand<AddEntityCommand<TrainsetDb>>().Execute(itemDb);
            var trainSetClass = GetTrainSetDto(scope, searchTemplate);
            return trainSetClass;
        }


        public void CreateEntityQueryTest()
        {
            var container = UnityConfig.Container;

            var settings = new AppTestSettings();
            var scope = new UniverseScope<UniverseDbTestContext>(settings, container);

            var sessionId = Guid.NewGuid();

            using (var runningTimeWatcher = new RunningTimeWatcher())
            {
                Console.WriteLine(@"Создание элемента в БД...");
                var searchTemplate = "89";

                var trainSetClass = GetTrainsetClassDto(searchTemplate, scope);
                if (trainSetClass == null)
                {
                    Console.WriteLine(
                        $@"По заданному шаблону поиска {searchTemplate} не найдено ни одного подходящего класса!");
                    Console.WriteLine(
                        @"Будет создан новый класс...");
                    trainSetClass = CreateTrainSetClass(scope, sessionId, searchTemplate);
                }

                var itemDb = new TrainsetItemDb
                {
                    SessionID = sessionId,
                    TrainsetItemValue = "01011001",
                    ClassName = trainSetClass.ClassName,
                    TrainsetClassId = trainSetClass.Id,
                    Created = DateTimeOffset.UtcNow
                };

                var result = scope.GetCommand<AddEntityCommand<TrainsetItemDb>>().Execute(itemDb);
                Console.WriteLine($@"Время выполнения команды: {runningTimeWatcher.TakeRunningTime():G}");

                var resultSfy = JsonConvert.SerializeObject(result, Formatting.Indented);
                Console.WriteLine(
                    $@"Результат создания элемента таблицы {nameof(TrainsetItemDb)}: {Environment.NewLine}{resultSfy}");
            }
        }

        public void CreateEntityQueryTransactionTest()
        {
            var container = UnityConfig.Container;

            var settings = new AppTestSettings();
            var scope = new UniverseScope<UniverseDbTestContext>(settings, container);

            var sessionId = Guid.NewGuid();

            using (var runningTimeWatcher = new RunningTimeWatcher())
            {
                Console.WriteLine(@"Создание элемента в БД в одной трансзакции...");
                var result = scope.UnitOfWork.Exec((context, transaction) =>
                {
                    var searchTemplate = "89";

                    var trainSetClass = GetTrainsetClassDto(searchTemplate, scope);
                    if (trainSetClass == null)
                    {
                        Console.WriteLine(
                            $@"По заданному шаблону поиска {searchTemplate} не найдено ни одного подходящего класса!");
                        Console.WriteLine(
                            @"Будет создан новый класс...");
                        trainSetClass = CreateTrainSetClass(scope, sessionId, searchTemplate);
                    }

                    var itemDb = new TrainsetItemDb
                    {
                        SessionID = sessionId,
                        TrainsetItemValue = "01011001",
                        ClassName = trainSetClass.ClassName,
                        TrainsetClassId = trainSetClass.Id,
                        Created = DateTimeOffset.UtcNow
                    };

                    return scope.GetCommand<AddEntityCommand<TrainsetItemDb>>().Execute(itemDb);
                });


                Console.WriteLine($@"Время выполнения команды: {runningTimeWatcher.TakeRunningTime():G}");

                if (result != null)
                {
                    var resultSfy = JsonConvert.SerializeObject(result, Formatting.Indented);
                    Console.WriteLine(
                        $@"Результат создания элемента таблицы {nameof(TrainsetItemDb)}: {Environment.NewLine}{resultSfy}");
                }
                else
                {
                    Console.WriteLine(
                        $@"Не удалось создать ни одного элемента в таблице {nameof(TrainsetItemDb)}!");
                }
            }
        }

        public void CreateEntitiesQueryTest()
        {
            var container = UnityConfig.Container;

            var settings = new AppTestSettings();
            var scope = new UniverseScope<UniverseDbTestContext>(settings, container);

            var sessionId = Guid.NewGuid();

            using (var runningTimeWatcher = new RunningTimeWatcher())
            {
                Console.WriteLine(@"Создание элемента в БД...");
                var searchTemplate = "89";

                var trainSetClass = GetTrainsetClassDto(searchTemplate, scope);
                if (trainSetClass == null)
                {
                    Console.WriteLine(
                        $@"По заданному шаблону поиска {searchTemplate} не найдено ни одного подходящего класса!");
                    Console.WriteLine(
                        @"Будет создан новый класс...");
                    trainSetClass = CreateTrainSetClass(scope, sessionId, searchTemplate);
                }

                var creatingItemsCount = 8;

                var itemsDb = new List<TrainsetItemDb>();
                for (int i = 0; i < creatingItemsCount; i++)
                {
                    var itemDb = new TrainsetItemDb
                    {
                        SessionID = sessionId,
                        TrainsetItemValue = "01011001",
                        ClassName = trainSetClass.ClassName,
                        TrainsetClassId = trainSetClass.Id,
                        Created = DateTimeOffset.UtcNow
                    };
                    itemsDb.Add(itemDb);
                }

                var result = scope.GetCommand<AddEntitiesCommand<TrainsetItemDb>>().Execute(itemsDb);
                Console.WriteLine($@"Время выполнения команды: {runningTimeWatcher.TakeRunningTime():G}");

                var resultSfy = JsonConvert.SerializeObject(result, Formatting.Indented);
                Console.WriteLine(
                    $@"Результат создания элементов таблицы {nameof(TrainsetItemDb)}: {Environment.NewLine}{resultSfy}");
            }
        }

        public void ReadEntitiesQueryTest()
        {
            var container = UnityConfig.Container;

            var settings = new AppTestSettings();
            var scope = new UniverseScope<UniverseDbTestContext>(settings, container);

            using (var runningTimeWatcher = new RunningTimeWatcher())
            {
                Console.WriteLine(@"Чтение данных из БД...");

                var resultSelectById = scope.GetQuery<SelectEntitiesQuery<TrainsetItemDb, TrainsetItemDto>>().Execute(
                    EntityReqHelper.GetEqReq(
                        MetaInfoHelper.FieldMap(
                            MetaInfoHelper.MapRule<TrainsetItemDb>(nameof(TrainsetItemDto.Id), x => x.Id)
                        ),
                        eqfieldName: nameof(TrainsetItemDto.Id),
                        eqvalue: "42",
                        countOnPage: 30,
                        pageIndex: 1
                    ),
                    projection: item => new TrainsetItemDto
                    {
                        Id = item.Id,
                        ClassName = item.ClassName,
                        TrainsetItemValue = item.TrainsetItemValue
                    }
                );

                var resultSfy = JsonConvert.SerializeObject(resultSelectById, Formatting.Indented);
                Console.WriteLine($@"Элемент таблицы {nameof(TrainsetItemDb)}: {Environment.NewLine}{resultSfy}");

                var resultGetEntitiesByClassName = scope.GetQuery<SelectEntitiesQuery<TrainsetItemDb, TrainsetItemDto>>().Execute(
                    EntityReqHelper.GetInTextsReq(
                        MetaInfoHelper.FieldMap(
                            MetaInfoHelper.MapRule<TrainsetItemDb>(nameof(TrainsetItemDto.Id), x => x.Id),
                            MetaInfoHelper.MapRule<TrainsetItemDb>("ClassName", x => x.TrainsetClass.ClassName)
                        ),
                        searchfieldName: "ClassName",
                        patternValues: new List<string> { "42" },
                        countOnPage: 30,
                        pageIndex: 1
                    ),
                    projection: item => new TrainsetItemDto
                    {
                        Id = item.Id,
                        ClassName = item.ClassName,
                        TrainsetItemValue = item.TrainsetItemValue,
                        TrainsetClass = item.TrainsetClassId.HasValue
                            ? new TrainsetClassDto
                            {
                                Id = item.TrainsetClass.Id,
                                ClassName = item.TrainsetClass.ClassName,
                                TrainSet = item.TrainsetClass.TrainsetId.HasValue ? new TrainsetDto
                                {
                                    Id = item.TrainsetClass.Trainset.Id,
                                    Name = item.TrainsetClass.Trainset.Name
                                } : null
                            }
                            : null
                    }
                );

                resultSfy = JsonConvert.SerializeObject(resultGetEntitiesByClassName, Formatting.Indented);
                Console.WriteLine($@"Элемент таблицы {nameof(TrainsetItemDb)}: {Environment.NewLine}{resultSfy}");

                var resultSelectEntitiesByClassName = scope.GetQuery<SelectEntitiesQuery<TrainsetItemDb, TrainsetItemDto>>().Execute(
                    EntityReqHelper.GetNeInReq(
                        MetaInfoHelper.FieldMap(
                            MetaInfoHelper.MapRule<TrainsetItemDb>(nameof(TrainsetItemDto.Id), x => x.Id),
                            MetaInfoHelper.MapRule<TrainsetItemDb>("ClassName", x => x.TrainsetClass.ClassName)
                        ),
                        searchfieldName: "ClassName",
                        antaSearchValues: new List<string> { "2084", "89" },
                        countOnPage: 30,
                        pageIndex: 1
                    ),
                    projection: item => new TrainsetItemDto
                    {
                        Id = item.Id,
                        ClassName = item.ClassName,
                        TrainsetItemValue = item.TrainsetItemValue
                    }
                );

                resultSfy = JsonConvert.SerializeObject(resultSelectEntitiesByClassName, Formatting.Indented);
                Console.WriteLine($@"Элемент таблицы {nameof(TrainsetItemDb)}: {Environment.NewLine}{resultSfy}");

                try
                {
                    var resultSelectEntitiesByClassNamePlusValue = scope.GetQuery<SelectEntitiesQuery<TrainsetItemDb, TrainsetItemDto>>().Execute(
                        EntityReqHelper.GetContainsReq(
                            MetaInfoHelper.FieldMap(
                                MetaInfoHelper.MapRule<TrainsetItemDto>(nameof(TrainsetItemDto.Id), x => x.Id),
                                MetaInfoHelper.MapRule<TrainsetItemDto>("ClassName+TrainSetItemValue",
                                    x => x.TrainsetClass.ClassName + " " + x.TrainsetItemValue)
                            ),
                            searchFieldName: "ClassName+TrainSetItemValue",
                            patternValue: "89 01011001",
                            countOnPage: 30,
                            pageIndex: 1
                        ),
                        projection: item => new TrainsetItemDto
                        {
                            Id = item.Id,
                            TrainsetItemValue = item.TrainsetItemValue,
                            TrainsetClass = item.TrainsetClassId.HasValue
                                ? new TrainsetClassDto
                                {
                                    Id = item.TrainsetClass.Id,
                                    ClassName = item.TrainsetClass.ClassName
                                }
                                : null
                        }
                    );

                    Console.WriteLine($@"Время выполнения запроса: {runningTimeWatcher.TakeRunningTime():G}");

                    resultSfy = JsonConvert.SerializeObject(resultSelectEntitiesByClassNamePlusValue, Formatting.Indented);
                    Console.WriteLine($@"Элемент таблицы {nameof(TrainsetItemDb)}: {Environment.NewLine}{resultSfy}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public void UpdateEntitiesCommandTest()
        {
            var container = UnityConfig.Container;

            var settings = new AppTestSettings();
            var scope = new UniverseScope<UniverseDbTestContext>(settings, container);

            var sessionId = Guid.NewGuid();

            using (var runningTimeWatcher = new RunningTimeWatcher())
            {
                Console.WriteLine(@"Обновление элементов в БД...");
                var searchTemplate = "01011001";

                var trainSetItemsDb =
                    scope.GetQuery<GetEntitiesQuery<TrainsetItemDb>>().Execute(
                        EntityReqHelper.GetContainsReq(
                            MetaInfoHelper.FieldMap(
                                MetaInfoHelper.MapRule<TrainsetItemDb>(nameof(TrainsetItemDb.Id), x => x.Id),
                                MetaInfoHelper.MapRule<TrainsetItemDb>(nameof(TrainsetItemDb.TrainsetItemValue), x => x.TrainsetItemValue)),
                            searchFieldName: nameof(TrainsetItemDb.TrainsetItemValue),
                            patternValue: searchTemplate
                    ));

                if (trainSetItemsDb == null || trainSetItemsDb.Items.Count == 0)
                {
                    Console.WriteLine(
                        $@"По заданному шаблону поиска {searchTemplate} не найдено ни одного подходящего элемента!");
                    return;
                }

                foreach (var trainSetItemDb in trainSetItemsDb.Items)
                {
                    trainSetItemDb.SessionID = sessionId;
                }

                var result = scope.GetCommand<UpdateEntitiesCommand<TrainsetItemDb>>().Execute(trainSetItemsDb.Items);
                Console.WriteLine($@"Время выполнения команды: {runningTimeWatcher.TakeRunningTime():G}");

                var resultSfy = JsonConvert.SerializeObject(result, Formatting.Indented);
                Console.WriteLine(
                    $@"Результат обновления элементов таблицы {nameof(TrainsetItemDb)}: {Environment.NewLine}{resultSfy}");
            }
        }

        public void DeleteEntitiesCommandTest()
        {
            var container = UnityConfig.Container;

            var settings = new AppTestSettings();
            var scope = new UniverseScope<UniverseDbTestContext>(settings, container);

            var deletingItemsCount = 3;

            using (var runningTimeWatcher = new RunningTimeWatcher())
            {
                Console.WriteLine(@"Обновление элементов в БД...");

                var lastItemsIds = scope.GetQuery<SelectEntitiesQuery<TrainsetItemDb, TrainsetItemDto>>().Execute(
                    EntityReqHelper.GetAnyReq(MetaInfoHelper.FieldMap(
                        MetaInfoHelper.MapRule<TrainsetItemDb>(nameof(TrainsetItemDb.Id), x => x.Id)))
                        .Sorting(EntityReqHelper.GetSortRule(nameof(TrainsetItemDb.Id), SortDirection.Desc)), 
                    trainSetItem => new TrainsetItemDto
                    {
                        Id = trainSetItem.Id
                    }
                ).Items.Take(deletingItemsCount).Select(x => x.Id.ToString()).ToList();

                var deletingItems = scope.GetQuery<GetEntitiesQuery<TrainsetItemDb>>().Execute(
                    EntityReqHelper.GetInReq(
                        MetaInfoHelper.FieldMap(
                            MetaInfoHelper.MapRule<TrainsetItemDb>(nameof(TrainsetItemDb.Id), x => x.Id),
                            MetaInfoHelper.MapRule<TrainsetItemDb>(nameof(TrainsetItemDb.TrainsetItemValue),
                                x => x.TrainsetItemValue)),
                        searchfieldName: nameof(TrainsetItemDb.Id),
                        searchvalues: lastItemsIds
                    )).Items;

                var result = scope.GetCommand<DeleteEntitiesCommand<TrainsetItemDb>>().Execute(deletingItems);
                Console.WriteLine($@"Время выполнения команды: {runningTimeWatcher.TakeRunningTime():G}");

                var resultSfy = JsonConvert.SerializeObject(result, Formatting.Indented);
                Console.WriteLine(
                    $@"Результат обновления элементов таблицы {nameof(TrainsetItemDb)}: {Environment.NewLine}{resultSfy}");
            }
        }
    }
}