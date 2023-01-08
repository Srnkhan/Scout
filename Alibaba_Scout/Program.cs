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

                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.Host("localhost", "/", h => {
                                h.Username("guest");
                                h.Password("guest");
                            });

                            cfg.ConfigureEndpoints(context);
                        });
                        x.AddRequestClient<ZeroLayerScoutConsumer>();
                    });
                    services.AddHostedService<Worker>();

                    services
                       .AddDbContext<AlibabaScoutDbContext>(opt =>
                       opt.UseSqlServer("Server=localhost,1433;Database=Scout;User Id=sa;Password=Password123;TrustServerCertificate=True"))
                       //.AddDbContext<BloggingContext>(opt => opt.UseInMemoryDatabase("UnitOfWork"))
                       .AddUnitOfWork<AlibabaScoutDbContext>()
                       .AddCustomRepository<Category, CategoryRepository>();
                });
    }
}
