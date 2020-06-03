﻿using Autofac;
using ESFA.DC.EAS.DataService;
using ESFA.DC.EAS.DataService.Interface;
using ESFA.DC.EAS.Interface.Validation;
using ESFA.DC.EAS.ValidationService;

namespace ESFA.DC.EAS.Stateless.Modules
{
    public class ValidationModule : Module
    {
        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<FileValidationService>().As<IFileValidationService>();
            containerBuilder.RegisterType<EasValidationService>().As<IValidationService>();
            containerBuilder.RegisterType<ValidationErrorRetrievalService>().As<IValidationErrorRetrievalService>();
            containerBuilder.RegisterType<ValidationErrorRuleService>().As<IValidationErrorRuleService>();
        }
    }
}
