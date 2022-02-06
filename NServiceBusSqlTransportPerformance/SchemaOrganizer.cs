using Microsoft.Data.SqlClient;

namespace NServiceBusSqlTransportPerformance
{
    internal class SchemaOrganizer
    {
        private readonly string connectionString;

        public SchemaOrganizer(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task CleanUp()
        {
            using (var sqlConn = new SqlConnection(connectionString))
            {
                await sqlConn.OpenAsync();
                var dropCmd = sqlConn.CreateCommand();
                dropCmd.CommandText = @"DECLARE @cmd varchar(4000)
DECLARE cmds CURSOR FOR
SELECT 'drop table [' + Table_Name + ']'
FROM INFORMATION_SCHEMA.TABLES
WHERE Table_Name LIKE 'PerfTestQueue%'

OPEN cmds
WHILE 1 = 1
BEGIN
    FETCH cmds INTO @cmd
    IF @@fetch_status != 0 BREAK
    EXEC(@cmd)
END
CLOSE cmds;
DEALLOCATE cmds";
                await dropCmd.ExecuteNonQueryAsync();
                await sqlConn.CloseAsync();
            }
        }
    }
}
