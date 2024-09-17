using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.Paganation
{
    public interface IPaganationDto
    {
        int CurrentPage { get; }
        int TotalPage { get; }
        bool HasNext { get; }
        bool HasPrev { get; }
    }
}
