using SmartElectronicsApi.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Core.Entities
{
    public class Branch:BaseEntity
    {
        public string Name { get; set; }
        public ICollection<BranchCampaign> branchCampaigns { get; set; }

    }
}
