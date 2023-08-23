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

using System.Data.Entity.Migrations;

namespace Universe.Framework.DataAccess.Test.Migrations
{
    public partial class IntialDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TrainsetClasses",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ClassName = c.String(),
                        TrainsetId = c.Long(),
                        SessionID = c.Guid(nullable: false),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Trainsets", t => t.TrainsetId)
                .Index(t => t.TrainsetId);
            
            CreateTable(
                "dbo.Trainsets",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        SessionID = c.Guid(nullable: false),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TrainsetItems",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ClassName = c.String(),
                        TrainsetItemValue = c.String(),
                        TrainsetClassId = c.Long(),
                        SessionID = c.Guid(nullable: false),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TrainsetClasses", t => t.TrainsetClassId)
                .Index(t => t.TrainsetClassId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TrainsetItems", "TrainsetClassId", "dbo.TrainsetClasses");
            DropForeignKey("dbo.TrainsetClasses", "TrainsetId", "dbo.Trainsets");
            DropIndex("dbo.TrainsetItems", new[] { "TrainsetClassId" });
            DropIndex("dbo.TrainsetClasses", new[] { "TrainsetId" });
            DropTable("dbo.TrainsetItems");
            DropTable("dbo.Trainsets");
            DropTable("dbo.TrainsetClasses");
        }
    }
}
