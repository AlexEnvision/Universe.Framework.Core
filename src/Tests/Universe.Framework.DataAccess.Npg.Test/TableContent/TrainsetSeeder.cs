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
using System.Data.Entity;
using System.Linq;
using Universe.DataAccess.TableContent.Framework;
using Universe.Framework.DataAccess.Npg.Test.Models.Traiset;

namespace Universe.Framework.DataAccess.Npg.Test.TableContent
{
    public class TrainsetSeeder : EntitySeeder<TrainsetDb>
    {
        protected override void Seed(DbSet<TrainsetDb> set)
        {
            var sessionId = Guid.NewGuid();
            var creationDate = DateTimeOffset.Now;
            var list = new List<TrainsetDb> {
                new TrainsetDb {
                    Name = "Trainset001",
                    SessionID = sessionId,
                    Created = creationDate
                },
                new TrainsetDb {
                    Name = "Trainset002",
                    SessionID = sessionId,
                    Created = creationDate
                },
                new TrainsetDb {
                    Name = "Trainset003",
                    SessionID = sessionId,
                    Created = creationDate
                },
                new TrainsetDb {
                    Name = "Trainset004",
                    SessionID = sessionId,
                    Created = creationDate
                },
                new TrainsetDb {
                    Name = "Trainset005",
                    SessionID = sessionId,
                    Created = creationDate
                },
                new TrainsetDb {
                    Name = "Trainset006",
                    SessionID = sessionId,
                    Created = creationDate
                },
                new TrainsetDb {
                    Name = "Trainset007",
                    SessionID = sessionId,
                    Created = creationDate
                }
            };

            foreach (var item in list)
            {
                var dbItem = set.FirstOrDefault(_ => _.Name == item.Name);

                if (dbItem == null)
                {
                    set.Add(item);
                }
                else
                {
                    dbItem.Name = item.Name;
                }
            }
        }
    }
}