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
                await CreatePage();

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
                int index = 0;
                var categories = urlList.Select(url =>
                {
                    string name = nameList[index++];
                    return new Category(name , url);
                });
                await CategoryRepository.InsertAsync(categories);
                await UnitOfWork.SaveChangesAsync();
                await CurrentPage.Browser.CloseAsync();
                await context.RespondAsync<OperationResult>(OperationResult.SuccessResult("Successfully done"));
            }
            catch (System.Exception ex)
            {
                await context.RespondAsync<OperationResult>(OperationResult.SuccessResult($"Error {ex.ToString()}"));
            }

        }
        private async Task CreatePage()
        {
            this.CurrentPage = await ScoutBuilderDirector
                                .NewPage
                                .Header(false)
                                .With(1920)
                                .Height(1080)
                                .Url("https://www.aliexpress.us/")
                                .Browser("970485")
                                .PrepareAsync();
        }

    }
}