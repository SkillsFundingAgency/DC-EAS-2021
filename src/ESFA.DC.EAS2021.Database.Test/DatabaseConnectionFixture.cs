using System.Configuration;
using ESFA.DC.DatabaseTesting;
using ESFA.DC.DatabaseTesting.Interface;


namespace ESFA.DC.EAS2021.Database.Test
{
    public sealed class DatabaseConnectionFixture
    {
        public DatabaseConnectionFixture()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Database"].ConnectionString;
            IDbConnectorConfiguration dbConnectorConfiguration = new DbConnectorConfiguration(connectionString);
            IDbConnector dbConnector = new DbConnector(dbConnectorConfiguration);
            SchemaTests = new SchemaTests(dbConnector);
        }

        public SchemaTests SchemaTests { get; }
    }
}
