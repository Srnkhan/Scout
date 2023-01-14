using Alibaba_Scout.Contracts;
using Company.Consumers;
using Contracts;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Scout.Core.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Alibaba_Scout.Workers
{
    public class ZeroLayerWorker : BackgroundService
    {
        IRequestClient<ZeroLayerScoutConsumer> _client;
        private readonly ILogger<ZeroLayerWorker> Logger;

        public ZeroLayerWorker(IServiceProvider serviceProvider, ILogger<ZeroLayerWorker> logger)
        {
            _client = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IRequestClient<ZeroLayerScoutConsumer>>();
            Logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                var timeout = TimeSpan.FromSeconds(30);
                using var source = new CancellationTokenSource(timeout);

                var response = await _client.GetResponse<OperationResult>(new ZeroLayerScoutConsumer { Value = "Merhaba " }, source.Token);
                Logger.LogInformation($"Zero Layer Result : {JsonConvert.SerializeObject(response)}");
                Logger.LogInformation($" Sleep 5 Minutes At : {DateTime.UtcNow.ToString()}");
                source.Dispose();

                await Task.Delay(60000 * 5, stoppingToken); //SLEEP 5 MINUTES
            }
        }
    }
}
