using Microsoft.AspNetCore.Http.HttpResults;
using SmartElectronicsApi.Application.Dtos;
using SmartElectronicsApi.Application.Dtos.Campaign;
using SmartElectronicsApi.Application.Dtos.Category;
using SmartElectronicsApi.Application.Exceptions;
using SmartElectronicsApi.Application.Extensions;
using SmartElectronicsApi.Application.Interfaces;
using SmartElectronicsApi.Core.Entities;
using SmartElectronicsApi.DataAccess.Data.Implementations;

namespace SmartElectronicsApi.Application.Implementations
{
    public class CampaignService: ICampaignService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CampaignService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<string> CreateCampaign(CreateCampaignDto createCampaignDto)
        {
            await _unitOfWork.CampaignRepository.BeginTransactionAsync();
            try
            {
                var mappedImage = createCampaignDto.formFile.Save(Directory.GetCurrentDirectory(), "img");
                var productList = new List<Product>();
                
                var newCampaign = new Campaign()
                {
                    Title = createCampaignDto.Title.Trim(),
                    Description = createCampaignDto.Description ?? null,
                    StartDate = createCampaignDto.StartDate,
                    EndDate = createCampaignDto.EndDate,
                    ImageUrl = mappedImage,
                    DiscountPercentageValue = createCampaignDto.DiscountPercentage ?? null,
                    
                };
                await _unitOfWork.CampaignRepository.Create(newCampaign);
                _unitOfWork.Commit();
                if (createCampaignDto?.ProductIds?.Count != 0)
                {

                    foreach (var productId in createCampaignDto.ProductIds)
                    {
                        var existedProduct = await _unitOfWork.productRepository.GetEntity(s => s.Id == productId);
                        if (existedProduct is null)
                            throw new CustomException(400, $"Could not find product id {productId}");
                        var newCampaignProduct = new CampaignProduct()
                        {
                            ProductId = productId,
                            CampaignId = newCampaign.Id,
                        };
                        await _unitOfWork.CampaignProductRepository.Create(newCampaignProduct);
                    }
                    _unitOfWork.Commit();
                }
                await _unitOfWork.CampaignRepository.CommitTransactionAsync();
                return "succesfully created";
            }
            catch (Exception ex)
            {
                await _unitOfWork.CampaignRepository.RollbackTransactionAsync();
                throw new CustomException(500, $"System error:{ex.Message} or {ex.InnerException?.Message ?? "None"}");
            }

        }
        public async Task<PaginatedResponse<CampaignListItemDto>> GetAllForAdmin(int pageNumber = 1, int pageSize = 10)
        {
            var totalCount = (await _unitOfWork.CampaignRepository.GetAll()).Count();
            var allCampaigns = await _unitOfWork.CampaignRepository.GetAll(s => s.IsDeleted == false, (pageNumber - 1) * pageSize, pageSize);
            var mappedCampaignlistItemDto = allCampaigns.Select(allCampaigns => new CampaignListItemDto() {
                Title = allCampaigns.Title,
                Description= allCampaigns.Description,
                DiscountPercentageValue=allCampaigns.DiscountPercentageValue,
                EndDate = allCampaigns.EndDate,
                ImageUrl = allCampaigns.ImageUrl,
                StartDate = allCampaigns.StartDate,
                
            }).ToList();
            var paginatedResult = new PaginatedResponse<CampaignListItemDto>
            {
                Data = mappedCampaignlistItemDto,
                TotalRecords = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

             return paginatedResult; ;
        }
    }
}
