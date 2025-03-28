using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.Campaign
{
    public class CreateCampaignDto
    {
        public required IFormFile formFile;
        public required string Title { get; set; }
        public string? Description { get; set; }
        public required DateTimeOffset StartDate { get; set; }
        public required DateTimeOffset EndDate { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public List<int>? ProductIds { get; set; }
    }
}
