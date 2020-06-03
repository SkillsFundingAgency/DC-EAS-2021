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
    public class ValidationErrorRetrievalService : IValidationErrorRetrievalService
    {
        private readonly IEasdbContext _easdbContext;
        private readonly ILogger _logger;

        public ValidationErrorRetrievalService(
            IEasdbContext easdbContext,
            ILogger logger)
        {
            _easdbContext = easdbContext;
            _logger = logger;
        }

        public async Task<IEnumerable<ValidationError>> GetValidationErrorsAsync(int UkPrn, CancellationToken cancellationToken)
        {
            IEnumerable<ValidationError> validationErrors = new List<ValidationError>();

            var sourceFile = await _easdbContext.SourceFiles
                .Where(x => x.Ukprn == UkPrn.ToString())
                .OrderByDescending(x => x.FilePreparationDate)
                .FirstOrDefaultAsync(cancellationToken);

            if (sourceFile != null)
            {
                validationErrors = await _easdbContext.ValidationErrors?
                    .Where(x => x.SourceFileId == sourceFile.SourceFileId)
                    .ToListAsync(cancellationToken) ?? Enumerable.Empty<ValidationError>();
            }

            return validationErrors;
        }
    }
}
