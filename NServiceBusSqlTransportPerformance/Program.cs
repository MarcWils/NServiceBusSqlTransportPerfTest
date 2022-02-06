using NServiceBus;
using NServiceBusSqlTransportPerformance.Commands;

namespace NServiceBusSqlTransportPerformance
{

    class Program
    {
        const string connectionString = "Server=tcp:***.database.windows.net,1433;Initial Catalog=***;Persist Security Info=False;User ID=marcw;Password=***!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        const int numberOfEndpoints = 15;

        public static async Task Main()
        {
            Console.WriteLine("Started");

            var endpointInstances = new List<IEndpointInstance>();
            for (int i = 0; i < numberOfEndpoints; i++)
            {
                var endpointConfiguration = new EndpointConfiguration($"PerfTestQueue{i}");
                var sqlTranport = endpointConfiguration.UseTransport<SqlServerTransport>()
                    .ConnectionString(connectionString);
                sqlTranport.Transactions(TransportTransactionMode.TransactionScope);
                sqlTranport.NativeDelayedDelivery();
                endpointConfiguration.EnableInstallers();
                endpointConfiguration.Conventions().DefiningCommandsAs(t => t.Name.EndsWith("Command"));
                var endpointInstance = await Endpoint.Start(endpointConfiguration);
                endpointInstances.Add(endpointInstance);
            }

            Console.WriteLine("Endpoints started, press <Enter> to stop");
            foreach (var endpointInstance in endpointInstances)
            {
                for (int j = 0; j < 100; j++)
                {
                    await endpointInstance.SendLocal(new TestCommand());
                }
            }
            Console.ReadLine();
            endpointInstances.ForEach(async endpointInstance => await endpointInstance.Stop());
            Console.WriteLine("Endpoints stopped");
            Console.WriteLine("Cleaning up queues");
            var schemaOrganizer = new SchemaOrganizer(connectionString);
            await schemaOrganizer.CleanUp();
            Console.WriteLine("All done");
        }
    }
}