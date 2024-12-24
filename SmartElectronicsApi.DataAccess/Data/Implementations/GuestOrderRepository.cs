using SmartElectronicsApi.Core.Entities;
using SmartElectronicsApi.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.DataAccess.Data.Implementations
{
    public class GuestOrderRepository : Repository<GuestOrder>, IGuestOrderRepository
    {
        public GuestOrderRepository(SmartElectronicsDbContext context) : base(context)
        {
        }
    }
}
