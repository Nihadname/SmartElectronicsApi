﻿using SmartElectronicsApi.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Core.Entities
{
    public class CampaignProduct:BaseEntity
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int CampaignId { get; set; }
        public Campaign Campaign { get; set; }
    }
}
