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
using Universe.CQRS.Dal.Commands;
using Universe.CQRS.Dal.Queries;
using Universe.CQRS.Extensions;
using Universe.CQRS.Infrastructure;
using Universe.Diagnostic;
using Universe.Framework.ConsoleApp.Tests.CQRS.Base;
using Universe.Framework.ConsoleApp.Tests.Infrastructure;
using Universe.Framework.DataAccess.Test;
using Universe.Framework.DataAccess.Test.Models.Traiset;

namespace Universe.Framework.ConsoleApp.Tests.CQRS
{
    /// <summary>
    ///     Тест массовой загрузки данных в БД
    /// <author>Alex Envision</author>
    /// </summary>
    public class BatchProcessesTests : BaseCommandQueryTests
    {
        public void Run()
        {
            var container = UnityConfig.Container;

            var settings = new AppTestSettings();
            var scope = new UniverseScope<UniverseDbTestContext>(settings, container);

            var sessionId = Guid.NewGuid();
            TrainsetDb trainSetDb = GenerateDataDb();
            MarkEntitiesTemporaryIds(sessionId, trainSetDb);

            using (var runningTimeWatcher = new RunningTimeWatcher())
            using (var spdWatcher = new SpeedProcessingDataWatcher(runningTimeWatcher))
            {
                Console.WriteLine(@"Запущена вставка данных в БД...");
                BatchCommand(scope, trainSetDb, sessionId);
                Console.WriteLine($@"Выполнена команда массовой вставки. Время выполнения: {runningTimeWatcher.TakeRunningTime():G}");

                var speedResult = spdWatcher.EstimateTotalSpeed(trainSetDb);
                Console.WriteLine($@"Объём отправленных данных: {speedResult.DataSizeInMb} Мб.");
                Console.WriteLine($@"Скорость массовой вставки: {speedResult.Speed} Мб/сек.");
            }
        }

        private void BatchCommand(UniverseScope<UniverseDbTestContext> scope, TrainsetDb trainSetDb, Guid sessionId)
        {
            // Batch-команда на создание, либо обновление
            scope.GetCommand<AddEntitiesBatchCommand<TrainsetDb>>()
                .Execute(x => x.Id, trainSetDb)
                .BatchProcess(
                    keySelector: x => x.Id,
                    parentEntitiesAfterUpdate: scope
                        .GetQuery<SelectEntitiesQuery<TrainsetDb, ExchangeBatchEntity>>().Execute(
                            EntityReqHelper.GetEqReq(
                                MetaInfoHelper.FieldMap(
                                    MetaInfoHelper.MapRule<ExchangeBatchEntity>(nameof(ExchangeBatchEntity.Id),
                                        x => x.Id),
                                    MetaInfoHelper.MapRule<ExchangeBatchEntity>(
                                        nameof(ExchangeBatchEntity.SessionId), x => x.SessionId)
                                ),
                                eqfieldName: nameof(ExchangeBatchEntity.SessionId),
                                eqvalue: sessionId.ToString(),
                                allItemsAsOnePage: true
                            ),
                            projection: s => new ExchangeBatchEntity
                            {
                                Id = s.Id,
                                SessionId = s.SessionID
                            }).Items,
                    entitiesDict: trainSetDb.TrainsetClasses
                        .GroupBy(g => g.Trainset)
                        .ToDictionary(g => new ExchangeBatchEntity { Id = g.Key.Id, SessionId = g.Key.SessionID },
                            x => x.ToList()),
                    parentKeySetterFunc: (items, parentItem) =>
                    {
                        foreach (var item in items)
                        {
                            item.Trainset.Id = parentItem.Id;
                            item.TrainsetId = parentItem.Id;
                        }

                        return items;
                    })
                .BatchProcess(
                    keySelector: x => x.Id,
                    parentEntitiesAfterUpdate: scope
                        .GetQuery<SelectEntitiesQuery<TrainsetClassDb, ExchangeBatchEntity>>().Execute(
                            EntityReqHelper.GetEqReq(
                                MetaInfoHelper.FieldMap(
                                    MetaInfoHelper.MapRule<ExchangeBatchEntity>(nameof(ExchangeBatchEntity.Id),
                                        x => x.Id),
                                    MetaInfoHelper.MapRule<ExchangeBatchEntity>(
                                        nameof(ExchangeBatchEntity.SessionId), x => x.SessionId)
                                ),
                                eqfieldName: nameof(ExchangeBatchEntity.SessionId),
                                eqvalue: sessionId.ToString(),
                                allItemsAsOnePage: true
                            ),
                            projection: s => new ExchangeBatchEntity
                            {
                                Id = s.Id,
                                SessionId = s.SessionID
                            }).Items,
                    entitiesDict: trainSetDb.TrainsetClasses.SelectMany(x => x.TrainsetsItems)
                        .GroupBy(g => g.TrainsetClass)
                        .ToDictionary(g => new ExchangeBatchEntity { Id = g.Key.Id, SessionId = g.Key.SessionID },
                            x => x.ToList()),
                    parentKeySetterFunc: (items, parentItem) =>
                    {
                        foreach (var item in items)
                        {
                            item.TrainsetClass.Id = parentItem.Id;
                            item.TrainsetClassId = parentItem.Id;
                        }

                        return items;
                    });
        }

        private static void MarkEntitiesTemporaryIds(Guid sessionId, TrainsetDb trainSetDb)
        {
            var trainsetClassIndex = 1;
            var trainsetItemIndex = 1;

            var trainsetTemp = new TrainsetDb { Id = 1, SessionID = sessionId };
            foreach (var trainsetClassDb in trainSetDb.TrainsetClasses)
            {
                var trainsetClassTemp = new TrainsetClassDb { Id = trainsetClassIndex, SessionID = sessionId };
                foreach (var trainsetItemDb in trainsetClassDb.TrainsetsItems)
                {
                    trainsetItemDb.TrainsetClass = trainsetClassTemp;
                    trainsetItemDb.SessionID = sessionId;

                    trainsetItemIndex++;
                }

                trainsetClassDb.Trainset = trainsetTemp;
                trainsetClassDb.SessionID = sessionId;
                trainsetClassIndex++;
            }

            trainSetDb.SessionID = sessionId;
        }

        /// <summary>
        /// Генерация данных в количестве 15КК записей
        /// </summary>
        /// <returns></returns>
        public TrainsetDb GenerateDataDb()
        {
            var classesAmount = 1000;
            var classesItemsAmount = 15000;

            var classes = new List<TrainsetClassDb>();
            for (int i = 0; i < classesAmount; i++)
            {
                var tClass = new TrainsetClassDb
                {
                    ClassName = $"TrainSetClass{i + 1}",
                    Created = DateTimeOffset.Now
                };
                var items = new List<TrainsetItemDb>();
                for (int j = 0; j < classesItemsAmount; j++)
                {
                    var item = new TrainsetItemDb
                    {
                        ClassName = tClass.ClassName,
                        Created = DateTimeOffset.Now,
                        TrainsetItemValue = $"TrainSetValue{j + 1}"
                    };

                    items.Add(item);
                }

                tClass.TrainsetsItems = items;
                classes.Add(tClass);
            }

            var trainsetDb = new TrainsetDb
            {
                Created = DateTimeOffset.Now,
                Name = "Trainset002",
                TrainsetClasses = classes
            };

            return trainsetDb;
        }
    }
}
