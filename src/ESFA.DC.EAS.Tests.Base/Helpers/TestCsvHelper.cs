using System;
using System.IO;
using System.Linq;
using CsvHelper.Configuration;
using ESFA.DC.EAS.Tests.Base.Models;
using FluentAssertions;
using Microsoft.VisualBasic.FileIO;
using Xunit;

namespace ESFA.DC.EAS.Tests.Base.Helpers
{
    public static class TestCsvHelper
    {
        public static void CheckCsv(string csv, params CsvEntry[] csvEntries)
        {
            try
            {
                using (TextReader textReader = new StringReader(csv))
                {
                    using (TextFieldParser textFieldParser = new TextFieldParser(textReader))
                    {
                        textFieldParser.TextFieldType = FieldType.Delimited;
                        textFieldParser.Delimiters = new[] { "," };
                        foreach (CsvEntry csvEntry in csvEntries)
                        {
                            string[] currentRow = textFieldParser.ReadFields();
                            CheckHeader(currentRow, csvEntry.Mapper);
                            for (int i = 0; i < csvEntry.DataRows; i++)
                            {
                                currentRow = textFieldParser.ReadFields();
                                CheckRow(currentRow, csvEntry.Mapper);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Assert.Null(ex);
            }
        }

        private static void CheckRow(string[] currentRow, ClassMap classMap)
        {
            currentRow.Length.Should().Be(classMap.MemberMaps.Count);
        }

        private static void CheckHeader(string[] currentRow, ClassMap classMap)
        {
            string[] names = classMap.MemberMaps.OrderBy(x => x.Data.Index).Select(x => x.Data.Names[0]).ToArray();
            currentRow.Should().BeEquivalentTo(names);
        }
    }
}
