using Microsoft.AspNetCore.Http;
using SmartElectronicsApi.Application.Dtos;
using SmartElectronicsApi.Application.Dtos.Campaign;
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
        private readonly IHttpContextAccessor _contextAccessor;
        public CampaignService(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
           
        }
        private string GetBaseUrl()
        {
            var httpContext = _contextAccessor.HttpContext;
            if (httpContext == null)
                throw new InvalidOperationException("No HTTP context available");

            var request = httpContext.Request;
            var uriBuilder = new UriBuilder(request.Scheme,
                request.Host.Host,
                request.Host.Port ?? -1);
            return uriBuilder.Uri.AbsoluteUri;
        }
        public async Task<string> CreateCampaign(CreateCampaignDto createCampaignDto)
        {
            
            await _unitOfWork.CampaignRepository.BeginTransactionAsync();
            try
            {
                var isTitleExist=await _unitOfWork.CampaignRepository.isExists(s=>s.Title.ToLower()==createCampaignDto.Title.ToLower());
                if (isTitleExist)
                    throw new CustomException(400, "Name already exisits");
                var mappedImage = createCampaignDto.formFile.Save(Directory.GetCurrentDirectory(), "img");
                var productList = new List<Product>();
                
                var newCampaign = new Campaign()
                {
                   
                    Title = createCampaignDto.Title.Trim(),
                    Description = createCampaignDto.Description ?? null,
                    StartDate = createCampaignDto.StartDate,
                    EndDate = createCampaignDto.EndDate,
                    ImageUrl = GetBaseUrl() + "img/"+ mappedImage,
                    DiscountPercentageValue = createCampaignDto.DiscountPercentage ?? 0m,
                    CreatedTime=DateTime.Now,
                    
                };
                await _unitOfWork.CampaignRepository.Create(newCampaign);
                _unitOfWork.Commit();
                if (createCampaignDto?.ProductIds?.Count != 0&&createCampaignDto.ProductIds is not null)
                {
                    var productIds = createCampaignDto?.ProductIds?.Distinct().ToList();

                    foreach (var productId in productIds)
                    {
                        var isProductExist = await _unitOfWork.productRepository.isExists(s => s.Id == productId);
                        if (!isProductExist)
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
                if(createCampaignDto?.BranchIds?.Count!=0&&createCampaignDto?.BranchIds is not null)
                {
                    var branchIds=createCampaignDto?.BranchIds?.Distinct().ToList();
                    foreach(var branchId in branchIds)
                    {
                        var isBranchExist = await _unitOfWork.BranchRepository.isExists(s => s.Id == branchId);
                        if (!isBranchExist)
                            throw new CustomException(400, $"Could not find branch id {branchId}");
                        var newBranchCampaign = new BranchCampaign()
                        {
                            BranchId = branchId,
                            CampaignId = newCampaign.Id,
                        };
                        await _unitOfWork.BranchCampaignRepository.Create(newBranchCampaign);
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
                Id = allCampaigns.Id,
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
        public async Task ApplyDiscountPriceToProduct(int? Id)
        {
            if(Id == null) throw new ArgumentNullException(nameof(Id));
            var existedCampaign=await _unitOfWork.CampaignRepository.GetEntity(s=>s.Id==Id);
            if(existedCampaign == null)
                throw new CustomException(404,nameof(existedCampaign));
            if(existedCampaign.DiscountPercentageValue == 0m)
            {
                throw new Exception("Campaign does not have a discount percentage");
            }
            decimal discountPercentage = existedCampaign.DiscountPercentageValue.Value;
            if(existedCampaign.CampaignProducts.Count == 0&&existedCampaign.CampaignProducts is null)
            {
                foreach (var product in existedCampaign.CampaignProducts.Select(s=>s.Product))
                {
                    product.DiscountPercentage = discountPercentage;
                    product.DiscountedPrice = product.Price - (product.Price * (discountPercentage / 100));
                    await _unitOfWork.productRepository.Update(product);
                }
                _unitOfWork.Commit();
            }
        }
    }
}
