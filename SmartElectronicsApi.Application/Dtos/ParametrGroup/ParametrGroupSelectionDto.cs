using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.ParametrGroup
{
    public class ParametrGroupSelectionDto
    {
        public int ParametrGroupId { get; set; }  
        public List<int> ParametrValueIds { get; set; }
    }
}
