using Microsoft.EntityFrameworkCore;
using SmartElectronicsApi.Core.Entities;
using SmartElectronicsApi.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.DataAccess.Data.Implementations
{
    public class CampaignRepository : Repository<Campaign>, ICampaignRepository
    {
        private readonly SmartElectronicsDbContext _context;
        public CampaignRepository(SmartElectronicsDbContext context) : base(context)
        {
            _context = context;
        }
        public IEnumerable<Campaign> GetNonAppliedCampaigns()
        {
            var campaigns = _context.campaigns.Where(s => !s.isApplied).OrderBy(s => s.CreatedTime).ToList()
                .Select(s => new Campaign
                {
                    Id = s.Id,
                    isApplied = true,
                    DiscountPercentageValue = s.DiscountPercentageValue,
                });
            return campaigns;
        }
    }
}