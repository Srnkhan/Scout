namespace Company.Consumers
{
    using System.Threading.Tasks;
    using MassTransit;
    using Contracts;
    using Microsoft.Extensions.Logging;
    using Arch.EntityFrameworkCore.UnitOfWork;
    using Alibaba_Scout.Modals;
    using Microsoft.Extensions.Configuration;
    using Scout.Core.Commands;
    using Scout.Queries;
    using Scout.Domain;
    using Scout.Core.Builders;
    using PuppeteerSharp;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Reflection.Metadata;
    using Alibaba_Scout.Modals.Categories;
    using System.Linq;
    using Alibaba_Scout.Contracts;
    using System;

    public class ZeroLayerScoutConsumerConsumer :
        IConsumer<ZeroLayerScoutConsumer>
    {
        private readonly ILogger<ZeroLayerScoutConsumerConsumer> Logger;
        private readonly IConfiguration Configuration;
        private readonly IUnitOfWork UnitOfWork;
        private readonly IRepository<Category> CategoryRepository;
        private IPage CurrentPage;
        

        public ZeroLayerScoutConsumerConsumer(ILogger<ZeroLayerScoutConsumerConsumer> logger,
            IUnitOfWork unitOfWork, 
            IConfiguration configuration)
        {
            Logger = logger;
            UnitOfWork = unitOfWork;
            Configuration = configuration;
            CategoryRepository = UnitOfWork.GetRepository<Category>();
        }
        public async Task Consume(ConsumeContext<ZeroLayerScoutConsumer> context)
        {
            try
            {
                Logger.LogInformation($"Zero Layer Started At : {DateTime.UtcNow.ToString()}");
                await CreatePageAsync();

                var urlListResponse = await new ScoutCommand(CommandType.JsQuery,
                                                        new ScoutCommands(CurrentPage),
                                                        new JsQuery { Query = Configuration.GetSection("Queries").GetValue<string>("CategoryListUrlQuery") })
                                                        .ExecuteAsync();
                var nameListResponse = await new ScoutCommand(CommandType.JsQuery,
                                                        new ScoutCommands(CurrentPage),
                                                        new JsQuery { Query = Configuration.GetSection("Queries").GetValue<string>("CategoryListTextQuery") })
                                                        .ExecuteAsync();
                var urlList = JsonConvert.DeserializeObject<List<string>>(urlListResponse.ResultObject.ToString());
                var nameList = JsonConvert.DeserializeObject<List<string>>(nameListResponse.ResultObject.ToString());
                await CurrentPage.Browser.CloseAsync();

                int index = 0;
                var categories = urlList.Select(url =>
                {
                    string name = nameList[index].ToString();
                    ++index;
                    return new Category(name , url);
                }).ToList();
                index = 0;
                await CreateOrUpdateAsync(categories);

                Logger.LogInformation($"Zero Layer Done At : {DateTime.UtcNow.ToString()}");
                await context.RespondAsync<OperationResult>(OperationResult.SuccessResult("Successfully done"));
            }
            catch (System.Exception ex)
            {
                await context.RespondAsync<OperationResult>(OperationResult.SuccessResult($"Error {ex.ToString()}"));
            }

        }
        private async Task CreatePageAsync()
        {
            this.CurrentPage = await ScoutBuilderDirector
                                .NewPage
                                .Header(true)
                                .With(1920)
                                .Height(1080)
                                .Url("https://www.aliexpress.us/")
                                .Browser("970485")
                                .PrepareAsync();
        }
        private async Task CreateOrUpdateAsync(IEnumerable<Category> categories )
        {

            var allCategories = (await CategoryRepository.GetAllAsync()).ToList();
            var newCategories = categories.Where(category => !allCategories.Select(x => x.Name).Contains(category.Name)).ToList();
            var oldCategories = allCategories.Where(category => !categories.Select(x => x.Name).Contains(category.Name)).ToList();
            Logger.LogInformation($"Zero Layer At : {DateTime.UtcNow.ToString()} New Categories : {JsonConvert.SerializeObject(newCategories)}");
            Logger.LogInformation($"Zero Layer At : {DateTime.UtcNow.ToString()} Old Categories : {JsonConvert.SerializeObject(oldCategories)}");

            if (newCategories.Any())            
                await CategoryRepository.InsertAsync(newCategories); 
            
            if (oldCategories.Any())
            {
                foreach(var category in oldCategories)
                {
                    CategoryRepository.Delete(category);
                }
            }

            if (newCategories.Any() || oldCategories.Any())
                await UnitOfWork.SaveChangesAsync();


        }
    }
}