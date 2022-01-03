using ElasticSearchExample.Data.DAL.Repository.Core;
using ElasticSearchExample.Web.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nest;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ElasticSearchExample.Web.Helper
{
    public class ElasticSearchCreateIndexService : IHostedService, IDisposable
    {
        private int executionCount = 0;
        public IServiceProvider Services { get; }
        private Timer _timer = null!;
        public ElasticSearchCreateIndexService(IServiceProvider services)
        {
            Services = services;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(5));

            return Task.CompletedTask;
        }
        private void DoWork(object? state)
        {
            var count = Interlocked.Increment(ref executionCount);

            using (var scope = Services.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var personList = unitOfWork.PersonRepository.GetAll().Where(p => p.IsElastic == false).ToList().Select(p => new ElasticSearchViewModel()
                {
                    Id = p.Id,
                    FullName = p.FirstName + " " + p.LastName
                });
                if (personList.Count() > 0)
                {
                    var elasticNode = new ElasticSearchHelper();
                    var client = elasticNode.ElasticClientNode;
                    var response = client.IndexManyAsync(personList.Take(2000));

                    var personAll = unitOfWork.PersonRepository.GetAll();
                    foreach (var personItem in personAll)
                    {
                        personItem.IsElastic = true;
                    }
                    unitOfWork.Commit();
                }
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
