using Alibaba_Scout.Contracts;
using Contracts;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Alibaba_Scout.Workers
{
    internal class FirstLayerWorker : BackgroundService
    {
        IRequestClient<FirstLayerConsumer> _client;
        private readonly ILogger<ZeroLayerWorker> Logger;

        public FirstLayerWorker(IServiceProvider serviceProvider, ILogger<ZeroLayerWorker> logger)
        {
            _client = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IRequestClient<FirstLayerConsumer>>();
            Logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {

                    var response = await _client.GetResponse<OperationResult>(new FirstLayerConsumer { Value = "Merhaba " }, default, RequestTimeout.After(h: 3));
                    Logger.LogInformation($"First Layer Result : {JsonConvert.SerializeObject(response)}");
                    Logger.LogInformation($"First Layer Sleep 5 Minutes At : {DateTime.UtcNow.ToString()}");

                    await Task.Delay(60000 * 5, stoppingToken); //SLEEP 5 MINUTES
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }
    }
}
