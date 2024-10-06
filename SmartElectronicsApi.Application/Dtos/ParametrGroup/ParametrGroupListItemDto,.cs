using SmartElectronicsApi.Application.Dtos.ParametrValue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.ParametrGroup
{
    public class ParametrGroupListItemDto
    {
        public string Name { get; set; }
        public List<ParametrValueListItemDto> parametrValues { get; set; }

    }
}
