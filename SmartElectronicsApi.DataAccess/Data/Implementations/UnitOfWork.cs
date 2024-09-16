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
        public UnitOfWork(SmartElectronicsDbContext smartElectronicsDbContext)
        {
            categoryRepository= new CategoryRepository(smartElectronicsDbContext);
            sliderRepository= new SliderRepository(smartElectronicsDbContext);
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
