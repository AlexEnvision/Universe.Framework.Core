using System;
using System.Collections.Generic;
using System.Text;
using Universe.CQRS.Infrastructure.Json;
using Universe.CQRS.Models.Base;

namespace Universe.Framework.ConsoleApp.Tests.IO
{
    public class UniverseModelDeserializerTest
    {
        public void Run()
        {
            var example = 
            @"{
                ""ids"": ""20132; 20134"",
                ""type"": ""Req""
            }";

            var model = new UniverseModelDeserializer().Deserialize<Req>(example);

            var ids = model.Ids;
        }

        public class Req : EntityDto
        {
            public string Ids { get; set; }
        }
    }
}
