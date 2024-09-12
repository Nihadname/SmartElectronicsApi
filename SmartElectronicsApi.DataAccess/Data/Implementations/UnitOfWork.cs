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
        public IAuthRepository authRepository { get; private set; }

        public ICategoryRepository categoryRepository { get; private set; }
        public UnitOfWork(SmartElectronicsDbContext smartElectronicsDbContext)
        {
            authRepository= new AuthRepository(smartElectronicsDbContext);
            categoryRepository= new CategoryRepository(smartElectronicsDbContext);
            _smartElectronicsDbContext = smartElectronicsDbContext;
        }

       
    }
}
