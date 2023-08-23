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
using System.Linq.Expressions;
using Newtonsoft.Json;
using Unity;
using Universe.CQRS.Dal.Base.MetaInfo;
using Universe.CQRS.Dal.Commands;
using Universe.CQRS.Dal.Queries;
using Universe.CQRS.Infrastructure;
using Universe.CQRS.Models.Condition;
using Universe.CQRS.Models.Filter.Custom;
using Universe.CQRS.Models.Req;
using Universe.CQRS.Models.Sort;
using Universe.Diagnostic;
using Universe.Framework.ConsoleApp.Tests.CQRS.Base;
using Universe.Framework.ConsoleApp.Tests.CQRS.Models;
using Universe.Framework.ConsoleApp.Tests.Infrastructure;
using Universe.Framework.DataAccess.Test;
using Universe.Framework.DataAccess.Test.Models.Traiset;
using BetweenConfiguration = Universe.CQRS.Models.Condition.BetweenConfiguration;

namespace Universe.Framework.ConsoleApp.Tests.CQRS
{
    /// <summary>
    ///     Тест запросов и команд.
    /// <author>Alex Envision</author>
    /// </summary>
    public class CommandQueryTests : BaseCommandQueryTests
    {
        public CommandQueryTests()
        {
            PrepareToStart();
        }

        private void PrepareToStart()
        {
            Console.WriteLine(@"Готовится запуск CommandQueryTests...");
        }

