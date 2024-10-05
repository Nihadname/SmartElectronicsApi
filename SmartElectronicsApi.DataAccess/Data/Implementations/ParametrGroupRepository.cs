using SmartElectronicsApi.Core.Entities;
using SmartElectronicsApi.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.DataAccess.Data.Implementations
{
    public class ParametrGroupRepository : Repository<ParametrGroup>, IParametrGroupRepository
    {
        public ParametrGroupRepository(SmartElectronicsDbContext context) : base(context)
        {
        }
    }
}
