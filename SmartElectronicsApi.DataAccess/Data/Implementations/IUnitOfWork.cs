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
        public IAuthRepository authRepository { get; }
        public ICategoryRepository   categoryRepository { get; }
    }
}
