using NServiceBus;
using NServiceBusSqlTransportPerformance.Commands;

namespace NServiceBusSqlTransportPerformance.Handlers
{
    internal class TestCommandHandler : IHandleMessages<TestCommand>
    {
        static int NumberOfExecutions = 0;

        public async Task Handle(TestCommand message, IMessageHandlerContext context)
        {
            ThroughputMonitor.Measure();
            await context.SendLocal(new TestCommand());
        }
    }
}
