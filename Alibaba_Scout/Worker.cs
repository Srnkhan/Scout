﻿using Alibaba_Scout.Contracts;
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

namespace Alibaba_Scout
{
    public class Worker : BackgroundService
    {
        IRequestClient<ZeroLayerScoutConsumer> _client;
        private readonly ILogger<Worker> Logger;

        public Worker(IServiceProvider serviceProvider, ILogger<Worker> logger)
        {
            _client = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IRequestClient<ZeroLayerScoutConsumer>>();
            Logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                var response = await _client.GetResponse<OperationResult>(new ZeroLayerScoutConsumer { Value = "Merhaba " });
                Logger.LogInformation($"Zero Layer Result : {JsonConvert.SerializeObject(response)} Sleep At : {DateTime.UtcNow.ToString()} \n");
                await Task.Delay(60000, stoppingToken);
            }
        }
    }
}
