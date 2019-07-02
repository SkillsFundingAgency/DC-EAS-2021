﻿using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.Model;

namespace ESFA.DC.EAS1819.Interface.Reports
{
    public interface IReportingController
    {
        Task ProduceReportsAsync(
            IList<EasCsvRecord> models,
            IList<ValidationErrorModel> errors,
            EasFileInfo sourceFile,
            CancellationToken cancellationToken);

        Task FileLevelErrorReportAsync(
            IList<EasCsvRecord> models,
            EasFileInfo fileInfo,
            IList<ValidationErrorModel> errors,
            CancellationToken cancellationToken);
    }
}
