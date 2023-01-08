namespace Company.Consumers
{
    using MassTransit;

    public class ZeroLayerScoutConsumerConsumerDefinition :
        ConsumerDefinition<ZeroLayerScoutConsumerConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<ZeroLayerScoutConsumerConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseMessageRetry(r => r.Intervals(500, 1000));
        }
    }
}