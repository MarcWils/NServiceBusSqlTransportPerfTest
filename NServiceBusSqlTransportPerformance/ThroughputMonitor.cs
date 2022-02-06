namespace NServiceBusSqlTransportPerformance
{
    internal static class ThroughputMonitor
    {
        static int numberOfExecutions = 0;
        static DateTime startedAt;
        static object sync = new object();

        internal static void Measure()
        {
            Interlocked.Increment(ref numberOfExecutions);
            if (startedAt == default)
            {
                startedAt = DateTime.Now;
            }
            if (numberOfExecutions % 100 == 0)
            {
                lock (sync)
                {
                    var elapsed = (DateTime.Now - startedAt).TotalSeconds;
                    Console.Clear();
                    Console.WriteLine($"----------------");
                    Console.WriteLine($"Througput: {Math.Round(numberOfExecutions / elapsed, 1)}");
                    Console.WriteLine($"Seconds elapases: {elapsed}");
                    Console.WriteLine($"Number of executions: {numberOfExecutions}");
                    Console.WriteLine($"----------------");
                }
            }
        }
    }
}
