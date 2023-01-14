using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Contracts;
using System.Reflection.Metadata;
using Alibaba_Scout.Modals;
using Microsoft.EntityFrameworkCore;
using Arch.EntityFrameworkCore.UnitOfWork;
using Alibaba_Scout.Modals.Categories;
using Alibaba_Scout.Contracts;
using Company.Consumers;
using Alibaba_Scout.Workers;
using Alibaba_Scout.Modals.Productions;

namespace Alibaba_Scout
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                await CreateHostBuilder(args).Build().RunAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMassTransit(x =>
                    {
                        x.SetKebabCaseEndpointNameFormatter();

                        var entryAssembly = Assembly.GetEntryAssembly();

                        x.AddConsumers(entryAssembly);
                        x.AddSagaStateMachines(entryAssembly);
                        x.AddSagas(entryAssembly);
                        x.AddActivities(entryAssembly);
                        x.AddConsumer<ZeroLayerScoutConsumerConsumer>(typeof(ZeroLayerScoutConsumerConsumerDefinition));
                        x.AddConsumer<FirstLayerConsumerConsumer>(typeof(FirstLayerConsumerConsumerDefinition));
                        x.AddConsumer<FirstLayerTraverseConsumerConsumer>(typeof(FirstLayerTraverseConsumerConsumerDefinition));

                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.Host("208.64.33.68", "/", h => {
                                h.Username("guest");
                                h.Password("guest");
                            });

                            cfg.ConfigureEndpoints(context);
                        });
                        x.AddRequestClient<OperationResult>();
                    });
                    //services.AddHostedService<ZeroLayerWorker>();
                    services.AddHostedService<FirstLayerWorker>();

                    services
                       .AddDbContext<AlibabaScoutDbContext>(opt =>
                       opt.UseSqlServer("Data Source=208.64.33.68, 52359;Database=Scout;User Id=srnk;Password=Mrkb6895!;TrustServerCertificate=True"))
                       //.AddDbContext<BloggingContext>(opt => opt.UseInMemoryDatabase("UnitOfWork"))
                       .AddUnitOfWork<AlibabaScoutDbContext>()
                       .AddCustomRepository<Category, CategoryRepository>()
                       .AddCustomRepository<Production, ProductionRepository>();
                });
    }
}
