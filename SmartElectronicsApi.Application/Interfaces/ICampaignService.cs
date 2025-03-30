using SmartElectronicsApi.Application.Dtos;
using SmartElectronicsApi.Application.Dtos.Campaign;
using SmartElectronicsApi.Application.Dtos.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Interfaces
{
    public interface ICampaignService
    {
        Task<string> CreateCampaign(CreateCampaignDto createCampaignDto);
        Task<PaginatedResponse<CampaignListItemDto>> GetAllForAdmin(int pageNumber = 1, int pageSize = 10);
        Task ApplyDiscountPriceToProduct(int? Id);
    }
}
