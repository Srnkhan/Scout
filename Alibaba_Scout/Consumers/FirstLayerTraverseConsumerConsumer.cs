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
    using Alibaba_Scout.Modals.Productions;

    public class FirstLayerTraverseConsumerConsumer :
        IConsumer<FirstLayerTraverseConsumer>
    {
        private readonly ILogger<FirstLayerTraverseConsumerConsumer> Logger;
        private readonly IConfiguration Configuration;
        private readonly IUnitOfWork UnitOfWork;
        private readonly IRepository<Production> ProductRepository;
        private IPage CurrentPage;
        public FirstLayerTraverseConsumerConsumer(ILogger<FirstLayerTraverseConsumerConsumer> logger, IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            Logger = logger;
            Configuration = configuration;
            UnitOfWork = unitOfWork;
            ProductRepository = UnitOfWork.GetRepository<Production>();
        }

        public async Task Consume(ConsumeContext<FirstLayerTraverseConsumer> context)
        {
            Logger.LogInformation($"First Layer Traverse Started At : {DateTime.UtcNow.ToString()} , " +
                $"Category Name : {context.Message.CategoryName} , " +
                $"Category Url : {context.Message.CategoryUrl}");
            CurrentPage = await CreatePageAsync(context.Message.CategoryUrl);

            var categories = await ProductRepository.GetAllAsync();
            try
            {
                int page = 1;
                var productions = new List<string>();
                while (true)
                {
                    //await Task.Delay(2000);

                    productions = await ScrollPage(CurrentPage);

                    Logger.LogInformation($"Page : {page},Productions : {JsonConvert.SerializeObject(productions)}");

                    var nextPage = await new ScoutCommand(CommandType.JsQuery,
                                                         new ScoutCommands(CurrentPage),
                                                         new JsQuery { Query = Configuration.GetSection("Queries").GetValue<string>("ProductListNextPagnationQuery"), WaitForSelector = ".site-logo" })
                                                         .ExecuteAsync();
                    var lastPaginationCss = await new ScoutCommand(CommandType.JsQuery,
                                                         new ScoutCommands(CurrentPage),
                                                         new JsQuery { Query = Configuration.GetSection("Queries").GetValue<string>("ProductListLastPagnationClassesQuery"), WaitForSelector = ".site-logo" })
                                                         .ExecuteAsync();
                    if (lastPaginationCss.ResultObject.ToString().Contains("notAllowed"))
                        break;
                    page++;
                }
                await CurrentPage.Browser.CloseAsync();
                await CreateOrUpdateAsync(context.Message.CategoryId, productions);

            }
            catch (Exception ex)
            {
                await context.RespondAsync<OperationResult>(OperationResult.ErrorResult(ex.Message));

            }
            Logger.LogInformation($"First Layer Traverse Done At : {DateTime.UtcNow.ToString()}");

            await context.RespondAsync<OperationResult>(OperationResult.SuccessResult("Successfully done"));
        }
        private async Task CreateOrUpdateAsync(Guid categoryId, IEnumerable<string> productUrlList)
        {
            var productions = await ProductRepository.GetAllAsync(product => product.CategoryId == categoryId, null, null, true, false);
            var newProductions = productUrlList.Where(product => !productions.Select(x => x.Url).Contains(product)).ToList();
            var removeProductions = productions.Where(product => !productUrlList.Contains(product.Url)).ToList();
            if (newProductions.Any())
                await ProductRepository.InsertAsync(newProductions.Select(x => new Production(x , "","",categoryId)));
            if(removeProductions.Any())
                ProductRepository.Delete(removeProductions);
            await UnitOfWork.SaveChangesAsync();
        }

        private async Task<IPage> CreatePageAsync(string url)
        {
            return await ScoutBuilderDirector
                                .NewPage
                                .Header(false)
                                .With(1920)
                                .Height(1080)
                                .Url(url)
                                .Browser("970485")
                                .PrepareAsync();
        }

        /// <summary>
        /// Scroll Page as windows size
        /// </summary>
        /// <returns></returns>
        private async Task<List<string>> ScrollPage(IPage currentPage)
        {
            int index = 0;
            var productions = new List<string>();
            while (index < 10)
            {
                await new ScoutCommand(CommandType.ScrolDown, new ScoutCommands(currentPage), new ScrollQuery { ScrollHeight = 1200 }).ExecuteAsync();
                var productionsResponse = await new ScoutCommand(CommandType.JsQuery,
                                                                       new ScoutCommands(currentPage),
                                                                       new JsQuery
                                                                       {
                                                                           Query = Configuration.GetSection("Queries").GetValue<string>("ProductListUrlQuery"),
                                                                           WaitForSelector = ".site-logo"
                                                                       })
                                                                       .ExecuteAsync();
                productions = JsonConvert.DeserializeObject<List<string>>(productionsResponse.ResultObject.ToString());
                productions = productions.Distinct().ToList();
                index++;
            }
            productions = productions.Distinct().ToList();
            return productions;
        }
    }
}