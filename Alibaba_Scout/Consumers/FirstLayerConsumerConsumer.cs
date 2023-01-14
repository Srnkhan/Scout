namespace Company.Consumers
{
    using System.Threading.Tasks;
    using MassTransit;
    using Contracts;
    using Alibaba_Scout.Contracts;
    using Arch.EntityFrameworkCore.UnitOfWork;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Alibaba_Scout.Modals.Categories;
    using System;
    using Scout.Core.Builders;
    using PuppeteerSharp;
    using Scout.Core.Commands;
    using Scout.Queries;
    using Scout.Domain;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using MassTransit.Util;
    using System.Linq;

    public class FirstLayerConsumerConsumer :
        IConsumer<FirstLayerConsumer>
    {
        private readonly ILogger<ZeroLayerScoutConsumerConsumer> Logger;
        private readonly IConfiguration Configuration;
        private readonly IUnitOfWork UnitOfWork;
        private readonly IRepository<Category> CategoryRepository;
        private readonly IRequestClient<FirstLayerTraverseConsumer> _client;

        public FirstLayerConsumerConsumer(ILogger<ZeroLayerScoutConsumerConsumer> logger, IConfiguration configuration, IUnitOfWork unitOfWork, IRequestClient<FirstLayerTraverseConsumer> client)
        {
            Logger = logger;
            Configuration = configuration;
            UnitOfWork = unitOfWork;
            CategoryRepository = UnitOfWork.GetRepository<Category>();
            _client = client;
        }

        public async Task Consume(ConsumeContext<FirstLayerConsumer> context)
        {
            Logger.LogInformation($"First Layer Started At : {DateTime.UtcNow.ToString()}");
            var categories = await CategoryRepository.GetAllAsync();
            foreach (var category in categories)
            {
                try
                {
                    var response = await _client.GetResponse<OperationResult>(new FirstLayerTraverseConsumer {CategoryId = category.Id, CategoryUrl = category.Url,CategoryName = category.Name }, 
                        default, 
                        RequestTimeout.After(m: 10));
                }
                catch (Exception ex)
                {
                    await context.RespondAsync<OperationResult>(OperationResult.SuccessResult("Successfully done"));

                }

            }
            Logger.LogInformation($"First Layer Done At : {DateTime.UtcNow.ToString()}");

            await context.RespondAsync<OperationResult>(OperationResult.SuccessResult("Successfully done"));
        }
    }
}