namespace Company.Consumers
{
    using Alibaba_Scout.Contracts;
    using MassTransit;

    public class FirstLayerTraverseConsumerConsumerDefinition :
        ConsumerDefinition<FirstLayerTraverseConsumerConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, 
            IConsumerConfigurator<FirstLayerTraverseConsumerConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseMessageRetry(r => r.Intervals(500, 1000));
        }
    }
}