        private TrainsetClassDto GetTrainsetClassDto(string searchTemplate, UniverseScope<UniverseDbTestContext> scope)
        {
            var fieldMapContainer = new FieldMapContainer<TrainsetClassDb>
            {
                FieldMap = new Dictionary<string, Expression<Func<TrainsetClassDb, object>>>
                {
                    {"ClassName", x => x.ClassName},
                }
            };

            var filter = new ContainsConfiguration
            {
                LeftOperand = new FieldArgumentConfiguration
                {
                    Field = new FieldConfiguration
                    {
                        SpFieldName = "ClassName",
                    }
                },
                RightOperand = new ValueArgumentConfiguration
                {
                    Expression = searchTemplate
                }
            };

            var filters = new List<ConditionConfiguration> { filter };

            var trainSetClass =
                scope.GetQuery<SelectEntitiesQuery<TrainsetClassDb, TrainsetClassDto>>().Execute(
                    new GetEntitiesReq
                    {
                        FieldMapContainer = fieldMapContainer,
                        Filters = filters
                    },
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
            var fieldMapContainer = new FieldMapContainer<TrainsetDb>
            {
                FieldMap = new Dictionary<string, Expression<Func<TrainsetDb, object>>>
                {
                    {"Name", x => x.Name},
                }
            };

            var filter = new ContainsConfiguration
            {
                LeftOperand = new FieldArgumentConfiguration
                {
                    Field = new FieldConfiguration
                    {
                        SpFieldName = "Name",
                    }
                },
                RightOperand = new ValueArgumentConfiguration
                {
                    Expression = searchTemplate
                }
            };

            var filters = new List<ConditionConfiguration> { filter };

            var trainSet =
                scope.GetQuery<SelectEntitiesQuery<TrainsetDb, TrainsetDto>>().Execute(
                    new GetEntitiesReq
                    {
                        FieldMapContainer = fieldMapContainer,
                        Filters = filters
                    },
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

        public void CreateEntityCommandTest()
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

        public void CreateAndUndoEntityCommandTest()
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

                var createCommand = scope.GetCommand<AddEntityCommand<TrainsetItemDb>>();
                var result = createCommand.Execute(itemDb);
                Console.WriteLine($@"Время выполнения команды: {runningTimeWatcher.TakeRunningTime():G}");
                runningTimeWatcher.Reset();
                runningTimeWatcher.Continue();

                var resultSfy = JsonConvert.SerializeObject(result, Formatting.Indented);
                Console.WriteLine(
                    $@"Результат создания элемента таблицы {nameof(TrainsetItemDb)}: {Environment.NewLine}{resultSfy}");

                Console.WriteLine(@"Откат создания данных из БД...");

                var undoResult = createCommand.Undo();

                Console.WriteLine($@"Время выполнения отката команды: {runningTimeWatcher.TakeRunningTime():G}");

                var rResultSfy = JsonConvert.SerializeObject(undoResult, Formatting.Indented);
                Console.WriteLine(
                    $@"Удалён элемент таблицы {nameof(TrainsetItemDb)}: {Environment.NewLine}{rResultSfy}");
            }
        }

        public void CreateEntityCommandTransactionTest()
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

        public void CreateEntitiesCommandTest()
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
                    var itemDb = new TrainsetItemDb {
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

        public void ReadEntityQueryTest()
        {
            var container = UnityConfig.Container;

            var settings = new AppTestSettings();
            var scope = new UniverseScope<UniverseDbTestContext>(settings, container);

            using (var runningTimeWatcher = new RunningTimeWatcher())
            {
                Console.WriteLine(@"Чтение данных из БД...");
                var result = scope.GetQuery<SelectEntityQuery<TrainsetItemDb, TrainsetItemDto>>().Execute(
                    value: "42",
                    searchItemRule: item => item.Id,
                    projection: item => new TrainsetItemDto
                    {
                        Id = item.Id,
                        ClassName = item.ClassName,
                        TrainsetItemValue = item.TrainsetItemValue,
                        TrainsetClass = item.TrainsetClassId.HasValue
                            ? new TrainsetClassDto
                            {
                                Id = item.TrainsetClass.Id,
                                SessionId = item.TrainsetClass.SessionID,
                                TrainSet = item.TrainsetClass.TrainsetId.HasValue
                                    ? new TrainsetDto
                                    {
                                        Id = item.TrainsetClass.Trainset.Id,
                                        Name = item.TrainsetClass.Trainset.Name
                                    }
                                    : null,
                                Items = item.TrainsetClass.TrainsetsItems.Where(x => x.Id == 89).Select(
                                    classItem => new TrainsetItemShortDto
                                    {
                                        Id = classItem.Id,
                                        ClassName = classItem.ClassName
                                    }
                                ).ToList()
                            }
                            : null
                    }
                );

                Console.WriteLine($@"Время выполнения запроса: {runningTimeWatcher.TakeRunningTime():G}");

                var resultSfy = JsonConvert.SerializeObject(result, Formatting.Indented);
                Console.WriteLine($@"Элемент таблицы {nameof(TrainsetItemDb)}: {Environment.NewLine}{resultSfy}");
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

                var fieldMapContainer = new FieldMapContainer<TrainsetClassDb> {
                    FieldMap = new Dictionary<string, Expression<Func<TrainsetClassDb, object>>> {
                        {"ClassName", x => x.ClassName},
                    }
                };

                var filters = new List<ConditionConfiguration>();
                var filter = new ContainsConfiguration {
                    LeftOperand = new FieldArgumentConfiguration {
                        Field = new FieldConfiguration {
                            SpFieldName = "ClassName",
                        }
                    },
                    RightOperand = new ValueArgumentConfiguration {
                        Expression = "89"
                    }
                };
                filters.Add(filter);

                var orTrainSetItemFilter = new OrConfiguration {
                    Operands = filters
                };

                var result = scope.GetQuery<SelectEntitiesQuery<TrainsetItemDb, TrainsetItemDto>>().Execute(
                    new GetEntitiesReq {
                        FieldMapContainer = fieldMapContainer,
                        Filters = new List<ConditionConfiguration> {
                            orTrainSetItemFilter
                        }
                    },
                    projection: item => new TrainsetItemDto {
                        Id = item.Id,
                        ClassName = item.ClassName,
                        TrainsetItemValue = item.TrainsetItemValue,
                        TrainsetClass = item.TrainsetClassId.HasValue
                            ? new TrainsetClassDto {
                                Id = item.TrainsetClass.Id,
                                SessionId = item.TrainsetClass.SessionID,
                                TrainSet = item.TrainsetClass.TrainsetId.HasValue
                                    ? new TrainsetDto
                                    {
                                        Id = item.TrainsetClass.Trainset.Id,
                                        Name = item.TrainsetClass.Trainset.Name
                                    } : null
                            } : null
                    }
                );

                Console.WriteLine($@"Время выполнения запроса: {runningTimeWatcher.TakeRunningTime():G}");

                var resultSfy = JsonConvert.SerializeObject(result, Formatting.Indented);
                Console.WriteLine($@"Элемент таблицы {nameof(TrainsetItemDb)}: {Environment.NewLine}{resultSfy}");
            }
        }


        public void ReadEntitiesQueryByDatePeriodTest()
        {
            var container = UnityConfig.Container;

            var settings = new AppTestSettings();
            var scope = new UniverseScope<UniverseDbTestContext>(settings, container);

            using (var runningTimeWatcher = new RunningTimeWatcher())
            {
                Console.WriteLine(@"Чтение данных из БД...");

                var fieldMapContainer = new FieldMapContainer<TrainsetClassDb>
                {
                    FieldMap = new Dictionary<string, Expression<Func<TrainsetClassDb, object>>> {
                        {nameof(TrainsetClassDb.Created), x => x.ClassName},
                    }
                };

                var filters = new List<ConditionConfiguration>();
                var filter = new BetweenConfiguration
                {
                    LeftOperand = new FieldArgumentConfiguration
                    {
                        Field = new FieldConfiguration
                        {
                            SpFieldName = nameof(TrainsetClassDb.Created),
                        }
                    },
                    RightOperand = new BetweenArgumentConfiguration
                    {
                        Value = new DataTimePeriod
                        {
                            Start = new DateTimeOffset(2022, 1, 1, 1, 1, 1, new TimeSpan()),
                            End = DateTimeOffset.UtcNow
                        }
                    }
                };
                filters.Add(filter);

                var orTrainSetItemFilter = new OrConfiguration
                {
                    Operands = filters
                };

                var req = new GetEntitiesReq
                {
                    FieldMapContainer = fieldMapContainer,
                    Filters = new List<ConditionConfiguration>
                    {
                        orTrainSetItemFilter
                    }
                };

                var reqsfy = JsonConvert.SerializeObject(req, Formatting.Indented);

                var result = scope.GetQuery<SelectEntitiesQuery<TrainsetItemDb, TrainsetItemDto>>().Execute(
                    req,
                    projection: item => new TrainsetItemDto
                    {
                        Id = item.Id,
                        ClassName = item.ClassName,
                        TrainsetItemValue = item.TrainsetItemValue,
                        TrainsetClass = item.TrainsetClassId.HasValue
                            ? new TrainsetClassDto
                            {
                                Id = item.TrainsetClass.Id,
                                SessionId = item.TrainsetClass.SessionID,
                                TrainSet = item.TrainsetClass.TrainsetId.HasValue
                                    ? new TrainsetDto
                                    {
                                        Id = item.TrainsetClass.Trainset.Id,
                                        Name = item.TrainsetClass.Trainset.Name
                                    }
                                    : null
                            }
                            : null,
                        Created = item.Created
                    }
                );

                Console.WriteLine($@"Время выполнения запроса: {runningTimeWatcher.TakeRunningTime():G}");

                var resultSfy = JsonConvert.SerializeObject(result, Formatting.Indented);
                Console.WriteLine($@"Элемент таблицы {nameof(TrainsetItemDb)}: {Environment.NewLine}{resultSfy}");
            }
        }

        public void UpdateEntityCommandTest()
        {
            var container = UnityConfig.Container;

            var settings = new AppTestSettings();
            var scope = new UniverseScope<UniverseDbTestContext>(settings, container);

            var sessionId = Guid.NewGuid();

            using (var runningTimeWatcher = new RunningTimeWatcher())
            {
                Console.WriteLine(@"Обновление элемента в БД...");
                var searchTemplate = "01011001";

                var trainSetItemDb =
                    scope.GetQuery<GetEntityQuery<TrainsetItemDb>>().Execute(
                        dto => dto.TrainsetItemValue == searchTemplate
                    );

                if (trainSetItemDb == null)
                {
                    Console.WriteLine(
                        $@"По заданному шаблону поиска {searchTemplate} не найдено ни одного подходящего элемента!");
                    return;
                }

                trainSetItemDb.SessionID = sessionId;

                var result = scope.GetCommand<UpdateEntityCommand<TrainsetItemDb>>().Execute(trainSetItemDb);
                Console.WriteLine($@"Время выполнения команды: {runningTimeWatcher.TakeRunningTime():G}");

                var resultSfy = JsonConvert.SerializeObject(result, Formatting.Indented);
                Console.WriteLine(
                    $@"Результат обновления элемента таблицы {nameof(TrainsetItemDb)}: {Environment.NewLine}{resultSfy}");
            }
        }

        public void UpdateEntityAndUndoCommandTest()
        {
            var container = UnityConfig.Container;

            var settings = new AppTestSettings();
            var scope = new UniverseScope<UniverseDbTestContext>(settings, container);

            var sessionId = Guid.NewGuid();

            using (var runningTimeWatcher = new RunningTimeWatcher())
            {
                Console.WriteLine(@"Обновление элемента в БД...");
                var searchTemplate = "01011001";

                var trainSetItemDb =
                    scope.GetQuery<GetEntityQuery<TrainsetItemDb>>().Execute(
                        dto => dto.TrainsetItemValue == searchTemplate
                    );

                if (trainSetItemDb == null)
                {
                    Console.WriteLine(
                        $@"По заданному шаблону поиска {searchTemplate} не найдено ни одного подходящего элемента!");
                    return;
                }

                trainSetItemDb.SessionID = sessionId;

                var updateCommand = scope.GetCommand<UpdateEntityRbCommand<TrainsetItemDb>>();
                var result = updateCommand.Execute(trainSetItemDb);
                Console.WriteLine($@"Время выполнения команды: {runningTimeWatcher.TakeRunningTime():G}");

                var resultSfy = JsonConvert.SerializeObject(result, Formatting.Indented);
                Console.WriteLine(
                    $@"Результат обновления элемента таблицы {nameof(TrainsetItemDb)}: {Environment.NewLine}{resultSfy}");

                Console.WriteLine(@"Откат обновления данных из БД...");

                var undoResult = updateCommand.Undo();

                Console.WriteLine($@"Время выполнения отката команды: {runningTimeWatcher.TakeRunningTime():G}");

                var rResultSfy = JsonConvert.SerializeObject(undoResult, Formatting.Indented);
                Console.WriteLine(
                    $@"Восстановлено исходное состояние элемента таблицы {nameof(TrainsetItemDb)}: {Environment.NewLine}{rResultSfy}");
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
                var fieldMapContainer = new FieldMapContainer<TrainsetItemDb>
                {
                    FieldMap = new Dictionary<string, Expression<Func<TrainsetItemDb, object>>>
                    {
                        { nameof(TrainsetItemDb.Id), x => x.Id},
                        { nameof(TrainsetItemDb.TrainsetItemValue), x => x.TrainsetItemValue},
                    }
                };

                var filter = new ContainsConfiguration
                {
                    LeftOperand = new FieldArgumentConfiguration
                    {
                        Field = new FieldConfiguration
                        {
                            SpFieldName = "TrainsetItemValue",
                        }
                    },
                    RightOperand = new ValueArgumentConfiguration
                    {
                        Expression = searchTemplate
                    }
                };

                var filters = new List<ConditionConfiguration> { filter };

                var trainSetItemsDb =
                    scope.GetQuery<GetEntitiesQuery<TrainsetItemDb>>().Execute(
                        new GetEntitiesReq
                        {
                            FieldMapContainer = fieldMapContainer,
                            Filters = filters
                        }
                    );

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

        public void DeleteEntityQueryTest()
        {
            var container = UnityConfig.Container;

            var settings = new AppTestSettings();
            var scope = new UniverseScope<UniverseDbTestContext>(settings, container);

            using (var runningTimeWatcher = new RunningTimeWatcher())
            {
                Console.WriteLine($@"Удаление данных из БД. Удаляется последний элемент из таблицы {nameof(TrainsetItemDb)}...");

                var itemsCount = scope.GetQuery<GetEntitiesCountQuery<TrainsetItemDb>>().Execute();
                Console.WriteLine($@"Количество элементов в таблице {nameof(TrainsetItemDb)} до удаления: {itemsCount}");

                var lastItem = scope.GetQuery<GetEntitiesQuery<TrainsetItemDb>>().Execute(
                    new GetEntitiesReq
                    {
                        FieldMapContainer = new FieldMapContainer<TrainsetItemDb> {
                            FieldMap = new Dictionary<string, Expression<Func<TrainsetItemDb, object>>> {
                                { "Id", x => x.Id }
                            }
                        },
                        Sorting = new List<SortConfiguration> {
                            new SortConfiguration {
                                Field = "Id",
                                Direction = SortDirection.Desc
                            }
                        }
                    }
                ).Items.FirstOrDefault();

                var deleteCommand = scope.GetCommand<DeleteEntityCommand<TrainsetItemDb>>();
                var deleteResult = deleteCommand.Execute(lastItem);

                Console.WriteLine($@"Время выполнения команды: {runningTimeWatcher.TakeRunningTime():G}");
                var itemsCountAfter = scope.GetQuery<GetEntitiesCountQuery<TrainsetItemDb>>().Execute();
                Console.WriteLine($@"Количество элементов в таблице {nameof(TrainsetItemDb)} после удаления: {itemsCountAfter}");

                var dResultSfy = JsonConvert.SerializeObject(deleteResult, Formatting.Indented);
                Console.WriteLine($@"Элемент таблицы {nameof(TrainsetItemDb)}: {Environment.NewLine}{dResultSfy}");
            }
        }

        public void DeleteEntitiesQueryTest()
        {
            var container = UnityConfig.Container;

            var settings = new AppTestSettings();
            var scope = new UniverseScope<UniverseDbTestContext>(settings, container);

            var deletingItemsCount = 3;

            using (var runningTimeWatcher = new RunningTimeWatcher())
            {
                Console.WriteLine($@"Удаление данных из БД. Удаляется {deletingItemsCount} последних элемента из таблицы {nameof(TrainsetItemDb)}...");

                var itemsCount = scope.GetQuery<GetEntitiesCountQuery<TrainsetItemDb>>().Execute();
                Console.WriteLine($@"Количество элементов в таблице {nameof(TrainsetItemDb)} до удаления: {itemsCount}");

                var lastItems = scope.GetQuery<SelectEntitiesQuery<TrainsetItemDb, TrainsetItemDto>>().Execute(
                    new GetEntitiesReq
                    {
                        FieldMapContainer = new FieldMapContainer<TrainsetItemDb>
                        {
                            FieldMap = new Dictionary<string, Expression<Func<TrainsetItemDb, object>>> {
                                { "Id", x => x.Id }
                            }
                        },
                        Sorting = new List<SortConfiguration> {
                            new SortConfiguration {
                                Field = "Id",
                                Direction = SortDirection.Desc
                            }
                        }
                    }, trainSetItem => new TrainsetItemDto
                    {
                        Id = trainSetItem.Id
                    }
                ).Items.Take(deletingItemsCount).ToList();

                var filtersInternal = new List<ConditionConfiguration>();
                foreach (var trainSetItemDto in lastItems)
                {
                    var filter = new EqConfiguration {
                        LeftOperand = new FieldArgumentConfiguration
                        {
                            Field = new FieldConfiguration
                            {
                                SpFieldName = "Id",
                            }
                        },
                        RightOperand = new ValueArgumentConfiguration
                        {
                            Expression = trainSetItemDto.Id.ToString()
                        }
                    };
                    filtersInternal.Add(filter);
                }

                var filters = new List<ConditionConfiguration>
                {
                    new OrConfiguration
                    {
                        Operands = filtersInternal
                    }
                };

                var deletingItems = scope.GetQuery<GetEntitiesQuery<TrainsetItemDb>>().Execute(new GetEntitiesReq {
                    FieldMapContainer = new FieldMapContainer<TrainsetItemDb>
                    {
                        FieldMap = new Dictionary<string, Expression<Func<TrainsetItemDb, object>>>
                        {
                            {"Id", x => x.Id}
                        }
                    },
                    Filters = filters,
                    Sorting = new List<SortConfiguration>
                    {
                        new SortConfiguration
                        {
                            Field = "Id",
                            Direction = SortDirection.Desc
                        }
                    }
                }).Items;

                var deleteCommand = scope.GetCommand<DeleteEntitiesCommand<TrainsetItemDb>>();
                var deleteResult = deleteCommand.Execute(deletingItems);

                Console.WriteLine($@"Время выполнения команды: {runningTimeWatcher.TakeRunningTime():G}");
                var itemsCountAfter = scope.GetQuery<GetEntitiesCountQuery<TrainsetItemDb>>().Execute();
                Console.WriteLine($@"Количество элементов в таблице {nameof(TrainsetItemDb)} после удаления: {itemsCountAfter}");

                var dResultSfy = JsonConvert.SerializeObject(deleteResult, Formatting.Indented);
                Console.WriteLine($@"Элемент таблицы {nameof(TrainsetItemDb)}: {Environment.NewLine}{dResultSfy}");
            }
        }

        public void GetEntitiesDirectSqlQueryTest()
        {
            var container = UnityConfig.Container;

            var scope = container.Resolve<UniverseScope<UniverseDbTestContext>>();

            using (var runningTimeWatcher = new RunningTimeWatcher())
            {
                var sqlQuery = @"
                SELECT TOP (10)[Id]
                      ,[ClassName]
                      ,[TrainsetItemValue]
                      ,[TrainsetClassId]
                      ,[SessionID]
                      ,[Created]
                FROM [UniverseDbTestDb].[dbo].[TrainsetItems]";

                var entities = scope.GetQuery<DirectSqlQuery<TrainsetItemDb>>().Execute(sqlQuery);

                Console.WriteLine($@"Время выполнения команды: {runningTimeWatcher.TakeRunningTime():G}");
                Console.WriteLine($@"Количество элементов в таблице {nameof(TrainsetItemDb)}: {entities.Items.Count}");

                var dResultSfy = JsonConvert.SerializeObject(entities, Formatting.Indented);
                Console.WriteLine($@"Элементы таблицы {nameof(TrainsetItemDb)}: {Environment.NewLine}{dResultSfy}");
            }
        }
    }
}