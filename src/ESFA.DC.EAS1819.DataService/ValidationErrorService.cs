using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.DataService.Interface;
using ESFA.DC.EAS1819.EF;
using ESFA.DC.EAS1819.EF.Interface;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.EAS1819.DataService
{
    public class ValidationErrorService : IValidationErrorService
    {
        private readonly IRepository<ValidationError> _validationErroRepository;
        private readonly IRepository<SourceFile> _sourcefileRepository;
        private readonly IEasdbContext _easdbContext;
        private readonly ILogger _logger;

        public ValidationErrorService(
            IRepository<ValidationError> validationErroRepository,
            IRepository<SourceFile> sourcefileRepository,
            IEasdbContext easdbContext,
            ILogger logger)
        {
            _validationErroRepository = validationErroRepository;
            _sourcefileRepository = sourcefileRepository;
            _easdbContext = easdbContext;
            _logger = logger;
        }

        public async Task<int> LogErrorSourceFileAsync(SourceFile sourceFile)
        {
            _sourcefileRepository.Insert(sourceFile);
            return sourceFile.SourceFileId;
        }

        public async Task LogValidationErrorsAsync(List<ValidationError> validationErrors, CancellationToken cancellationToken)
        {
            using (var transaction = _easdbContext.Database.BeginTransaction())
            {
                try
                {
                    foreach (var validationError in validationErrors)
                    {
                        _easdbContext.ValidationErrors.Add(validationError);
                    }

                    await _easdbContext.SaveChangesAsync(cancellationToken);
                    transaction.Commit();
                    _logger.LogInfo("EAS - Log validation errors successful.");
                }
                catch (Exception exception)
                {
                    _logger.LogError("EAS - Log validation errors failed", exception);

                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task<List<ValidationError>> GetValidationErrorsAsync(string UkPrn)
        {
            var validationErrors = new List<ValidationError>();
            var sourceFile = _sourcefileRepository.TableNoTracking.Where(x => x.UKPRN.Equals(UkPrn))
                .OrderByDescending(x => x.FilePreparationDate).FirstOrDefault();
            if (sourceFile != null)
            {
                validationErrors = _validationErroRepository.TableNoTracking.Where(x => x.SourceFileId == sourceFile.SourceFileId).ToList();
            }

            return validationErrors;
        }
    }
}
