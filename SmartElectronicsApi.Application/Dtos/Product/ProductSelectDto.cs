using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.Product
{
    public sealed record ProductSelectDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
