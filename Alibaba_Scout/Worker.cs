using Alibaba_Scout.Contracts;
using Company.Consumers;
using Contracts;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scout.Core.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Alibaba_Scout
{
    public class Worker : BackgroundService
    {
        IRequestClient<ZeroLayerScoutConsumer> _client;

        public Worker(IServiceProvider serviceProvider)
        {
            _client = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IRequestClient<ZeroLayerScoutConsumer>>();
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                var response = await _client.GetResponse<OperationResult>(new ZeroLayerScoutConsumer { Value = "Merhaba " });

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
