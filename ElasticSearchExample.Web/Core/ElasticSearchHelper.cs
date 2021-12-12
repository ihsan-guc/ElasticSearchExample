using Microsoft.Extensions.Configuration;
using Nest;
using System;

namespace ElasticSearchExample.Web.Core
{
    public class ElasticSearchHelper
    {
        public ElasticClient ElasticClientNode { get; set; }
        public ElasticSearchHelper()
        {
            var esNode = new Uri(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("MyField")["ElasticConnectionSettings"].ToString());

            var searchIndex = new ConnectionSettings(esNode)
                .DisablePing()
                .SniffOnStartup(false)
                .SniffOnConnectionFault(false)
                .DefaultIndex(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build()
                .GetSection("MyField")["ElasticIndex"].ToString());

            ElasticClientNode = new ElasticClient(searchIndex);
            
            var indexName = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build()
                .GetSection("MyField")["ElasticIndex"].ToString();

            CheckExistsAndCreatSearch(indexName);
        }
        public void CheckExistsAndCreatSearch(string indexName)
        {
            
            if (!ElasticClientNode.Indices.Exists(indexName).Exists)
            {
                var newIndexName = indexName + System.DateTime.Now.Ticks;

                var indexSettings = new IndexSettings();
                indexSettings.NumberOfReplicas = 1;
                indexSettings.NumberOfShards = 3;

                var response = ElasticClientNode.Indices.Create(newIndexName, index =>
                   index.Map<ElasticSearchViewModel>(m => m.AutoMap()
                          )
                  .InitializeUsing(new IndexState() { Settings = indexSettings })
                  .Aliases(a => a.Alias(indexName)));

            }
        }
    }
    public class ElasticSearchViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
    }
}
