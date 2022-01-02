using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using ElasticSearchExample.Web.Core;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BenchmarkConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var result = BenchmarkRunner.Run<BenchMarkDemo>();
            Console.WriteLine("Bencmark Testting.!");
        }
    }
    public class BenchMarkDemo
    {
        [Benchmark]
        public List<ElasticSearchViewModel> ElasticSearchFuzzinessOneRatio()
        {
            var value = "Eura";
            var elasticNode = new ElasticSearchHelper();
            var client = elasticNode.ElasticClientNode;
            var dataList = client.Search<ElasticSearchViewModel>(s =>
            s.Query(q => q.Bool(b => b.Should(sh => sh.Fuzzy(f => f.Field(fi => fi.FullName).Fuzziness(Fuzziness.EditDistance(1)).Boost(2)
                   .Value(value))
            , m => m.Match(mq => mq.Field(f => f.FullName).Query(value).Operator(Operator.And).Fuzziness(Fuzziness.EditDistance(1))))
            )).Size(10));
            return dataList.Documents.ToList();
        }
        [Benchmark]
        public List<ElasticSearchViewModel> ElasticSearchFuzzinessTwoRatio()
        {
            var value = "Eura";
            var elasticNode = new ElasticSearchHelper();
            var client = elasticNode.ElasticClientNode;
            var dataList = client.Search<ElasticSearchViewModel>(s =>
            s.Query(q => q.Bool(b => b.Should(sh => sh.Fuzzy(f => f.Field(fi => fi.FullName).Fuzziness(Fuzziness.EditDistance(2)).Boost(2)
                   .Value(value))
            , m => m.Match(mq => mq.Field(f => f.FullName).Query(value).Operator(Operator.And).Fuzziness(Fuzziness.EditDistance(2))))
            )).Size(10));
            return dataList.Documents.ToList();
        }
    }
}
