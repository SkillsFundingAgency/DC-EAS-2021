using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using ESFA.DC.EAS1819.DataService.Interface;
using ESFA.DC.EAS1819.EF;
using ESFA.DC.EAS1819.Model;
using ESFA.DC.EAS1819.Service.Interface;
using ESFA.DC.EAS1819.Service.Mapper;

namespace ESFA.DC.EAS1819.Service.Import
{
    public class ImportManager : IImportManager
    {
        private readonly Guid _submissionId;
        private readonly IRepository<PaymentTypes> _paymentTypeRepository;
        private readonly IEasSubmissionService _easSubmissionService;
        private readonly IEasPaymentService _easPaymentService;

        public ImportManager(IEasSubmissionService easSubmissionService, IEasPaymentService easPaymentService)
        {
            _easSubmissionService = easSubmissionService;
            _easPaymentService = easPaymentService;
        }

        public ImportManager(Guid submissionId, IEasSubmissionService easSubmissionService, IEasPaymentService easPaymentService)
            : this(easSubmissionService, easPaymentService)
        {
            _submissionId = submissionId;
        }

        public void ImportEasCsv(TextReader reader)
        {
            var paymentTypes = _easPaymentService.GetAllPaymentTypes();
            var csv = new CsvReader(reader);
            csv.Configuration.HasHeaderRecord = true;
            csv.Configuration.RegisterClassMap<EasCsvRecordMapper>();
            var records = csv.GetRecords<EasCsvRecord>();
            var submissionId = _submissionId != (Guid.Empty) ? _submissionId : Guid.NewGuid();
            //var easSubmission = new EasSubmission()
            //{
            //    SubmissionId = submissionId,
            //    CollectionPeriod = 7,
            //    DeclarationChecked = true,
            //    NilReturn = false,
            //    ProviderName = "MK",
            //    Ukprn = "123465",
            //    UpdatedOn = DateTime.Now,
            //};
            var submissionValuesList = new List<EasSubmissionValues>();
            foreach (var easRecord in records)
            {
                var paymentName = easRecord.EarningsAdjustment.Trim().ToLower() + ": " +
                                  easRecord.FundingLine.Trim().ToLower();
                var paymentType = paymentTypes.FirstOrDefault(x => x.PaymentName.Trim().ToLower().Equals(paymentName));
                if (paymentType is null)
                {
                    throw new Exception($"PaymentType:  {paymentName} does not exist.");
                }

                var easSubmissionValues = new EasSubmissionValues()
                {
                    PaymentId = paymentType.PaymentId,
                    CollectionPeriod = easRecord.Month,
                    PaymentValue = easRecord.Value,
                    SubmissionId = submissionId,
                };
                submissionValuesList.Add(easSubmissionValues);
            }

            //easSubmission.SubmissionValues = submissionValuesList;
            //_easSubmissionService.PersistEasSubmission(easSubmission);
            _easSubmissionService.PersistEasSubmissionValues(submissionValuesList);
        }
    }
}
