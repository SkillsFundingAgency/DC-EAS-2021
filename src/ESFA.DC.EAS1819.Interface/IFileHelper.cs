using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.Model;
using ESFA.DC.JobContextManager.Model.Interface;

namespace ESFA.DC.EAS1819.Interface
{
    public interface IFileHelper
    {
        EasFileInfo GetEASFileInfo(IJobContextMessage jobContextMessage);
    }
}
