﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.DataService.Interface;
using ESFA.DC.EAS1819.EF;

namespace ESFA.DC.EAS1819.DataService
{
    public class ValidationErrorService : IValidationErrorService
    {
        private readonly IRepository<ValidationError> _validationErroRepository;
        private readonly IRepository<SourceFile> _sourcefileRepository;

        public ValidationErrorService(IRepository<ValidationError> validationErroRepository, IRepository<SourceFile> sourcefileRepository)
        {
            _validationErroRepository = validationErroRepository;
            _sourcefileRepository = sourcefileRepository;
        }

        public void LogValidationError(ValidationError validationError)
        {
            _validationErroRepository.Insert(validationError);
        }

        public int LogErrorSourceFile(SourceFile sourceFile)
        {
            _sourcefileRepository.Insert(sourceFile);
            return sourceFile.SourceFileId;
        }
    }
}
