using System.Collections.Generic;
using ESFA.DC.DatabaseTesting.Model;
using Xunit;

namespace ESFA.DC.EAS1819.Database.Test
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
                ExpectedColumn.CreateUniqueIdentifier("Submission_Id",1,false),
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
                ExpectedColumn.CreateUniqueIdentifier("Submission_Id",1,false),
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
                ExpectedColumn.CreateBit("FM36", 3, false),
                ExpectedColumn.CreateNvarChar("PaymentTypeDescription", 4, true),
                ExpectedColumn.CreateInt("FundingLineId", 5, true),
                ExpectedColumn.CreateInt("AdjustmentTypeId", 6, true),
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

        [Fact]
        public void CheckColumnsSourceFile()
        {
            List<ExpectedColumn> expectedColumns = new List<ExpectedColumn>
                {
                    ExpectedColumn.CreateInt("SourceFileId", 1, false),
                    ExpectedColumn.CreateNvarChar("FileName", 2, false),
                    ExpectedColumn.CreateDateTime("FilePreparationDate",3,false),
                    ExpectedColumn.CreateNvarChar("UKPRN", 4, false),
                    ExpectedColumn.CreateDateTime("DateTime",5,true)
                };
            _fixture.SchemaTests.AssertTableColumnsExist("dbo", "SourceFile", expectedColumns, true);
        }


        [Fact]
        public void CheckColumnsValidationError()
        {
            List<ExpectedColumn> expectedColumns = new List<ExpectedColumn>
            {
                ExpectedColumn.CreateInt("SourceFileId", 1, false),
                ExpectedColumn.CreateInt("ValidationError_Id", 2, false),
                ExpectedColumn.CreateUniqueIdentifier("RowId", 3, true),
                ExpectedColumn.CreateVarChar("RuleId",4,true),
                ExpectedColumn.CreateVarChar("FundingLine",5,true),
                ExpectedColumn.CreateVarChar("AdjustmentType",6,true),
                ExpectedColumn.CreateVarChar("CalendarYear",7,true),
                ExpectedColumn.CreateVarChar("CalendarMonth",8,true),
                ExpectedColumn.CreateVarChar("Severity", 9, true),
                ExpectedColumn.CreateVarChar("ErrorMessage",10,true),
                ExpectedColumn.CreateVarChar("Value",11,true),
                ExpectedColumn.CreateDateTime("CreatedOn",12,true)
            };
            _fixture.SchemaTests.AssertTableColumnsExist("dbo", "ValidationError", expectedColumns, true);
        }

        [Fact]
        public void CheckColumnsAdjustmentType()
        {
            List<ExpectedColumn> expectedColumns = new List<ExpectedColumn>
            {
                ExpectedColumn.CreateInt("Id", 1, false),
                ExpectedColumn.CreateNvarChar("Name",2,false)
            };
            _fixture.SchemaTests.AssertTableColumnsExist("dbo", "AdjustmentType", expectedColumns, true);
        }

        [Fact]
        public void CheckColumnsFundingLine()
        {
            List<ExpectedColumn> expectedColumns = new List<ExpectedColumn>
            {
                ExpectedColumn.CreateInt("Id", 1, false),
                ExpectedColumn.CreateNvarChar("Name",2,false)
            };
            _fixture.SchemaTests.AssertTableColumnsExist("dbo", "FundingLine", expectedColumns, true);
        }

        [Fact]
        public void CheckColumnsContractType()
        {
            List<ExpectedColumn> expectedColumns = new List<ExpectedColumn>
            {
                ExpectedColumn.CreateInt("Id", 1, false),
                ExpectedColumn.CreateNvarChar("Name",2,false)
            };
            _fixture.SchemaTests.AssertTableColumnsExist("dbo", "ContractType", expectedColumns, true);
        }

        [Fact]
        public void CheckColumnsFundingLineContractTypeMapping()
        {
            List<ExpectedColumn> expectedColumns = new List<ExpectedColumn>
            {
                ExpectedColumn.CreateInt("FundingLineId", 1, false),
                ExpectedColumn.CreateInt("ContractTypeId",2,false)
            };
            _fixture.SchemaTests.AssertTableColumnsExist("dbo", "FundingLineContractTypeMapping", expectedColumns, true);
        }

    }
}
