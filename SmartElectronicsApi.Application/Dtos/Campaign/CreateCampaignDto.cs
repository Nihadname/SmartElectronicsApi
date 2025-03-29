using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.Campaign
{
    public sealed record CreateCampaignDto
    {
        public required IFormFile formFile { get; init; }
        public required string Title { get; init; }
        public string? Description { get; init; }
        public required DateTimeOffset StartDate { get; init; }
        public required DateTimeOffset EndDate { get; init; }
        public decimal? DiscountPercentage { get; init; }
        public List<int>? ProductIds { get; init; }
        public List<int>? BranchIds { get; init; }
    }
}
