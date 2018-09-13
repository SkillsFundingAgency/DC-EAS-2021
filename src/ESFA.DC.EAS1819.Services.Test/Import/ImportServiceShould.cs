using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using ESFA.DC.EAS1819.Data;
using ESFA.DC.EAS1819.Interface;
using ESFA.DC.EAS1819.Services.Data;
using ESFA.DC.EAS1819.Services.Import;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ESFA.DC.EAS1819.Services.Test.Import
{
    public class ImportServiceShould
    {
        [Fact]
        public void ImportEasCsv()
        {
            var optionsBuilder = new DbContextOptionsBuilder<EasdbContext>();
            var connString = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location).AppSettings.Settings["EasdbConnectionString"].Value;
            optionsBuilder.UseSqlServer(connString);

            IRepository<PaymentTypes> paymentRepository = new Repository<PaymentTypes>(context: new EasdbContext(optionsBuilder.Options));
            IRepository<EasSubmission> easSubmissionRepository = new Repository<EasSubmission>(context: new EasdbContext(optionsBuilder.Options));
            IRepository<EasSubmissionValues> easSubmissionValuesRepository = new Repository<EasSubmissionValues>(context: new EasdbContext(optionsBuilder.Options));
            EasSubmissionService easSubmissionService = new EasSubmissionService(easSubmissionRepository, easSubmissionValuesRepository);
            EasPaymentService easPaymentService = new EasPaymentService(paymentRepository);

            var submissionId = Guid.NewGuid();
            ImportManager importManager = new ImportManager(submissionId, easSubmissionService, easPaymentService);
            using (TextReader fileReader = File.OpenText(@"SampleEASFiles\EAS-10033670-1819-20180912-144437-03.csv"))
            {
                importManager.ImportEasCsv(fileReader);
            }

            var easSubmissionValues = easSubmissionService.GetEasSubmissionValues(submissionId);
            Assert.NotEmpty(easSubmissionValues);
            Assert.Equal(2, easSubmissionValues.Count);
        }
    }
}
