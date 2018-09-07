using System.Collections.Generic;
using ESFA.DC.DatabaseTesting.Model;
using ESFA.DC.EAS1819.Database.Test;
using Xunit;

namespace ESFA.DC.ILR1819.DataStore.Database.Test
{
    public sealed class SchemaDboTests : IClassFixture<DatabaseConnectionFixture>
    {
        private readonly DatabaseConnectionFixture _fixture;

        public SchemaDboTests(DatabaseConnectionFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void CheckColumnsEASSubmission()
        {
            List<ExpectedColumn> expectedColumns = new List<ExpectedColumn>
            {
                new ExpectedColumn("Submission_Id","uniqueidentifier",false,1),
                ExpectedColumn.CreateNvarChar("UKPRN", 2, false),
                ExpectedColumn.CreateInt("CollectionPeriod", 3, false),
                ExpectedColumn.CreateNvarChar("ProviderName", 4, false),
                ExpectedColumn.CreateDateTime("UpdatedOn", 5, false),
                ExpectedColumn.CreateBit("DeclarationChecked", 6, false),
                ExpectedColumn.CreateBit("NilReturn", 7, false),
                ExpectedColumn.CreateNvarChar("UpdatedBy", 8, true),

            };
            _fixture.SchemaTests.AssertTableColumnsExist("dbo", "EAS_Submission", expectedColumns, true);
        }

        [Fact]
        public void CheckColumnsEASSubmissionValues()
        {
            List<ExpectedColumn> expectedColumns = new List<ExpectedColumn>
            {
                new ExpectedColumn("Submission_Id","uniqueidentifier",false,1),
                ExpectedColumn.CreateInt("CollectionPeriod", 2, false),
                ExpectedColumn.CreateInt("Payment_Id", 3, false),
                ExpectedColumn.CreateDecimal("PaymentValue", 4, false,10,2)
            };
            _fixture.SchemaTests.AssertTableColumnsExist("dbo", "EAS_Submission_Values", expectedColumns, true);
        }

        [Fact]
        public void CheckColumnsPayment_Types()
        {
           List<ExpectedColumn> expectedColumns = new List<ExpectedColumn>
            {
                ExpectedColumn.CreateInt("Payment_Id", 1, false),
                ExpectedColumn.CreateNvarChar("PaymentName", 2, false),
            };
            _fixture.SchemaTests.AssertTableColumnsExist("dbo", "Payment_Types", expectedColumns, true);
        }


        [Fact]
        public void CheckColumnsVersionInfo()
        {
            List<ExpectedColumn> expectedColumns = new List<ExpectedColumn>
            {
                ExpectedColumn.CreateInt("Version", 1, false),
                ExpectedColumn.CreateDate("Date", 2, false),
            };
            _fixture.SchemaTests.AssertTableColumnsExist("dbo", "VersionInfo", expectedColumns, true);
        }

    }
}
