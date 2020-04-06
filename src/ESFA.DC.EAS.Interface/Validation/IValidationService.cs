﻿using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.Model;

namespace ESFA.DC.EAS.Interface.Validation
{
   public interface IValidationService
   {
       ValidationErrorModel ValidateFile(StreamReader streamReader, out List<EasCsvRecord> easCsvRecords);

       Task<List<ValidationErrorModel>> ValidateDataAsync(EasFileInfo fileInfo, List<EasCsvRecord> easCsvRecords, CancellationToken cancellationToken);

       Task LogValidationErrorsAsync(List<ValidationErrorModel> validationErrors, EasFileInfo fileInfo, CancellationToken cancellationToken);
   }
}