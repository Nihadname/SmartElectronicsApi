using SmartElectronicsApi.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.DataAccess.Data.Implementations
{
    public interface IUnitOfWork
    {
        public ICategoryRepository   categoryRepository { get; }
        public ISliderRepository sliderRepository { get; }
        public ISubCategoryRepository subCategoryRepository { get; }
        public IBrandRepository brandRepository { get; }
        public ISettingRepository settingRepository { get; }
        public IAddressRepository addressRepository { get; }
        public IColorRepository colorRepository { get; }
        public ISubscriberRepository subscriberRepository { get; }
        public IProductRepository productRepository {get; }
        public IProductColorRepository ProductColorRepository { get; }
        public IProductImageRepository ProductImageRepository { get; }
        public IParametrGroupRepository parametricGroupRepository { get; }
        public IParametrValueRepository parametrValueRepository { get; }
        public IProductVariationRepository productVariationRepository { get; }
        public IProductVariationColorRepository ProductVariationColorRepository { get; }
        public IContactRepository ContactRepository { get; }
        public IBasketRepository BasketRepository { get; }
        public IBasketProductRepository BasketProductRepository { get; }
        public void Commit();

    }
}
