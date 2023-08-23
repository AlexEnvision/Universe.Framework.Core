using System;
using Universe.REST.Adapter;
using Universe.REST.Models;

namespace Universe.Framework.ConsoleApp.Tests.REST
{
    public class DuckDuckGoTest
    {
        public string SearchSystemName => "http://api.duckduckgo.com/";  //?q=x&format=json

        public void Run()
        {
            string request = "GPU";

            var adapter = new DataSourceAdapter(SearchSystemName);
            var response = adapter.CreateGetRequest(SearchSystemName, Argument.Create("q", request),
                Argument.Create("format", "json"));

            Console.ReadLine();
        }
    }
}