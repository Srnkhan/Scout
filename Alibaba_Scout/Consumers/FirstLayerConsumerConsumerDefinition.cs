namespace Company.Consumers
{
    using MassTransit;

    public class FirstLayerConsumerConsumerDefinition :
        ConsumerDefinition<FirstLayerConsumerConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<FirstLayerConsumerConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseMessageRetry(r => r.Intervals(500, 1000));
        }
    }
}