using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.DataService.Constants;
using ESFA.DC.EAS.DataService.Interface.Postcodes;
using ESFA.DC.ReferenceData.Postcodes.Model;
using ESFA.DC.ReferenceData.Postcodes.Model.Interface;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.EAS.DataService.Postcodes
{
    public class PostcodesDataService : IPostcodesDataService
    {
        private readonly IPostcodesContext _postcodesContext;

        public PostcodesDataService(IPostcodesContext postcodesContext)
        {
            _postcodesContext = postcodesContext;
        }

        public async Task<IReadOnlyDictionary<int, string>> GetMcaShortCodesForSofCodes(IEnumerable<int> sofCodes, CancellationToken cancellationToken)
        {
            var sofCodeStrings = sofCodes.Select(x => x.ToString());

            var mcaSofs = await _postcodesContext.McaglaSofs.Where(x => sofCodeStrings.Contains(x.SofCode)).ToListAsync(cancellationToken);

            return BuildDictionary(mcaSofs);
        }

        private IReadOnlyDictionary<int, string> BuildDictionary(IEnumerable<McaglaSof> mcaGlaSofs)
        {
            return mcaGlaSofs.Where(s =>
                s.EffectiveFrom <= DataServiceConstants.AcademicYearStart
                && (!s.EffectiveTo.HasValue || DataServiceConstants.AcademicYearEnd.Date <= s.EffectiveTo))
                .GroupBy(x => x.SofCode)
                .ToDictionary(
                k => int.Parse(k.Key),
                v => v.OrderByDescending(x => x.EffectiveFrom).Select(x => x.McaglaShortCode).FirstOrDefault());
        }
    }
}
