using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.EAS.Model;
using ESFA.DC.JobContextManager.Model.Interface;

namespace ESFA.DC.EAS.Interface
{
    public interface IFileHelper
    {
        EasFileInfo GetEASFileInfo(IJobContextMessage jobContextMessage);
    }
}
