using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.DataService.Interface;
using ESFA.DC.EAS2021.EF;
using ESFA.DC.EAS2021.EF.Interface;
using ESFA.DC.Logging.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.EAS.DataService
{
    public class ValidationErrorService : IValidationErrorService
    {
        private readonly IEasdbContext _easdbContext;
        private readonly ILogger _logger;

        public ValidationErrorService(
            IEasdbContext easdbContext,
            ILogger logger)
        {
            _easdbContext = easdbContext;
            _logger = logger;
        }

        public async Task<int> LogErrorSourceFileAsync(SourceFile sourceFile, CancellationToken cancellationToken)
        {
            await _easdbContext.SourceFiles.AddAsync(sourceFile, cancellationToken);
            await _easdbContext.SaveChangesAsync(cancellationToken);
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

        public async Task<List<ValidationError>> GetValidationErrorsAsync(string UkPrn, CancellationToken cancellationToken)
        {
            List<ValidationError> validationErrors = new List<ValidationError>();
            SourceFile sourceFile = await _easdbContext.SourceFiles.Include(x => x.ValidationErrors).Where(x => x.Ukprn.Equals(UkPrn)).OrderByDescending(x => x.FilePreparationDate).FirstOrDefaultAsync(cancellationToken);
            if (sourceFile != null)
            {
                validationErrors = sourceFile.ValidationErrors.ToList();
            }

            return validationErrors;
        }
    }
}
