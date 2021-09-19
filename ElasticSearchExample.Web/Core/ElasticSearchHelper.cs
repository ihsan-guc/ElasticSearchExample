using Nest;
using System;

namespace ElasticSearchExample.Web.Core
{
    public static class ElasticSearchHelper
    {
        public static ConnectionSettings ConnectionSettings { get; set; }
        public static void CreateNewIndex()
        {
            var settings = new ConnectionSettings().DefaultIndex("defaultindex");
            var node = new Uri("http://localhost:9200/");
            var client = new ElasticClient(settings);
            
            //var node = new Uri("http://localhost:9200/");
            //var settings = new ConnectionSettings().DefaultMappingFor<ElasticSearchViewModel>(m => m.IndexName("personName_history"));
            //var client = new ElasticClient(settings);
        }
        public static ElasticClient ElasticClientNode()
        {
            var node = new Uri("http://localhost:9200/");
            //var settings = new ConnectionSettings().DefaultMappingFor<ElasticSearchViewModel>(m => m.IndexName("personName_history"));
            var settings = new ConnectionSettings().DefaultIndex("defaultindex");
            var client = new ElasticClient(settings);
            return client;
        }
    }
    public class ElasticSearchViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
    }
}
