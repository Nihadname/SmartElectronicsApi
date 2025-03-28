using SmartElectronicsApi.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.DataAccess.Data.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SmartElectronicsDbContext _smartElectronicsDbContext;

        public ICategoryRepository categoryRepository { get; private set; }
        public ISliderRepository sliderRepository { get; private set; }
        public IBrandRepository brandRepository { get; private set; }
        public ISettingRepository settingRepository { get; private set; }
        public ISubCategoryRepository subCategoryRepository { get; private set; }

        public IAddressRepository addressRepository { get; private set; }
        public IProductRepository productRepository { get; private set; }
        public IColorRepository colorRepository { get; private set; }
        public ISubscriberRepository subscriberRepository { get; private set; }
        public IProductColorRepository ProductColorRepository { get; private set; }
        public IProductImageRepository ProductImageRepository { get; private set; }

        public IParametrGroupRepository parametricGroupRepository { get; private set; }

        public IParametrValueRepository parametrValueRepository { get; private set; }
        public IProductVariationRepository productVariationRepository { get; private set; }
        public IProductVariationColorRepository ProductVariationColorRepository { get; private set; }
        public IContactRepository ContactRepository { get; private set; }
        public IBasketRepository BasketRepository { get; private set; }
        public IBasketProductRepository BasketProductRepository { get; private set; }
        public IOrderRepository OrderRepository { get; private set; }
        public IOrderItemRepository OrderItemRepository { get; private set; }
        public IWishListRepository WishListRepository { get; private set; }
        public IWishListProductRepository WishListProductRepository { get; private set; }
        public ICommentRepository CommentRepository { get; private set; }
        public ICommentImageRepository CommentImageRepository { get; private set; }
        public IGuestOrderRepository GuestOrderRepository { get; private set; }
        public ICampaignRepository CampaignRepository { get; private set; }
        public UnitOfWork(SmartElectronicsDbContext smartElectronicsDbContext)
        {
            categoryRepository= new CategoryRepository(smartElectronicsDbContext);
            sliderRepository= new SliderRepository(smartElectronicsDbContext);
            subCategoryRepository= new SubCategoryRepository(smartElectronicsDbContext);
            brandRepository= new BrandRepository(smartElectronicsDbContext);
            settingRepository= new SettingRepository(smartElectronicsDbContext);
            addressRepository= new AddressRepository(smartElectronicsDbContext);
            colorRepository= new ColorRepository(smartElectronicsDbContext);
            subscriberRepository= new SubscriberRepository(smartElectronicsDbContext);
            productRepository= new ProductRepository(smartElectronicsDbContext);
            ProductColorRepository= new ProductColorRepository(smartElectronicsDbContext);
            ProductImageRepository= new ProductImageRepository(smartElectronicsDbContext);
            parametricGroupRepository = new ParametrGroupRepository(smartElectronicsDbContext);
            parametrValueRepository = new ParametrValueRepository(smartElectronicsDbContext);
            productVariationRepository= new ProductVariationRepository(smartElectronicsDbContext);
            ProductVariationColorRepository= new ProductVariationColorRepository(   smartElectronicsDbContext);
            ContactRepository= new ContactRepository(smartElectronicsDbContext);
            BasketRepository= new BasketRepository(smartElectronicsDbContext);
            BasketProductRepository= new BasketProductRepository(smartElectronicsDbContext);
            OrderRepository= new OrderRepository(smartElectronicsDbContext);
           OrderItemRepository= new OrderItemRepository(smartElectronicsDbContext);
            WishListRepository= new WishListRepository( smartElectronicsDbContext);
            WishListProductRepository= new WishListProductRepository( smartElectronicsDbContext);   
            CommentRepository= new CommentRepository( smartElectronicsDbContext);
            CommentImageRepository= new CommentImageRepository( smartElectronicsDbContext);
            GuestOrderRepository= new GuestOrderRepository( smartElectronicsDbContext);
            CampaignRepository= new CampaignRepository( smartElectronicsDbContext);
        _smartElectronicsDbContext = smartElectronicsDbContext;
        }

        public void Commit()
        {
            _smartElectronicsDbContext.SaveChanges();
        }
        public void Dispose()
        {
            _smartElectronicsDbContext.Dispose();
        }
    }
}
